using SharpIpp.Model;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        public IppRequestMessage Construct(RestartJobRequest request) => ConstructIppRequest(request);

        public RestartJobResponse ConstructRestartJobResponse(IIppResponseMessage ippResponse) =>
            Construct<RestartJobResponse>(ippResponse);

        private static void ConfigureRestartJobRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<RestartJobRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.RestartJob};
                mapper.Map<IIppJobRequest, IppRequestMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, RestartJobResponse>((src, map) =>
            {
                var dst = new RestartJobResponse();
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });
        }
    }
}