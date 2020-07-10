using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Micro.Starter.Api.HealthCheck
{
    public class MemoryCheck : IHealthCheck
    {

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var allocated = GC.GetTotalMemory(true) / (1024.0 * 1024.0);
            var data = new Dictionary<string, object>()
            {
                { "AllocatedMegabytes", allocated },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) },
            };
            var status = allocated < 10 ? HealthStatus.Healthy : HealthStatus.Degraded;
            var result = new HealthCheckResult(status, "check if memory used is more than 10MB", null, data);
            return Task.FromResult(result);
        }
    }
}
