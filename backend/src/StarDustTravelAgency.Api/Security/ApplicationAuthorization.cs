// Defines stable authorization names shared by Program and API controllers.
// KeycloakRoleClaimsTransformation supplies the role used by the policy.
namespace StarDustTravelAgency.Api.Security;

/// <summary>
/// Names the journey-generation policy and its corresponding Keycloak role.
/// Program defines the policy; JourneysController requires it for POST.
/// </summary>
public static class ApplicationAuthorization
{
    public const string JourneyOperatorPolicy = "JourneyOperator";
    public const string JourneyOperatorRole = "journey-operator";
}
