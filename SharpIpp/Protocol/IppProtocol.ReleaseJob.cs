using SharpIpp.Model;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        public IppRequestMessage Construct(ReleaseJobRequest request) => ConstructIppRequest(request);

        public ReleaseJobResponse ConstructReleaseJobResponse(IIppResponseMessage ippResponse) =>
            Construct<ReleaseJobResponse>(ippResponse);

        private static void ConfigureReleaseJobRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<ReleaseJobRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.ReleaseJob};
                mapper.Map<IIppJobRequest, IppRequestMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, ReleaseJobResponse>((src, map) =>
            {
                var dst = new ReleaseJobResponse();
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });
        }
    }
}