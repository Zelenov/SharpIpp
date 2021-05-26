using System.Collections.Generic;

namespace SharpIpp.Model
{
    /// <summary>
    ///     https://tools.ietf.org/html/rfc2911#section-3.2.8
    /// </summary>
    public class ResumePrinterResponse : IIppResponseMessage
    {
        public IppVersion Version { get; set; }
        public IppStatusCode StatusCode { get; set; }
        public int RequestId { get; set; }
        public List<IppSection> Sections { get; } = new List<IppSection>();
    }
}