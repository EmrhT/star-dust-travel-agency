using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

// Serializes ASP.NET Core health reports into consistent JSON responses.
// Program assigns this writer to both liveness and readiness endpoints.
namespace StarDustTravelAgency.Api.Health;

/// <summary>
/// Converts HealthReport instances into JSON HTTP responses.
/// Program assigns WriteAsync to both mapped health endpoints.
/// </summary>
public static class HealthResponseWriter
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerDefaults.Web);

    // Writes one HealthReport to the HTTP response configured by Program.
    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration,
            }),
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, SerializerOptions));
    }
}
