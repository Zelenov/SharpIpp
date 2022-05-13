using System.Collections.Generic;
using System.IO;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Protocol
{
    public interface IIppRequestMessage
    {
        IppVersion Version { get; set; }

        IppOperation IppOperation { get; set; }

        int RequestId { get; set; }

        List<IppAttribute> OperationAttributes { get; }

        List<IppAttribute> JobAttributes { get; }

        Stream? Document { get; set; }
    }
}
