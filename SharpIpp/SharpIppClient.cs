using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp.Exceptions;
using SharpIpp.Model;
using SharpIpp.Protocol;

namespace SharpIpp
{
    public class SharpIppClient : ISharpIppClient
    {
        private readonly bool _disposeHttpClient;
        private readonly HttpClient _httpClient;
        private readonly IIppProtocol _ippProtocol = new IppProtocol();

        /// <summary>
        /// Status codes of <see cref="HttpResponseMessage"/> that are not successful,
        /// but response still contains valid ipp-data in the body that can be parsed for better error description
        /// Seems like they are printer specific
        /// </summary>
        public HttpStatusCode[] PlausibleHttpStatusCodes { get; set; } = new[]
        {
            HttpStatusCode.Continue,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.Forbidden,
            HttpStatusCode.UpgradeRequired
        };

        public SharpIppClient() : this(new HttpClient(), true)
        {
        }

        public SharpIppClient(HttpClient httpClient) : this(httpClient, false)
        {
        }

        internal SharpIppClient(HttpClient httpClient, bool disposeHttpClient)
        {
            _httpClient = httpClient;
            _disposeHttpClient = disposeHttpClient;
        }

        public void Dispose()
        {
            if (_disposeHttpClient)
                _httpClient.Dispose();
        }

        /// <summary>
        ///     Print-Job Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.1
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PrintJobResponse> PrintJobAsync(PrintJobRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructPrintJobResponse(ippResponse));
        }

        /// <summary>
        ///     Print-Uri Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.2
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PrintUriResponse> PrintUriAsync(PrintUriRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructPrintUriResponse(ippResponse));
        }

        /// <summary>
        ///     Validate-Job Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.3
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ValidateJobResponse> ValidateJobAsync(ValidateJobRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructValidateJobResponse(ippResponse));
        }

        /// <summary>
        ///     Create-Job Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.4
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CreateJobResponse> CreateJobAsync(CreateJobRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructCreateJobResponse(ippResponse));
        }

        /// <summary>
        ///     Pause-Printer Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.7
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PausePrinterResponse> PausePrinterAsync(PausePrinterRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructPausePrinterResponse(ippResponse));
        }

        /// <summary>
        ///     Resume-Printer Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.8
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResumePrinterResponse> ResumePrinterAsync(ResumePrinterRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructResumePrinterResponse(ippResponse));
        }

        /// <summary>
        ///     Purge-Jobs Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.9
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PurgeJobsResponse> PurgeJobsAsync(PurgeJobsRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructPurgeJobsResponse(ippResponse));
        }

        /// <summary>
        ///     Cancel-Job Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.3
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<CancelJobResponse> CancelJobAsync(CancelJobRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructCancelJobResponse(ippResponse));
        }

        /// <summary>
        ///     Hold-Job Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.5
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HoldJobResponse> HoldJobAsync(HoldJobRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructHoldJobResponse(ippResponse));
        }

        /// <summary>
        ///     Release-Job Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.6
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ReleaseJobResponse> ReleaseJobAsync(ReleaseJobRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructReleaseJobResponse(ippResponse));
        }

        /// <summary>
        ///     Restart-Job Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.7
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RestartJobResponse> RestartJobAsync(RestartJobRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructRestartJobResponse(ippResponse));
        }

        /// <summary>
        ///     Get-Printer-Attributes Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.5
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetPrinterAttributesResponse> GetPrinterAttributesAsync(GetPrinterAttributesRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructGetPrinterAttributesResponse(ippResponse));
        }

        /// <summary>
        ///     Get-Job-Attributes Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.4
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetJobAttributesResponse> GetJobAttributesAsync(GetJobAttributesRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructGetJobAttributesResponse(ippResponse));
        }

        /// <summary>
        ///     Send-Document Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.1
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SendDocumentResponse> SendDocumentAsync(SendDocumentRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructSendDocumentResponse(ippResponse));
        }

        /// <summary>
        ///     Send-URI Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.2
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SendUriResponse> SendUriAsync(SendUriRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructSendUriResponse(ippResponse));
        }

        /// <summary>
        ///     Get-Jobs Operation
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.6
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetJobsResponse> GetJobsAsync(GetJobsRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructGetJobsResponse(ippResponse));
        }

        /// <summary>
        ///     Custom Operation, not defined in the standard
        /// </summary>
        /// <param name="printerUri">printer uri</param>
        /// <param name="request">custom made request</param>
        /// <returns></returns>
        public async Task<IppResponseMessage> CustomOperationAsync(Uri printerUri, IppRequestMessage request)
        {
            return await SendAsync(printerUri, () => request, ippResponse => ippResponse);
        }

        private async Task<T> SendAsync<T>(Uri printer, Func<IppRequestMessage> constructRequestFunc,
            Func<IppResponseMessage, T> constructResponseFunc) where T : class
        {
            var httpPrinter = new UriBuilder(printer) {Scheme = "http", Port = printer.Port}.Uri;
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, httpPrinter);
            HttpResponseMessage? response;
            using (Stream stream = new MemoryStream())
            {
                var ippRequest = constructRequestFunc();
                _ippProtocol.Write(ippRequest, stream);
                stream.Seek(0, SeekOrigin.Begin);
                httpRequest.Content = new StreamContent(stream) {Headers = {{"Content-Type", "application/ipp"}}};
                response = await _httpClient.SendAsync(httpRequest);
            }

            Exception? httpException = null;
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                var plausibleHttpStatusCodes = PlausibleHttpStatusCodes;
                var isPlausibleHttpStatusCode = Array.IndexOf(plausibleHttpStatusCodes, response.StatusCode) >=0;
                if (!isPlausibleHttpStatusCode)
                    throw;

                httpException = ex;
            }

            IppResponseMessage? ippResponse;
            try
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                ippResponse = _ippProtocol.ReadIppResponse(responseStream);
                if (!ippResponse.IsSuccessfulStatusCode)
                    throw new IppResponseException($"Printer returned error code\n{ippResponse}", ippResponse);
            }
            catch
            {
                if (httpException == null)
                    throw;

                throw httpException;
            }

            if (httpException != null)
            {
                if (ippResponse != null)
                    throw new IppResponseException(httpException.Message, httpException, ippResponse);

                throw httpException;
            }

            var res = constructResponseFunc(ippResponse);
            return res;
        }
    }
}