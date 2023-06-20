using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;

namespace SharpIppServerExample.Models;

public class PrinterJob
{
    public PrinterJob(int id, string? userName, DateTimeOffset createdDateTime )
    {
        Id = id;
        UserName = userName;
        CreatedDateTime = createdDateTime;
    }
    public int Id { get; }
    public string? UserName { get; }
    public List<IIppRequest> Requests { get; set; } = new List<IIppRequest>();
    public DateTimeOffset CreatedDateTime { get; }
    public DateTimeOffset? CompletedDateTime { get; set; }
    public DateTimeOffset? ProcessingDateTime { get; set; }
}
