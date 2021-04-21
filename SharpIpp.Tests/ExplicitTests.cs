using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using SharpIpp.Model;

namespace SharpIpp.Tests
{
    [Explicit]
    public class ExplicitTests
    {
        private static IConfiguration Configuration =>
            new ConfigurationBuilder().SetBasePath(TestContext.CurrentContext.WorkDirectory)
               .AddJsonFile("appsettings.json", false, false)
               .Build();

        private SharpIppClient GetSharpIppClient =>
            new SharpIppClient(new HttpClient(new LoggingHandler(new HttpClientHandler())));

        [Test]
        public async Task PrintJobAsync()
        {
            using var client = GetSharpIppClient;
            var printer = new Uri(Configuration["PrinterUrl"]);
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "word-2-pages.pdf");
            await using var stream = File.Open(file, FileMode.Open);
            var request = new PrintJobRequest {PrinterUri = printer, Document = stream};
            var response = await client.PrintJobAsync(request);
            Console.WriteLine(JsonConvert.SerializeObject(response));
            Assert.Greater(response.JobId, 0);
            Assert.AreEqual(request.RequestId, response.RequestId);
        }

        [Test]
        public async Task GetPrinterAttributesAsync()
        {
            using var client = GetSharpIppClient;
            var printer = new Uri(Configuration["PrinterUrl"]);
            var request = new GetPrinterAttributesRequest {PrinterUri = printer};
            var response = await client.GetPrinterAttributesAsync(request);
            Console.WriteLine(JsonConvert.SerializeObject(response));
            Assert.AreEqual(request.RequestId, response.RequestId);
        }

        private class LoggingHandler : DelegatingHandler
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
                await File.WriteAllBytesAsync(requestFile, await request.Content.ReadAsByteArrayAsync(),
                    cancellationToken);
                var response = await base.SendAsync(request, cancellationToken);
                await File.WriteAllBytesAsync(responseFile, await response.Content.ReadAsByteArrayAsync(),
                    cancellationToken);
                return response;
            }
        }
    }
}