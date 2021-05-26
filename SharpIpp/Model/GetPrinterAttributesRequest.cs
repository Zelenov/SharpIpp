using System;
using System.Collections.Generic;

namespace SharpIpp.Model
{
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