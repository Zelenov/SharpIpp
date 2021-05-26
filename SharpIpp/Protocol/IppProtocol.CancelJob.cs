using SharpIpp.Model;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        public IppRequestMessage Construct(CancelJobRequest request) => ConstructIppRequest(request);

        public CancelJobResponse ConstructCancelJobResponse(IIppResponseMessage ippResponse) =>
            Construct<CancelJobResponse>(ippResponse);

        private static void ConfigureCancelJobRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<CancelJobRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.CancelJob};
                mapper.Map<IIppJobRequest, IppRequestMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, CancelJobResponse>((src, map) =>
            {
                var dst = new CancelJobResponse();
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });
        }
    }
}