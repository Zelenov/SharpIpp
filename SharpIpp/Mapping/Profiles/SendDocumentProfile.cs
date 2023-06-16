using System;
using System.Linq;
using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class SendDocumentProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<SendDocumentRequest, IppRequestMessage>((src, map) =>
            {
                if (src.Document == null && !src.LastDocument)
                {
                    throw new ArgumentException($"{nameof(src.Document)} must be set for non-last document");
                }

                var dst = new IppRequestMessage
                {
                    IppOperation = IppOperation.SendDocument, Document = src.Document,
                };
                map.Map<IIppJobRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                operation.Add(new IppAttribute(Tag.Boolean, "last-document", src.LastDocument));

                if (src.DocumentAttributes != null)
                {
                    map.Map(src.DocumentAttributes, dst);
                }

                return dst;
            });

            mapper.CreateMap<IIppRequestMessage, SendDocumentRequest>( ( src, map ) =>
            {
                var dst = new SendDocumentRequest
                {
                    DocumentAttributes = new DocumentAttributes()
                };
                map.Map<IIppRequestMessage, IIppJobRequest>( src, dst );
                dst.LastDocument = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.LastDocument )?.Value as bool? ?? false;
                map.Map( src, dst.DocumentAttributes );
                return dst;
            } );

            mapper.CreateMap<IppResponseMessage, SendDocumentResponse>((src, map) =>
            {
                var dst = new SendDocumentResponse();
                map.Map<IppResponseMessage, IIppJobResponse>(src, dst);
                return dst;
            });

            mapper.CreateMap<SendDocumentResponse, IppResponseMessage>( ( src, map ) =>
            {
                var dst = new IppResponseMessage();
                map.Map<IIppJobResponse, IppResponseMessage>( src, dst );
                return dst;
            } );
        }
    }
}
