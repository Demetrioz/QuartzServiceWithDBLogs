using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using Quartz;
using QuartzServiceWithDBLogs.Config;
using QuartzServiceWithDBLogs.Extensions;
using QuartzServiceWithDBLogs.Jobs;
using QuartzServiceWithDBLogs.Migrations;
using System;
using System.Threading.Tasks;

namespace QuartzServiceWithDBLogs
{
    public class Program
    {
        private static IConfiguration Configuration { get; set; }

        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    Configuration = config.AddJsonFile("appsettings.json", optional: false)
                        .AddJsonFile($"appsettings.{env}.json", optional: false)
                        .AddEnvironmentVariables()
                        .Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    QuartzSettings settings = new QuartzSettings();
                    Configuration.GetSection("QuartzSettings").Bind(settings);

                    // Make sure the database is available
                    services
                        .AddFluentMigratorCore()
                        .Configure<AssemblySourceOptions>(x => x.AssemblyNames = new[] { typeof(Version0001_Setup).Assembly.GetName().Name })
                        .ConfigureRunner(rb =>
                            rb.AddSqlServer()
                            .WithGlobalConnectionString(settings.ConnectionStrings["Database"])
                            .ScanIn(typeof(Version0001_Setup).Assembly).For.Migrations()
                        )
                        .AddLogging(lb => lb.AddFluentMigratorConsole())
                        .Configure<FluentMigratorLoggerOptions>(options =>
                         {
                             options.ShowSql = true;
                             options.ShowElapsedTime = true;
                         });

                    using (var scope = services.BuildServiceProvider().CreateScope())
                    {
                        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                        runner.MigrateUp();
                    }

                    // Add Quartz
                    services.Configure<QuartzOptions>(options =>
                    {
                        options.Scheduling.IgnoreDuplicates = true;
                        options.Scheduling.OverWriteExistingData = true;
                    });

                    // Configure Quartz
                    services.AddQuartz(q =>
                    {
                        q.SchedulerName = settings.InstanceName;

                        // Use SQL for job information
                        q.UsePersistentStore(options =>
                        {
                            options.UseProperties = true;
                            options.RetryInterval = TimeSpan.FromSeconds(15);
                            options.UseSqlServer(server =>
                            {
                                server.ConnectionString = settings.ConnectionStrings["Database"];
                                server.TablePrefix = "QRTZ_";
                            });
                            options.UseJsonSerializer();
                        });

                        // Allow scoped services, not just singleton and transient
                        q.UseMicrosoftDependencyInjectionScopedJobFactory();

                        // Register our jobs
                        q.RegisterJobToGroupWithSchedule<HelloWorld>(
                            "HelloWorld", 
                            "Testing",
                            settings.HelloWorldSchedule
                        );
                    });

                    services.AddHostedService<NotificationWorker>();

                    // Make sure Quartz waits for jobs to end before exiting
                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
                })
                .UseNLog();
    }
}