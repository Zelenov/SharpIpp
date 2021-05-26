using System.Collections.Generic;
using System.Linq;

namespace SharpIpp.Model
{
    public class IppResponseMessage : IIppResponseMessage
    {
        public bool IsSuccessfulStatusCode =>
            (short) StatusCode >= (short) IppStatusCode.SuccessfulOk &&
            (short) StatusCode <= (short) IppStatusCode.SuccessfulOkEventsComplete;

        public IppVersion Version { get; set; }
        public IppStatusCode StatusCode { get; set; }
        public int RequestId { get; set; }
        public List<IppSection> Sections { get; } = new List<IppSection>();

        public override string ToString() =>
            $"{nameof(Version)}: {Version}\n{nameof(StatusCode)}: {StatusCode}\n{nameof(RequestId)}: {RequestId}\nSections:\n{string.Join("\n", Sections.Select(s => s.ToString()))}";
    }
}