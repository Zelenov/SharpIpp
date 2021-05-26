using System.Collections.Generic;

namespace SharpIpp.Model
{
    public interface IIppRequest
    {
        IppVersion Version { get; set; }
        int RequestId { get; set; }
        string? RequestingUserName { get; set; }
        IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }
        IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }
}