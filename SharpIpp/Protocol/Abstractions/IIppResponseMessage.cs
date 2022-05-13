using System.Collections.Generic;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Protocol
{
    public interface IIppResponseMessage
    {
        IppVersion Version { get; set; }

        IppStatusCode StatusCode { get; set; }

        int RequestId { get; set; }

        List<IppSection> Sections { get; }
    }
}
