using System.Collections.Generic;

using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    public class CUPSGetPrintersResponse : IIppResponseMessage
    {
        public JobAttributes[] Jobs { get; set; } = null!;

        public IppVersion Version { get; set; } = IppVersion.CUPS10;

        public IppStatusCode StatusCode { get; set; }

        public int RequestId { get; set; } = 1;

        public List<IppSection> Sections { get; } = new List<IppSection>();
    }
}
