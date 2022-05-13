using System;
using System.Collections.Generic;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     <a href="https://datatracker.ietf.org/doc/html/rfc2911#section-3.3.4">Get-Job-Attributes Operation</a>
    ///     This REQUIRED operation allows a client to request the values of
    ///     attributes of a Job object and it is almost identical to the Get-
    ///     Printer-Attributes operation.  The only
    ///     differences are that the operation is directed at a Job object rather
    ///     than a Printer object, there is no "document-format" operation
    ///     attribute used when querying a Job object, and the returned attribute
    ///     group is a set of Job object attributes rather than a set of Printer
    ///     object attributes.
    /// </summary>
    public class GetPrinterAttributesRequest : IIppPrinterRequest
    {
        public IEnumerable<string>? RequestedAttributes { get; set; }

        public IppVersion Version { get; set; } = IppVersion.V11;

        public int RequestId { get; set; } = 1;

        public string? RequestingUserName { get; set; }

        public Uri PrinterUri { get; set; } = null!;

        public IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }

        public IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }
}
