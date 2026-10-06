using Microsoft.AspNetCore.Mvc;
using Npgsql;

// Exposes API connectivity status after verifying PostgreSQL with Npgsql.
// The React system client consumes the JSON response from this controller.
namespace StarDustTravelAgency.Api.Controllers;

/// <summary>
/// Uses NpgsqlDataSource to verify PostgreSQL for GET api/system/status.
/// frontend getSystemStatus consumes its SystemStatusResponse.
/// </summary>
[ApiController]
[Route("api/system")]
public sealed class SystemController(
    NpgsqlDataSource dataSource) : ControllerBase
{
    /// <summary>
    /// Queries PostgreSQL before returning data to frontend getSystemStatus.
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType<SystemStatusResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SystemStatusResponse>> GetStatus(
        CancellationToken cancellationToken)
    {
        await using var command = dataSource.CreateCommand("SELECT 1");
        await command.ExecuteScalarAsync(cancellationToken);

        return Ok(new SystemStatusResponse(
            "Star Dust Travel Agency API",
            "connected",
            DateTimeOffset.UtcNow));
    }
}

/// <summary>
/// Carries the status serialized by SystemController.GetStatus.
/// frontend SystemStatus is the corresponding TypeScript contract.
/// </summary>
public sealed record SystemStatusResponse(
    string Service,
    string Database,
    DateTimeOffset CheckedAtUtc);
