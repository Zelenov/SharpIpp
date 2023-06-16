using System;
using System.Collections.Generic;
using System.Linq;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Extensions;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class NewJobAttributesProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<NewJobAttributes, IppRequestMessage>((src, dst, map) =>
            {
                var operation = dst.OperationAttributes;
                var job = dst.JobAttributes;

                if (src.JobName != null)
                {
                    operation.Add(new IppAttribute(Tag.NameWithoutLanguage, JobAttribute.JobName, src.JobName));
                }

                if (src.IppAttributeFidelity != null)
                {
                    operation.Add(new IppAttribute(Tag.Boolean,
                        JobAttribute.IppAttributeFidelity,
                        src.IppAttributeFidelity.Value));
                }

                if (src.JobPriority != null)
                {
                    job.Add(new IppAttribute(Tag.Integer, JobAttribute.JobPriority, src.JobPriority.Value));
                }

                if (src.JobHoldUntil != null)
                {
                    job.Add(new IppAttribute(Tag.NameWithoutLanguage,
                        JobAttribute.JobHoldUntil,
                        map.Map<string>(src.JobHoldUntil.Value)));
                }

                if (src.MultipleDocumentHandling != null)
                {
                    job.Add(new IppAttribute(Tag.Keyword,
                        JobAttribute.MultipleDocumentHandling,
                        map.Map<string>(src.MultipleDocumentHandling.Value)));
                }

                if (src.Copies != null)
                {
                    job.Add(new IppAttribute(Tag.Integer, JobAttribute.Copies, src.Copies.Value));
                }

                if (src.Finishings != null)
                {
                    job.Add(new IppAttribute(Tag.Enum, JobAttribute.Finishings, (int)src.Finishings.Value));
                }

                if (src.PageRanges != null)
                {
                    job.AddRange(src.PageRanges.Select(pageRange =>
                        new IppAttribute(Tag.RangeOfInteger, JobAttribute.PageRanges, pageRange)));
                }

                if (src.Sides != null)
                {
                    job.Add(new IppAttribute(Tag.Keyword, JobAttribute.Sides, map.Map<string>(src.Sides.Value)));
                }

                if (src.NumberUp != null)
                {
                    job.Add(new IppAttribute(Tag.Integer, JobAttribute.NumberUp, src.NumberUp.Value));
                }

                if (src.OrientationRequested != null)
                {
                    job.Add(new IppAttribute(Tag.Enum, JobAttribute.OrientationRequested, (int)src.OrientationRequested.Value));
                }

                if (src.Media != null)
                {
                    job.Add(new IppAttribute(Tag.Keyword, JobAttribute.Media, src.Media));
                }

                if (src.PrinterResolution != null)
                {
                    job.Add(new IppAttribute(Tag.Resolution, JobAttribute.PrinterResolution, src.PrinterResolution.Value));
                }

                if (src.PrintQuality != null)
                {
                    job.Add(new IppAttribute(Tag.Enum, JobAttribute.PrintQuality, (int)src.PrintQuality.Value));
                }

                if (src.PrintScaling != null)
                {
                    job.Add(new IppAttribute(Tag.Keyword, JobAttribute.PrintScaling, map.Map<string>(src.PrintScaling.Value)));
                }

                dst.OperationAttributes.Populate(src.AdditionalOperationAttributes);
                dst.JobAttributes.Populate(src.AdditionalJobAttributes);
                return dst;
            });

            mapper.CreateMap<IIppRequestMessage, NewJobAttributes>( ( src, dst, map ) =>
            {
                dst.JobName = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.JobName )?.Value as string;
                dst.IppAttributeFidelity = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.IppAttributeFidelity )?.Value as bool?;
                dst.JobPriority = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.JobPriority )?.Value as int?;
                dst.JobHoldUntil = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.JobHoldUntil )?.Value is int jobHoldUntil && Enum.IsDefined( typeof( JobHoldUntil ), jobHoldUntil ) ? (JobHoldUntil)jobHoldUntil : null; 
                dst.MultipleDocumentHandling = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.MultipleDocumentHandling )?.Value is int multipleDocumentHandling && Enum.IsDefined( typeof( MultipleDocumentHandling ), multipleDocumentHandling ) ? (MultipleDocumentHandling)multipleDocumentHandling : null;
                dst.Copies = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.Copies )?.Value as int?;
                dst.Finishings = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.Finishings )?.Value is int finishings && Enum.IsDefined( typeof( Finishings ), finishings ) ? (Finishings)finishings : null;
                var pageRanges = src.JobAttributes.Where( x => x.Name == JobAttribute.PageRanges ).Select( x => x.Value ).OfType<Range>().ToArray();
                dst.PageRanges = pageRanges.Any() ? pageRanges : null;
                dst.Sides = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.Sides )?.Value is int sides && Enum.IsDefined( typeof( Sides ), sides ) ? (Sides)sides : null;
                dst.NumberUp = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.NumberUp )?.Value as int?;
                dst.OrientationRequested = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.OrientationRequested )?.Value is int orientationRequested && Enum.IsDefined( typeof( Orientation ), orientationRequested ) ? (Orientation)orientationRequested : null;
                dst.Media = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.Media )?.Value as string;
                dst.PrinterResolution = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.PrinterResolution )?.Value as Resolution?;
                dst.PrintQuality = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.PrintQuality )?.Value is int printQuality && Enum.IsDefined( typeof( PrintQuality ), printQuality ) ? (PrintQuality)printQuality : null;
                dst.PrintScaling = src.JobAttributes.FirstOrDefault( x => x.Name == JobAttribute.PrintScaling )?.Value is int printScaling && Enum.IsDefined( typeof( PrintScaling ), printScaling ) ? (PrintScaling)printScaling : null;
                var knownOperationAttributeNames = new List<string> { JobAttribute.JobName, JobAttribute.IppAttributeFidelity };
                dst.AdditionalOperationAttributes = src.OperationAttributes.Where( x => !knownOperationAttributeNames.Contains( x.Name ) ).ToList();
                var knownJobAttributeNames = new List<string> { JobAttribute.JobName, JobAttribute.IppAttributeFidelity, JobAttribute.JobPriority, JobAttribute.JobHoldUntil, JobAttribute.MultipleDocumentHandling, JobAttribute.Copies, JobAttribute.Finishings, JobAttribute.PageRanges, JobAttribute.Sides, JobAttribute.NumberUp, JobAttribute.OrientationRequested, JobAttribute.Media, JobAttribute.PrinterResolution, JobAttribute.PrintQuality, JobAttribute.PrintScaling };
                dst.AdditionalJobAttributes = src.JobAttributes.Where( x => !knownJobAttributeNames.Contains( x.Name ) ).ToList();
                return dst;
            } );
        }
    }
}
