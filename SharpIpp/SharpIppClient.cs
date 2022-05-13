using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using SharpIpp.Exceptions;
using SharpIpp.Mapping;
using SharpIpp.Mapping.Profiles;
using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Extensions;
using SharpIpp.Protocol.Models;

namespace SharpIpp
{
    public partial class SharpIppClient : ISharpIppClient
    {
        private static readonly Lazy<IMapper> MapperSingleton;

        private readonly bool _disposeHttpClient;
        private readonly HttpClient _httpClient;
        private readonly IIppProtocol _ippProtocol = new IppProtocol();

        static SharpIppClient()
        {
            MapperSingleton = new Lazy<IMapper>(MapperFactory);
        }

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

        private IMapper Mapper => MapperSingleton.Value;

        /// <summary>
        ///     Status codes of <see cref="HttpResponseMessage" /> that are not successful,
        ///     but response still contains valid ipp-data in the body that can be parsed for better error description
        ///     Seems like they are printer specific
        /// </summary>
        public HttpStatusCode[] PlausibleHttpStatusCodes { get; set; } =
        {
            HttpStatusCode.Continue,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.Forbidden,
            HttpStatusCode.UpgradeRequired,
        };

        /// <inheritdoc />
        public async Task<IIppResponseMessage> SendAsync(
            Uri printer,
            IIppRequestMessage ippRequest,
            CancellationToken cancellationToken = default)
        {
            var httpPrinter = new UriBuilder(printer) { Scheme = "http", Port = printer.Port }.Uri;
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, httpPrinter);

            HttpResponseMessage? response;

            using (Stream stream = new MemoryStream())
            {
                await _ippProtocol.WriteIppRequestAsync(ippRequest, stream, cancellationToken).ConfigureAwait(false);
                stream.Seek(0, SeekOrigin.Begin);
                httpRequest.Content = new StreamContent(stream) { Headers = { { "Content-Type", "application/ipp" } } };
                response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            Exception? httpException = null;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                var plausibleHttpStatusCodes = PlausibleHttpStatusCodes;
                var isPlausibleHttpStatusCode = Array.IndexOf(plausibleHttpStatusCodes, response.StatusCode) >= 0;

                if (!isPlausibleHttpStatusCode)
                {
                    throw;
                }

                httpException = ex;
            }

            IIppResponseMessage? ippResponse;

            try
            {
                using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                ippResponse = await _ippProtocol.ReadIppResponseAsync(responseStream, cancellationToken).ConfigureAwait(false);

                if (!ippResponse.IsSuccessfulStatusCode())
                {
                    throw new IppResponseException($"Printer returned error code\n{ippResponse}", ippResponse);
                }
            }
            catch
            {
                if (httpException == null)
                {
                    throw;
                }

                throw httpException;
            }

            if (httpException == null)
            {
                return ippResponse;
            }

            if (ippResponse != null)
            {
                throw new IppResponseException(httpException.Message, httpException, ippResponse);
            }

            throw httpException;
        }

        public void Dispose()
        {
            if (_disposeHttpClient)
            {
                _httpClient.Dispose();
            }
        }

        protected async Task<TOut> SendAsync<TIn, TOut>(
            TIn data,
            Func<TIn, IIppRequestMessage> constructRequestFunc,
            Func<IIppResponseMessage, TOut> constructResponseFunc,
            CancellationToken cancellationToken)
            where TIn : IIppRequest
            where TOut : IIppResponseMessage
        {
            var ippRequest = constructRequestFunc(data);
            var ippResponse = await SendAsync(data.PrinterUri, ippRequest, cancellationToken).ConfigureAwait(false);
            var res = constructResponseFunc(ippResponse);
            return res;
        }

        private IppRequestMessage ConstructIppRequest<T>(T request)
        {
            if (request == null)
            {
                throw new ArgumentException($"{nameof(request)}");
            }

            var ippRequest = Mapper.Map<T, IppRequestMessage>(request);
            return ippRequest;
        }

        public T Construct<T>(IIppResponseMessage ippResponse) where T : IIppResponseMessage
        {
            try
            {
                var r = Mapper.Map<T>(ippResponse);
                return r;
            }
            catch (Exception ex)
            {
                throw new IppResponseException("Ipp attributes mapping exception", ex, ippResponse);
            }
        }

        private static IMapper MapperFactory()
        {
            var mapper = new SimpleMapper();
            var assembly = Assembly.GetAssembly(typeof(TypesProfile));
            mapper.FillFromAssembly(assembly!);
            return mapper;
        }
    }
}
