using System;
using System.Collections.Generic;

namespace SharpIpp.Model
{
    public class GetJobAttributesRequest
    {
        public IppVersion IppVersion { get; set; } = IppVersion.V11;
        public string RequestingUserName { get; set; } = "anonymous";
        public Uri PrinterUri { get; set; } = null!;
        public int RequestId { get; set; } = 1;
        public Uri? JobUrl { get; set; }
        public int? JobId { get; set; }
        public IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }
        public IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }
}