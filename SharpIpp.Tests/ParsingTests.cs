using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SharpIpp.Model;
using SharpIpp.Protocol;

namespace SharpIpp.Tests
{
    public class ParsingTests
    {
        [Test]
        public async Task PrintJobResponse()
        {
            var protocol = new IppProtocol();
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "PrintJobResponse.bin");
            await using var stream = File.Open(file, FileMode.Open);
            var printJobResponse = protocol.ReadPrintJobResponse(stream);
            Console.WriteLine(JsonConvert.SerializeObject(printJobResponse, Formatting.Indented));
        }

        [Test]
        public async Task GetPrinterAttributesResponse()
        {
            var protocol = new IppProtocol();
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "GetPrinterAttributes.bin");
            await using var stream = File.Open(file, FileMode.Open);
            var printJobResponse = protocol.ReadGetPrinterAttributes(stream);
            Console.WriteLine(JsonConvert.SerializeObject(printJobResponse, Formatting.Indented));
            Assert.AreEqual(PrinterState.Idle, printJobResponse.PrinterState);
        }

        [Test]
        public async Task GetJobAttributes()
        {
            var protocol = new IppProtocol();
            var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "GetJobAttributes.bin");
            await using var stream = File.Open(file, FileMode.Open);
            var printJobResponse = protocol.ReadGetJobAttributes(stream);
            Console.WriteLine(JsonConvert.SerializeObject(printJobResponse, Formatting.Indented));
            Console.WriteLine(string.Join("\n", printJobResponse.AllAttributes.Values.SelectMany(s => s)));
        }
    }
}