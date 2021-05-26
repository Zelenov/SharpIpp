using SharpIpp.Model;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        public IppRequestMessage Construct(PurgeJobsRequest request) => ConstructIppRequest(request);

        public PurgeJobsResponse ConstructPurgeJobsResponse(IIppResponseMessage ippResponse) =>
            Construct<PurgeJobsResponse>(ippResponse);

        private static void ConfigurePurgeJobsRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<PurgeJobsRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.PurgeJobs};
                mapper.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, PurgeJobsResponse>((src, map) =>
            {
                var dst = new PurgeJobsResponse();
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });
        }
    }
}