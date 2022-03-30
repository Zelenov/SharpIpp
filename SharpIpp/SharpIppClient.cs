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
        /// This REQUIRED operation allows a client to submit a print job with
        /// only one document and supply the document data (rather than just a
        /// reference to the data).  
        /// Print-Job Operation
        /// https://tools.ietf.org/html/rfc2911#section-3.2.1
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PrintJobResponse> PrintJobAsync(PrintJobRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructPrintJobResponse(ippResponse));
        }

        /// <summary>
        /// This OPTIONAL operation is identical to the Print-Job operation
        /// <see cref="PrintJobAsync"/> except that a client supplies a URI reference to the
        /// document data using the "document-uri" (uri) operation attribute
        /// rather than including the document data itself.  Before
        /// returning the response, the Printer MUST validate that the Printer
        /// supports the retrieval method (e.g., http, ftp, etc.) implied by the
        /// URI, and MUST check for valid URI syntax.  If the client-supplied URI
        /// scheme is not supported, i.e. the value is not in the Printer
        /// object's "referenced-uri-scheme-supported" attribute, the Printer
        /// object MUST reject the request and return the 'client-error-uri-
        /// scheme-not-supported' status code.
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
        /// This REQUIRED operation is similar to the Print-Job operation
        /// <see cref="PrintJobAsync"/> except that a client supplies no document data and
        /// the Printer allocates no resources (i.e., it does not create a new
        /// Job object).  This operation is used only to verify capabilities of a
        /// printer object against whatever attributes are supplied by the client
        /// in the Validate-Job request.  By using the Validate-Job operation a
        /// client can validate that an identical Print-Job operation (with the
        /// document data) would be accepted. The Validate-Job operation also
        /// performs the same security negotiation as the Print-Job operation,
        /// so that a client can check that the client and
        /// Printer object security requirements can be met before performing a
        /// Print-Job operation.
        /// The Validate-Job operation does not accept a "document-uri" attribute
        /// in order to allow a client to check that the same Print-URI operation
        /// will be accepted, since the client doesn't send the data with the
        /// Print-URI operation.  The client SHOULD just issue the Print-URI
        /// request.
        /// The Printer object returns the same status codes, Operation
        /// Attributes (Group 1) and Unsupported Attributes (Group 2) as the
        /// Print-Job operation.  However, no Job Object Attributes (Group 3) are
        /// returned, since no Job object is created.
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
        /// This OPTIONAL operation is similar to the Print-Job operation
        /// (<see cref="PrintJobAsync"/>) except that in the Create-Job request, a client does
        /// not supply document data or any reference to document data.  Also,
        /// the client does not supply any of the "document-name", "document-
        /// format", "compression", or "document-natural-language" operation
        /// attributes.  This operation is followed by one or more Send-Document
        /// or Send-URI operations.  In each of those operation requests, the
        /// client OPTIONALLY supplies the "document-name", "document-format",
        /// and "document-natural-language" attributes for each document in the
        /// multi-document Job object.
        /// If a Printer object supports the Create-Job operation, it MUST also
        /// support the Send-Document operation and also MAY support the Send-URI
        /// operation.
        /// If the Printer object supports this operation, it MUST support the
        /// "multiple-operation-time-out" Printer attribute (<see cref="NewJobAttributes."/>).
        /// If the Printer object supports this operation, then it MUST support
        /// the "multiple-document-jobs-supported" Printer Description attribute
        /// (see section 4.4.16) and indicate whether or not it supports
        /// multiple-document jobs.
        /// If the Printer object supports this operation and supports multiple
        /// documents in a job, then it MUST support the "multiple-document-
        /// handling" Job Template job attribute with at least one value (see
        /// section 4.2.4) and the associated "multiple-document-handling-
        /// default" and "multiple-document-handling-supported" Job Template
        /// Printer attributes (<see cref="JobAttributes.MultipleDocumentHandling"/>).
        /// After the Create-Job operation has completed, the value of the "job-
        /// state" attribute is similar to the "job-state" after a Print-Job,
        /// even though no document-data has arrived.  A Printer MAY set the
        /// 'job-data-insufficient' value of the job's "job-state-reason"
        /// attribute to indicate that processing cannot begin until sufficient
        /// data has arrived and set the "job-state" to either 'pending' or
        /// 'pending-held'.  A non-spooling printer that doesn't implement the
        /// 'pending' job state may even set the "job-state" to 'processing',
        /// even though there is not yet any data to process. 
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

        public async Task<GetCUPSPrintersResponse> GetCUPSPrintersAsync(GetCUPSPrintersRequest request)
        {
            return await SendAsync(request.PrinterUri, () => _ippProtocol.Construct(request),
                ippResponse => _ippProtocol.ConstructGetCUPSPrintersResponse(ippResponse));
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

        protected async Task<T> SendAsync<T>(Uri printer, Func<IppRequestMessage> constructRequestFunc,
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