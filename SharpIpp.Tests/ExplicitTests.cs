using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using SharpIpp.Model;
using Range = SharpIpp.Model.Range;

namespace SharpIpp.Tests
{
    [Explicit]
    public class ExplicitTests
    {
        private readonly Lazy<ExplicitTestsOptions> Options = new Lazy<ExplicitTestsOptions>(() =>
        {
            var options = new ExplicitTestsOptions();
            Configuration.Bind(options);
            return options;
        });

        private static IConfiguration Configuration =>
            new ConfigurationBuilder().SetBasePath(TestContext.CurrentContext.WorkDirectory)
               .AddJsonFile("appsettings.json", false, false)
               .Build();

        private SharpIppClient GetSharpIppClient =>
            new SharpIppClient(new HttpClient(new LoggingHandler(new HttpClientHandler())));

        [Test]
        public async Task PrintJobAsync_Simple()
        {
            using var client = GetSharpIppClient;
            var printer = new Uri(Options.Value.PrinterUrl);
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "EmptyTwoPage.pdf");
            await using var stream = File.Open(file, FileMode.Open);
            var request = new PrintJobRequest {PrinterUri = printer, Document = stream};
            var response = await client.PrintJobAsync(request);
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
            Assert.Greater(response.JobId, 0);
            Assert.AreEqual(request.RequestId, response.RequestId);
        }

        [Test]
        public async Task PrintJobAsync_Full()
        {
            using var client = GetSharpIppClient;
            var printer = new Uri(Options.Value.PrinterUrl);
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "EmptyTwoPage.pdf");
            await using var stream = File.Open(file, FileMode.Open);
            var request = new PrintJobRequest
            {
                PrinterUri = printer,
                Document = stream,
                JobName = "Test Job",
                IppAttributeFidelity = false,
                DocumentName = "Document Name",
                DocumentFormat = "application/octet-stream",
                DocumentNaturalLanguage = "en",
                // JobPriority = 50, //unsupported most of the time
                // JobHoldUntil = JobHoldUntil.NoHold //unsupported most of the time
                MultipleDocumentHandling = MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
                Copies = 1,
                Finishings = Finishings.None,
                PageRanges = new[] {new Range(1, 1)},
                Sides = Sides.OneSided,
                NumberUp = 1,
                OrientationRequested = Orientation.Portrait,
                PrinterResolution = new Resolution(600, 600, ResolutionUnit.DotsPerInch),
                PrintQuality = PrintQuality.Normal
            };
            var response = await client.PrintJobAsync(request);
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
            Assert.Greater(response.JobId, 0);
            Assert.AreEqual(request.RequestId, response.RequestId);
        }

        [Test]
        public async Task GetPrinterAttributesAsync()
        {
            using var client = GetSharpIppClient;
            var printer = new Uri(Options.Value.PrinterUrl);
            var request = new GetPrinterAttributesRequest {PrinterUri = printer};
            var response = await client.GetPrinterAttributesAsync(request);
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
            Assert.AreEqual(request.RequestId, response.RequestId);
        }

        [Test]
        public async Task GetJobAttributesAsync()
        {
            using var client = GetSharpIppClient;
            var printer = new Uri(Options.Value.PrinterUrl);
            var request = new GetJobAttributesRequest {PrinterUri = printer, JobId = Options.Value.JobId};
            var response = await client.GetJobAttributesAsync(request);
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
            Assert.AreEqual(request.RequestId, response.RequestId);
        }
    }
}