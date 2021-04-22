using System.Collections.Generic;

namespace SharpIpp.Model
{
    public class IppRequest
    {
        public IppVersion IppVersion { get; set; }
        public IppOperation IppOperation { get; set; }
        public int RequestId { get; set; }
        public List<IppAttribute> OperationAttributes { get; } = new List<IppAttribute>();
        public List<IppAttribute> JobAttributes { get; } = new List<IppAttribute>();
    }
}