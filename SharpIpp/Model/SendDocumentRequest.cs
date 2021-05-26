using System;
using System.Collections.Generic;
using System.IO;

namespace SharpIpp.Model
{
    /// <summary>
    ///     https://tools.ietf.org/html/rfc2911#section-3.3.1
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