using Micro.Starter.Api.Models;
using Micro.Starter.Api.Repository;
using Micro.Starter.Api.Uuid;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Starter.Api.StartupExtensions
{
    public static class DependencyInjection
    {
        public static void ConfigureRequiredDependencies(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>();
            services.AddScoped<IWeatherRepository, WeatherRepository>();
            services.AddSingleton<IUuidService, UuidService>();
        }
    }
}