using System.Collections.Generic;
using System.Linq;

using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Extensions;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class GetCUPSPrintersProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<CUPSGetPrintersRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage { IppOperation = IppOperation.GetCUPSPrinters };
                map.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;

                if (src.Limit != null)
                {
                    operation.Add(new IppAttribute(Tag.Integer, JobAttribute.Limit, src.Limit.Value));
                }

                if (src.FirstPrinterName != null)
                {
                    operation.Add(new IppAttribute(Tag.Keyword, JobAttribute.FirstPrinterName, map.Map<string>(src.FirstPrinterName)));
                }

                if (src.PrinterId != null)
                {
                    operation.Add(new IppAttribute(Tag.Integer, JobAttribute.PrinterId, map.Map<string>(src.PrinterId)));
                }

                if (src.PrinterLocation != null)
                {
                    operation.Add(new IppAttribute(Tag.Keyword, JobAttribute.PrinterLocation, map.Map<string>(src.PrinterLocation)));
                }

                if (src.PrinterType != null)
                {
                    operation.Add(new IppAttribute(Tag.Enum, JobAttribute.PrinterType, (int)src.PrinterType.Value));
                }

                if (src.PrinterTypeMask != null)
                {
                    operation.Add(new IppAttribute(Tag.Enum, JobAttribute.PrinterTypeMask, (int)src.PrinterTypeMask.Value));
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

            mapper.CreateMap<IppRequestMessage, CUPSGetPrintersRequest>( ( src, map ) =>
            {
                var dst = new CUPSGetPrintersRequest();
                map.Map<IppRequestMessage, IIppPrinterRequest>( src, dst );
                dst.Limit = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.Limit )?.Value as int?;
                dst.FirstPrinterName = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.FirstPrinterName )?.Value as string;
                dst.PrinterId = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.PrinterId )?.Value as int?;
                dst.PrinterLocation = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.PrinterLocation )?.Value as string;
                dst.PrinterType = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.PrinterType )?.Value is int printerType ? (PrinterType)printerType : null;
                dst.PrinterTypeMask = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.PrinterTypeMask )?.Value is int printerTypeMask ? (PrinterType)printerTypeMask : null;
                var requestedAttributes = src.OperationAttributes.Where( x => x.Name == JobAttribute.RequestedAttributes ).Select( x => x.Value ).OfType<string>().ToArray();
                if ( requestedAttributes.Any() )
                    dst.RequestedAttributes = requestedAttributes;
                var knownOperationAttributeNames = new List<string> { JobAttribute.Limit, JobAttribute.FirstPrinterName, JobAttribute.PrinterId, JobAttribute.PrinterLocation, JobAttribute.PrinterType, JobAttribute.PrinterTypeMask, JobAttribute.RequestedAttributes };
                dst.AdditionalOperationAttributes = src.OperationAttributes.Where( x => !knownOperationAttributeNames.Contains( x.Name ) ).ToList();
                dst.AdditionalJobAttributes = src.JobAttributes;
                return dst;
            } );

            mapper.CreateMap<IppResponseMessage, CUPSGetPrintersResponse>((src, map) =>
            {
                var dst = new CUPSGetPrintersResponse { Jobs = map.Map<List<IppSection>, JobAttributes[]>(src.Sections) };
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<CUPSGetPrintersResponse, IppResponseMessage>( ( src, map ) =>
            {
                var dst = new IppResponseMessage();
                dst.Sections.AddRange( map.Map<JobAttributes[], List<IppSection>>( src.Jobs ) );
                map.Map<IIppResponseMessage, IppResponseMessage>( src, dst );
                return dst;
            } );
        }
    }
}
