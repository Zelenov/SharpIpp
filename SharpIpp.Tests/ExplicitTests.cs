using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SharpIpp.Exceptions;
using SharpIpp.Model;
using SharpIpp.Tests.Extensions;
using Range = SharpIpp.Model.Range;

namespace SharpIpp.Tests
{
    /// <summary>
    ///     Tests for the real printer.
    ///     add appsettings.json file to the root of this project with your printer settings
    ///     example:
    ///     {
    ///     "PrinterUrl": "ipp://127.0.0.1:631",
    ///     "DocumentUri": "https://github.com/Zelenov/SharpIpp/raw/master/SharpIpp.Tests/Resources/word-2-pages.pdf",
    ///     }
    /// </summary>
    [Explicit]
    [SingleThreaded]
    public class ExplicitTests
    {
        private static readonly Lazy<ExplicitTestsOptions> Options = new Lazy<ExplicitTestsOptions>(() =>
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

        private SharpIppClient GetSharpIppClientWithoutLog => new SharpIppClient();

        [Test]
        public async Task ValidateJobAsync_Simple()
        {
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "word-2-pages.pdf");
            await using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var request = new ValidateJobRequest {PrinterUri = Options.Value.PrinterUrl, Document = stream};
            await TestRequestAsync(request, (client, r) => client.ValidateJobAsync(r));
        }

        [Test]
        public async Task ValidateJobAsync_Full()
        {
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "word-2-pages.pdf");
            await using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var request = new ValidateJobRequest
            {
                PrinterUri = Options.Value.PrinterUrl,
                Document = stream,
                DocumentAttributes = new DocumentAttributes
                {
                    DocumentName = "Document Name",
                    DocumentFormat = "application/octet-stream",
                    DocumentNaturalLanguage = "en"
                },
                NewJobAttributes = new NewJobAttributes
                {
                    JobName = "Test Job",
                    IppAttributeFidelity = false,
                    // JobPriority = 50, //unsupported most of the time
                    // JobHoldUntil = JobHoldUntil.NoHold //unsupported most of the time
                    MultipleDocumentHandling = MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
                    Copies = 1,
                    Finishings = Finishings.None,
                    PageRanges = new[] {new Range(1, 2)},
                    Sides = Sides.OneSided,
                    NumberUp = 1,
                    OrientationRequested = Orientation.Portrait,
                    PrinterResolution = new Resolution(600, 600, ResolutionUnit.DotsPerInch),
                    PrintQuality = PrintQuality.Normal,
                    PrintScaling = PrintScaling.Fit
                }
            };

            await TestRequestAsync(request, (client, r) => client.ValidateJobAsync(r));
        }


        [Test]
        public async Task CreateJobAsync_Simple()
        {
            var request = new CreateJobRequest {PrinterUri = Options.Value.PrinterUrl};
            var response = await TestRequestAsync(request, (client, r) => client.CreateJobAsync(r));
            await CancelJobAsync(response.JobId);
        }


        [Test]
        public async Task CreateJobAsync_Full()
        {
            var request = new CreateJobRequest
            {
                PrinterUri = Options.Value.PrinterUrl,
                NewJobAttributes = new NewJobAttributes
                {
                    JobName = "Test Job",
                    IppAttributeFidelity = false,
                    // JobPriority = 50, //unsupported most of the time
                    // JobHoldUntil = JobHoldUntil.NoHold //unsupported most of the time
                    MultipleDocumentHandling = MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
                    Copies = 1,
                    Finishings = Finishings.None,
                    PageRanges = new[] {new Range(1, 2)},
                    Sides = Sides.OneSided,
                    NumberUp = 1,
                    OrientationRequested = Orientation.Portrait,
                    PrinterResolution = new Resolution(600, 600, ResolutionUnit.DotsPerInch),
                    PrintQuality = PrintQuality.Normal,
                    PrintScaling = PrintScaling.Fit
                }
            };
            var response = await TestRequestAsync(request, (client, r) => client.CreateJobAsync(r));
            await CancelJobAsync(response.JobId);
        }

        [Test]
        public async Task PrintJobAsync_Simple()
        {
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "word-2-pages.pdf");
            await using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var request = new PrintJobRequest {PrinterUri = Options.Value.PrinterUrl, Document = stream};
            await TestRequestAsync(request, (client, r) => client.PrintJobAsync(r));
        }

        [Test]
        public async Task PrintJobAsync_Full()
        {
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "EmptyTwoPage.pdf");
            await using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var request = new PrintJobRequest
            {
                PrinterUri = Options.Value.PrinterUrl,
                Document = stream,
                DocumentAttributes = new DocumentAttributes
                {
                    DocumentName = "Document Name",
                    DocumentFormat = "application/octet-stream",
                    DocumentNaturalLanguage = "en"
                },
                NewJobAttributes = new NewJobAttributes
                {
                    JobName = "Test Job",
                    IppAttributeFidelity = false,
                    // JobPriority = 50, //unsupported most of the time
                    // JobHoldUntil = JobHoldUntil.NoHold //unsupported most of the time
                    MultipleDocumentHandling = MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
                    Copies = 1,
                    Finishings = Finishings.None,
                    PageRanges = new[] {new Range(1, 2)},
                    Sides = Sides.OneSided,
                    NumberUp = 1,
                    OrientationRequested = Orientation.Portrait,
                    PrinterResolution = new Resolution(600, 600, ResolutionUnit.DotsPerInch),
                    PrintQuality = PrintQuality.Normal,
                    PrintScaling = PrintScaling.Fit
                }
            };
            await TestRequestAsync(request, (client, r) => client.PrintJobAsync(r));
        }

        [Test]
        public async Task PrintUriAsync_Simple()
        {
            var request = new PrintUriRequest
            {
                PrinterUri = Options.Value.PrinterUrl, DocumentUri = Options.Value.DocumentUri
            };
            await TestRequestAsync(request, (client, r) => client.PrintUriAsync(r));
        }

        [Test]
        public async Task PrintUriAsync_Full()
        {
            var request = new PrintUriRequest
            {
                PrinterUri = Options.Value.PrinterUrl,
                DocumentUri = Options.Value.DocumentUri,
                DocumentAttributes = new DocumentAttributes
                {
                    DocumentName = "Document Name",
                    DocumentFormat = "application/octet-stream",
                    DocumentNaturalLanguage = "en"
                },
                NewJobAttributes = new NewJobAttributes
                {
                    JobName = "Test Job",
                    IppAttributeFidelity = false,
                    // JobPriority = 50, //unsupported most of the time
                    // JobHoldUntil = JobHoldUntil.NoHold //unsupported most of the time
                    MultipleDocumentHandling = MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
                    Copies = 1,
                    Finishings = Finishings.None,
                    PageRanges = new[] {new Range(1, 2)},
                    Sides = Sides.OneSided,
                    NumberUp = 1,
                    OrientationRequested = Orientation.Portrait,
                    PrinterResolution = new Resolution(600, 600, ResolutionUnit.DotsPerInch),
                    PrintQuality = PrintQuality.Normal,
                    PrintScaling = PrintScaling.Fit
                }
            };
            await TestRequestAsync(request, (client, r) => client.PrintUriAsync(r));
        }

        [Test]
        public async Task GetPrinterAttributesAsync_Full()
        {
            var request = new GetPrinterAttributesRequest
            {
                PrinterUri = Options.Value.PrinterUrl, RequestedAttributes = new[] {PrinterAttribute.PagesPerMinute}
            };
            await TestRequestAsync(request, (client, r) => client.GetPrinterAttributesAsync(r));
        }

        [Test]
        public async Task GetPrinterAttributesAsync_Simple()
        {
            var request = new GetPrinterAttributesRequest {PrinterUri = Options.Value.PrinterUrl};
            await TestRequestAsync(request, (client, r) => client.GetPrinterAttributesAsync(r));
        }

        [Test]
        public async Task GetJobAttributesAsync()
        {
            await TestJobRequestAsync(async jobId =>
            {
                var request = new GetJobAttributesRequest {PrinterUri = Options.Value.PrinterUrl, JobId = jobId};
                return await TestRequestAsync(request, (client, r) => client.GetJobAttributesAsync(r));
            });
        }

        [Test]
        public async Task GetJobsAsync_Default()
        {
            var request = new GetJobsRequest {PrinterUri = Options.Value.PrinterUrl, WhichJobs = WhichJobs.Completed};
            await TestRequestAsync(request, (client, r) => client.GetJobsAsync(r));
        }

        [Test]
        public async Task GetJobsAsync_Full()
        {
            var request = new GetJobsRequest
            {
                PrinterUri = Options.Value.PrinterUrl,
                WhichJobs = WhichJobs.Completed,
                RequestedAttributes = new[]
                {
                    JobAttribute.Compression,
                    JobAttribute.Copies,
                    JobAttribute.DateTimeAtCompleted,
                    JobAttribute.DateTimeAtCreation,
                    JobAttribute.DateTimeAtProcessing,
                    JobAttribute.DocumentFormat,
                    JobAttribute.DocumentName,
                    JobAttribute.Finishings,
                    JobAttribute.IppAttributeFidelity,
                    JobAttribute.JobId,
                    JobAttribute.JobImpressions,
                    JobAttribute.JobImpressionsCompleted,
                    JobAttribute.JobKOctetsProcessed,
                    JobAttribute.JobMediaSheets,
                    JobAttribute.JobMediaSheetsCompleted,
                    JobAttribute.JobName,
                    JobAttribute.JobOriginatingUserName,
                    JobAttribute.JobOriginatingUserName,
                    JobAttribute.JobPrinterUpTime,
                    JobAttribute.JobPrinterUri,
                    JobAttribute.JobSheets,
                    JobAttribute.JobState,
                    JobAttribute.JobStateMessage,
                    JobAttribute.JobStateReasons,
                    JobAttribute.Media,
                    JobAttribute.MultipleDocumentHandling,
                    JobAttribute.NumberUp,
                    JobAttribute.OrientationRequested,
                    JobAttribute.PrinterResolution,
                    JobAttribute.PrintQuality,
                    JobAttribute.Sides,
                    JobAttribute.TimeAtCompleted,
                    JobAttribute.TimeAtCreation,
                    JobAttribute.TimeAtProcessing
                }
            };
            await TestRequestAsync(request, (client, r) => client.GetJobsAsync(r));
        }

        [Test]
        public async Task PausePrinterAsync()
        {
            try
            {
                var request = new PausePrinterRequest {PrinterUri = Options.Value.PrinterUrl};
                await TestRequestAsync(request, (client, r) => client.PausePrinterAsync(r));
            }
            finally
            {
                var request = new ResumePrinterRequest {PrinterUri = Options.Value.PrinterUrl};
                using var client = GetSharpIppClientWithoutLog;
                await client.ResumePrinterAsync(request);
            }
        }

        [Test]
        public async Task ResumePrinterAsync()
        {
            var request = new ResumePrinterRequest {PrinterUri = Options.Value.PrinterUrl};
            await TestRequestAsync(request, (client, r) => client.ResumePrinterAsync(r));
        }

        [Test]
        public async Task PurgeJobsAsync()
        {
            var request = new PurgeJobsRequest {PrinterUri = Options.Value.PrinterUrl};
            await TestRequestAsync(request, (client, r) => client.PurgeJobsAsync(r));
        }

        [Test]
        public async Task CancelJobAsync()
        {
            await TestJobRequestAsync(async jobId =>
            {
                var request = new CancelJobRequest {PrinterUri = Options.Value.PrinterUrl, JobId = jobId};
                return await TestRequestAsync(request, (client, r) => client.CancelJobAsync(r));
            });
        }

        [Test]
        public async Task HoldJobAsync()
        {
            await TestJobRequestAsync(async jobId =>
            {
                var request = new HoldJobRequest {PrinterUri = Options.Value.PrinterUrl, JobId = jobId};
                return await TestRequestAsync(request, (client, r) => client.HoldJobAsync(r));
            });
        }

        [Test]
        public async Task ReleaseJobAsync()
        {
            await TestJobRequestAsync(async jobId =>
            {
                var request = new ReleaseJobRequest {PrinterUri = Options.Value.PrinterUrl, JobId = jobId};
                return await TestRequestAsync(request, (client, r) => client.ReleaseJobAsync(r));
            });
        }

        [Test]
        public async Task RestartJobAsync()
        {
            await TestJobRequestAsync(async jobId =>
            {
                var request = new RestartJobRequest {PrinterUri = Options.Value.PrinterUrl, JobId = jobId};
                return await TestRequestAsync(request, (client, r) => client.RestartJobAsync(r));
            });
        }

        [Test]
        public async Task SendUriAsync_Full()
        {
            await TestJobRequestAsync(async jobId =>
            {
                var request = new SendUriRequest
                {
                    PrinterUri = Options.Value.PrinterUrl,
                    JobId = jobId,
                    DocumentUri = Options.Value.DocumentUri,
                    DocumentAttributes = new DocumentAttributes
                    {
                        DocumentName = "Document Name",
                        DocumentFormat = "application/octet-stream",
                        DocumentNaturalLanguage = "en"
                    }
                };

                return await TestJobRequestAsync(request, (client, r) => client.SendUriAsync(r));
            });
        }

        [Test]
        public async Task SendDocumentAsync_Full()
        {
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "word-2-pages.pdf");
            await using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            await TestJobRequestAsync(async jobId =>
            {
                var request = new SendDocumentRequest
                {
                    PrinterUri = Options.Value.PrinterUrl,
                    JobId = jobId,
                    Document = fileStream,
                    DocumentAttributes = new DocumentAttributes
                    {
                        DocumentName = "Document Name",
                        DocumentFormat = "application/octet-stream",
                        DocumentNaturalLanguage = "en"
                    }
                };

                return await TestJobRequestAsync(request, (client, r) => client.SendDocumentAsync(r));
            });
        }

        [Test]
        public async Task SendDocumentAsync_Simple()
        {
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "word-2-pages.pdf");
            await using var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            await TestJobRequestAsync(async jobId =>
            {
                var request = new SendDocumentRequest
                {
                    PrinterUri = Options.Value.PrinterUrl, JobId = jobId, Document = fileStream
                };

                return await TestJobRequestAsync(request, (client, r) => client.SendDocumentAsync(r));
            });
        }

        [Test]
        public async Task SendDocumentAsync_Multiple()
        {
            var file1 = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "word-2-pages.pdf");
            await using var fileStream1 = new FileStream(file1, FileMode.Open, FileAccess.Read);

            var file2 = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "word-2-pages.pdf");
            await TestJobRequestAsync(async jobId =>
            {
                await using var fileStream2 = new FileStream(file2, FileMode.Open, FileAccess.Read);
                var request1 = new SendDocumentRequest
                {
                    PrinterUri = Options.Value.PrinterUrl, JobId = jobId, Document = fileStream1
                };

                var request2 = new SendDocumentRequest
                {
                    PrinterUri = Options.Value.PrinterUrl, JobId = jobId, Document = fileStream2
                };

                await TestRequestAsync(request1, (client, r) =>
                {
                    r.JobId = jobId;
                    return client.SendDocumentAsync(r);
                });
                return await TestRequestAsync(request2, (client, r) =>
                {
                    r.JobId = jobId;
                    return client.SendDocumentAsync(r);
                });
            });
        }

        [Test]
        public async Task SendDocumentAsync_LastDocument()
        {
            await TestJobRequestAsync(async jobId =>
            {
                var request = new SendDocumentRequest
                {
                    PrinterUri = Options.Value.PrinterUrl, JobId = jobId, LastDocument = true
                };
                return await TestJobRequestAsync(request, (client, r) => client.SendDocumentAsync(r));
            });
        }

        private async Task<TOut> TestRequestAsync<TIn, TOut>(TIn request,
            Func<SharpIppClient, TIn, Task<TOut>> parseFunc) where TOut : IIppResponseMessage where TIn : IIppRequest
        {
            try
            {
                using var client = GetSharpIppClient;
                var res = await parseFunc(client, request);
                Test.AddJsonAttachment(res, "response.json");
                Assert.AreEqual(request.RequestId, res.RequestId);
                return res;
            }
            catch (IppResponseException ex)
            {
                Test.AddJsonAttachment(ex.ResponseMessage, "response.json");
                if (ex.ResponseMessage.StatusCode == IppStatusCode.ServerErrorOperationNotSupported)
                    Assert.Inconclusive("Operation Not Supported");
                if (ex.ResponseMessage.StatusCode == IppStatusCode.ServerErrorNotAcceptingJobs)
                    Assert.Inconclusive(
                        "Not Accepting Jobs. Check your printer. Printer is printing something, low on ink or paper is stuck");
                if (ex.ResponseMessage.StatusCode == IppStatusCode.ServerErrorMultipleDocumentJobsNotSupported)
                    Assert.Inconclusive("Multiple Document Jobs Not Supported");
                throw;
            }
        }

        private async Task<TOut> TestJobRequestAsync<TIn, TOut>(TIn request,
            Func<SharpIppClient, TIn, Task<TOut>> parseFunc) where TOut : IIppResponseMessage where TIn : IIppJobRequest
        {
            return await TestJobRequestAsync(jobId => TestRequestAsync(request, (client, r) =>
            {
                r.JobId = jobId;
                return parseFunc(client, r);
            }));
        }

        private async Task<TOut> TestJobRequestAsync<TOut>(Func<int, Task<TOut>> runFunc)
            where TOut : IIppResponseMessage
        {
            var jobId = await CreateJobAsync();
            try
            {
                var res = await runFunc(jobId);
                return res;
            }
            finally
            {
                await CancelJobAsync(jobId);
            }
        }

        private async Task CancelJobAsync(int jobId)
        {
            var request = new CancelJobRequest {PrinterUri = Options.Value.PrinterUrl, JobId = jobId};
            using var client = GetSharpIppClientWithoutLog;
            await client.CancelJobAsync(request);
        }

        private async Task<int> CreateJobAsync()
        {
            var request = new CreateJobRequest {PrinterUri = Options.Value.PrinterUrl};
            using var client = GetSharpIppClientWithoutLog;
            var response = await client.CreateJobAsync(request);
            return response.JobId;
        }
    }
}