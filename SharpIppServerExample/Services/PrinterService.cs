using Quartz;

namespace SharpIppServerExample.Services
{
    public class PrinterService : IJob
    {
        private readonly PrinterJobsService _printerService;

        public PrinterService( PrinterJobsService printerService)
        {
            _printerService = printerService;
        }

        public Task Execute( IJobExecutionContext context )
        {
            return Task.CompletedTask;
        }
    }
}
