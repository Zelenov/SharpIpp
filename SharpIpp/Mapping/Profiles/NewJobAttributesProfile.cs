using System.Linq;

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
                    operation.Add(new IppAttribute(Tag.NameWithoutLanguage, "job-name", src.JobName));
                }

                if (src.IppAttributeFidelity != null)
                {
                    operation.Add(new IppAttribute(Tag.Boolean,
                        "ipp-attribute-fidelity",
                        src.IppAttributeFidelity.Value));
                }

                if (src.JobPriority != null)
                {
                    job.Add(new IppAttribute(Tag.Integer, "job-priority", src.JobPriority.Value));
                }

                if (src.JobHoldUntil != null)
                {
                    job.Add(new IppAttribute(Tag.NameWithoutLanguage,
                        "job-hold-until",
                        map.Map<string>(src.JobHoldUntil.Value)));
                }

                if (src.MultipleDocumentHandling != null)
                {
                    job.Add(new IppAttribute(Tag.Keyword,
                        "multiple-document-handling",
                        map.Map<string>(src.MultipleDocumentHandling.Value)));
                }

                if (src.Copies != null)
                {
                    job.Add(new IppAttribute(Tag.Integer, "copies", src.Copies.Value));
                }

                if (src.Finishings != null)
                {
                    job.Add(new IppAttribute(Tag.Enum, "finishings", (int)src.Finishings.Value));
                }

                if (src.PageRanges != null)
                {
                    job.AddRange(src.PageRanges.Select(pageRange =>
                        new IppAttribute(Tag.RangeOfInteger, "page-ranges", pageRange)));
                }

                if (src.Sides != null)
                {
                    job.Add(new IppAttribute(Tag.Keyword, "sides", map.Map<string>(src.Sides.Value)));
                }

                if (src.NumberUp != null)
                {
                    job.Add(new IppAttribute(Tag.Integer, "number-up", src.NumberUp.Value));
                }

                if (src.OrientationRequested != null)
                {
                    job.Add(new IppAttribute(Tag.Enum, "orientation-srced", (int)src.OrientationRequested.Value));
                }

                if (src.Media != null)
                {
                    job.Add(new IppAttribute(Tag.Keyword, "media", src.Media));
                }

                if (src.PrinterResolution != null)
                {
                    job.Add(new IppAttribute(Tag.Resolution, "printer-resolution", src.PrinterResolution.Value));
                }

                if (src.PrintQuality != null)
                {
                    job.Add(new IppAttribute(Tag.Enum, "print-quality", (int)src.PrintQuality.Value));
                }

                if (src.PrintScaling != null)
                {
                    job.Add(new IppAttribute(Tag.Keyword, "print-scaling", map.Map<string>(src.PrintScaling.Value)));
                }

                dst.OperationAttributes.Populate(src.AdditionalOperationAttributes);
                dst.JobAttributes.Populate(src.AdditionalJobAttributes);
                return dst;
            });
        }
    }
}
