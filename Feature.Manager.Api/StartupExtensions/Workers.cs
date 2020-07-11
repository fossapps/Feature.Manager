using Feature.Manager.Api.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Feature.Manager.Api.StartupExtensions
{
    public static class Workers
    {
        public static void RegisterWorker(this IServiceCollection services)
        {
            services.AddHostedService<Worker>();
        }
    }
}
