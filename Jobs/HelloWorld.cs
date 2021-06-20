using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace QuartzServiceWithDBLogs.Jobs
{
    public class HelloWorld : IJob
    {
        private readonly ILogger<HelloWorld> Logger;

        public HelloWorld(ILogger<HelloWorld> logger)
        {
            Logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            Logger.LogInformation($"Hello World! It's {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
