using SharpIpp;
using SharpIpp.Protocol.Models;
using System.Collections.Concurrent;
using SharpIpp.Protocol;
using SharpIppServerExample.Models;
using SharpIpp.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using SharpIpp.Exceptions;

namespace SharpIppServerExample.Services;

public class PrinterJobsService : IDisposable, IAsyncDisposable
{
    private bool disposedValue;
    private int _newJobIndex = 0;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PrinterJobsService> _logger;
    private readonly SharpIppServer _ippServer;
    private readonly IOptions<PrinterOptions> _options;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;
    private bool _isPaused;
    private readonly ConcurrentDictionary<int, PrinterJob> _jobs = new();
    private readonly string _documentFormatDefault;
    private readonly DateTimeOffset _startTime;
    private readonly PrintScaling _printScalingDefault = PrintScaling.Auto;
    private readonly Sides _sidesDefault = Sides.OneSided;
    private readonly string _mediaDefault = "iso_a4_210x297mm";
    private readonly Resolution _printerResolutionDefault = new( 600, 600, ResolutionUnit.DotsPerInch );
    private readonly Finishings _finishingsDefault = Finishings.None;
    private readonly PrintQuality _printQualityDefault = PrintQuality.High;
    private readonly int _jobPriorityDefault = 1;
    private readonly int _copiesDefault = 1;
    private readonly Orientation _orientationRequestedDefault = Orientation.Portrait;
    private readonly JobHoldUntil _jobHoldUntil = JobHoldUntil.NoHold;

    public PrinterJobsService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<PrinterJobsService> logger,
        IOptions<PrinterOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _ippServer = new SharpIppServer();
        _options = options;
        _contentTypeProvider = new FileExtensionContentTypeProvider();
        _documentFormatDefault = _contentTypeProvider.Mappings[".pdf"];
        _startTime = DateTimeOffset.UtcNow.AddMinutes( -1 );
    }

    private int GetNextValue()
    {
        return Interlocked.Increment( ref _newJobIndex );
    }

    public async Task ProcessRequestAsync( Stream inputStream, Stream outputStream )
    {
        try
        {
            IIppRequest request = await _ippServer.ReceiveRequestAsync( inputStream );
            IIppResponseMessage response = request switch
            {
                CancelJobRequest x => await GetCancelJobResponseAsync(x),
                CreateJobRequest x => GetCreateJobResponse(x),
                CUPSGetPrintersRequest x => GetCUPSGetPrintersResponse(x),
                GetJobAttributesRequest x => GetGetJobAttributesResponse(x),
                GetJobsRequest x => GetGetJobsResponse(x),
                GetPrinterAttributesRequest x => GetGetPrinterAttributesResponse(x),
                HoldJobRequest x => await GetHoldJobResponseAsync(x),
                PausePrinterRequest x => GetPausePrinterResponse(x),
                PrintJobRequest x => await GetPrintJobResponseAsync(x),
                PrintUriRequest x => GetPrintUriResponse(x),
                PurgeJobsRequest x => await GetPurgeJobsResponseAsync( x),
                ReleaseJobRequest x => await GetReleaseJobResponseAsync(x),
                RestartJobRequest x => await GetRestartJobResponseAsync(x),
                ResumePrinterRequest x => GetResumePrinterResponse(x),
                SendDocumentRequest x => await GetSendDocumentResponseAsync( x),
                SendUriRequest x => await GetSendUriResponseAsync( x),
                ValidateJobRequest x => GetValidateJobResponse(x),
                _ => throw new NotImplementedException()
            };
            await _ippServer.SendResponseAsync( response, outputStream );
        }
        catch( IppRequestException ex )
        {
            _logger.LogError( ex, "Unable to process request" );
            var response = new IppResponseMessage
            {
                RequestId = ex.RequestMessage.RequestId,
                Version = ex.RequestMessage.Version,
                StatusCode = ex.StatusCode
            };
            var operation = new IppSection { Tag = SectionTag.OperationAttributesTag };
            operation.Attributes.Add( new IppAttribute( Tag.Charset, JobAttribute.AttributesCharset, "utf-8" ) );
            operation.Attributes.Add( new IppAttribute( Tag.NaturalLanguage, JobAttribute.AttributesNaturalLanguage, "en" ) );
            response.Sections.Add( operation );
            await _ippServer.SendRawResponseAsync( response, outputStream );
        }
        catch ( Exception ex )
        {
            _logger.LogError( ex, "Unable to process request" );
        }
    }

    private ValidateJobResponse GetValidateJobResponse( ValidateJobRequest request )
    {
        return new ValidateJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.SuccessfulOk
        };
    }

    private async Task<SendUriResponse> GetSendUriResponseAsync( SendUriRequest request )
    {
        var response = new SendUriResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.ClientErrorNotPossible
        };
        var jobId = GetJobId( request );
        if (!jobId.HasValue)
            return response;
        response.JobId = jobId.Value;
        response.JobUri = $"{GetPrinterUrl()}/{jobId.Value}";
        if(!_jobs.TryGetValue( jobId.Value, out var job ))
            return response;
        var copy = new PrinterJob( job );
        if (request.LastDocument)
        {
            if(!await copy.TrySetStateAsync( JobState.Pending, DateTimeOffset.UtcNow ))
                return response;
            _logger.LogDebug( "Job {id} has been moved to queue", job.Id );
        }
        request.DocumentAttributes ??= new DocumentAttributes();
        FillWithDefaultValues( request.DocumentAttributes );
        job.Requests.Add( request );
        _logger.LogDebug( "Document has been added to job {id}", job.Id );
        if (!_jobs.TryUpdate( jobId.Value, copy, job ))
            return response;
        response.StatusCode = IppStatusCode.SuccessfulOk;
        return response;
    }

    private async Task<SendDocumentResponse> GetSendDocumentResponseAsync( SendDocumentRequest request )
    {
        var response = new SendDocumentResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.ClientErrorNotPossible
        };
        var jobId = GetJobId( request );
        if (!jobId.HasValue)
            return response;
        response.JobId = jobId.Value;
        response.JobUri = $"{GetPrinterUrl()}/{jobId.Value}";
        if (!_jobs.TryGetValue( jobId.Value, out var job ))
            return response;
        var copy = new PrinterJob( job );
        if (request.LastDocument)
        {
            if (!await copy.TrySetStateAsync( JobState.Pending, DateTimeOffset.UtcNow ))
                return response;
            _logger.LogDebug( "Job {id} has been moved to queue", job.Id );
        }
        request.DocumentAttributes ??= new DocumentAttributes();
        FillWithDefaultValues( request.DocumentAttributes );
        job.Requests.Add( request );
        _logger.LogDebug( "Document has been added to job {id}", job.Id );
        if (!_jobs.TryUpdate( jobId.Value, copy, job ))
            return response;
        response.JobState = JobState.Pending;
        response.StatusCode = IppStatusCode.SuccessfulOk;
        return response;
    }

    private ReleaseJobResponse GetResumePrinterResponse( ResumePrinterRequest request )
    {
        _isPaused = false;
        return new ReleaseJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version
        };
    }

    private async Task<ReleaseJobResponse> GetRestartJobResponseAsync( RestartJobRequest request )
    {
        var response = new ReleaseJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.ClientErrorNotPossible
        };
        var jobId = GetJobId( request );
        if (!jobId.HasValue)
            return response;
        if (!_jobs.TryGetValue( jobId.Value, out var job ))
            return response;
        var copy = new PrinterJob( job );
        if (!await copy.TrySetStateAsync( JobState.Pending, DateTimeOffset.UtcNow ))
            return response;
        if (!_jobs.TryUpdate( jobId.Value, copy, job ))
            return response;
        response.StatusCode = IppStatusCode.SuccessfulOk;
        return response;
    }

    private async Task<ReleaseJobResponse> GetReleaseJobResponseAsync( ReleaseJobRequest request )
    {
        var response = new ReleaseJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.ClientErrorNotPossible
        };
        var jobId = GetJobId( request );
        if (!jobId.HasValue)
            return response;
        if (!_jobs.TryGetValue( jobId.Value, out var job ))
            return response;
        var copy = new PrinterJob( job );
        if (!await copy.TrySetStateAsync( JobState.Pending, DateTimeOffset.UtcNow ))
            return response;
        if (!_jobs.TryUpdate( jobId.Value, copy, job ))
            return response;
        response.StatusCode = IppStatusCode.SuccessfulOk;
        _logger.LogDebug( "Job {id} has been released", jobId );
        return response;
    }

    private async Task<PurgeJobsResponse> GetPurgeJobsResponseAsync( PurgeJobsRequest request )
    {
        foreach (var id in _jobs.Values.Where( x => x.State != JobState.Processing ).Select(x => x.Id))
        {
            if (_jobs.TryRemove( id, out var job ))
                await job.DisposeAsync();
        }
        return new PurgeJobsResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.SuccessfulOk
        };
    }

    private PrintUriResponse GetPrintUriResponse( PrintUriRequest request )
    {
        var response = new PrintUriResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            JobState = JobState.Pending,
            StatusCode = IppStatusCode.ClientErrorNotPossible
        };
        var job = new PrinterJob( GetNextValue(), request.RequestingUserName, DateTimeOffset.UtcNow );
        response.JobId = job.Id;
        response.JobUri = $"{GetPrinterUrl()}/{job.Id}";
        request.DocumentAttributes ??= new();
        FillWithDefaultValues( request.DocumentAttributes );
        request.NewJobAttributes ??= new();
        FillWithDefaultValues( job.Id, request.NewJobAttributes );
        job.Requests.Add( request );
        if (!_jobs.TryAdd( job.Id, job ))
            return response;
        response.StatusCode = IppStatusCode.SuccessfulOk;
        _logger.LogDebug( "Job {id} has been added to queue", job.Id );
        return response;
    }

    private PausePrinterResponse GetPausePrinterResponse( PausePrinterRequest request )
    {
        _isPaused = true;
        return new PausePrinterResponse
        {
            RequestId = request.RequestId,
            Version = request.Version
        };
    }

    private async Task<HoldJobResponse> GetHoldJobResponseAsync( HoldJobRequest request )
    {
        var response = new HoldJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.ClientErrorNotPossible
        };
        var jobId = GetJobId( request );
        if (!jobId.HasValue)
            return response;
        if (!_jobs.TryGetValue( jobId.Value, out var job ))
            return response;
        var copy = new PrinterJob( job );
        if (!await copy.TrySetStateAsync( null, DateTimeOffset.UtcNow ))
            return response;
        if (!_jobs.TryUpdate( jobId.Value, copy, job ))
            return response;
        response.StatusCode = IppStatusCode.SuccessfulOk;
        _logger.LogDebug( "Job {id} has been held", jobId );
        return response;
    }

    private GetPrinterAttributesResponse GetGetPrinterAttributesResponse( GetPrinterAttributesRequest request )
    {
        var options = _options.Value;
        bool IsRequired( string attributeName ) => request.RequestedAttributes?.Contains( attributeName ) ?? true;
        return new GetPrinterAttributesResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.SuccessfulOk,
            PrinterState = !IsRequired( PrinterAttribute.PrinterState)
                ? null
                : _jobs.Values.Any( x => x.State == JobState.Pending || x.State == JobState.Processing ) ? PrinterState.Processing : PrinterState.Idle,
            PrinterStateReasons = !IsRequired( PrinterAttribute.PrinterStateReasons) ? null : new string[] { "none" },
            CharsetConfigured = !IsRequired( PrinterAttribute.CharsetConfigured ) ? null : "utf-8",
            CharsetSupported = !IsRequired( PrinterAttribute.CharsetSupported) ? null :new string[] { "utf-8" },
            NaturalLanguageConfigured = !IsRequired( PrinterAttribute.NaturalLanguageConfigured) ? null : "en-us",
            GeneratedNaturalLanguageSupported = !IsRequired( PrinterAttribute.GeneratedNaturalLanguageSupported ) ? null : new string[] { "en-us" },
            PrinterIsAcceptingJobs = !IsRequired( PrinterAttribute.PrinterIsAcceptingJobs) ? null : true,
            PrinterMakeAndModel = !IsRequired( PrinterAttribute.PrinterMakeAndModel) ? null : options.Name,
            PrinterName = !IsRequired( PrinterAttribute.PrinterName) ? null : options.Name,
            PrinterInfo = !IsRequired( PrinterAttribute.PrinterInfo) ? null : options.Name,
            IppVersionsSupported = !IsRequired( PrinterAttribute.IppVersionsSupported ) ? null : new[] { new IppVersion( 1, 0 ), IppVersion.V11 },
            DocumentFormatDefault = !IsRequired( PrinterAttribute.DocumentFormatDefault) ? null : _documentFormatDefault,
            ColorSupported = !IsRequired( PrinterAttribute.ColorSupported) ? null : true,
            PrinterCurrentTime = !IsRequired( PrinterAttribute.PrinterCurrentTime) ? null : DateTimeOffset.Now,
            OperationsSupported = !IsRequired( PrinterAttribute.OperationsSupported) ? null : new[]
            {
                IppOperation.PrintJob,
                IppOperation.PrintUri,
                IppOperation.ValidateJob,
                IppOperation.CreateJob,
                IppOperation.SendDocument,
                IppOperation.SendUri,
                IppOperation.CancelJob,
                IppOperation.GetJobAttributes,
                IppOperation.GetJobs,
                IppOperation.GetPrinterAttributes,
                IppOperation.HoldJob,
                IppOperation.ReleaseJob,
                IppOperation.RestartJob,
                IppOperation.PausePrinter,
                IppOperation.ResumePrinter
            },
            QueuedJobCount = !IsRequired( PrinterAttribute.QueuedJobCount) ? null : _jobs.Values.Where( x => x.State == JobState.Pending || x.State == JobState.Processing ).Count(),
            DocumentFormatSupported = !IsRequired( PrinterAttribute.DocumentFormatSupported) ? null : new string[] { _documentFormatDefault },
            MultipleDocumentJobsSupported = !IsRequired( PrinterAttribute.MultipleDocumentJobsSupported) ? null : true,
            CompressionSupported = !IsRequired( PrinterAttribute.CompressionSupported) ? null : new Compression[] { Compression.None },
            PrinterLocation = !IsRequired( PrinterAttribute.PrinterLocation) ? null : "Internet",
            PrintScalingDefault = !IsRequired( PrinterAttribute.PrintScalingDefault) ? null : _printScalingDefault,
            PrintScalingSupported = !IsRequired( PrinterAttribute.PrintScalingSupported) ? null : new PrintScaling[] { _printScalingDefault },
            PrinterUriSupported = !IsRequired( PrinterAttribute.PrinterUriSupported) ? null : new string[] { GetPrinterUrl() },
            UriAuthenticationSupported = !IsRequired( PrinterAttribute.UriAuthenticationSupported) ? null : new[] { UriAuthentication.None },
            UriSecuritySupported = !IsRequired( PrinterAttribute.UriSecuritySupported) ? null : new[] { GetUriSecuritySupported() },
            PrinterUpTime = !IsRequired( PrinterAttribute.PrinterUpTime) ? null : (int)(DateTimeOffset.UtcNow - _startTime).TotalSeconds,
            MediaDefault = !IsRequired( PrinterAttribute.MediaDefault) ? null : _mediaDefault,
            MediaColDefault = !IsRequired( PrinterAttribute.MediaDefault) ? null : _mediaDefault,
            MediaSupported = !IsRequired( PrinterAttribute.MediaSupported) ? null : new string[] { _mediaDefault },
            SidesDefault = !IsRequired( PrinterAttribute.SidesDefault) ? null : _sidesDefault,
            SidesSupported = !IsRequired( PrinterAttribute.SidesSupported) ? null : Enum.GetValues( typeof( Sides ) ).Cast<Sides>().Where( x => x != Sides.Unsupported ).ToArray(),
            PdlOverrideSupported = !IsRequired( PrinterAttribute.PdlOverrideSupported) ? null : "attempted",
            MultipleOperationTimeOut = !IsRequired( PrinterAttribute.MultipleOperationTimeOut ) ? null : 120,
            FinishingsDefault = !IsRequired( PrinterAttribute.FinishingsDefault) ? null : _finishingsDefault,
            PrinterResolutionDefault = !IsRequired( PrinterAttribute.PrinterResolutionDefault) ? null : _printerResolutionDefault,
            PrinterResolutionSupported = !IsRequired( PrinterAttribute.PrinterResolutionSupported) ? null : new Resolution[] { _printerResolutionDefault },
            PrintQualityDefault = !IsRequired( PrinterAttribute.PrintQualityDefault) ? null : _printQualityDefault,
            PrintQualitySupported = !IsRequired( PrinterAttribute.PrintQualitySupported ) ? null : new[] { _printQualityDefault },
            JobPriorityDefault = !IsRequired( PrinterAttribute.JobPriorityDefault ) ? null : _jobPriorityDefault,
            JobPrioritySupported = !IsRequired( PrinterAttribute.JobPrioritySupported ) ? null : _jobPriorityDefault,
            CopiesDefault = !IsRequired( PrinterAttribute.CopiesDefault ) ? null : _copiesDefault,
            CopiesSupported = !IsRequired( PrinterAttribute.CopiesSupported ) ? null : new SharpIpp.Protocol.Models.Range( _copiesDefault, _copiesDefault ),
            OrientationRequestedDefault = !IsRequired( PrinterAttribute.OrientationRequestedDefault ) ? null : _orientationRequestedDefault,
            OrientationRequestedSupported = !IsRequired( PrinterAttribute.OrientationRequestedSupported ) ? null : Enum.GetValues( typeof( Orientation ) ).Cast<Orientation>().Where( x => x != Orientation.Unsupported ).ToArray(),
            PageRangesSupported = !IsRequired( PrinterAttribute.PageRangesSupported) ? null : false,
            PagesPerMinute = !IsRequired( PrinterAttribute.PagesPerMinute ) ? null : 20,
            PagesPerMinuteColor = !IsRequired( PrinterAttribute.PagesPerMinuteColor ) ? null : 20,
            PrinterMoreInfo = !IsRequired( PrinterAttribute.PrinterMoreInfo ) ? null : GetPrinterMoreInfo(),
            JobHoldUntilSupported = !IsRequired( PrinterAttribute.JobHoldUntilSupported ) ? null : new[] { JobHoldUntil.NoHold },
            JobHoldUntilDefault = !IsRequired( PrinterAttribute.JobHoldUntilDefault ) ? null : JobHoldUntil.NoHold,
            ReferenceUriSchemesSupported = !IsRequired( PrinterAttribute.ReferenceUriSchemesSupported ) ? null : new[] { UriScheme.Ftp, UriScheme.Http, UriScheme.Https },
        };
    }

    private UriSecurity GetUriSecuritySupported()
    {
        var request = _httpContextAccessor.HttpContext?.Request ?? throw new Exception( "Unable to access HttpContext" );
        return request.IsHttps ? UriSecurity.Tls : UriSecurity.None;
    }

    private GetJobsResponse GetGetJobsResponse( GetJobsRequest request )
    {
        IEnumerable<PrinterJob> jobs = _jobs.Values;
        jobs = request.WhichJobs switch
        {
            WhichJobs.Completed => jobs.Where( x => x.State == JobState.Completed || x.State == JobState.Aborted || x.State == JobState.Canceled ),
            WhichJobs.NotCompleted => jobs.Where( x => x.State == JobState.Processing || x.State == JobState.Pending ),
            _ => jobs.Where( x => x.State.HasValue )
        };
        if (request.MyJobs ?? false)
            jobs = jobs.Where( x => x.UserName?.Equals( request.RequestingUserName ) ?? false );
        jobs = jobs.OrderByDescending( x => x.State ).ThenByDescending( x => x.Id );
        if (request.Limit.HasValue)
            jobs = jobs.Take( request.Limit.Value );
        return new GetJobsResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.SuccessfulOk,
            Jobs = jobs.Select( x => GetJobAttributes( x, request.RequestedAttributes, true ) ).ToArray()
        };
    }

    private GetJobAttributesResponse GetGetJobAttributesResponse( GetJobAttributesRequest request )
    {
        var response = new GetJobAttributesResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.ClientErrorNotPossible,
            JobAttributes = new JobAttributes()
        };
        var jobId = GetJobId( request );
        if(!jobId.HasValue)
            return response;
        if(!_jobs.TryGetValue(jobId.Value, out var job))
            return response;
        response.JobAttributes = GetJobAttributes( job, request.RequestedAttributes, false );
        response.StatusCode = IppStatusCode.SuccessfulOk;
        return response;
    }

    private JobAttributes GetJobAttributes(PrinterJob job, string[]? requestedAttributes, bool isBatch )
    {
        var jobAttributes = job.Requests.Select( x => x switch
        {
            CreateJobRequest createJobRequest => createJobRequest.NewJobAttributes,
            PrintJobRequest printJobRequest => printJobRequest.NewJobAttributes,
            PrintUriRequest printUriRequest => printUriRequest.NewJobAttributes,
            _ => null,
        } ).FirstOrDefault( x => x != null );
        var documentAttributes = job.Requests.Select(x => x switch
        {
            PrintJobRequest printJobRequest => printJobRequest.DocumentAttributes,
            PrintUriRequest printUriRequest => printUriRequest.DocumentAttributes,
            SendDocumentRequest sendDocumentRequest => sendDocumentRequest.DocumentAttributes,
            SendUriRequest sendUriRequest => sendUriRequest.DocumentAttributes,
            _ => null,
        }).FirstOrDefault( x => x != null );
        bool IsRequired( string attributeName ) => (requestedAttributes?.Contains( "all" ) ?? false) || (requestedAttributes?.Contains( attributeName ) ?? !isBatch);
        var attributes = new JobAttributes
        {
            JobId = job.Id,
            JobUri = $"{GetPrinterUrl()}/{job.Id}",
            JobPrinterUri = !IsRequired( JobAttribute.JobPrinterUri ) ? null : GetPrinterUrl(),
            JobState = !IsRequired( JobAttribute.JobState ) ? null : job.State,
            JobStateReasons = !IsRequired( JobAttribute.JobState ) ? null : new[] { JobStateReason.None },
            DateTimeAtCreation = !IsRequired( JobAttribute.DateTimeAtCreation ) ? null : job.CreatedDateTime,
            TimeAtCreation = !IsRequired( JobAttribute.TimeAtCreation ) ? null : (int)(job.CreatedDateTime - _startTime).TotalSeconds,
            DateTimeAtProcessing = !IsRequired( JobAttribute.DateTimeAtProcessing ) ? null : job.ProcessingDateTime,
            TimeAtProcessing = !IsRequired( JobAttribute.TimeAtProcessing ) || !job.ProcessingDateTime.HasValue ? null : (int)(job.ProcessingDateTime.Value - _startTime).TotalSeconds,
            DateTimeAtCompleted = !IsRequired( JobAttribute.DateTimeAtCompleted ) ? null : job.CompletedDateTime,
            TimeAtCompleted = !IsRequired( JobAttribute.TimeAtCompleted ) || !job.CompletedDateTime.HasValue ? null : (int)(job.CompletedDateTime.Value - _startTime).TotalSeconds,
            Compression = !IsRequired( JobAttribute.Compression ) ? null : documentAttributes?.Compression,
            DocumentFormat = !IsRequired( JobAttribute.DocumentFormat ) ? null : documentAttributes?.DocumentFormat,
            DocumentName = !IsRequired( JobAttribute.DocumentName ) ? null : documentAttributes?.DocumentName,
            Copies = !IsRequired( JobAttribute.Copies ) ? null : jobAttributes?.Copies,
            Finishings = !IsRequired( JobAttribute.Finishings ) ? null : jobAttributes?.Finishings,
            IppAttributeFidelity = !IsRequired( JobAttribute.IppAttributeFidelity ) ? null : jobAttributes?.IppAttributeFidelity,
            JobName = !IsRequired( JobAttribute.JobName ) ? null : jobAttributes?.JobName,
            JobOriginatingUserName = !IsRequired( JobAttribute.JobOriginatingUserName ) ? null : job.UserName,
            MultipleDocumentHandling = !IsRequired( JobAttribute.MultipleDocumentHandling ) ? null : jobAttributes?.MultipleDocumentHandling,
            NumberUp = !IsRequired( JobAttribute.NumberUp ) ? null : jobAttributes?.NumberUp,
            OrientationRequested = !IsRequired( JobAttribute.OrientationRequested ) ? null : jobAttributes?.OrientationRequested,
            PrinterResolution = !IsRequired( JobAttribute.PrinterResolution ) ? null : jobAttributes?.PrinterResolution,
            Media = !IsRequired( JobAttribute.Media ) ? null : jobAttributes?.Media,
            PrintQuality = !IsRequired( JobAttribute.PrintQuality ) ? null : jobAttributes?.PrintQuality,
            Sides = !IsRequired( JobAttribute.Sides ) ? null : jobAttributes?.Sides,
            JobPrinterUpTime = !IsRequired( JobAttribute.JobPrinterUpTime ) ? null : (int)(DateTimeOffset.UtcNow - _startTime).TotalSeconds
        };
        return attributes;
    }

    private CUPSGetPrintersResponse GetCUPSGetPrintersResponse( CUPSGetPrintersRequest request )
    {
        return new CUPSGetPrintersResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.SuccessfulOk
        };
    }

    private CreateJobResponse GetCreateJobResponse( CreateJobRequest request )
    {
        var response = new CreateJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            JobState = JobState.Pending,
            StatusCode = IppStatusCode.ClientErrorNotPossible,
            JobStateReasons = new[] { JobStateReason.None }
        };
        var job = new PrinterJob( GetNextValue(), request.RequestingUserName, DateTimeOffset.UtcNow );
        response.JobId = job.Id;
        response.JobUri = $"{GetPrinterUrl()}/{job.Id}";
        request.NewJobAttributes ??= new();
        FillWithDefaultValues( job.Id, request.NewJobAttributes );
        job.Requests.Add( request );
        if (!_jobs.TryAdd( job.Id, job ))
            return response;
        response.StatusCode = IppStatusCode.SuccessfulOk;
        _logger.LogDebug( "Job {id} has been added to queue", job.Id );
        return response;
    }

    private async Task<CancelJobResponse> GetCancelJobResponseAsync( CancelJobRequest request )
    {
        var response = new CancelJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.ClientErrorNotPossible
        };
        var jobId = GetJobId( request );
        if (!jobId.HasValue)
            return response;
        if (!_jobs.TryGetValue( jobId.Value, out var job ))
            return response;
        var copy = new PrinterJob( job );
        if (!await copy.TrySetStateAsync( JobState.Canceled, DateTimeOffset.UtcNow ))
            return response;
        if (!_jobs.TryUpdate( jobId.Value, copy, job ))
            return response;
        response.StatusCode = IppStatusCode.SuccessfulOk;
        _logger.LogDebug( "Job {id} has been canceled", jobId );
        return response;
    }

    public async Task<PrinterJob?> GetPendingJobAsync()
    {
        if ( _isPaused )
            return null;
        foreach (var job in _jobs.Values.Where( x => x.State == JobState.Pending ).OrderBy( x => x.Id ))
        {
            var copy = new PrinterJob( job );
            if (!await copy.TrySetStateAsync( JobState.Processing, DateTimeOffset.UtcNow ))
                continue;
            if (!_jobs.TryUpdate( job.Id, copy, job ))
                continue;
            return copy;
        }
        return null;
    }

    public async Task AddCompletedJobAsync( int jobId )
    {
        if (!_jobs.TryGetValue( jobId, out var job ))
            return;
        var copy = new PrinterJob( job );
        if (!await copy.TrySetStateAsync( JobState.Completed, DateTimeOffset.UtcNow ))
            return;
        if(!_jobs.TryUpdate(jobId,copy, job ))
            return;
        _logger.LogDebug( "Job {id} has been completed", job.Id );
    }

    public async Task AddAbortedJobAsync( int jobId )
    {
        if (!_jobs.TryGetValue( jobId, out var job ))
            return;
        var copy = new PrinterJob( job );
        if (!await copy.TrySetStateAsync( JobState.Aborted, DateTimeOffset.UtcNow ))
            return;
        if (!_jobs.TryUpdate( jobId, copy, job ))
            return;
        _logger.LogDebug( "Job {id} has been aborted", job.Id );
    }

    private async Task<PrintJobResponse> GetPrintJobResponseAsync( PrintJobRequest request )
    {
        var response = new PrintJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            JobState = JobState.Pending,
            StatusCode = IppStatusCode.ClientErrorNotPossible,
            JobStateReasons = new[] { JobStateReason.None }
        };
        var job = new PrinterJob( GetNextValue(), request.RequestingUserName, DateTimeOffset.UtcNow );
        response.JobId = job.Id;
        response.JobUri = $"{GetPrinterUrl()}/{job.Id}";
        request.NewJobAttributes ??= new();
        FillWithDefaultValues( job.Id, request.NewJobAttributes );
        request.DocumentAttributes ??= new();
        FillWithDefaultValues( request.DocumentAttributes );
        job.Requests.Add( request );
        if (!await job.TrySetStateAsync( JobState.Pending, DateTimeOffset.UtcNow ))
            return response;
        if (!_jobs.TryAdd( job.Id, job ))
            return response;
        response.StatusCode = IppStatusCode.SuccessfulOk;
        _logger.LogDebug( "Job {id} has been added to queue", job.Id );
        return response;
    }

    private string GetPrinterUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request ?? throw new Exception( "Unable to access HttpContext" );
        return $"ipp://{request.Host}{request.PathBase}{request.Path}";
    }

    private string GetPrinterMoreInfo()
    {
        var request = _httpContextAccessor.HttpContext?.Request ?? throw new Exception( "Unable to access HttpContext" );
        return $"{request.Scheme}://{request.Host}{request.PathBase}";
    }

    private int? GetJobId( IIppJobRequest request )
    {
        if ( request.JobUrl != null && int.TryParse( request.JobUrl.Segments.LastOrDefault(), out int idFromUri))
            return idFromUri;
        return request.JobId;
    }

    private void FillWithDefaultValues(int jobId, NewJobAttributes attributes)
    {
        attributes.PrintScaling ??= _printScalingDefault;
        attributes.Sides ??= _sidesDefault;
        attributes.Media ??= _mediaDefault;
        attributes.PrinterResolution ??= _printerResolutionDefault;
        attributes.Finishings ??= _finishingsDefault;
        attributes.PrintQuality ??= _printQualityDefault;
        attributes.JobPriority ??= _jobPriorityDefault;
        attributes.Copies ??= _copiesDefault;
        attributes.OrientationRequested ??= _orientationRequestedDefault;
        attributes.JobHoldUntil ??= _jobHoldUntil;
        if (string.IsNullOrEmpty( attributes.JobName ))
            attributes.JobName = $"Job {jobId}";
    }

    private void FillWithDefaultValues(DocumentAttributes attributes)
    {
        if (string.IsNullOrEmpty( attributes.DocumentFormat ))
            attributes.DocumentFormat = _documentFormatDefault;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait( false );
        Dispose( disposing: false );
        GC.SuppressFinalize( this );
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        foreach (var job in _jobs.Values)
            await job.DisposeAsync();
        _jobs.Clear();
    }

    protected virtual void Dispose( bool disposing )
    {
        if (disposedValue)
            return;
        if (disposing)
        {
            foreach (var job in _jobs.Values)
                job.Dispose();
            _jobs.Clear();
        }
        disposedValue = true;
    }

    public void Dispose()
    {
        Dispose( disposing: true );
        GC.SuppressFinalize( this );
    }
}
