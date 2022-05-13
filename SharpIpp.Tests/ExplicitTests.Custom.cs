using System;
using System.Threading.Tasks;

using NUnit.Framework;

using SharpIpp.Exceptions;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;
using SharpIpp.Tests.Extensions;

namespace SharpIpp.Tests;

public partial class ExplicitTests
{
    [Test]
    public async Task CustomOperationAsync()
    {
        var request = new IppRequestMessage
        {
            RequestId = 1,
            IppOperation = (IppOperation)0x000A,
            Version = IppVersion.V11,
        };

        request.OperationAttributes.AddRange(new[]
        {
            new IppAttribute(Tag.Charset, "attributes-charset", "utf-8"),
            new IppAttribute(Tag.NaturalLanguage, "attributes-natural-language", "en"),
            new IppAttribute(Tag.NameWithoutLanguage, "requesting-user-name", "test"),
            new IppAttribute(Tag.Uri, "printer-uri", "localhost"),
            new IppAttribute(Tag.Integer, "job-id", 0),
        });
        await TestCustomRequestAsync(request, (client, r) => client.SendAsync(Options.Value.PrinterUrl, r));
    }


    private async Task TestCustomRequestAsync<TIn, TOut>(
        TIn request,
        Func<ISharpIppClient, TIn, Task<TOut>> parseFunc) where TOut : IIppResponseMessage where TIn : IIppRequestMessage
    {
        try
        {
            using var client = GetSharpIppClient;
            var res = await parseFunc(client, request);
            Test.AddJsonAttachment(res, "response.json");
            Assert.AreEqual(request.RequestId, res.RequestId);
        }
        catch (IppResponseException ex)
        {
            Test.AddJsonAttachment(ex.ResponseMessage, "response.json");

            if (ex.ResponseMessage.StatusCode == IppStatusCode.ServerErrorOperationNotSupported)
            {
                Assert.Inconclusive("Operation Not Supported");
            }

            if (ex.ResponseMessage.StatusCode == IppStatusCode.ServerErrorNotAcceptingJobs)
            {
                Assert.Inconclusive("Not Accepting Jobs. Check your printer. Printer is printing something, low on ink or paper is stuck");
            }

            if (ex.ResponseMessage.StatusCode == IppStatusCode.ServerErrorMultipleDocumentJobsNotSupported)
            {
                Assert.Inconclusive("Multiple Document Jobs Not Supported");
            }

            throw;
        }
    }
}
