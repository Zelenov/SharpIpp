using System;
using System.Collections.Generic;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    public interface IIppRequest
    {
        Uri PrinterUri { get; set; }

        IppVersion Version { get; set; }

        int RequestId { get; set; }

        string? RequestingUserName { get; set; }

        IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }

        IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }
}
