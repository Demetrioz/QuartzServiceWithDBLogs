using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuartzServiceWithDBLogs
{
    public class NotificationWorker : BackgroundService
    {
        private readonly ILogger<NotificationWorker> Logger;
        public NotificationWorker(ILogger<NotificationWorker> logger)
        {
            Logger = logger;

            // Send notification that service has started
            Logger.LogInformation($"Worker started at {DateTimeOffset.Now}");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            // Send notification that service has stopped
            Logger.LogInformation($"Worker stopped at {DateTimeOffset.Now}");
            return base.StopAsync(cancellationToken);
        }
    }
}
