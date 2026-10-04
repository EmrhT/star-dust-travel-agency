using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace StarDustTravelAgency.Api.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController(NpgsqlDataSource dataSource) : ControllerBase
{
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

public sealed record SystemStatusResponse(
    string Service,
    string Database,
    DateTimeOffset CheckedAtUtc);
