using System;
using System.Collections.Generic;

using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Extensions;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class IppJobProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<IIppJobRequest, IppRequestMessage>((src, dst, map) =>
            {
                map.Map<IIppRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;

                if (src.JobUrl != null)
                {
                    operation.Add(new IppAttribute(Tag.Uri, "job-uri", src.JobUrl.ToString()));
                }
                else if (src.JobId != null)
                {
                    operation.Add(new IppAttribute(Tag.Uri, "printer-uri", src.PrinterUri.ToString()));
                    operation.Add(new IppAttribute(Tag.Integer, "job-id", src.JobId.Value));
                }
                else
                {
                    throw new ArgumentException($"JobTarget must have {nameof(GetJobAttributesRequest.JobUrl)} or {nameof(GetJobAttributesRequest.JobId)} set");
                }

                return dst;
            });

            mapper.CreateMap<IppResponseMessage, IIppJobResponse>((src, dst, map) =>
            {
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                map.Map(src.AllAttributes(), dst);
                return dst;
            });

            mapper.CreateMap<IDictionary<string, IppAttribute[]>, IIppJobResponse>((
                src,
                dst,
                map) =>
            {
                dst.JobUri = map.MapFromDic<string>(src, JobAttribute.JobUri);
                dst.JobId = map.MapFromDic<int>(src, JobAttribute.JobId);
                dst.JobState = map.MapFromDic<JobState>(src, JobAttribute.JobState);
                dst.JobStateReasons = map.MapFromDicSet<string[]>(src, JobAttribute.JobStateReasons);
                dst.JobStateMessage = map.MapFromDic<string?>(src, JobAttribute.JobStateMessage);
                dst.NumberOfInterveningJobs = map.MapFromDic<int?>(src, JobAttribute.NumberOfInterveningJobs);
                return dst;
            });
        }
    }
}
