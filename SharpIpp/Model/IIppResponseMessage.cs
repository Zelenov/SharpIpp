using System.Collections.Generic;

namespace SharpIpp.Model
{
    public interface IIppResponseMessage
    {
        IppVersion Version { get; set; }
        IppStatusCode StatusCode { get; set; }
        int RequestId { get; set; }
        List<IppSection> Sections { get; }
    }
}