using System.Security.Claims;
using StarDustTravelAgency.Api.Security;

// Verifies conversion from Keycloak resource_access JSON to role claims.
// ApplicationAuthorization policies consume the transformed identity.
namespace StarDustTravelAgency.Api.Tests;

/// <summary>
/// Exercises KeycloakRoleClaimsTransformation without issuing a real token.
/// Keycloak's evaluated token shape supplies the representative JSON.
/// </summary>
public sealed class KeycloakRoleClaimsTransformationTests
{
    /// <summary>
    /// Verifies the configured API client role becomes an ASP.NET role.
    /// </summary>
    [Fact]
    public async Task Transform_adds_configured_api_client_role()
    {
        const string resourceAccess =
            """
            {
              "star-dust-api": {
                "roles": ["journey-operator"]
              }
            }
            """;
        var identity = new ClaimsIdentity(
            [new Claim("resource_access", resourceAccess)],
            authenticationType: "test");
        var principal = new ClaimsPrincipal(identity);
        var transformation = new KeycloakRoleClaimsTransformation(
            "star-dust-api");

        var result = await transformation.TransformAsync(principal);

        Assert.True(result.IsInRole("journey-operator"));
    }

    /// <summary>
    /// Verifies malformed role JSON grants no application permission.
    /// </summary>
    [Fact]
    public async Task Transform_rejects_malformed_resource_access()
    {
        var identity = new ClaimsIdentity(
            [new Claim("resource_access", "not-json")],
            authenticationType: "test");
        var principal = new ClaimsPrincipal(identity);
        var transformation = new KeycloakRoleClaimsTransformation(
            "star-dust-api");

        var result = await transformation.TransformAsync(principal);

        Assert.False(result.IsInRole("journey-operator"));
    }
}
