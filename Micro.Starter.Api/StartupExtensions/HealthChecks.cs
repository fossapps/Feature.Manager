using System.Linq;
using System.Threading.Tasks;
using Micro.Starter.Api.HealthCheck;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Micro.Starter.Api.StartupExtensions
{
    public static class HealthChecks
    {
        public static void ConfigureHealthChecks(this IServiceCollection services)
        {
            services
                .AddHealthChecks()
                .AddCheck<ConnectionToDbCheck>(nameof(ConnectionToDbCheck))
                .AddCheck<MemoryCheck>(nameof(MemoryCheck));
        }
        public static void ConfigureHealthCheckEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = WriteResponse,
                AllowCachingResponses = false,
            });
        }

        private static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(
                            p => new JProperty(p.Key, p.Value))))))))));
            return httpContext.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }


    }
}