using System;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Extensions.Configuration;
using App.Metrics.Formatters.InfluxDB;
using Micro.Starter.Api.Models;
using Micro.Starter.Api.StartupExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Micro.Starter.Api
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationContext>>();
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>().Database;
                await db.MigrateOrFail(logger);
            }
            catch (RetryLimitExceededException e)
            {
                logger.LogCritical(e, "Error connecting to database, application can't start");
                Environment.ExitCode = 1;
                return;
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureMetricsWithDefaults((context, builder) =>
                        {
                            builder.Configuration.ReadFrom(context.Configuration);
                            builder.Report.ToInfluxDb(options =>
                            {
                                options.FlushInterval = TimeSpan.FromSeconds(5);
                                context.Configuration.GetSection("MetricsOptions").Bind(options);
                                options.MetricsOutputFormatter = new MetricsInfluxDbLineProtocolOutputFormatter();
                            });
                        });
                    webBuilder.UseMetrics();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
