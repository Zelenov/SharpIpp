using System;
using System.Collections.Generic;
using System.IO;

namespace SharpIpp.Model
{
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