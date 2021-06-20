using Quartz;

namespace QuartzServiceWithDBLogs.Extensions
{
    [DisallowConcurrentExecution]
    public static class QuartzExtensions
    {
        public static void RegisterJobToGroupWithSchedule<T>(
            this IServiceCollectionQuartzConfigurator quartz,
            string name,
            string group,
            string cronSchedule
        ) where T : IJob
        {
            JobKey key = new JobKey(name, group);
            quartz.AddJob<T>(options => options.WithIdentity(key));
            quartz.AddTrigger(options =>
                options.ForJob(key)
                .WithIdentity($"{name}-Trigger")
                .WithCronSchedule(cronSchedule)
            );
        }
    }
}
