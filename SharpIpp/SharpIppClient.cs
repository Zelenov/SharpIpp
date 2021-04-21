using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SharpIpp.Model;
using SharpIpp.Protocol;

namespace SharpIpp
{
    public class SharpIppClient : IDisposable
    {
        private readonly bool _disposeHttpClient;
        private readonly HttpClient _httpClient;
        private readonly IppProtocol _ippProtocol = new IppProtocol();

        public SharpIppClient() : this(new HttpClient(), true)
        {
        }

        public SharpIppClient(HttpClient httpClient) : this(httpClient, false)
        {
        }

        private SharpIppClient(HttpClient httpClient, bool disposeHttpClient)
        {
            _httpClient = httpClient;
            _disposeHttpClient = disposeHttpClient;
        }

        public void Dispose()
        {
            if (_disposeHttpClient)
                _httpClient?.Dispose();
        }

        /// <summary>
        /// Print-Job Operation
        /// https://tools.ietf.org/html/rfc2911#section-3.2.1
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PrintJobResponse> PrintJobAsync(PrintJobRequest request)
        {
            return await SendAsync(request.PrinterUri, 
                stream => _ippProtocol.Write(request, stream),
                stream => _ippProtocol.ReadPrintJobResponse(stream));
        }
        /// <summary>
        /// Get-Printer-Attributes Operation
        /// https://tools.ietf.org/html/rfc2911#section-3.2.5
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetPrinterAttributesResponse> GetPrinterAttributesAsync(GetPrinterAttributesRequest request)
        {
            return await SendAsync(request.PrinterUri, 
                stream => _ippProtocol.Write(request, stream),
                stream => _ippProtocol.ReadGetPrinterAttributes(stream));
        }

        private async Task<T> SendAsync<T>(Uri printer, Action<Stream> writeAction, Func<Stream, T> readAction)
            where T : class
        {
            var httpPrinter = new UriBuilder(printer) {Scheme = "http", Port = printer.Port}.Uri;
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, httpPrinter);
            using Stream stream = new MemoryStream();
            writeAction(stream);
            stream.Seek(0, SeekOrigin.Begin);
            httpRequest.Content = new StreamContent(stream) {Headers = {{"Content-Type", "application/ipp"}}};
            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync();
            var res = readAction(responseStream);
            return res;
        }
    }
}