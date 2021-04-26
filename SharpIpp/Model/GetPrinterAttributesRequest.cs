using System;
using System.Collections.Generic;

namespace SharpIpp.Model
{
    public class GetPrinterAttributesRequest
    {
        public IppVersion IppVersion { get; set; } = IppVersion.V11;
        public int RequestId { get; set; } = 1;
        public Uri PrinterUri { get; set; } = null!;
        public IEnumerable<string>? RequestedAttributes { get; set; }
        public IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }
        public IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }
}