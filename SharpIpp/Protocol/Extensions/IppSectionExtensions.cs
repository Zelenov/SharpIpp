using System.Collections.Generic;
using System.Linq;
using SharpIpp.Model;

namespace SharpIpp.Protocol.Extensions
{
    internal static class IppSectionExtensions
    {
        public static IDictionary<string, IppAttribute[]> AllAttributes(this IppSection ippSection) =>
            ippSection.Attributes.GroupBy(x => x.Name).ToDictionary(g => g.Key, g => g.ToArray());
    }
}