using System.Collections.Generic;
using System.Linq;

namespace SharpIpp.Model
{
    public class IppResponse
    {
        public IppVersion Version { get; set; }
        public IppStatusCode StatusCode { get; set; }
        public int RequestId { get; set; }
        public List<IppAttribute> OperationAttributes { get; } = new List<IppAttribute>();
        public List<IppAttribute> JobAttributes { get; } = new List<IppAttribute>();
        public List<IppAttribute> PrinterAttributes { get; } = new List<IppAttribute>();
        public List<IppAttribute> UnsupportedAttributes { get; } = new List<IppAttribute>();
        public List<IppAttribute> OtherAttributes { get; } = new List<IppAttribute>();

        public bool IsSuccessfulStatusCode =>
            (short) StatusCode >= (short) IppStatusCode.SuccessfulOk &&
            (short) StatusCode <= (short) IppStatusCode.SuccessfulOkEventsComplete;

        public IDictionary<string, IppAttribute[]> Attributes =>
            OperationAttributes.Concat(JobAttributes)
               .Concat(PrinterAttributes)
               .Concat(UnsupportedAttributes)
               .Concat(OtherAttributes)
               .GroupBy(x => x.Name)
               .ToDictionary(g => g.Key, g => g.ToArray());

        public override string ToString() =>
            $"{nameof(Version)}: {Version}\n{nameof(StatusCode)}: {StatusCode}\n{nameof(RequestId)}: {RequestId}\nAttributes:\n{string.Join("\n", Attributes.Values.SelectMany(s => s))}";
    }
}