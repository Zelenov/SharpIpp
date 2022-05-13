using System;
using System.Collections.Generic;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     <a href="https://tools.ietf.org/html/rfc2911#section-3.3.2">Send-URI Operation</a>
    ///     This OPTIONAL operation is identical to the Send-Document operation
    ///     except that a client MUST supply a URI reference
    ///     ("document-uri" operation attribute) rather than the document data
    ///     itself.  If a Printer object supports this operation, clients can use
    ///     both Send-URI or Send-Document operations to add new documents to an
    ///     existing multi-document Job object.  However, if a client needs to
    ///     indicate that the previous Send-URI or Send-Document was the last
    ///     document,  the client MUST use the Send-Document operation with no
    ///     document data and the "last-document" flag set to 'true' (rather than
    ///     using a Send-URI operation with no "document-uri" operation
    ///     attribute).
    /// </summary>
    public class SendUriRequest : IIppDocumentSequenceRequest
    {
        public Uri? DocumentUri { get; set; }

        public DocumentAttributes? DocumentAttributes { get; set; }

        public IppVersion Version { get; set; } = IppVersion.V11;

        public int RequestId { get; set; } = 1;

        public Uri PrinterUri { get; set; } = null!;

        public Uri? JobUrl { get; set; }

        public int? JobId { get; set; }

        public bool LastDocument { get; set; }

        public string? RequestingUserName { get; set; }

        public IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }

        public IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }
}
