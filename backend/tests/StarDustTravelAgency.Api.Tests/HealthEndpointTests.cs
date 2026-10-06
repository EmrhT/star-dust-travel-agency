using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

// Verifies API health and OpenAPI routes through an in-memory test server.
// A custom Program factory isolates readiness behavior from real PostgreSQL.
namespace StarDustTravelAgency.Api.Tests;

/// <summary>
/// Calls Program endpoints through UnavailablePostgresApiFactory.
/// It verifies both health-check classes and Swagger registration.
/// </summary>
public sealed class HealthEndpointTests
    : IClassFixture<UnavailablePostgresApiFactory>
{
    private readonly HttpClient _client;

    // Creates an HTTP client backed by the shared unavailable-database factory.
    public HealthEndpointTests(UnavailablePostgresApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Verifies ProcessLivenessHealthCheck without a PostgreSQL dependency.
    /// </summary>
    [Fact]
    public async Task Live_is_healthy_when_postgres_is_unavailable()
    {
        var response = await _client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Verifies PostgresReadinessHealthCheck reports its failed dependency.
    /// </summary>
    [Fact]
    public async Task Ready_is_unhealthy_when_postgres_is_unavailable()
    {
        var response = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    /// <summary>
    /// Verifies Program exposes Swagger when its configuration enables it.
    /// </summary>
    [Fact]
    public async Task OpenApi_document_is_available_when_enabled()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

/// <summary>
/// Boots Program through WebApplicationFactory with unreachable PostgreSQL.
/// HealthEndpointTests uses its HttpClient to exercise API routes.
/// </summary>
public sealed class UnavailablePostgresApiFactory
    : WebApplicationFactory<Program>
{
    // Overrides database and Swagger settings before the test host starts.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            "ConnectionStrings:Postgres",
            "Host=127.0.0.1;Port=65432;Database=star_dust;" +
            "Username=star_dust;Password=test;Timeout=1;Command Timeout=1");
        builder.UseSetting("Swagger:Enabled", "true");
    }
}
