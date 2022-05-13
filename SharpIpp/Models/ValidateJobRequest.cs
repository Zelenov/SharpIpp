using System;
using System.Collections.Generic;
using System.IO;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     <a href="https://datatracker.ietf.org/doc/html/rfc2911#section-3.2.3">Validate-Job Operation</a>
    ///     This REQUIRED operation is similar to the Print-Job operation
    ///     except that a client supplies no document data and
    ///     the Printer allocates no resources (i.e., it does not create a new
    ///     Job object).  This operation is used only to verify capabilities of a
    ///     printer object against whatever attributes are supplied by the client
    ///     in the Validate-Job request.  By using the Validate-Job operation a
    ///     client can validate that an identical Print-Job operation (with the
    ///     document data) would be accepted. The Validate-Job operation also
    ///     performs the same security negotiation as the Print-Job operation
    ///     (see section 8), so that a client can check that the client and
    ///     Printer object security requirements can be met before performing a
    ///     Print-Job operation.
    /// </summary>
    public class ValidateJobRequest : IIppPrinterRequest
    {
        public Stream Document { get; set; } = null!;

        public NewJobAttributes? NewJobAttributes { get; set; }

        public DocumentAttributes? DocumentAttributes { get; set; }

        public IppVersion Version { get; set; } = IppVersion.V11;

        public int RequestId { get; set; } = 1;

        public Uri PrinterUri { get; set; } = null!;

        public string? RequestingUserName { get; set; }

        public IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }

        public IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }
}
