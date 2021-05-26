using System.Collections.Generic;
using System.IO;

namespace SharpIpp.Model
{
    public class IppRequestMessage : IIppRequestMessage
    {
        public Stream? Document { get; set; }
        public IppVersion Version { get; set; }
        public IppOperation IppOperation { get; set; }
        public int RequestId { get; set; }
        public List<IppAttribute> OperationAttributes { get; } = new List<IppAttribute>();
        public List<IppAttribute> JobAttributes { get; } = new List<IppAttribute>();
    }
}