using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

// Converts Keycloak client roles into standard ASP.NET Core role claims.
// Program registers it so authorization policies can call RequireRole.
namespace StarDustTravelAgency.Api.Security;

/// <summary>
/// Reads roles for the configured API from Keycloak resource_access JSON.
/// ASP.NET authorization consumes the resulting ClaimTypes.Role claims.
/// </summary>
public sealed class KeycloakRoleClaimsTransformation(
    string apiClientId) : IClaimsTransformation
{
    private const string ResourceAccessClaim = "resource_access";

    /// <summary>
    /// Adds API client roles to an authenticated identity and fails closed.
    /// </summary>
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity ||
            !identity.IsAuthenticated)
        {
            return Task.FromResult(principal);
        }

        var resourceAccess = principal.FindFirstValue(ResourceAccessClaim);
        if (string.IsNullOrWhiteSpace(resourceAccess))
        {
            return Task.FromResult(principal);
        }

        try
        {
            using var document = JsonDocument.Parse(resourceAccess);
            if (!TryGetRoles(document.RootElement, out var roles))
            {
                return Task.FromResult(principal);
            }

            foreach (var roleElement in roles.EnumerateArray())
            {
                var role = roleElement.GetString();
                if (!string.IsNullOrWhiteSpace(role) &&
                    !identity.HasClaim(ClaimTypes.Role, role))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
        }
        catch (JsonException)
        {
            // A malformed role claim grants no roles, preserving fail-closed
            // authorization while JWT validation still handles token trust.
        }

        return Task.FromResult(principal);
    }

    /// <summary>
    /// Locates the configured client's roles array inside resource_access.
    /// </summary>
    private bool TryGetRoles(
        JsonElement resourceAccess,
        out JsonElement roles)
    {
        roles = default;

        return resourceAccess.ValueKind == JsonValueKind.Object &&
            resourceAccess.TryGetProperty(apiClientId, out var clientAccess) &&
            clientAccess.ValueKind == JsonValueKind.Object &&
            clientAccess.TryGetProperty("roles", out roles) &&
            roles.ValueKind == JsonValueKind.Array;
    }
}
