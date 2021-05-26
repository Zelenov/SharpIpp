namespace SharpIpp.Model
{
    public interface IIppDocumentSequenceRequest : IIppJobRequest
    {
        bool LastDocument { get; set; }
    }
}