using Microsoft.Extensions.Diagnostics.HealthChecks;

// Implements an in-process liveness check registered by Program.
// Container health monitoring calls it without depending on PostgreSQL.
namespace StarDustTravelAgency.Api.Health;

/// <summary>
/// Supplies the dependency-free process health result.
/// Program registers it under the HealthCheckTags.Live group.
/// </summary>
public sealed class ProcessLivenessHealthCheck : IHealthCheck
{
    // Returns a healthy result for the liveness endpoint mapped in Program.
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            HealthCheckResult.Healthy("The API process is running."));
    }
}
