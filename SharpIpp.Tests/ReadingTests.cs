using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Newtonsoft.Json;

using NUnit.Framework;

using SharpIpp.Mapping;
using SharpIpp.Mapping.Profiles;
using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Tests.Extensions;

using Snapper;

namespace SharpIpp.Tests;

[TestFixture]
//[Snapper.Attributes.UpdateSnapshots]
public class ReadingTests
{
    private IMapper _mapper;

    private IppProtocol _protocol;
    [OneTimeSetUp]
    public void TearUp()
    {
        _protocol = new IppProtocol();
        _mapper = new SimpleMapper();
        var assembly = Assembly.GetAssembly(typeof(TypesProfile));
        _mapper.FillFromAssembly(assembly!);
    }

    [Test]
    public async Task CancelJobResponse()
    {
        var ippResponse = await ReadIppResponse("CancelJob.bin");
        var response = _mapper.Map<CancelJobResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task CreateJobResponse()
    {
        var ippResponse = await ReadIppResponse("CreateJob.bin");
        var response = _mapper.Map<CreateJobResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task GetJobAttributesResponse()
    {
        var ippResponse = await ReadIppResponse("GetJobAttributes.bin");
        var response = _mapper.Map<GetJobAttributesResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task GetJobsResponse()
    {
        var ippResponse = await ReadIppResponse("GetJobs.bin");
        var response = _mapper.Map<GetJobsResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task GetJobsResponse_Selected()
    {
        var ippResponse = await ReadIppResponse("GetJobs_Selected.bin");
        var response = _mapper.Map<GetJobsResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task GetPrinterAttributesResponse()
    {
        var ippResponse = await ReadIppResponse("GetPrinterAttributes.bin");
        var response = _mapper.Map<GetPrinterAttributesResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task GetPrinterAttributesResponse_Selected()
    {
        var ippResponse = await ReadIppResponse("GetPrinterAttributes_Selected.bin");
        var response = _mapper.Map<GetPrinterAttributesResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task HoldJobResponse()
    {
        var ippResponse = await ReadIppResponse("HoldJob.bin");
        var response = _mapper.Map<HoldJobResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task PausePrinterResponse()
    {
        var ippResponse = await ReadIppResponse("PausePrinter.bin");
        var response = _mapper.Map<PausePrinterResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task PrintJobResponse()
    {
        var ippResponse = await ReadIppResponse("PrintJob.bin");
        var response = _mapper.Map<PrintJobResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task PrintJobResponse_Unsupported()
    {
        var ippResponse = await ReadIppResponse("PrintJob_Unsupported.bin");
        var response = _mapper.Map<PrintJobResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task PrintUriResponse()
    {
        var ippResponse = await ReadIppResponse("PrintUri.bin");
        var response = _mapper.Map<PrintUriResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task PurgeJobsResponse()
    {
        var ippResponse = await ReadIppResponse("PurgeJobs.bin");
        var response = _mapper.Map<PurgeJobsResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task ReleaseJobResponse()
    {
        var ippResponse = await ReadIppResponse("ReleaseJob.bin");
        var response = _mapper.Map<ReleaseJobResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task RestartJobResponse()
    {
        var ippResponse = await ReadIppResponse("RestartJob.bin");
        var response = _mapper.Map<RestartJobResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task ResumePrinterResponse()
    {
        var ippResponse = await ReadIppResponse("ResumePrinter.bin");
        var response = _mapper.Map<ResumePrinterResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task SendDocumentResponse()
    {
        var ippResponse = await ReadIppResponse("SendDocument.bin");
        var response = _mapper.Map<SendDocumentResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task SendUriResponse()
    {
        var ippResponse = await ReadIppResponse("SendUri.bin");
        var response = _mapper.Map<SendUriResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task ValidateJobResponse()
    {
        var ippResponse = await ReadIppResponse("ValidateJob.bin");
        var response = _mapper.Map<ValidateJobResponse>(ippResponse);
        CheckResponse(response);
    }

    [Test]
    public async Task ValidateJobResponse_Unsupported()
    {
        var ippResponse = await ReadIppResponse("ValidateJob_Unsupported.bin");
        var response = _mapper.Map<ValidateJobResponse>(ippResponse);
        CheckResponse(response);
    }

    private void CheckResponse(IIppResponseMessage response)
    {
        Assert.NotNull(response);
        Test.AddJsonAttachment(response, "response.json");
        Test.AddJsonAttachment(response.Sections, "attributes.json");
        Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        response.ShouldMatchSnapshot();
    }
    
    private Task<IIppResponseMessage> ReadIppResponse(string binFileName)
    {
        var file = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Resources", "ReadingTests", binFileName);
        using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
        var ippResponse = _protocol.ReadIppResponseAsync(stream);
        return ippResponse;
    }
}
