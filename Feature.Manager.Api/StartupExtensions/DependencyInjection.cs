using Feature.Manager.Api.Models;
using Feature.Manager.Api.Uuid;
using Microsoft.Extensions.DependencyInjection;

namespace Feature.Manager.Api.StartupExtensions
{
    public static class DependencyInjection
    {
        public static void ConfigureRequiredDependencies(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>();
            services.AddSingleton<IUuidService, UuidService>();
        }
    }
}