using System;
using System.Threading.Tasks;
using SharpIpp.Model;

namespace SharpIpp
{
    public interface ISharpIppClient : IDisposable
    {
        Task<PrintJobResponse> PrintJobAsync(PrintJobRequest request);
        Task<PrintUriResponse> PrintUriAsync(PrintUriRequest request);
        Task<ValidateJobResponse> ValidateJobAsync(ValidateJobRequest request);
        Task<CreateJobResponse> CreateJobAsync(CreateJobRequest request);
        Task<GetPrinterAttributesResponse> GetPrinterAttributesAsync(GetPrinterAttributesRequest request);
        Task<GetJobAttributesResponse> GetJobAttributesAsync(GetJobAttributesRequest request);
        Task<PausePrinterResponse> PausePrinterAsync(PausePrinterRequest request);
        Task<ResumePrinterResponse> ResumePrinterAsync(ResumePrinterRequest request);
        Task<PurgeJobsResponse> PurgeJobsAsync(PurgeJobsRequest request);
        Task<CancelJobResponse> CancelJobAsync(CancelJobRequest request);
        Task<HoldJobResponse> HoldJobAsync(HoldJobRequest request);
        Task<ReleaseJobResponse> ReleaseJobAsync(ReleaseJobRequest request);
        Task<RestartJobResponse> RestartJobAsync(RestartJobRequest request);
        Task<SendDocumentResponse> SendDocumentAsync(SendDocumentRequest request);
        Task<SendUriResponse> SendUriAsync(SendUriRequest request);
        Task<GetJobsResponse> GetJobsAsync(GetJobsRequest request);
        Task<IppResponseMessage> CustomOperationAsync(Uri printerUri, IppRequestMessage request);
    }
}