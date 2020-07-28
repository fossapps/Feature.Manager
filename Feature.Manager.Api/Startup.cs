using System.Text.Json.Serialization;
using Feature.Manager.Api.Configs;
using Feature.Manager.Api.StartupExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Feature.Manager.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConfiguration(Configuration);
            services.AddMetrics();
            services.ConfigureRequiredDependencies();
            services.ConfigureHealthChecks();
            services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.SetupCors(Configuration.GetSection("CorsConfig").Get<CorsConfig>());
            services.ConfigureSwagger();
            services.RegisterWorker();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<CorsConfig> corsConfig)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseCorsPolicy(corsConfig.Value);
            app.UseAuthorization();
            app.AddSwaggerWithUi();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.ConfigureHealthCheckEndpoint();
            });
        }
    }
}
