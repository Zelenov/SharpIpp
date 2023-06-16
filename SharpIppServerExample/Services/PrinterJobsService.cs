using SharpIpp;
using System.Diagnostics;
using SharpIpp.Protocol.Models;
using System.Collections.Concurrent;
using System.Threading;
using SharpIpp.Protocol;
using SharpIppServerExample.Models;
using Quartz.Util;
using System.Diagnostics.Metrics;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Http;
using static Quartz.Logging.OperationName;
using SharpIpp.Models;

namespace SharpIppServerExample.Services;

public class PrinterJobsService
{
    private int _newJobIndex = 0;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SharpIppServer _ippServer;
    private bool _isPaused;
    private readonly ConcurrentDictionary<int, PrinterJob> _createdJobs = new();
    private readonly ConcurrentDictionary<int, PrinterJob> _pendingJobs = new();
    private readonly ConcurrentDictionary<int, PrinterJob> _failedJobs = new();
    private readonly ConcurrentDictionary<int, PrinterJob> _suspendedJobs = new();
    private readonly ConcurrentQueue<int> _completedJobs = new();

    public PrinterJobsService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _ippServer = new SharpIppServer();
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
            Debug.WriteLine( request.GetType().FullName );
            IIppResponseMessage response = request switch
            {
                CancelJobRequest x => GetCancelJobResponse( x ),
                CreateJobRequest x => GetCreateJobResponse( x ),
                CUPSGetPrintersRequest x => GetCUPSGetPrintersResponse( x ),
                GetJobAttributesRequest x => GetGetJobAttributesResponse(x),
                GetJobsRequest x => GetGetJobsResponse( x ),
                GetPrinterAttributesRequest x => GetGetPrinterAttributesResponse( x ),
                HoldJobRequest x => GetHoldJobResponse(x),
                PausePrinterRequest x => GetPausePrinterResponse( x ),
                PrintJobRequest x => GetPrintJobResponse( x ),
                PrintUriRequest x => GetPrintUriResponse( x ),
                PurgeJobsRequest x => GetPurgeJobsResponse( x ),
                ReleaseJobRequest x => GetReleaseJobResponse( x ),
                RestartJobRequest x => GetRestartJobResponse( x ),
                ResumePrinterRequest x => GetResumePrinterResponse( x ),
                SendDocumentRequest x => GetSendDocumentResponse( x ),
                SendUriRequest x => GetSendUriResponse( x ),
                _ => throw new NotImplementedException()
            };
            await _ippServer.SendResponseAsync( response, outputStream );
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( ex );
        }
    }

    private SendUriResponse GetSendUriResponse( SendUriRequest request )
    {
        var isSuccess = false;
        if ( request.JobId.HasValue && _createdJobs.TryRemove( request.JobId.Value, out var job ) )
        {
            job.Requests.Add( request );
            _pendingJobs.TryAdd( job.Id, job );
            isSuccess = true;
        }
        else if ( request.JobId.HasValue && _pendingJobs.TryRemove( request.JobId.Value, out job ) )
        {
            job.Requests.Add( request );
            _pendingJobs.TryAdd( job.Id, job );
            isSuccess = true;
        }
        return new SendUriResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = isSuccess ? IppStatusCode.SuccessfulOk : IppStatusCode.ClientErrorNotPossible,
            JobId = request.JobId ?? 0,
            JobUri = $"{GetPrinterUrl()}/{request.JobId ?? 0}"
        };
    }

    private SendDocumentResponse GetSendDocumentResponse( SendDocumentRequest request )
    {
        var isSuccess = false;
        JobState jobState = JobState.Processing;
        if ( request.JobId.HasValue && _createdJobs.TryRemove( request.JobId.Value, out var job ) )
        {
            job.Requests.Add( request );
            _pendingJobs.TryAdd( job.Id, job );
            isSuccess = true;
        }
        else if ( request.JobId.HasValue && _pendingJobs.TryRemove( request.JobId.Value, out job ) )
        {
            job.Requests.Add( request );
            _pendingJobs.TryAdd( job.Id, job );
            isSuccess = true;
        }
        return new SendDocumentResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = isSuccess ? IppStatusCode.SuccessfulOk : IppStatusCode.ClientErrorNotPossible,
            JobId = request.JobId ?? 0,
            JobUri = $"{GetPrinterUrl()}/{request.JobId ?? 0}",
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
        var isRestarted = false;
        if(request.JobId.HasValue && _failedJobs.TryRemove( request.JobId.Value, out var job ) )
        {
            _pendingJobs.TryAdd( job.Id, job );
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
        var isReleased = false;
        if ( request.JobId.HasValue && _suspendedJobs.TryRemove( request.JobId.Value, out var job ) )
        {
            _pendingJobs.TryAdd( request.JobId.Value, job );
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
        var job = new PrinterJob( GetNextValue() );
        job.Requests.Add( request );
        _createdJobs.TryAdd( job.Id, job );
        return new PrintUriResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            JobId = job.Id,
            JobState = JobState.Pending,
            StatusCode = IppStatusCode.SuccessfulOk,
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
        var isHeld = request.JobId.HasValue && _pendingJobs.TryRemove( request.JobId.Value, out var job );
        return new HoldJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = isHeld ? IppStatusCode.SuccessfulOk : IppStatusCode.ClientErrorNotPossible
        };
    }

    private GetPrinterAttributesResponse GetGetPrinterAttributesResponse( GetPrinterAttributesRequest request )
    {
        return new GetPrinterAttributesResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.SuccessfulOk,
            PrinterState = PrinterState.Idle,
            PrinterStateMessage = "idle",
            PrinterStateReasons = new string[] { "idle" },
            PrintScalingDefault = PrintScaling.Auto,
            PrintScalingSupported = Enum.GetValues( typeof( PrintScaling ) ).Cast<PrintScaling>().ToArray(),
            CharsetConfigured = "utf-8",
            CharsetSupported = new string[] { "utf-8" },
            NaturalLanguageConfigured = "en",
            GeneratedNaturalLanguageSupported = new string[] { "en" },
            PrinterIsAcceptingJobs = true,
            PrinterMakeAndModel = "SharpIpp",
            PrinterMoreInfo = "SharpIpp",
            PrinterName = "SharpIpp",
            PrinterInfo = "SharpIpp example",
            IppVersionsSupported = new string[] { "1.1" },
            DocumentFormatDefault = "application/pdf",
            ColorSupported = true,
            PrinterCurrentTime = DateTimeOffset.Now,
            OperationsSupported = Enum.GetValues( typeof( IppOperation ) ).Cast<IppOperation>().ToArray(),
            QueuedJobCount = _pendingJobs.Count
        };
    }

    private GetJobsResponse GetGetJobsResponse( GetJobsRequest request )
    {
        return new GetJobsResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.SuccessfulOk
        };
    }

    private GetJobAttributesResponse GetGetJobAttributesResponse( GetJobAttributesRequest request )
    {
        return new GetJobAttributesResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = IppStatusCode.SuccessfulOk
        };
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
        var job = new PrinterJob( GetNextValue() );
        _createdJobs.TryAdd( job.Id, job );
        return new CreateJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            JobId = job.Id,
            JobState = JobState.Pending,
            StatusCode = IppStatusCode.SuccessfulOk,
            JobUri = $"{GetPrinterUrl()}/{job.Id}"
        };
    }

    private CancelJobResponse GetCancelJobResponse( CancelJobRequest request )
    {
        var jobId = request.JobId;
        var isDeleted = jobId.HasValue
            && ( !_createdJobs.TryRemove( jobId.Value, out _ )
            || !_pendingJobs.TryRemove( jobId.Value, out _ )
            || !_suspendedJobs.TryRemove( jobId.Value, out _ ) );
        return new CancelJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            StatusCode = isDeleted ? IppStatusCode.SuccessfulOk : IppStatusCode.ClientErrorNotPossible
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
                return Task.FromResult<PrinterJob?>( job );
        }
    }

    public Task AddCompletedJob(int jobId )
    {
        _completedJobs.Enqueue( jobId );
        return Task.CompletedTask;
    }

    private PrintJobResponse GetPrintJobResponse( PrintJobRequest request )
    {
        var job = new PrinterJob( GetNextValue() );
        job.Requests.Add( request );
        _createdJobs.TryAdd( job.Id, job );
        return new PrintJobResponse
        {
            RequestId = request.RequestId,
            Version = request.Version,
            JobId = job.Id,
            JobState = JobState.Pending,
            StatusCode = IppStatusCode.SuccessfulOk,
            JobUri = $"{GetPrinterUrl()}/{job.Id}"
        };
    }

    private string GetPrinterUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request ?? throw new Exception( "Unable to access HttpContext" );
        return $"ipp://{request.Host}{request.PathBase}";
    }
}
