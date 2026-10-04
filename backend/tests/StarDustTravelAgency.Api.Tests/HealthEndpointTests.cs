using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace StarDustTravelAgency.Api.Tests;

public sealed class HealthEndpointTests : IClassFixture<UnavailablePostgresApiFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(UnavailablePostgresApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Live_is_healthy_when_postgres_is_unavailable()
    {
        var response = await _client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Ready_is_unhealthy_when_postgres_is_unavailable()
    {
        var response = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task OpenApi_document_is_available_when_enabled()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public sealed class UnavailablePostgresApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting(
            "ConnectionStrings:Postgres",
            "Host=127.0.0.1;Port=65432;Database=star_dust;Username=star_dust;Password=test;Timeout=1;Command Timeout=1");
        builder.UseSetting("Swagger:Enabled", "true");
    }
}
