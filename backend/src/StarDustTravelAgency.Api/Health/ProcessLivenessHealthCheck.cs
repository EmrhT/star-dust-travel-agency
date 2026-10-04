using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace StarDustTravelAgency.Api.Health;

public sealed class ProcessLivenessHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("The API process is running."));
    }
}
