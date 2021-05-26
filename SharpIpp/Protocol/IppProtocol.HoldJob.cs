using SharpIpp.Model;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        public IppRequestMessage Construct(HoldJobRequest request) => ConstructIppRequest(request);

        public HoldJobResponse ConstructHoldJobResponse(IIppResponseMessage ippResponse) =>
            Construct<HoldJobResponse>(ippResponse);

        private static void ConfigureHoldJobRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<HoldJobRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.HoldJob};
                mapper.Map<IIppJobRequest, IppRequestMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, HoldJobResponse>((src, map) =>
            {
                var dst = new HoldJobResponse();
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });
        }
    }
}