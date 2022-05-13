using System;
using System.Collections.Generic;
using System.IO;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     <a href="https://tools.ietf.org/html/rfc2911#section-3.3.1">Send-Document Operation</a>
    ///     This OPTIONAL operation allows a client to create a multi-document
    ///     Job object that is initially "empty" (contains no documents).  In the
    ///     Create-Job response, the Printer object returns the Job object's URI
    ///     (the "job-uri" attribute) and the Job object's 32-bit identifier (the
    ///     "job-id" attribute).  For each new document that the client desires
    ///     to add, the client uses a Send-Document operation.  Each Send-
    ///     Document Request contains the entire stream of document data for one
    ///     document.
    /// </summary>
    public class SendDocumentRequest : IIppDocumentSequenceRequest
    {
        public Stream? Document { get; set; }

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
