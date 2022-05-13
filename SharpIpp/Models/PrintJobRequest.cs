using System;
using System.Collections.Generic;
using System.IO;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     <a href="https://datatracker.ietf.org/doc/html/rfc2911#section-3.2.1">Print-Job Operation</a>
    ///     This REQUIRED operation allows a client to submit a print job with
    ///     only one document and supply the document data (rather than just a
    ///     reference to the data).
    /// </summary>
    public class PrintJobRequest : IIppPrinterRequest
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
