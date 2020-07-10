using Micro.Starter.Api.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Starter.Api.StartupExtensions
{
    public static class Workers
    {
        public static void RegisterWorker(this IServiceCollection services)
        {
            services.AddHostedService<Worker>();
        }
    }
}
