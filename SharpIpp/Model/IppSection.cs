using System.Collections.Generic;
using System.Linq;

namespace SharpIpp.Model
{
    public class IppSection
    {
        public SectionTag Tag { get; set; }
        public List<IppAttribute> Attributes { get; } = new List<IppAttribute>();

        public override string ToString() =>
            $"{nameof(Tag)}: {Tag}\n\nAttributes:\n{string.Join("\n", Attributes.Select(s => s.ToString()))}";
    }
}