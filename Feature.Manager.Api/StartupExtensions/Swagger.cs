using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Feature.Manager.Api.StartupExtensions
{
    public static class Swagger
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.DescribeAllEnumsAsStrings();
                c.CustomOperationIds(e =>
                {
                    var methodName = e.ActionDescriptor.RouteValues["action"];
                    return $"{e.ActionDescriptor.RouteValues["controller"]}_{methodName}";
                });
                c.ResolveConflictingActions(apiDescription => apiDescription.Last());
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Title",
                    Version = "v1"
                });
            });

        }

        public static void AddSwaggerWithUi(this IApplicationBuilder app)
        {
            app.UseSwagger(x => x.SerializeAsV2 = true);
            app.UseSwaggerUI(x =>
            {
                x.DisplayOperationId();
                x.RoutePrefix = "swagger";
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                x.RoutePrefix = string.Empty;
            });
        }
    }
}
