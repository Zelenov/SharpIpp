using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Quartz;
using SharpIppServerExample.Services;

var builder = WebApplication.CreateBuilder( args );
builder.Services
    .Configure<KestrelServerOptions>( options => options.AllowSynchronousIO = true )
    .AddSingleton<PrinterJobsService>()
    .AddHttpContextAccessor()
    .AddQuartz( q =>
    {
        q.UseMicrosoftDependencyInjectionJobFactory();
        var jobKey = new JobKey( "printerQueue" );
        q.AddJob<PrinterService>( opts => opts.WithIdentity( jobKey ) );
        q.AddTrigger( opts => opts
            .ForJob( jobKey )
            .WithIdentity( $"printerQueue-trigger" )
            .WithCronSchedule( "0/10 * * * * ?" ) );
    } )
    .AddQuartzHostedService( q => q.WaitForJobsToComplete = true );

var app = builder.Build();
app.MapGet( "/", () => "IPP printer" );
app.MapPost( "/", async (HttpContext context, PrinterJobsService printerService) =>
{
    await printerService.ProcessRequestAsync( context.Request.Body, context.Response.Body );
} );
app.Run();
