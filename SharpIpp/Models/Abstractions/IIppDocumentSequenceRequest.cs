namespace SharpIpp.Models
{
    public interface IIppDocumentSequenceRequest : IIppJobRequest
    {
        bool LastDocument { get; set; }
    }
}
