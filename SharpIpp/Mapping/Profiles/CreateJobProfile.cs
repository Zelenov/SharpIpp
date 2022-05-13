using SharpIpp.Models;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class CreateJobProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<CreateJobRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage { IppOperation = IppOperation.CreateJob };
                map.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);

                if (src.NewJobAttributes != null)
                {
                    map.Map(src.NewJobAttributes, dst);
                }

                return dst;
            });

            mapper.CreateMap<IppResponseMessage, CreateJobResponse>((src, map) =>
            {
                var dst = new CreateJobResponse();
                map.Map<IppResponseMessage, IIppJobResponse>(src, dst);
                return dst;
            });
        }
    }
}
