using System;

using SharpIpp.Models;
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
                operation.Add(new IppAttribute(Tag.Boolean, "last-document", src.LastDocument));

                if (src.DocumentUri != null && !src.LastDocument)
                {
                    operation.Add(new IppAttribute(Tag.Uri, "document-uri", src.DocumentUri.ToString()));
                }

                if (src.DocumentAttributes != null)
                {
                    map.Map(src.DocumentAttributes, dst);
                }

                return dst;
            });

            mapper.CreateMap<IppResponseMessage, SendUriResponse>((src, map) =>
            {
                var dst = new SendUriResponse();
                map.Map<IppResponseMessage, IIppJobResponse>(src, dst);
                return dst;
            });
        }
    }
}
