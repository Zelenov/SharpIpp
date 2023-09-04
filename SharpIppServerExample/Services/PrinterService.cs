using Microsoft.AspNetCore.StaticFiles;
using Quartz;
using SharpIpp.Models;
using SharpIpp.Protocol.Models;
using System;
using System.Drawing.Printing;
using System.Security.Policy;
using static Quartz.Logging.OperationName;

namespace SharpIppServerExample.Services
{
    public class PrinterService : IJob
    {
        private readonly PrinterJobsService _printerJobService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<PrinterService> _logger;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public PrinterService(
            PrinterJobsService printerService,
            IWebHostEnvironment env,
            ILogger<PrinterService> logger)
        {
            _printerJobService = printerService;
            _env = env;
            _logger = logger;
            _contentTypeProvider = new FileExtensionContentTypeProvider();
            Directory.CreateDirectory( Path.Combine( _env.ContentRootPath, "jobs" ) );
        }

        public async Task Execute( IJobExecutionContext context )
        {
            var job = await _printerJobService.GetPendingJobAsync();
            if ( job == null )
                return;
            try
            {
                for ( var i = 0; i < job.Requests.Count; i++ )
                {
                    var prefix = $"{job.Id}.{i}";
                    switch (job.Requests[i])
                    {
                        case PrintJobRequest printJobRequest:
                            await SaveAsync( prefix, printJobRequest );
                            break;
                        case SendDocumentRequest sendJobRequest:
                            await SaveAsync( prefix, sendJobRequest );
                            break;
                        case SendUriRequest sendUriRequest:
                            await SaveAsync( prefix, sendUriRequest );
                            break;
                    }
                }
                await _printerJobService.AddCompletedJobAsync( job.Id );
                _logger.LogDebug( "Job {id} has been finished", job.Id );
            }
            catch ( Exception ex )
            {
                _logger.LogError( ex, "Unable to finish job" );
                await _printerJobService.AddFailedJobAsync( job.Id );
            }
        }

        private async Task SaveAsync( string prefix, PrintJobRequest request )
        {
            if ( request.Document == null )
                return;
            request.Document.Seek( 0, SeekOrigin.Begin );
            await SaveAsync( request.Document, GetFileName( prefix, request.DocumentAttributes ) );
            await request.Document.DisposeAsync();
        }

        private async Task SaveAsync( string prefix, SendDocumentRequest request )
        {
            if ( request.Document == null )
                return;
            request.Document.Seek( 0, SeekOrigin.Begin );
            await SaveAsync( request.Document, GetFileName( prefix, request.DocumentAttributes ) );
            await request.Document.DisposeAsync();
        }

        private async Task SaveAsync( string prefix, SendUriRequest request )
        {
            if(request.DocumentUri == null)
                return;
            using var client = new HttpClient();
            using var result = await client.GetAsync( request.DocumentUri );
            if ( !result.IsSuccessStatusCode )
                return;
            using var stream = await result.Content.ReadAsStreamAsync();
            await SaveAsync( stream, GetFileName( prefix, request.DocumentAttributes, Path.GetFileNameWithoutExtension( request.DocumentUri.LocalPath ), Path.GetExtension( request.DocumentUri.LocalPath ) ) );
        }

        private string GetFileName(string prefix, DocumentAttributes? documentAttributes, string? alternativeDocumentName = null, string? alternativeExtension = null )
        {
            var extension = documentAttributes?.DocumentFormat == null
                ? null
                : _contentTypeProvider.Mappings.Where( x => x.Value == documentAttributes.DocumentFormat ).Select( x => x.Key ).FirstOrDefault();
            return $"{prefix}_{documentAttributes?.DocumentName ?? alternativeDocumentName ?? "no-name"}{extension ?? alternativeExtension ?? ".unknown"}";
        }

        private async Task SaveAsync(Stream stream, string fileName)
        {
            var path = Path.Combine( _env.ContentRootPath, "jobs", fileName );
            using var fileStream = new FileStream( path, FileMode.OpenOrCreate );
            await stream.CopyToAsync( fileStream );
        }
    }
}
