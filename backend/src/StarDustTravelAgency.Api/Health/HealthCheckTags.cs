// Centralizes tags that Program uses to separate health-check endpoints.
// Each registered check selects a tag from this shared vocabulary.
namespace StarDustTravelAgency.Api.Health;

/// <summary>
/// Names live and ready groups used by Program.
/// Program uses them for registration and endpoint filtering.
/// </summary>
public static class HealthCheckTags
{
    public const string Live = "live";
    public const string Ready = "ready";
}
