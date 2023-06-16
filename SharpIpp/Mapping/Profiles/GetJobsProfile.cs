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
                    operation.Add(new IppAttribute(Tag.Integer, JobAttribute.Limit, src.Limit.Value));
                }

                if (src.WhichJobs != null)
                {
                    operation.Add(new IppAttribute(Tag.Keyword, JobAttribute.WhichJobs, map.Map<string>(src.WhichJobs.Value)));
                }

                if (src.MyJobs != null)
                {
                    operation.Add(new IppAttribute(Tag.Boolean, JobAttribute.MyJobs, map.Map<string>(src.MyJobs.Value)));
                }

                if (src.RequestedAttributes != null)
                {
                    operation.AddRange(src.RequestedAttributes.Select(requestedAttribute =>
                        new IppAttribute(Tag.Keyword, JobAttribute.RequestedAttributes, requestedAttribute)));
                }

                dst.OperationAttributes.Populate(src.AdditionalOperationAttributes);
                dst.JobAttributes.Populate(src.AdditionalJobAttributes);
                return dst;
            });

            mapper.CreateMap<IIppRequestMessage, GetJobsRequest>( ( src, map ) =>
            {
                var dst = new GetJobsRequest();
                map.Map<IIppRequestMessage, IIppPrinterRequest>( src, dst );
                dst.Limit = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.Limit )?.Value as int?;
                dst.WhichJobs = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.WhichJobs )?.Value as WhichJobs?;
                dst.MyJobs = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.MyJobs )?.Value as bool?;
                var requestedAttributes = src.OperationAttributes.Where( x => x.Name == JobAttribute.RequestedAttributes ).Select( x => x.Value ).OfType<string>().ToArray();
                if ( requestedAttributes.Any() )
                    dst.RequestedAttributes = requestedAttributes;
                var knownOperationAttributeNames = new List<string> { JobAttribute.Limit, JobAttribute.WhichJobs, JobAttribute.MyJobs, JobAttribute.RequestedAttributes };
                dst.AdditionalOperationAttributes = src.OperationAttributes.Where( x => !knownOperationAttributeNames.Contains( x.Name ) ).ToList();
                dst.AdditionalJobAttributes = src.JobAttributes;
                return dst;
            } );

            // https://tools.ietf.org/html/rfc2911#section-3.3.4.2
            mapper.CreateMap<IppResponseMessage, GetJobsResponse>((src, map) =>
            {
                var dst = new GetJobsResponse { Jobs = map.Map<List<IppSection>, JobAttributes[]>(src.Sections) };
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<GetJobsResponse, IppResponseMessage>((src, map) =>
            {
                var dst = new IppResponseMessage();
                dst.Sections.AddRange(map.Map<JobAttributes[], List<IppSection>>(src.Jobs));
                map.Map<IIppResponseMessage, IppResponseMessage>(src, dst);
                return dst;
            } );

            //https://tools.ietf.org/html/rfc2911#section-4.4
            mapper.CreateMap<List<IppSection>, JobAttributes[]>((src, map) =>
                src.Where(x => x.Tag == SectionTag.JobAttributesTag)
                    .Select(x => map.Map<JobAttributes>(x.AllAttributes()))
                    .ToArray());

            mapper.CreateMap<JobAttributes[], List<IppSection>>( (src, map) =>
            {
                return src.Select(x =>
                {
                    var section = new IppSection { Tag = SectionTag.JobAttributesTag };
                    section.Attributes.AddRange( map.Map<IDictionary<string, IppAttribute[]>>( x ).Values.SelectMany( x => x ) );
                    return section;
                }).ToList();
            });
        }
    }
}
