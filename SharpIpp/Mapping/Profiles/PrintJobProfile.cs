using System;

using SharpIpp.Models;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class PrintJobProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<PrintJobRequest, IppRequestMessage>((src, map) =>
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (src.Document == null)
                {
                    throw new ArgumentException($"{nameof(src.Document)} must be set");
                }

                var dst = new IppRequestMessage { IppOperation = IppOperation.PrintJob, Document = src.Document };
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

            mapper.CreateMap<IppResponseMessage, PrintJobResponse>((src, map) =>
            {
                var dst = new PrintJobResponse();
                map.Map<IppResponseMessage, IIppJobResponse>(src, dst);
                return dst;
            });
        }
    }
}
