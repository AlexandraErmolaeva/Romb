using App.Metrics.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Romb.Application.HealthChecks
{
    public class RedisHealthCheck(
        ILogger<RedisHealthCheck> logger,
    IOptions<HealthOptions> options,
    IConfiguration conf)
    : IHealthCheck
    {
        Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> IHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
