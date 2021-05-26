using SharpIpp.Model;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        public IppRequestMessage Construct(PausePrinterRequest request) => ConstructIppRequest(request);

        public PausePrinterResponse ConstructPausePrinterResponse(IIppResponseMessage ippResponse) =>
            Construct<PausePrinterResponse>(ippResponse);

        private static void ConfigurePausePrinterRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<PausePrinterRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.PausePrinter};
                mapper.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, PausePrinterResponse>((src, map) =>
            {
                var dst = new PausePrinterResponse();
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });
        }
    }
}