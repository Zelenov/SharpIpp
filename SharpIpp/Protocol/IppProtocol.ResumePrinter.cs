using SharpIpp.Model;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        public IppRequestMessage Construct(ResumePrinterRequest request) => ConstructIppRequest(request);

        public ResumePrinterResponse ConstructResumePrinterResponse(IIppResponseMessage ippResponse) =>
            Construct<ResumePrinterResponse>(ippResponse);

        private static void ConfigureResumePrinterRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<ResumePrinterRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.ResumePrinter};
                mapper.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, ResumePrinterResponse>((src, map) =>
            {
                var dst = new ResumePrinterResponse();
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });
        }
    }
}