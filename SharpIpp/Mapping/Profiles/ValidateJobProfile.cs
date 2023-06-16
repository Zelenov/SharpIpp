using System;

using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class ValidateJobProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<ValidateJobRequest, IppRequestMessage>((src, map) =>
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (src.Document == null)
                {
                    throw new ArgumentException($"{nameof(src.Document)} must be set");
                }

                var dst = new IppRequestMessage { IppOperation = IppOperation.ValidateJob, Document = src.Document };
                map.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);

                if (src.NewJobAttributes != null)
                {
                    map.Map(src.NewJobAttributes, dst);
                }

                if (src.DocumentAttributes != null)
                {
                    map.Map(src.DocumentAttributes, dst);
                }

                return dst;
            });

            mapper.CreateMap<IIppRequestMessage, ValidateJobRequest>( ( src, map ) =>
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if ( src.Document == null )
                {
                    throw new ArgumentException( $"{nameof( src.Document )} must be set" );
                }

                var dst = new ValidateJobRequest
                {
                    NewJobAttributes = new NewJobAttributes(),
                    DocumentAttributes = new DocumentAttributes()
                };
                map.Map<IIppRequestMessage, IIppPrinterRequest>( src, dst );
                map.Map( src, dst.NewJobAttributes );
                map.Map( src, dst.DocumentAttributes );
                return dst;
            } );

            mapper.CreateMap<IppResponseMessage, ValidateJobResponse>((src, map) =>
            {
                var dst = new ValidateJobResponse();
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<ValidateJobResponse, IppResponseMessage>( ( src, map ) =>
            {
                var dst = new IppResponseMessage();
                map.Map<IIppResponseMessage, IppResponseMessage>( src, dst );
                return dst;
            } );
        }
    }
}
