using System.Threading;
using System.Threading.Tasks;

using SharpIpp.Models;

namespace SharpIpp
{
    public partial class SharpIppClient
    {
        /// <inheritdoc />
        public Task<CancelJobResponse> CancelJobAsync(CancelJobRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<CancelJobResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<CreateJobResponse> CreateJobAsync(CreateJobRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<CreateJobResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<GetJobAttributesResponse> GetJobAttributesAsync(GetJobAttributesRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<GetJobAttributesResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<GetJobsResponse> GetJobsAsync(GetJobsRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<GetJobsResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<GetPrinterAttributesResponse> GetPrinterAttributesAsync(GetPrinterAttributesRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<GetPrinterAttributesResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<HoldJobResponse> HoldJobAsync(HoldJobRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<HoldJobResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PausePrinterResponse> PausePrinterAsync(PausePrinterRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<PausePrinterResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PrintJobResponse> PrintJobAsync(PrintJobRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<PrintJobResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PrintUriResponse> PrintUriAsync(PrintUriRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<PrintUriResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<PurgeJobsResponse> PurgeJobsAsync(PurgeJobsRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<PurgeJobsResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ReleaseJobResponse> ReleaseJobAsync(ReleaseJobRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<ReleaseJobResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<RestartJobResponse> RestartJobAsync(RestartJobRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<RestartJobResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ResumePrinterResponse> ResumePrinterAsync(ResumePrinterRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<ResumePrinterResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<SendDocumentResponse> SendDocumentAsync(SendDocumentRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<SendDocumentResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<SendUriResponse> SendUriAsync(SendUriRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<SendUriResponse>, cancellationToken);
        }

        /// <inheritdoc />
        public Task<ValidateJobResponse> ValidateJobAsync(ValidateJobRequest request, CancellationToken cancellationToken = default)
        {
            return SendAsync(request, ConstructIppRequest, Construct<ValidateJobResponse>, cancellationToken);
        }
    }
}
