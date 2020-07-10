using Micro.Starter.Api.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Starter.Api.StartupExtensions
{
    public static class Configuration
    {
        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseConfig>(configuration.GetSection("DatabaseConfig"));
            services.Configure<SlackLoggingConfig>(configuration.GetSection("Logging").GetSection("Slack"));
        }
    }
}
