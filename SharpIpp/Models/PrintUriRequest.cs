using System;
using System.Collections.Generic;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     <a href="https://datatracker.ietf.org/doc/html/rfc2911#section-3.2.2">Print-URI Operation</a>
    ///     This OPTIONAL operation is identical to the Print-Job operation
    ///     except that a client supplies a URI reference to the
    ///     document data using the "document-uri" (uri) operation attribute (in
    ///     Group 1) rather than including the document data itself.  Before
    ///     returning the response, the Printer MUST validate that the Printer
    ///     supports the retrieval method (e.g., http, ftp, etc.) implied by the
    ///     URI, and MUST check for valid URI syntax.  If the client-supplied URI
    ///     scheme is not supported, i.e. the value is not in the Printer
    ///     object's "referenced-uri-scheme-supported" attribute, the Printer
    ///     object MUST reject the request and return the 'client-error-uri-
    ///     scheme-not-supported' status code.
    /// </summary>
    public class PrintUriRequest : IIppPrinterRequest
    {
        public Uri DocumentUri { get; set; } = null!;

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
