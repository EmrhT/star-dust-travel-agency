# Authentication and authorization design

This design protects existing API behavior without inventing permissions for
features that do not exist. It is reviewed before any Keycloak objects or
application authentication code are created.

The realm was created through the Admin Console so its boundary was visible.
`scripts/configure-keycloak-realm.sh` records the idempotent CLI equivalent and
detects the manually created realm without replacing it. Later client and role
slices will extend automation only after their designs are approved.

## Security boundary

Authentication answers who made a request. Authorization decides whether that
authenticated identity may perform the requested operation.

ASP.NET Core uses a fallback policy that requires authentication by default.
New controller actions are therefore protected even if a
developer forgets to add an authorization attribute. Only deliberately
selected infrastructure endpoints opt out.

## Realm

Application identities belong to a dedicated `star-dust` realm. The Keycloak
`master` realm remains an administration boundary and must not contain Star
Dust users, clients, or application roles.

The canonical issuer will be:

```text
https://auth-stardust.no-name.win/realms/star-dust
```

## Clients

### `star-dust-web`

This public OpenID Connect client represents the React application running in
the browser. A browser cannot safely keep a client secret, so the client uses
Authorization Code flow with PKCE `S256` and no secret.

It was created through the Admin Console and verified in Keycloak storage with
Standard flow enabled, other grant types disabled, and PKCE required as
`S256`.

Initial browser locations will be exact rather than wildcarded:

```text
Home:                 https://stardust.no-name.win/
Redirect URI:         https://stardust.no-name.win/auth/callback
Post-logout redirect: https://stardust.no-name.win/
Web origin:           https://stardust.no-name.win
```

### `star-dust-api`

This client represents the ASP.NET Core resource server. It does not perform
interactive login. It supplies the API audience and owns API-specific client
roles.

Tokens accepted by ASP.NET Core must contain both:

```text
iss = https://auth-stardust.no-name.win/realms/star-dust
aud = star-dust-api
```

Checking the audience prevents a valid token intended only for another client
from being reused against the Star Dust API.

The `star-dust-web` dedicated client scope has an audience mapper that adds
`star-dust-api` to access tokens but not ID tokens. Both viewer and operator
tokens can therefore target the API independently of their assigned roles.

## Minimum role model

Only one application role is currently justified:

```text
star-dust-api client role: journey-operator
```

Every authenticated user may read journey data. A user needs the
`journey-operator` role to generate journey records.

We will not create `Customer` or application `Admin` roles yet. There is no
booking or administration behavior to authorize, and speculative roles create
complexity without protecting anything.

Keycloak's administrative roles are separate from application roles. A Star
Dust operator must never receive Keycloak realm-management privileges merely
because the application calls that person an operator.

The `stardust-viewer` test user has no API client role. The
`stardust-operator` test user has `journey-operator` assigned directly, which
provides a controlled pair for testing successful and forbidden requests.

## Endpoint policy

| Endpoint | Authentication | Additional authorization |
| --- | --- | --- |
| `GET /api/system/status` | Required | None |
| `GET /api/journeys` | Required | None |
| `POST /api/journeys/generate-week` | Required | `journey-operator` |
| `GET /health/live` | Anonymous | Infrastructure probe |
| `GET /health/ready` | Anonymous | Infrastructure probe |

Health endpoints are intentional exceptions. Docker and orchestrators must be
able to decide whether a process should receive traffic without possessing a
human access token.

Swagger and OpenAPI remain development tools rather than public application
endpoints. Direct backend publication should be loopback-only while HAProxy
exposes only the intended application and authentication hostnames.

## Browser login flow

```text
React redirects the browser to Keycloak
    → user authenticates
    → Keycloak returns a short-lived authorization code
    → React proves the PKCE verifier and exchanges the code
    → Keycloak returns an access token for star-dust-api
    → React sends Authorization: Bearer <token> to /api
    → ASP.NET Core validates signature, expiry, issuer, and audience
    → ASP.NET Core evaluates the endpoint authorization policy
```

The official `keycloak-js` adapter implements Authorization Code flow with
PKCE `S256`. `AuthProvider` exposes identity state to React without exposing
token details. Access and refresh tokens remain in adapter memory rather than
durable browser storage, reducing their availability to injected script.

Before each generated-client request, `authenticatedHttp.fetch()` calls
`getAccessToken()`. That function refreshes a token with less than 30 seconds
remaining and returns it for the `Authorization: Bearer` header. The NSwag
output receives this custom fetch object, so generated files remain untouched.

Runtime Keycloak URL, realm, and client ID values come from `app-config.js`,
which nginx creates from container environment variables before serving the
React bundle.

## Default-deny backend implementation

`Program.cs` configures JWT Bearer validation with the realm authority and API
audience. Its fallback policy makes authentication the default, while the two
health endpoints use explicit anonymous metadata. `JourneysController` adds
the named `JourneyOperator` policy to journey generation.

Keycloak stores client roles in the nested `resource_access` JSON claim.
`KeycloakRoleClaimsTransformation.TransformAsync()` extracts only roles under
`star-dust-api` and adds standard ASP.NET Core role claims. Malformed or
unrelated role data grants nothing, so authorization fails closed.

This order matters:

```text
valid token?
    ├── no  → HTTP 401 Unauthorized
    └── yes → required role present?
                ├── no  → HTTP 403 Forbidden
                └── yes → controller action executes
```

An HTTP `401` means the caller has not established a valid identity. An HTTP
`403` means the caller is authenticated but lacks permission for that action.

## Deliberately deferred decisions

The following belong to later behavior and should not be designed early:

- Customer roles and booking ownership rules.
- Administrative journey-editing roles.
- External identity-provider brokering.
- Service accounts and machine-to-machine clients.
- Production token lifetimes, multi-factor rules, and recovery policy.
