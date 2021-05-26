using System.IO;
using SharpIpp.Model;

namespace SharpIpp.Protocol
{
    internal interface IIppProtocol
    {
        IppResponseMessage ReadIppResponse(Stream stream);
        void Write(IppRequestMessage ippRequestMessage, Stream stream);
        IppRequestMessage Construct(CancelJobRequest request);
        CancelJobResponse ConstructCancelJobResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(CreateJobRequest request);
        CreateJobResponse ConstructCreateJobResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(GetJobAttributesRequest request);
        GetJobAttributesResponse ConstructGetJobAttributesResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(GetJobsRequest request);
        GetJobsResponse ConstructGetJobsResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(GetPrinterAttributesRequest request);
        GetPrinterAttributesResponse ConstructGetPrinterAttributesResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(HoldJobRequest request);
        HoldJobResponse ConstructHoldJobResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(PausePrinterRequest request);
        PausePrinterResponse ConstructPausePrinterResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(PrintJobRequest request);
        PrintJobResponse ConstructPrintJobResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(PrintUriRequest request);
        PrintUriResponse ConstructPrintUriResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(PurgeJobsRequest request);
        PurgeJobsResponse ConstructPurgeJobsResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(ReleaseJobRequest request);
        ReleaseJobResponse ConstructReleaseJobResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(RestartJobRequest request);
        RestartJobResponse ConstructRestartJobResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(ResumePrinterRequest request);
        ResumePrinterResponse ConstructResumePrinterResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(SendDocumentRequest request);
        SendDocumentResponse ConstructSendDocumentResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(SendUriRequest request);
        SendUriResponse ConstructSendUriResponse(IIppResponseMessage ippResponse);
        IppRequestMessage Construct(ValidateJobRequest request);
        ValidateJobResponse ConstructValidateJobResponse(IIppResponseMessage ippResponse);
    }
}