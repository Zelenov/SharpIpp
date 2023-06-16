using System;
using System.Collections.Generic;
using System.Linq;

using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Extensions;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class GetJobAttributesProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<GetJobAttributesRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage { IppOperation = IppOperation.GetJobAttributes };
                map.Map<IIppJobRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;

                if (src.RequestedAttributes != null)
                {
                    operation.AddRange(src.RequestedAttributes.Select(requestedAttribute =>
                        new IppAttribute(Tag.Keyword, JobAttribute.RequestedAttributes, requestedAttribute)));
                }

                dst.OperationAttributes.Populate(src.AdditionalOperationAttributes);
                dst.JobAttributes.Populate(src.AdditionalJobAttributes);
                return dst;
            });

            mapper.CreateMap<IIppRequestMessage, GetJobAttributesRequest>( ( src, map ) =>
            {
                var dst = new GetJobAttributesRequest();
                map.Map<IIppRequestMessage, IIppJobRequest>( src, dst );
                var requestedAttributes = src.OperationAttributes.Where( x => x.Name == JobAttribute.RequestedAttributes ).Select( x => x.Value ).OfType<string>().ToArray();
                if ( requestedAttributes.Any() )
                    dst.RequestedAttributes = requestedAttributes;
                var knownOperationAttributeNames = new List<string> { JobAttribute.RequestedAttributes };
                dst.AdditionalOperationAttributes = src.OperationAttributes.Where( x => !knownOperationAttributeNames.Contains( x.Name ) ).ToList();
                dst.AdditionalJobAttributes = src.JobAttributes;
                return dst;
            } );

            //https://tools.ietf.org/html/rfc2911#section-4.4
            mapper.CreateMap<IppResponseMessage, GetJobAttributesResponse>((src, map) =>
            {
                var dst = new GetJobAttributesResponse { JobAttributes = map.Map<JobAttributes>(src.AllAttributes()) };
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<GetJobAttributesResponse, IppResponseMessage>( ( src, map ) =>
            {
                var dst = new IppResponseMessage();
                map.Map<IIppResponseMessage, IppResponseMessage>( src, dst );
                var section = new IppSection { Tag = SectionTag.JobAttributesTag };
                section.Attributes.AddRange( map.Map<IDictionary<string, IppAttribute[]>>( src.JobAttributes ).Values.SelectMany( x => x ) );
                dst.Sections.Add(section );
                return dst;
            } );

            mapper.CreateMap<IDictionary<string, IppAttribute[]>, JobAttributes>((src, map) => new JobAttributes
            {
                Compression = map.MapFromDic<Compression?>(src, JobAttribute.Compression),
                Copies = map.MapFromDic<int?>(src, JobAttribute.Copies),
                DateTimeAtCompleted = map.MapFromDic<DateTimeOffset?>(src, JobAttribute.DateTimeAtCompleted),
                DateTimeAtCreation = map.MapFromDic<DateTimeOffset?>(src, JobAttribute.DateTimeAtCreation),
                DateTimeAtProcessing = map.MapFromDic<DateTimeOffset?>(src, JobAttribute.DateTimeAtProcessing),
                DocumentFormat = map.MapFromDic<string?>(src, JobAttribute.DocumentFormat),
                DocumentName = map.MapFromDic<string?>(src, JobAttribute.DocumentName),
                Finishings = map.MapFromDic<Finishings?>(src, JobAttribute.Finishings),
                IppAttributeFidelity = map.MapFromDic<bool?>(src, JobAttribute.IppAttributeFidelity),
                JobId = map.MapFromDic<int?>(src, JobAttribute.JobId),
                JobUri = map.MapFromDic<string?>(src, JobAttribute.JobUri),
                JobImpressions = map.MapFromDic<int?>(src, JobAttribute.JobImpressions),
                JobImpressionsCompleted = map.MapFromDic<int?>(src, JobAttribute.JobImpressionsCompleted),
                JobKOctetsProcessed = map.MapFromDic<int?>(src, JobAttribute.JobKOctetsProcessed),
                JobMediaSheets = map.MapFromDic<int?>(src, JobAttribute.JobMediaSheets),
                JobMediaSheetsCompleted = map.MapFromDic<int?>(src, JobAttribute.JobMediaSheetsCompleted),
                JobName = map.MapFromDic<string?>(src, JobAttribute.JobName),
                JobOriginatingUserName = map.MapFromDic<string?>(src, JobAttribute.JobOriginatingUserName),
                JobOriginatingUserNameLanguage =
                    map.MapFromDicLanguage(src, JobAttribute.JobOriginatingUserNameLanguage),
                JobPrinterUpTime = map.MapFromDic<DateTime?>(src, JobAttribute.JobPrinterUpTime),
                JobPrinterUri = map.MapFromDic<string?>(src, JobAttribute.JobPrinterUri),
                JobSheets = map.MapFromDic<JobSheets?>(src, JobAttribute.JobSheets),
                JobState = map.MapFromDic<JobState?>(src, JobAttribute.JobState),
                JobStateMessage = map.MapFromDic<string?>(src, JobAttribute.JobStateMessage),
                JobStateReasons = map.MapFromDicSetNull<string[]?>(src, JobAttribute.JobStateReasons),
                Media = map.MapFromDic<string?>(src, JobAttribute.Media),
                MultipleDocumentHandling =
                    map.MapFromDic<MultipleDocumentHandling?>(src, JobAttribute.MultipleDocumentHandling),
                NumberUp = map.MapFromDic<int?>(src, JobAttribute.NumberUp),
                OrientationRequested = map.MapFromDic<Orientation?>(src, JobAttribute.OrientationRequested),
                PrinterResolution = map.MapFromDic<Resolution?>(src, JobAttribute.PrinterResolution),
                PrintQuality = map.MapFromDic<PrintQuality?>(src, JobAttribute.PrintQuality),
                Sides = map.MapFromDic<Sides?>(src, JobAttribute.Sides),
                TimeAtCompleted = map.MapFromDic<DateTime?>(src, JobAttribute.TimeAtCompleted),
                TimeAtCreation = map.MapFromDic<DateTime?>(src, JobAttribute.TimeAtCreation),
                TimeAtProcessing = map.MapFromDic<DateTime?>(src, JobAttribute.TimeAtProcessing),
            });

            mapper.CreateMap<JobAttributes, IDictionary<string, IppAttribute[]>>( ( src, map ) =>
            {
                var dic = new Dictionary<string, IppAttribute[]>();
                if ( src.Compression != null )
                    dic.Add( JobAttribute.Compression, new IppAttribute[] { new IppAttribute( Tag.Keyword, JobAttribute.Compression, map.Map<string>( src.Compression ) ) } );
                if ( src.Copies != null )
                    dic.Add( JobAttribute.Copies, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.Copies, src.Copies.Value ) } );
                if ( src.DateTimeAtCompleted != null )
                    dic.Add( JobAttribute.DateTimeAtCompleted, new IppAttribute[] { new IppAttribute( Tag.DateTime, JobAttribute.DateTimeAtCompleted, src.DateTimeAtCompleted.Value ) } );
                if ( src.DateTimeAtCreation != null )
                    dic.Add( JobAttribute.DateTimeAtCreation, new IppAttribute[] { new IppAttribute( Tag.DateTime, JobAttribute.DateTimeAtCreation, src.DateTimeAtCreation.Value ) } );
                if ( src.DateTimeAtProcessing != null )
                    dic.Add( JobAttribute.DateTimeAtProcessing, new IppAttribute[] { new IppAttribute( Tag.DateTime, JobAttribute.DateTimeAtProcessing, src.DateTimeAtProcessing.Value ) } );
                if ( src.DocumentFormat != null )
                    dic.Add( JobAttribute.DocumentFormat, new IppAttribute[] { new IppAttribute( Tag.MimeMediaType, JobAttribute.DocumentFormat, src.DocumentFormat ) } );
                if ( src.DocumentName != null )
                    dic.Add( JobAttribute.DocumentName, new IppAttribute[] { new IppAttribute( Tag.NameWithoutLanguage, JobAttribute.DocumentName, src.DocumentName ) } );
                if ( src.Finishings != null )
                    dic.Add( JobAttribute.Finishings, new IppAttribute[] { new IppAttribute( Tag.Enum, JobAttribute.Finishings, (int)src.Finishings.Value ) } );
                if(src.IppAttributeFidelity != null )
                    dic.Add( JobAttribute.IppAttributeFidelity, new IppAttribute[] { new IppAttribute( Tag.Boolean, JobAttribute.IppAttributeFidelity, src.IppAttributeFidelity.Value ) } );
                if ( src.JobId != null )
                    dic.Add( JobAttribute.JobId, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.JobId, src.JobId.Value ) } );
                if( src.JobUri != null )
                    dic.Add( JobAttribute.JobUri, new IppAttribute[] { new IppAttribute( Tag.Uri, JobAttribute.JobUri, src.JobUri ) } );
                if ( src.JobImpressions != null )
                    dic.Add( JobAttribute.JobImpressions, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.JobImpressions, src.JobImpressions.Value ) } );
                if ( src.JobImpressionsCompleted != null )
                    dic.Add( JobAttribute.JobImpressionsCompleted, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.JobImpressionsCompleted, src.JobImpressionsCompleted.Value ) } );
                if ( src.JobKOctetsProcessed != null )
                    dic.Add( JobAttribute.JobKOctetsProcessed, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.JobKOctetsProcessed, src.JobKOctetsProcessed.Value ) } );
                if ( src.JobMediaSheets != null )
                    dic.Add( JobAttribute.JobMediaSheets, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.JobMediaSheets, src.JobMediaSheets.Value ) } );
                if ( src.JobMediaSheetsCompleted != null )
                    dic.Add( JobAttribute.JobMediaSheetsCompleted, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.JobMediaSheetsCompleted, src.JobMediaSheetsCompleted.Value ) } );
                if ( src.JobName != null )
                    dic.Add( JobAttribute.JobName, new IppAttribute[] { new IppAttribute( Tag.NameWithoutLanguage, JobAttribute.JobName, src.JobName ) } );
                if ( src.JobOriginatingUserName != null )
                    dic.Add( JobAttribute.JobOriginatingUserName, new IppAttribute[] { new IppAttribute( Tag.NameWithoutLanguage, JobAttribute.JobOriginatingUserName, src.JobOriginatingUserName ) } );
                if ( src.JobOriginatingUserNameLanguage != null )
                    dic.Add( JobAttribute.JobOriginatingUserNameLanguage, new IppAttribute[] { new IppAttribute( Tag.NaturalLanguage, JobAttribute.JobOriginatingUserNameLanguage, src.JobOriginatingUserNameLanguage ) } );
                if ( src.JobPrinterUpTime != null )
                    dic.Add( JobAttribute.JobPrinterUpTime, new IppAttribute[] { new IppAttribute( Tag.DateTime, JobAttribute.JobPrinterUpTime, src.JobPrinterUpTime.Value ) } );
                if ( src.JobPrinterUri != null )
                    dic.Add( JobAttribute.JobPrinterUri, new IppAttribute[] { new IppAttribute( Tag.Uri, JobAttribute.JobPrinterUri, src.JobPrinterUri ) } );
                if ( src.JobSheets != null )
                    dic.Add( JobAttribute.JobSheets, new IppAttribute[] { new IppAttribute( Tag.Keyword, JobAttribute.JobSheets, map.Map<string>( src.JobSheets ) ) } );
                if ( src.JobState != null )
                    dic.Add( JobAttribute.JobState, new IppAttribute[] { new IppAttribute( Tag.Enum, JobAttribute.JobState, (int)src.JobState.Value ) } );
                if ( src.JobStateMessage != null )
                    dic.Add( JobAttribute.JobStateMessage, new IppAttribute[] { new IppAttribute( Tag.TextWithoutLanguage, JobAttribute.JobStateMessage, src.JobStateMessage ) } );
                if ( src.JobStateReasons != null )
                    dic.Add( JobAttribute.JobStateReasons, new IppAttribute[] { new IppAttribute( Tag.Keyword, JobAttribute.JobStateReasons, map.Map<string>( src.JobStateReasons ) ) } );
                if ( src.Media != null )
                    dic.Add( JobAttribute.Media, new IppAttribute[] { new IppAttribute( Tag.Keyword, JobAttribute.JobStateReasons, src.Media ) } );
                if ( src.MultipleDocumentHandling != null )
                    dic.Add( JobAttribute.MultipleDocumentHandling, new IppAttribute[] { new IppAttribute( Tag.Keyword, JobAttribute.MultipleDocumentHandling, map.Map<string>( src.MultipleDocumentHandling ) ) } );
                if ( src.NumberUp != null )
                    dic.Add( JobAttribute.NumberUp, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.NumberUp, src.NumberUp.Value ) } );
                if ( src.OrientationRequested != null )
                    dic.Add( JobAttribute.OrientationRequested, new IppAttribute[] { new IppAttribute( Tag.Enum, JobAttribute.OrientationRequested, (int)src.OrientationRequested.Value ) } );
                if ( src.PrinterResolution != null )
                    dic.Add( JobAttribute.PrinterResolution, new IppAttribute[] { new IppAttribute( Tag.Resolution, JobAttribute.PrinterResolution, src.PrinterResolution.Value ) } );
                if ( src.PrintQuality != null )
                    dic.Add( JobAttribute.PrintQuality, new IppAttribute[] { new IppAttribute( Tag.Enum, JobAttribute.PrintQuality, (int)src.PrintQuality.Value ) } );
                if ( src.Sides != null )
                    dic.Add( JobAttribute.Sides, new IppAttribute[] { new IppAttribute( Tag.Keyword, JobAttribute.Sides, map.Map<string>( src.Sides ) ) } );
                if ( src.TimeAtCompleted != null )
                    dic.Add( JobAttribute.TimeAtCompleted, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.TimeAtCompleted, map.Map<int>( src.TimeAtCompleted ) ) } );
                if ( src.TimeAtCreation != null )
                    dic.Add( JobAttribute.TimeAtCreation, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.TimeAtCreation, map.Map<int>( src.TimeAtCreation ) ) } );
                if ( src.TimeAtProcessing != null )
                    dic.Add( JobAttribute.TimeAtProcessing, new IppAttribute[] { new IppAttribute( Tag.Integer, JobAttribute.TimeAtProcessing, map.Map<int>( src.TimeAtProcessing ) ) } );
                return dic;
            } );
        }
    }
}
