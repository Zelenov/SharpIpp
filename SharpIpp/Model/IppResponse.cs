using System.Collections.Generic;
using System.Linq;

namespace SharpIpp.Model
{
    public class IppSection
    {
        public SectionTag Tag { get; set; } 
        public List<IppAttribute> Attributes { get; } = new List<IppAttribute>();
    }

    public class IppResponse
    {
        public IppVersion Version { get; set; }
        public IppStatusCode StatusCode { get; set; }
        public int RequestId { get; set; }
        public List<IppSection> Sections { get; } = new List<IppSection>();

        public bool IsSuccessfulStatusCode =>
            (short) StatusCode >= (short) IppStatusCode.SuccessfulOk &&
            (short) StatusCode <= (short) IppStatusCode.SuccessfulOkEventsComplete;

        public IDictionary<string, IppAttribute[]> Attributes =>
            Sections.SelectMany(x=>x.Attributes)
               .GroupBy(x => x.Name)
               .ToDictionary(g => g.Key, g => g.ToArray());

        public override string ToString() =>
            $"{nameof(Version)}: {Version}\n{nameof(StatusCode)}: {StatusCode}\n{nameof(RequestId)}: {RequestId}\nAttributes:\n{string.Join("\n", Attributes.Values.SelectMany(s => s))}";
    }
}