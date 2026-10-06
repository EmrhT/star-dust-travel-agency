using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

// Implements readiness by querying the Npgsql source registered in Program.
// Docker Compose uses the resulting endpoint before starting the frontend.
namespace StarDustTravelAgency.Api.Health;

/// <summary>
/// Uses NpgsqlDataSource to test whether PostgreSQL accepts queries.
/// Program registers it under the HealthCheckTags.Ready group.
/// </summary>
public sealed class PostgresReadinessHealthCheck(
    NpgsqlDataSource dataSource) : IHealthCheck
{
    // Executes a minimal query and translates its outcome into a health result.
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var command = dataSource.CreateCommand("SELECT 1");
            await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy(
                "PostgreSQL is accepting queries.");
        }
        catch (Exception)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL is not ready.");
        }
    }
}
