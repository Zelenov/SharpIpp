using System.Collections.Generic;
using System.Linq;

using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Extensions;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class GetJobsProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            // https://tools.ietf.org/html/rfc2911#section-3.3.4.1
            mapper.CreateMap<GetJobsRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage { IppOperation = IppOperation.GetJobs };
                map.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;

                if (src.Limit != null)
                {
                    operation.Add(new IppAttribute(Tag.Integer, "requesting-user-name", src.Limit.Value));
                }

                if (src.WhichJobs != null)
                {
                    operation.Add(new IppAttribute(Tag.Keyword, "which-jobs", map.Map<string>(src.WhichJobs.Value)));
                }

                if (src.MyJobs != null)
                {
                    operation.Add(new IppAttribute(Tag.Boolean, "my-jobs", map.Map<string>(src.MyJobs.Value)));
                }

                if (src.RequestedAttributes != null)
                {
                    operation.AddRange(src.RequestedAttributes.Select(requestedAttribute =>
                        new IppAttribute(Tag.Keyword, "requested-attributes", requestedAttribute)));
                }

                dst.OperationAttributes.Populate(src.AdditionalOperationAttributes);
                dst.JobAttributes.Populate(src.AdditionalJobAttributes);
                return dst;
            });

            // https://tools.ietf.org/html/rfc2911#section-3.3.4.2
            mapper.CreateMap<IppResponseMessage, GetJobsResponse>((src, map) =>
            {
                var dst = new GetJobsResponse { Jobs = map.Map<List<IppSection>, JobAttributes[]>(src.Sections) };
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });

            //https://tools.ietf.org/html/rfc2911#section-4.4
            mapper.CreateMap<List<IppSection>, JobAttributes[]>((src, map) =>
                src.Where(x => x.Tag == SectionTag.JobAttributesTag)
                    .Select(x => map.Map<JobAttributes>(x.AllAttributes()))
                    .ToArray());
        }
    }
}
