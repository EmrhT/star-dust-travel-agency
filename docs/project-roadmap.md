# Project roadmap

This roadmap keeps the learning sequence explicit. Each milestone is reviewed
before implementation, and application changes remain small enough to inspect.

## Current milestone: understand the handwritten HTTP boundary

- [x] Trace frontend startup and provider composition.
- [x] Trace the system-status request and response.
- [x] Trace the journey-week query and table rendering.
- [x] Trace journey-week generation and TanStack Query cache refresh.

The handwritten `fetch()` functions remain in place through this milestone so
their URL construction, headers, serialization, errors, and cancellation are
visible before tooling generates equivalent code.

## Next milestone: generate a typed client from OpenAPI

Recommended timing: immediately after the journey-generation sequence diagram
and before Keycloak or additional booking and administration endpoints.

### Why this is the right boundary

- The project will have both a GET and a POST flow to compare with generated
  code.
- The current API is small enough to review every generated type and method.
- New endpoints can use the chosen generation workflow from their beginning.
- Authentication can later be added through one shared client configuration
  point instead of repeated changes to handwritten request functions.

### Evaluation and implementation plan

1. [x] Inspect the current OpenAPI document as the language-neutral API
   contract.
2. [x] Compare TypeScript generators for browser `fetch()` support, abort
   signals, enum handling, generated-code readability, and deterministic
   builds.
3. [x] Select NSwag `14.7.1` and record the choice and tradeoffs in
   `docs/decisions/0002-openapi-typescript-client.md`.
4. [x] Improve operation identifiers, response declarations, validation
   errors, and schemas where generation exposes ambiguity.
5. [x] Add a repeatable generation command and a clearly marked generated
   output directory. Generated files must not be edited manually.
6. [x] Replace handwritten transport details while retaining small
   application-facing adapters where they improve naming or TanStack Query
   integration.
   - [x] Integrate `SystemClient.getStatus()` through `api/system.ts`.
   - [x] Integrate `JourneysClient.getWeek()` through `api/journeys.ts`.
   - [x] Integrate `JourneysClient.generateWeek()` through `api/journeys.ts`.
7. [x] Preserve `AbortSignal` propagation, runtime API-base configuration,
   JSON enum behavior, and ASP.NET Core `ProblemDetails` errors. Shared
   generated-client conventions live in `api/client-support.ts`.
8. [x] Add a platform-neutral stale-client check whose non-zero exit code lets
   CI reject an OpenAPI or generated-client drift.
9. [ ] Complete full-stack verification without introducing a browser-test
   framework solely for the current single screen.
   - [x] Run all backend tests and the stale-client check.
   - [x] Run TypeScript checks and both production container builds.
   - [x] Smoke-test GET, POST, validation, and idempotency through nginx.
   - [ ] Confirm React rendering and the Generate button in a real browser.
10. [x] Update affected comments and draw.io diagrams to distinguish generated
    client calls from browser `fetch()` and TanStack Query behavior.

### Completion criteria

- Application code no longer duplicates backend DTO shapes by hand where the
  OpenAPI document can generate them.
- API calls remain typed from React through the generated client boundary.
- Contract drift is detected automatically rather than during browser use.
- A new developer can regenerate the same client with one documented command.
- Generated transport code remains separate from handwritten UI and query
  orchestration code.

## Later milestones

- [ ] Complete health-check, EF Core model, and seeding sequence diagrams.
- [ ] Integrate Keycloak authentication and authorization.
  - [x] Define a pinned local service and isolated PostgreSQL boundary.
  - [x] Provision the dedicated Keycloak role and database.
  - [x] Verify startup, health, Admin Console, and restart persistence.
  - [x] Establish a common HTTPS issuer through the HAProxy ingress.
  - [x] Design the realm, clients, and minimum application roles.
    - [x] Create and verify the dedicated `star-dust` realm.
    - [x] Create and verify the public `star-dust-web` client.
    - [x] Create and verify the `star-dust-api` resource client.
    - [x] Create and verify the `journey-operator` API client role.
  - [x] Create test users and assign the operator role to only one user.
  - [x] Add and verify the `star-dust-api` access-token audience mapper.
  - [x] Add default-deny ASP.NET Core authentication and authorization.
    - [x] Validate Keycloak signature, lifetime, issuer, and API audience.
    - [x] Transform API client roles and require `journey-operator` on POST.
    - [x] Keep health probes anonymous and document the runtime sequence.
  - [ ] Add React Authorization Code + PKCE and generated-client tokens.
    - [x] Integrate the official adapter through an AuthProvider boundary.
    - [x] Refresh and attach tokens through the generated-client fetch hook.
    - [x] Deploy exact runtime identity configuration through nginx.
    - [ ] Verify viewer and operator behavior through real browser logins.
- [ ] Add booking behavior in small domain, API, and frontend slices.
- [ ] Add administration workflows only after their authorization model is
  defined.

