using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class CancelJobProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<CancelJobRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage { IppOperation = IppOperation.CancelJob };
                map.Map<IIppJobRequest, IppRequestMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<IIppRequestMessage, CancelJobRequest>( ( src, map ) =>
            {
                var dst = new CancelJobRequest();
                map.Map<IIppRequestMessage, IIppJobRequest>( src, dst );
                return dst;
            } );

            mapper.CreateMap<IppResponseMessage, CancelJobResponse>((src, map) =>
            {
                var dst = new CancelJobResponse();
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<CancelJobResponse, IppResponseMessage>((src, map) =>
            {
                var dst = new IppResponseMessage();
                map.Map<IIppResponseMessage, IppResponseMessage>(src, dst);
                return dst;
            });
        }
    }
}
