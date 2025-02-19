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
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
