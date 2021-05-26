using System.Collections.Generic;
using System.Linq;
using SharpIpp.Model;

namespace SharpIpp.Protocol.Extensions
{
    internal static class IppResponseExtensions
    {
        public static IDictionary<string, IppAttribute[]> AllAttributes(this IppResponseMessage ippResponseMessage) =>
            ippResponseMessage.Sections.SelectMany(x => x.Attributes)
               .GroupBy(x => x.Name)
               .ToDictionary(g => g.Key, g => g.ToArray());
    }
}