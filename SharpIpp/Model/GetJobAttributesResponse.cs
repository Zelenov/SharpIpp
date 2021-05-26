using System.Collections.Generic;

namespace SharpIpp.Model
{
    public class GetJobAttributesResponse : IIppResponseMessage
    {
        public JobAttributes JobAttributes { get; set; } = null!;
        public IppVersion Version { get; set; } = IppVersion.V11;
        public IppStatusCode StatusCode { get; set; }
        public int RequestId { get; set; }
        public List<IppSection> Sections { get; } = new List<IppSection>();
    }
}