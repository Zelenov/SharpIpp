using System.Collections.Generic;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Protocol.Extensions
{
    internal static class ListExtensions
    {
        public static void Populate(this List<IppAttribute> list, IEnumerable<IppAttribute>? other)
        {
            if (other == null)
            {
                return;
            }

            foreach (var additionalAttribute in other)
            {
                list.RemoveAll(x => x.Name == additionalAttribute.Name);
                list.Add(additionalAttribute);
            }
        }
    }
}
