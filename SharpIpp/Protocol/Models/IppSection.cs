using System.Collections.Generic;
using System.Linq;

namespace SharpIpp.Protocol.Models
{
    public class IppSection
    {
        public SectionTag Tag { get; set; }

        public List<IppAttribute> Attributes { get; } = new List<IppAttribute>();

        public override string ToString()
        {
            return $"{nameof(Tag)}: {Tag}\n\nAttributes:\n{string.Join("\n", Attributes.Select(s => s.ToString()))}";
        }
    }
}
