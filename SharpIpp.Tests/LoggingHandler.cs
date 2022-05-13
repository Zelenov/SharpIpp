using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using SharpIpp.Tests.Extensions;

namespace SharpIpp.Tests;

internal class LoggingHandler : DelegatingHandler
{
    public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var fileName = TestContext.CurrentContext.Test.Name;
        var requestFile = fileName + ".request.bin";
        var responseFile = fileName + ".response.bin";
        Test.AddBinaryAttachment(await request.Content.ReadAsByteArrayAsync(), requestFile);
        var response = await base.SendAsync(request, cancellationToken);
        Test.AddBinaryAttachment(await response.Content.ReadAsByteArrayAsync(), responseFile);
        return response;
    }
}
