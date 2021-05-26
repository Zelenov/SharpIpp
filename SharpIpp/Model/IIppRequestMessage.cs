using System.Collections.Generic;

namespace SharpIpp.Model
{
    public interface IIppRequestMessage
    {
        IppVersion Version { get; set; }
        IppOperation IppOperation { get; set; }
        int RequestId { get; set; }
        List<IppAttribute> OperationAttributes { get; }
        List<IppAttribute> JobAttributes { get; }
    }
}