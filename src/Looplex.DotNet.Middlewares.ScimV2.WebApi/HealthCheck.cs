using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Looplex.DotNet.Middlewares.ScimV2.WebApi;

public class HealthCheck() : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
    }
}
