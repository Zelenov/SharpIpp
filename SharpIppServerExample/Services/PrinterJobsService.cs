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

public class PrinterJobsService
{
    private int _newJobIndex = 0;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PrinterJobsService> _logger;
    private readonly SharpIppServer _ippServer;
    private readonly IOptions<PrinterOptions> _options;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;
    private bool _isPaused;
    private readonly ConcurrentDictionary<int, PrinterJob> _createdJobs = new();
    private readonly ConcurrentDictionary<int, PrinterJob> _pendingJobs = new();
    private readonly ConcurrentDictionary<int, PrinterJob> _failedJobs = new();
    private readonly ConcurrentDictionary<int, PrinterJob> _suspendedJobs = new();
    private readonly ConcurrentDictionary<int, PrinterJob> _canceledJobs = new();
    private readonly ConcurrentDictionary<int, PrinterJob> _completedJobs = new();
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
                HoldJobRequest x => GetHoldJobResponse(x),
                PausePrinterRequest x => GetPausePrinterResponse(x),
                PrintJobRequest x => GetPrintJobResponse(x),
                PrintUriRequest x => GetPrintUriResponse(x),
                PurgeJobsRequest x => GetPurgeJobsResponse(x),
                ReleaseJobRequest x => GetReleaseJobResponse(x),
                RestartJobRequest x => GetRestartJobResponse(x),
                ResumePrinterRequest x => GetResumePrinterResponse(x),
                SendDocumentRequest x => GetSendDocumentResponse(x),
                SendUriRequest x => GetSendUriResponse(x),
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

    private SendUriResponse GetSendUriResponse( SendUriRequest request )
    {
        var isSuccess = false;
        var jobId = GetJobId( request );
        if ( jobId.HasValue && _createdJobs.TryRemove( jobId.Value, out var job ) )
        {
            job.Requests.Add( request );
            _logger.LogDebug( "Document has been added to job {id}", job.Id );
            if ( request.LastDocument )
            {
                _pendingJobs.TryAdd( job.Id, job );
                _logger.LogDebug( "Job {id} has been moved to queue", jobId );
            }   
            else
                _createdJobs.TryAdd( job.Id, job );
            isSuccess = true;
        }
        return new SendUriResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = isSuccess ? IppStatusCode.SuccessfulOk : IppStatusCode.ClientErrorNotPossible,
            JobId = jobId ?? 0,
            JobUri = $"{GetPrinterUrl()}/{jobId ?? 0}"
        };
    }

    private SendDocumentResponse GetSendDocumentResponse( SendDocumentRequest request )
    {
        var isSuccess = false;
        JobState jobState = JobState.Processing;
        var jobId = GetJobId( request );
        if ( jobId.HasValue && _createdJobs.TryRemove( jobId.Value, out var job ) )
        {
            request.DocumentAttributes ??= new();
            FillWithDefaultValues( request.DocumentAttributes );
            job.Requests.Add( request );
            _logger.LogDebug( "Document has been added to job {id}", jobId );
            if ( request.LastDocument )
            {
                _pendingJobs.TryAdd( job.Id, job );
                _logger.LogDebug( "Job {id} has been moved to queue", job.Id );
            }
            else
                _createdJobs.TryAdd( job.Id, job );
            isSuccess = true;
        }
        return new SendDocumentResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = isSuccess ? IppStatusCode.SuccessfulOk : IppStatusCode.ClientErrorNotPossible,
            JobId = jobId ?? 0,
            JobUri = $"{GetPrinterUrl()}/{jobId ?? 0}",
            JobState = jobState
        };
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

    private ReleaseJobResponse GetRestartJobResponse( RestartJobRequest request )
    {
        var jobId = GetJobId( request );
        var isRestarted = false;
        if( jobId.HasValue && _failedJobs.TryRemove( jobId.Value, out var job ) )
        {
            _pendingJobs.TryAdd( jobId.Value, job );
            isRestarted = true;
        }
        return new ReleaseJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = isRestarted ? IppStatusCode.SuccessfulOk : IppStatusCode.ClientErrorNotPossible
        };
    }

    private ReleaseJobResponse GetReleaseJobResponse( ReleaseJobRequest request )
    {
        var jobId = GetJobId( request );
        var isReleased = false;
        if ( jobId.HasValue && _suspendedJobs.TryRemove( jobId.Value, out var job ) )
        {
            _pendingJobs.TryAdd( jobId.Value, job );
            _logger.LogDebug( "Job {id} has been released", jobId );
            isReleased = false;
        }
        return new ReleaseJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = isReleased ? IppStatusCode.SuccessfulOk : IppStatusCode.ClientErrorNotPossible
        };
    }

    private PurgeJobsResponse GetPurgeJobsResponse( PurgeJobsRequest request )
    {
        _createdJobs.Clear();
        _pendingJobs.Clear();
        _suspendedJobs.Clear();
        return new PurgeJobsResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.SuccessfulOk
        };
    }

    private PrintUriResponse GetPrintUriResponse( PrintUriRequest request )
    {
        var job = new PrinterJob( GetNextValue(), request.RequestingUserName, DateTimeOffset.UtcNow );
        request.DocumentAttributes ??= new();
        FillWithDefaultValues( request.DocumentAttributes );
        request.NewJobAttributes ??= new();
        FillWithDefaultValues( request.NewJobAttributes );
        job.Requests.Add( request );
        _createdJobs.TryAdd( job.Id, job );
        _logger.LogDebug( "Job {id} has been added to queue", job.Id );
        return new PrintUriResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            JobState = JobState.Pending,
            StatusCode = IppStatusCode.SuccessfulOk,
            JobId = job.Id,
            JobUri = $"{GetPrinterUrl()}/{job.Id}"
        };
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

    private HoldJobResponse GetHoldJobResponse( HoldJobRequest request )
    {
        var jobId = GetJobId( request );
        var isHeld = false;
        if ( jobId.HasValue && _pendingJobs.TryRemove( jobId.Value, out var job ) )
        {
            _suspendedJobs.TryAdd( jobId.Value, job );
            _logger.LogDebug( "Job {id} has been suspended", jobId );
            isHeld = true;
        }
        return new HoldJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = isHeld ? IppStatusCode.SuccessfulOk : IppStatusCode.ClientErrorNotPossible
        };
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
            PrinterState = !IsRequired( PrinterAttribute.PrinterState) ? null : _pendingJobs.IsEmpty ? PrinterState.Idle : PrinterState.Processing,
            PrinterStateReasons = !IsRequired( PrinterAttribute.PrinterStateReasons) ? null : new string[] { "none" },
            CharsetConfigured = !IsRequired( PrinterAttribute.CharsetConfigured ) ? null : "utf-8",
            CharsetSupported = !IsRequired( PrinterAttribute.CharsetSupported) ? null :new string[] { "utf-8" },
            NaturalLanguageConfigured = !IsRequired( PrinterAttribute.NaturalLanguageConfigured) ? null : "en-us",
            GeneratedNaturalLanguageSupported = !IsRequired( PrinterAttribute.GeneratedNaturalLanguageSupported ) ? null : new string[] { "en-us" },
            PrinterIsAcceptingJobs = !IsRequired( PrinterAttribute.PrinterIsAcceptingJobs) ? null : true,
            PrinterMakeAndModel = !IsRequired( PrinterAttribute.PrinterMakeAndModel) ? null : options.Name,
            PrinterName = !IsRequired( PrinterAttribute.PrinterName) ? null : options.Name,
            PrinterInfo = !IsRequired( PrinterAttribute.PrinterInfo) ? null : options.Name,
            IppVersionsSupported = !IsRequired( PrinterAttribute.IppVersionsSupported) ? null : new string[] { "1.0", "1.1" },
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
            QueuedJobCount = !IsRequired( PrinterAttribute.QueuedJobCount) ? null : _pendingJobs.Count,
            DocumentFormatSupported = !IsRequired( PrinterAttribute.DocumentFormatSupported) ? null : new string[] { _documentFormatDefault },
            MultipleDocumentJobsSupported = !IsRequired( PrinterAttribute.MultipleDocumentJobsSupported) ? null : true,
            CompressionSupported = !IsRequired( PrinterAttribute.CompressionSupported) ? null : new Compression[] { Compression.None },
            PrinterLocation = !IsRequired( PrinterAttribute.PrinterLocation) ? null : "Internet",
            PrintScalingDefault = !IsRequired( PrinterAttribute.PrintScalingDefault) ? null : _printScalingDefault,
            PrintScalingSupported = !IsRequired( PrinterAttribute.PrintScalingSupported) ? null : new PrintScaling[] { _printScalingDefault },
            PrinterUriSupported = !IsRequired( PrinterAttribute.PrinterUriSupported) ? null : new string[] { GetPrinterUrl() },
            UriAuthenticationSupported = !IsRequired( PrinterAttribute.UriAuthenticationSupported) ? null : new string[] { "none" },
            UriSecuritySupported = !IsRequired( PrinterAttribute.UriSecuritySupported) ? null : new string[] { GetUriSecuritySupported() },
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
            ReferenceUriSchemesSupported = !IsRequired( PrinterAttribute.ReferenceUriSchemesSupported ) ? null : new[] { "ftp" },
        };
    }

    private string GetUriSecuritySupported()
    {
        var request = _httpContextAccessor.HttpContext?.Request ?? throw new Exception( "Unable to access HttpContext" );
        return request.IsHttps ? "tls" : "none";
    }

    private GetJobsResponse GetGetJobsResponse( GetJobsRequest request )
    {
        var jobs = GetJobs(
            !request.WhichJobs.HasValue || request.WhichJobs.Value == WhichJobs.NotCompleted,
            !request.WhichJobs.HasValue || request.WhichJobs.Value == WhichJobs.Completed,
            ( request.MyJobs ?? false ) ? request.RequestingUserName : null,
            request.Limit ?? int.MaxValue )
            .Select( x => GetJobAttributes( x.Job, x.JobState, request.RequestedAttributes ) )
            .ToArray();
        return new GetJobsResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.SuccessfulOk,
            Jobs = jobs
        };
    }

    private GetJobAttributesResponse GetGetJobAttributesResponse( GetJobAttributesRequest request )
    {
        var jobId = GetJobId( request );
        var (job, jobState) = jobId.HasValue ? GetJob( jobId.Value ) : (null, JobState.Aborted);
        var jobAttributes = job != null ? GetJobAttributes( job, jobState, request.RequestedAttributes ) : null;
        return new GetJobAttributesResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = jobAttributes == null ? IppStatusCode.ClientErrorNotPossible : IppStatusCode.SuccessfulOk,
            JobAttributes = jobAttributes ?? new JobAttributes()
        };
    }

    private JobAttributes GetJobAttributes(PrinterJob job, JobState jobState, string[]? requestedAttributes)
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
        var userName = job.Requests.Select( x => x switch
        {
            CreateJobRequest createJobRequest => createJobRequest.RequestingUserName,
            PrintJobRequest printJobRequest => printJobRequest.RequestingUserName,
            PrintUriRequest printUriRequest => printUriRequest.RequestingUserName,
            SendDocumentRequest sendDocumentRequest => sendDocumentRequest.RequestingUserName,
            SendUriRequest sendUriRequest => sendUriRequest.RequestingUserName,
            _ => null,
        } ).FirstOrDefault( x => x != null );
        bool IsRequired( string attributeName ) => requestedAttributes?.Contains( attributeName ) ?? true;
        var attributes = new JobAttributes
        {
            JobId = job.Id,
            JobUri = $"{GetPrinterUrl()}/{job.Id}",
            JobPrinterUri = GetPrinterUrl()
        };
        if ( IsRequired( JobAttribute.JobState ) )
            attributes.JobState = jobState;
        if ( IsRequired( JobAttribute.DateTimeAtCreation ) )
            attributes.DateTimeAtCreation = job.CreatedDateTime;
        if ( IsRequired( JobAttribute.TimeAtCreation ) )
            attributes.TimeAtCreation = (int)(job.CreatedDateTime - _startTime).TotalSeconds;
        if ( IsRequired( JobAttribute.DateTimeAtProcessing ) )
            attributes.DateTimeAtProcessing = job.ProcessingDateTime;
        if ( IsRequired( JobAttribute.TimeAtProcessing ) && job.ProcessingDateTime.HasValue )
            attributes.TimeAtProcessing = (int)(job.ProcessingDateTime.Value - _startTime ).TotalSeconds;
        if ( IsRequired( JobAttribute.DateTimeAtCompleted ) )
            attributes.DateTimeAtCompleted = job.CompletedDateTime;
        if ( IsRequired( JobAttribute.TimeAtCompleted ) && job.CompletedDateTime.HasValue )
            attributes.TimeAtCompleted = (int)(job.CompletedDateTime.Value - _startTime ).TotalSeconds;
        if ( IsRequired( JobAttribute.Compression ) )
            attributes.Compression = documentAttributes?.Compression;
        if ( IsRequired( JobAttribute.DocumentFormat ) )
            attributes.DocumentFormat = documentAttributes?.DocumentFormat;
        if ( IsRequired( JobAttribute.DocumentName ) )
            attributes.DocumentName = documentAttributes?.DocumentName;
        if ( IsRequired( JobAttribute.Copies ) )
            attributes.Copies = jobAttributes?.Copies;
        if ( IsRequired( JobAttribute.Finishings ) )
            attributes.Finishings = jobAttributes?.Finishings;
        if ( IsRequired( JobAttribute.IppAttributeFidelity ) )
            attributes.IppAttributeFidelity = jobAttributes?.IppAttributeFidelity;
        if ( IsRequired( JobAttribute.JobName ) )
            attributes.JobName = jobAttributes?.JobName;
        if ( IsRequired( JobAttribute.JobOriginatingUserName ) )
            attributes.JobOriginatingUserName = userName;
        if ( IsRequired( JobAttribute.MultipleDocumentHandling ) )
            attributes.MultipleDocumentHandling = jobAttributes?.MultipleDocumentHandling;
        if ( IsRequired( JobAttribute.NumberUp ) )
            attributes.NumberUp = jobAttributes?.NumberUp;
        if ( IsRequired( JobAttribute.OrientationRequested ) )
            attributes.OrientationRequested = jobAttributes?.OrientationRequested;
        if ( IsRequired( JobAttribute.PrinterResolution ) )
            attributes.PrinterResolution = jobAttributes?.PrinterResolution;
        if ( IsRequired( JobAttribute.Media ) )
            attributes.Media = jobAttributes?.Media;
        if ( IsRequired( JobAttribute.PrintQuality ) )
            attributes.PrintQuality = jobAttributes?.PrintQuality;
        if ( IsRequired( JobAttribute.Sides ) )
            attributes.Sides = jobAttributes?.Sides;
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
        var job = new PrinterJob( GetNextValue(), request.RequestingUserName, DateTimeOffset.UtcNow );
        request.NewJobAttributes ??= new();
        FillWithDefaultValues( request.NewJobAttributes );
        job.Requests.Add( request );
        _createdJobs.TryAdd( job.Id, job );
        _logger.LogDebug( "Job {id} has been created", job.Id );
        return new CreateJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            JobState = JobState.Pending,
            StatusCode = IppStatusCode.SuccessfulOk,
            JobId = job.Id,
            JobUri = $"{GetPrinterUrl()}/{job.Id}"
        };
    }

    private async Task<CancelJobResponse> GetCancelJobResponseAsync( CancelJobRequest request )
    {
        var jobId = GetJobId( request );
        var isCanceled = false;
        if (jobId.HasValue
            && ( _createdJobs.TryRemove( jobId.Value, out var job )
            || _pendingJobs.TryRemove( jobId.Value, out job )
            || _suspendedJobs.TryRemove( jobId.Value, out job ) ))
        {
            _canceledJobs.TryAdd( job.Id, job );
            foreach ( var ippRequest in job.Requests )
            {
                var document = ippRequest switch
                {
                    PrintJobRequest printJobRequest => printJobRequest.Document,
                    SendDocumentRequest sendDocumentRequest => sendDocumentRequest.Document,
                    _ => null
                };
                if ( document != null )
                    await document.DisposeAsync();
            }
            isCanceled = true;
            _logger.LogDebug( "Job {id} has been canceled", jobId );
        }   
        else
            _logger.LogDebug( "Unable to cancel job {id}", jobId );
        return new CancelJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = isCanceled ? IppStatusCode.SuccessfulOk : IppStatusCode.ClientErrorNotPossible
        };
    }

    public Task<PrinterJob?> GetPendingJobAsync()
    {
        if ( _isPaused )
            return Task.FromResult<PrinterJob?>( null );
        while ( true )
        {
            if ( _pendingJobs.IsEmpty )
                return Task.FromResult<PrinterJob?>( null );
            var keys = _pendingJobs.Keys;
            if ( !keys.Any() )
                return Task.FromResult<PrinterJob?>( null );
            if ( _pendingJobs.TryRemove( keys.OrderBy( x => x ).Min(), out PrinterJob? job ) )
            {
                return Task.FromResult<PrinterJob?>( job );
            }
        }
    }

    public Task AddCompletedJobAsync( PrinterJob job )
    {
        _completedJobs.TryAdd( job.Id, job );
        return Task.CompletedTask;
    }

    public Task AddFailedJobAsync( PrinterJob job )
    {
        _failedJobs.TryAdd( job.Id, job );
        return Task.CompletedTask;
    }

    private PrintJobResponse GetPrintJobResponse( PrintJobRequest request )
    {
        var job = new PrinterJob( GetNextValue(), request.RequestingUserName, DateTimeOffset.UtcNow );
        request.DocumentAttributes ??= new();
        FillWithDefaultValues( request.DocumentAttributes );
        request.NewJobAttributes ??= new();
        FillWithDefaultValues( request.NewJobAttributes );
        job.Requests.Add( request );
        _pendingJobs.TryAdd( job.Id, job );
        _logger.LogDebug( "Job {id} has been added to queue", job.Id );
        return new PrintJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            JobState = JobState.Pending,
            JobStateReasons = new[] { "none" },
            StatusCode = IppStatusCode.SuccessfulOk,
            JobId = job.Id,
            JobUri = $"{GetPrinterUrl()}/{job.Id}"
        };
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

    private (PrinterJob? Job, JobState JobState) GetJob(int jobId)
    {
        if ( _createdJobs.TryGetValue( jobId, out var job ) )
            return ( job, JobState.Pending );
        if ( _pendingJobs.TryGetValue( jobId, out job ) )
            return ( job, JobState.Pending );
        if ( _suspendedJobs.TryGetValue( jobId, out job ) )
            return (job, JobState.PendingHeld);
        if ( _completedJobs.TryGetValue( jobId, out job ) )
            return ( job, JobState.Completed );
        if ( _failedJobs.TryGetValue( jobId, out job ) )
            return (job, JobState.Aborted);
        if ( _canceledJobs.TryGetValue( jobId, out job ) )
            return ( job, JobState.Canceled );
        return (null, JobState.Aborted);
    }
    private List<(PrinterJob Job, JobState JobState)> GetJobs( bool returnNotCompleted, bool returnCompleted, string? userName, int limit)
    {
        var list = new List<(PrinterJob Job, JobState JobState)>();
        if ( returnNotCompleted )
            list.AddRange( _createdJobs.Values
                .Where( x => userName == null || x.UserName == userName )
                .Take( limit - list.Count )
                .Select( x => (x, JobState.Pending) ) );
        if( returnNotCompleted && list.Count < limit )
            list.AddRange( _createdJobs.Values
                .Where( x => userName == null || x.UserName == userName )
                .Take( limit - list.Count )
                .Select( x => (x, JobState.Pending) ) );
        if ( returnNotCompleted && list.Count < limit )
            list.AddRange( _suspendedJobs.Values
                .Where( x => userName == null || x.UserName == userName )
                .Take( limit - list.Count )
                .Select( x => (x, JobState.PendingHeld) ) );
        if ( returnCompleted && list.Count < limit )
            list.AddRange( _completedJobs.Values
                .Where( x => userName == null || x.UserName == userName )
                .Take( limit - list.Count )
                .Select( x => (x, JobState.Completed) ) );
        if ( returnCompleted && list.Count < limit )
            list.AddRange( _failedJobs.Values
                .Where( x => userName == null || x.UserName == userName )
                .Take( limit - list.Count )
                .Select( x => (x, JobState.Aborted) ) );
        if ( returnCompleted && list.Count < limit )
            list.AddRange( _canceledJobs.Values
                .Where( x => userName == null || x.UserName == userName )
                .Take( limit - list.Count )
                .Select( x => (x, JobState.Canceled) ) ); ;
        return list;
    }

    private void FillWithDefaultValues(NewJobAttributes attributes)
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
    }

    private void FillWithDefaultValues(DocumentAttributes attributes)
    {
        attributes.DocumentFormat ??= _documentFormatDefault;
    }
}
