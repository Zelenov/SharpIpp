using SharpIpp.Mapping;
using SharpIpp.Mapping.Profiles;
using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpIpp;

public partial class SharpIppServer
{
    private static readonly Lazy<IMapper> MapperSingleton;
    private readonly IIppProtocol _ippProtocol = new IppProtocol();
    private IMapper Mapper => MapperSingleton.Value;

    static SharpIppServer()
    {
        MapperSingleton = new Lazy<IMapper>( MapperFactory );
    }

    public Task<IIppRequestMessage> ReceiveRawRequestAsync(
        Stream stream,
        CancellationToken cancellationToken = default )
    {
        return _ippProtocol.ReadIppRequestAsync( stream, cancellationToken );
    }

    public async Task<IIppRequest> ReceiveRequestAsync(
        Stream stream,
        CancellationToken cancellationToken = default )
    {
        var request = await ReceiveRawRequestAsync( stream, cancellationToken );
        return request.IppOperation switch
        {
            IppOperation.CancelJob => Mapper.Map<IIppRequestMessage, CancelJobRequest>( request ),
            IppOperation.CreateJob => Mapper.Map<IIppRequestMessage, CreateJobRequest>( request ),
            IppOperation.GetCUPSPrinters => Mapper.Map<IIppRequestMessage, CUPSGetPrintersRequest>( request ),
            IppOperation.GetJobAttributes => Mapper.Map<IIppRequestMessage, GetJobAttributesRequest>( request ),
            IppOperation.GetJobs => Mapper.Map<IIppRequestMessage, GetJobsRequest>( request ),
            IppOperation.GetPrinterAttributes => Mapper.Map<IIppRequestMessage, GetPrinterAttributesRequest>( request ),
            IppOperation.HoldJob => Mapper.Map<IIppRequestMessage, HoldJobRequest>( request ),
            IppOperation.PausePrinter => Mapper.Map<IIppRequestMessage, PausePrinterRequest>( request ),
            IppOperation.PrintJob => Mapper.Map<IIppRequestMessage, PrintJobRequest>( request ),
            IppOperation.PrintUri => Mapper.Map<IIppRequestMessage, PrintUriRequest>( request ),
            IppOperation.PurgeJobs => Mapper.Map<IIppRequestMessage, PurgeJobsRequest>( request ),
            IppOperation.ReleaseJob => Mapper.Map<IIppRequestMessage, ReleaseJobRequest>( request ),
            IppOperation.RestartJob => Mapper.Map<IIppRequestMessage, RestartJobRequest>( request ),
            IppOperation.ResumePrinter => Mapper.Map<IIppRequestMessage, ResumePrinterRequest>( request ),
            IppOperation.SendDocument => Mapper.Map<IIppRequestMessage, SendDocumentRequest>( request ),
            IppOperation.SendUri => Mapper.Map<IIppRequestMessage, SendUriRequest>( request ),
            IppOperation.ValidateJob => Mapper.Map<IIppRequestMessage, ValidateJobRequest>( request ),
            _ => throw new NotImplementedException( $"Unable to handle {request.IppOperation} operation" )
        };
    }

    public Task SendRawResponseAsync(
        IIppResponseMessage ippResponseMessage,
        Stream stream,
        CancellationToken cancellationToken = default )
    {
        if ( ippResponseMessage == null )
        {
            throw new ArgumentException( $"{nameof( ippResponseMessage )}" );
        }
        if ( stream == null )
        {
            throw new ArgumentException( $"{nameof( stream )}" );
        }
        return _ippProtocol.WriteIppResponseAsync( ippResponseMessage, stream, cancellationToken );
    }

    public Task SendResponseAsync<T>(
        T ippResponsMessage,
        Stream stream,
        CancellationToken cancellationToken = default ) where T : IIppResponseMessage
    {
        if ( ippResponsMessage == null )
        {
            throw new ArgumentException( $"{nameof( ippResponsMessage )}" );
        }
        if ( stream == null )
        {
            throw new ArgumentException( $"{nameof( stream )}" );
        }
        var ippResponse = Mapper.Map<IppResponseMessage>( ippResponsMessage );
        return _ippProtocol.WriteIppResponseAsync( ippResponse, stream, cancellationToken );
    }

    private static IMapper MapperFactory()
    {
        var mapper = new SimpleMapper();
        var assembly = Assembly.GetAssembly( typeof( TypesProfile ) );
        mapper.FillFromAssembly( assembly! );
        return mapper;
    }
}
