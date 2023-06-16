using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;

namespace SharpIppServerExample.Models;

public class PrinterJob
{
    public PrinterJob(int id)
    {
        Id = id;
    }
    public int Id { get; }
    public List<IIppRequest> Requests { get; set; } = new List<IIppRequest>();
    public JobState State { get; set; }
}
