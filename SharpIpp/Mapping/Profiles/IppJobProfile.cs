using System;
using System.Collections.Generic;
using System.Linq;
using SharpIpp.Exceptions;
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
                    operation.Add(new IppAttribute(Tag.Uri, JobAttribute.JobUri, src.JobUrl.ToString()));
                }
                else if (src.JobId != null)
                {
                    operation.Add(new IppAttribute(Tag.Uri, JobAttribute.PrinterUri, src.PrinterUri.ToString()));
                    operation.Add(new IppAttribute(Tag.Integer, JobAttribute.JobId, src.JobId.Value));
                }
                else
                {
                    throw new IppRequestException($"JobTarget must have {nameof(GetJobAttributesRequest.JobUrl)} or {nameof(GetJobAttributesRequest.JobId)} set", dst, IppStatusCode.ClientErrorBadRequest );
                }

                return dst;
            });

            mapper.CreateMap<IIppRequestMessage, IIppJobRequest>( ( src, dst, map ) =>
            {
                map.Map<IIppRequestMessage, IIppRequest>( src, dst );
                if ( Uri.TryCreate( src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.JobUri )?.Value as string, UriKind.RelativeOrAbsolute, out Uri jobUri ) )
                    dst.JobUrl = jobUri;
                if ( Uri.TryCreate( src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.PrinterUri )?.Value as string, UriKind.RelativeOrAbsolute, out Uri printerUri ) )
                    dst.PrinterUri = printerUri;
                dst.JobId = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.JobId )?.Value as int?;
                return dst;
            } );

            mapper.CreateMap<IppResponseMessage, IIppJobResponse>((src, dst, map) =>
            {
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                map.Map(src.AllAttributes(), dst);
                return dst;
            });

            mapper.CreateMap<IIppJobResponse, IppResponseMessage>( ( src, dst, map ) =>
            {
                map.Map<IIppResponseMessage, IppResponseMessage>( src, dst );
                var section = new IppSection { Tag = SectionTag.JobAttributesTag };
                section.Attributes.AddRange( map.Map<IIppJobResponse, IDictionary<string, IppAttribute[]>>( src ).Values.SelectMany( x => x ) );
                dst.Sections.Add( section );
                return dst;
            } );

            mapper.CreateMap<IDictionary<string, IppAttribute[]>, IIppJobResponse>((
                src,
                dst,
                map) =>
            {
                dst.JobUri = map.MapFromDic<string>(src, JobAttribute.JobUri);
                dst.JobId = map.MapFromDic<int>(src, JobAttribute.JobId);
                dst.JobState = map.MapFromDic<JobState>(src, JobAttribute.JobState);
                dst.JobStateReasons = map.MapFromDicSet<JobStateReason[]>(src, JobAttribute.JobStateReasons);
                dst.JobStateMessage = map.MapFromDic<string?>(src, JobAttribute.JobStateMessage);
                dst.NumberOfInterveningJobs = map.MapFromDic<int?>(src, JobAttribute.NumberOfInterveningJobs);
                return dst;
            });

            mapper.CreateMap<IIppJobResponse, IDictionary<string, IppAttribute[]>>( (
                src,
                dst,
                map ) =>
            {
                var dic = new Dictionary<string, IppAttribute[]>
                {
                    { JobAttribute.JobUri, new IppAttribute[] { new IppAttribute( Tag.Uri, JobAttribute.JobUri, src.JobUri ) } },
                    { JobAttribute.JobId, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.JobId, src.JobId ) } },
                    { JobAttribute.JobState, new IppAttribute[] { new IppAttribute( Tag.Enum, JobAttribute.JobState, (int)src.JobState ) } }
                };
                if ( src.JobStateReasons?.Any() ?? false )
                    dic.Add( JobAttribute.JobStateReasons, src.JobStateReasons.Select( x => new IppAttribute( Tag.Keyword, JobAttribute.JobStateReasons, map.Map<string>( x ) ) ).ToArray() );
                if( src.JobStateMessage != null )
                    dic.Add( JobAttribute.JobStateMessage, new IppAttribute[] { new IppAttribute( Tag.TextWithoutLanguage, JobAttribute.JobStateMessage, src.JobStateMessage ) } );
                if(src.NumberOfInterveningJobs != null )
                    dic.Add( JobAttribute.JobStateMessage, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.JobStateMessage, src.NumberOfInterveningJobs.Value ) } );
                return dic;
            } );
        }
    }
}
