using System.Linq;
using Feature.Manager.Api.Configs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Feature.Manager.Api.StartupExtensions
{
    public static class Cors
    {
        public static void SetupCors(this IServiceCollection services, CorsConfig config)
        {
            services.AddCors(x =>
            {
                x.AddPolicy("development", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
                x.AddPolicy("production", builder =>
                {
                    builder.WithOrigins(config.Origins.ToArray())
                        .WithMethods(config.Headers.ToArray())
                        .AllowAnyMethod();
                    if (!config.AllowCredentials)
                    {
                        builder.DisallowCredentials();
                        return;
                    }
                    builder.AllowCredentials();
                });
            });
        }

        public static void UseCorsPolicy(this IApplicationBuilder app, CorsConfig config)
        {
            app.UseCors(config.PolicyToUse);
        }
    }
}
