using System;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using SharpIpp.Model;
using SharpIpp.Protocol;
using SharpIpp.Tests.Extensions;
using Snapper;
using Snapper.Attributes;

namespace SharpIpp.Tests
{
    [TestFixture]
    [UpdateSnapshots()]
    public class ReadingTests
    {
        [OneTimeSetUp]
        public void TearUp()
        {
            _protocol = new IppProtocol();
        }

        [Test]
        public void CancelJobResponse()
        {
            var ippResponse = ReadIppResponse("CancelJob.bin");
            var response = _protocol.ConstructCancelJobResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void CreateJobResponse()
        {
            var ippResponse = ReadIppResponse("CreateJob.bin");
            var response = _protocol.ConstructCreateJobResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void GetJobAttributesResponse()
        {
            var ippResponse = ReadIppResponse("GetJobAttributes.bin");
            var response = _protocol.ConstructGetJobAttributesResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void GetJobsResponse()
        {
            var ippResponse = ReadIppResponse("GetJobs.bin");
            var response = _protocol.ConstructGetJobsResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void GetJobsResponse_Selected()
        {
            var ippResponse = ReadIppResponse("GetJobs_Selected.bin");
            var response = _protocol.ConstructGetJobsResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void GetPrinterAttributesResponse()
        {
            var ippResponse = ReadIppResponse("GetPrinterAttributes.bin");
            var response = _protocol.ConstructGetPrinterAttributesResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void GetPrinterAttributesResponse_Selected()
        {
            var ippResponse = ReadIppResponse("GetPrinterAttributes_Selected.bin");
            var response = _protocol.ConstructGetPrinterAttributesResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void HoldJobResponse()
        {
            var ippResponse = ReadIppResponse("HoldJob.bin");
            var response = _protocol.ConstructHoldJobResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void PausePrinterResponse()
        {
            var ippResponse = ReadIppResponse("PausePrinter.bin");
            var response = _protocol.ConstructPausePrinterResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void PrintJobResponse()
        {
            var ippResponse = ReadIppResponse("PrintJob.bin");
            var response = _protocol.ConstructPrintJobResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void PrintJobResponse_Unsupported()
        {
            var ippResponse = ReadIppResponse("PrintJob_Unsupported.bin");
            var response = _protocol.ConstructPrintJobResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void PrintUriResponse()
        {
            var ippResponse = ReadIppResponse("PrintUri.bin");
            var response = _protocol.ConstructPrintUriResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void PurgeJobsResponse()
        {
            var ippResponse = ReadIppResponse("PurgeJobs.bin");
            var response = _protocol.ConstructPurgeJobsResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void ReleaseJobResponse()
        {
            var ippResponse = ReadIppResponse("ReleaseJob.bin");
            var response = _protocol.ConstructReleaseJobResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void RestartJobResponse()
        {
            var ippResponse = ReadIppResponse("RestartJob.bin");
            var response = _protocol.ConstructRestartJobResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void ResumePrinterResponse()
        {
            var ippResponse = ReadIppResponse("ResumePrinter.bin");
            var response = _protocol.ConstructResumePrinterResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void SendDocumentResponse()
        {
            var ippResponse = ReadIppResponse("SendDocument.bin");
            var response = _protocol.ConstructSendDocumentResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void SendUriResponse()
        {
            var ippResponse = ReadIppResponse("SendUri.bin");
            var response = _protocol.ConstructSendUriResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void ValidateJobResponse()
        {
            var ippResponse = ReadIppResponse("ValidateJob.bin");
            var response = _protocol.ConstructValidateJobResponse(ippResponse);
            CheckResponse(response);
        }

        [Test]
        public void ValidateJobResponse_Unsupported()
        {
            var ippResponse = ReadIppResponse("ValidateJob_Unsupported.bin");
            var response = _protocol.ConstructValidateJobResponse(ippResponse);
            CheckResponse(response);
        }

        private IppProtocol _protocol;

        public void CheckResponse(IIppResponseMessage response)
        {
            Assert.NotNull(response);
            Test.AddJsonAttachment(response, "response.json");
            Test.AddJsonAttachment(response.Sections, "attributes.json");
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
            SnapperExtensions.ShouldMatchSnapshot(response);
        }

        private IIppResponseMessage ReadIppResponse(string binFileName)
        {
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "ReadingTests", binFileName);
            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            return _protocol.ReadIppResponse(stream);
        }
    }
}