using SharpIpp.Models;
using SharpIpp.Protocol.Models;

namespace SharpIppServerExample.Models;

public class PrinterJob : IEquatable<PrinterJob>, IDisposable, IAsyncDisposable
{
    private bool disposedValue;

    public PrinterJob( int id, string? userName, DateTimeOffset createdDateTime )
    {
        Id = id;
        UserName = userName;
        CreatedDateTime = createdDateTime;
    }

    /// <summary>
    /// Create shallow copy of the original object
    /// </summary>
    /// <param name="printerJob"></param>
    public PrinterJob( PrinterJob printerJob )
    {
        Id = printerJob.Id;
        UserName = printerJob.UserName;
        CreatedDateTime = printerJob.CreatedDateTime;
        CompletedDateTime = printerJob.CompletedDateTime;
        ProcessingDateTime = printerJob.ProcessingDateTime;
        State = printerJob.State;
        Requests = printerJob.Requests;
    }

    public int Id { get; }
    public JobState? State { get; private set; }
    public string? UserName { get; }
    public List<IIppRequest> Requests { get; set; } = new List<IIppRequest>();
    public DateTimeOffset CreatedDateTime { get; }
    public DateTimeOffset? CompletedDateTime { get; set; }
    public DateTimeOffset? ProcessingDateTime { get; set; }
    public bool IsNew => !State.HasValue && !ProcessingDateTime.HasValue;
    public bool IsHold => !State.HasValue && ProcessingDateTime.HasValue;

    public bool Equals( PrinterJob? other )
    {
        return other != null
            && Id == other.Id
            && State == other.State
            && UserName == other.UserName
            && CreatedDateTime == other.CreatedDateTime
            && CompletedDateTime == other.CompletedDateTime
            && ProcessingDateTime == other.ProcessingDateTime
            && other.Requests.SequenceEqual( Requests );
    }

    public override bool Equals( object? obj )
    {
        return ReferenceEquals( this, obj ) || obj is PrinterJob other && Equals( other );
    }

    public override int GetHashCode()
    {
        return HashCode.Combine( Id, State, Requests );
    }

    public static bool operator ==( PrinterJob? left, PrinterJob? right )
    {
        return Equals( left, right );
    }

    public static bool operator !=( PrinterJob? left, PrinterJob? right )
    {
        return !Equals( left, right );
    }

    public async Task<bool> TrySetStateAsync( JobState? state, DateTimeOffset dateTime )
    {
        switch (state)
        {
            case null when State == JobState.Pending:
                State = state;
                return true;
            case JobState.Pending when !State.HasValue || State == JobState.Aborted:
                State = state;
                return true;
            case JobState.Processing when State == JobState.Pending:
                State = state;
                ProcessingDateTime = dateTime;
                return true;
            case JobState.Canceled when !State.HasValue || State == JobState.Pending:
                await ClearDocumentStreamsAsync();
                State = state;
                ProcessingDateTime = dateTime;
                CompletedDateTime = dateTime;
                return true;
            case JobState.Completed when State == JobState.Processing:
                await ClearDocumentStreamsAsync();
                State = state;
                CompletedDateTime = dateTime;
                return true;
            case JobState.Aborted when State == JobState.Processing:
                State = state;
                CompletedDateTime = dateTime;
                return true;
            default:
                return false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait( false );
        Dispose( disposing: false );
        GC.SuppressFinalize( this );
    }

    protected virtual async ValueTask ClearDocumentStreamsAsync()
    {
        foreach (var ippRequest in Requests)
        {
            switch (ippRequest)
            {
                case PrintJobRequest printJobRequest when printJobRequest.Document != null:
                    await printJobRequest.Document.DisposeAsync().ConfigureAwait( false );
                    printJobRequest.Document = Stream.Null;
                    break;
                case SendDocumentRequest sendDocumentRequest when sendDocumentRequest.Document != null:
                    await sendDocumentRequest.Document.DisposeAsync().ConfigureAwait( false );
                    sendDocumentRequest.Document = Stream.Null;
                    break;
            }
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        foreach (var ippRequest in Requests)
        {
            switch (ippRequest)
            {
                case PrintJobRequest printJobRequest when printJobRequest.Document != null:
                    await printJobRequest.Document.DisposeAsync().ConfigureAwait( false );
                    break;
                case SendDocumentRequest sendDocumentRequest when sendDocumentRequest.Document != null:
                    await sendDocumentRequest.Document.DisposeAsync().ConfigureAwait( false );
                    break;
            }
        }
        Requests.Clear();
    }

    protected virtual void Dispose( bool disposing )
    {
        if (disposedValue)
            return;
        if (disposing)
        {
            foreach (var ippRequest in Requests)
            {
                switch (ippRequest)
                {
                    case PrintJobRequest printJobRequest when printJobRequest.Document != null:
                        printJobRequest.Document.Dispose();
                        break;
                    case SendDocumentRequest sendDocumentRequest when sendDocumentRequest.Document != null:
                        sendDocumentRequest.Document.Dispose();
                        break;
                }
            }
            Requests.Clear();
        }
        disposedValue = true;
    }

    public void Dispose()
    {
        Dispose( disposing: true );
        GC.SuppressFinalize( this );
    }
}
