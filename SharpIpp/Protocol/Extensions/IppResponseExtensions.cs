using System.Collections.Generic;
using System.Linq;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Protocol.Extensions
{
    public static class IppResponseMessageExtensions
    {
        public static bool IsSuccessfulStatusCode(this IIppResponseMessage message)
        {
            return (short)message.StatusCode >= (short)IppStatusCode.SuccessfulOk &&
                   (short)message.StatusCode <= (short)IppStatusCode.SuccessfulOkEventsComplete;
        }

        public static IDictionary<string, IppAttribute[]> AllAttributes(this IIppResponseMessage ippResponseMessage)
        {
            return ippResponseMessage.Sections.SelectMany(x => x.Attributes)
                .GroupBy(x => x.Name)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
    }
}
