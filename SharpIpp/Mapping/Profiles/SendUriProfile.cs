using System;
using System.Linq;
using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class SendUriProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<SendUriRequest, IppRequestMessage>((src, map) =>
            {
                if (src.DocumentUri == null && !src.LastDocument)
                {
                    throw new ArgumentException($"{nameof(src.DocumentUri)} must be set for non-last document");
                }

                var dst = new IppRequestMessage { IppOperation = IppOperation.SendUri };
                map.Map<IIppJobRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                operation.Add(new IppAttribute(Tag.Boolean, JobAttribute.LastDocument, src.LastDocument));

                if (src.DocumentUri != null)
                {
                    operation.Add(new IppAttribute(Tag.Uri, JobAttribute.DocumentUri, src.DocumentUri.ToString()));
                }

                if (src.DocumentAttributes != null)
                {
                    map.Map(src.DocumentAttributes, dst);
                }

                return dst;
            });

            mapper.CreateMap<IIppRequestMessage, SendUriRequest>( ( src, map ) =>
            {
                var dst = new SendUriRequest { DocumentAttributes = new DocumentAttributes() };
                map.Map<IIppRequestMessage, IIppJobRequest>( src, dst );
                dst.LastDocument = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.LastDocument )?.Value as bool? ?? false;
                if ( Uri.TryCreate( src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.DocumentUri )?.Value as string, UriKind.RelativeOrAbsolute, out Uri documentUri ) )
                    dst.DocumentUri = documentUri;
                map.Map( src, dst.DocumentAttributes );
                return dst;
            } );

            mapper.CreateMap<IppResponseMessage, SendUriResponse>((src, map) =>
            {
                var dst = new SendUriResponse();
                map.Map<IppResponseMessage, IIppJobResponse>(src, dst);
                return dst;
            });

            mapper.CreateMap<SendUriResponse, IppResponseMessage>( ( src, map ) =>
            {
                var dst = new IppResponseMessage();
                map.Map<IIppJobResponse, IppResponseMessage>( src, dst );
                return dst;
            } );
        }
    }
}
