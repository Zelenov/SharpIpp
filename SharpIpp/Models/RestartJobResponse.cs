using System.Collections.Generic;

using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     https://tools.ietf.org/html/rfc2911#section-3.3.7
    /// </summary>
    public class RestartJobResponse : IIppResponseMessage
    {
        public IppVersion Version { get; set; } = IppVersion.V11;

        public IppStatusCode StatusCode { get; set; }

        public int RequestId { get; set; }

        public List<IppSection> Sections { get; } = new List<IppSection>();
    }
}
