using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SharpIpp.Tests
{
    internal class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "output");
            var fileName = TestContext.CurrentContext.Test.Name;
            var requestFile = Path.Combine(filePath, fileName + ".request.bin");
            var responseFile = Path.Combine(filePath, fileName + ".response.bin");
            Directory.CreateDirectory(filePath);
            await File.WriteAllBytesAsync(requestFile, await request.Content.ReadAsByteArrayAsync(), cancellationToken);
            var response = await base.SendAsync(request, cancellationToken);
            await File.WriteAllBytesAsync(responseFile, await response.Content.ReadAsByteArrayAsync(),
                cancellationToken);
            return response;
        }
    }
}