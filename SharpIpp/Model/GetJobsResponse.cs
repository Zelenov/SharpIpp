using System.Collections.Generic;

namespace SharpIpp.Model
{
    public class GetJobsResponse
    {
        public IppVersion IppVersion { get; set; } = IppVersion.V11;
        public int RequestId { get; set; } = 1;
        public IDictionary<string, IppAttribute[]> AllAttributes { get; set; } = null!;
    }
}