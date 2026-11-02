# Project roadmap

This roadmap keeps the learning sequence explicit. Each milestone is reviewed
before implementation, and application changes remain small enough to inspect.

## Learning project status: complete

Star Dust Travel Agency is complete as a **learning project**, not as a
production-complete travel-agency product. Its purpose was to teach the major
connections in a modern web application through a small number of complete
vertical slices. Those slices now cover:

- browser rendering, React state, forms, and client-side routing;
- TanStack Query request and cache lifecycles;
- OpenAPI and NSwag-generated TypeScript clients;
- nginx proxying and container networking;
- ASP.NET Core routing, model binding, validation, and service boundaries;
- Keycloak authentication, JWT validation, authorization, and record ownership;
- EF Core modeling, migrations, queries, and PostgreSQL persistence; and
- serialization, error contracts, and end-to-end request tracing.

Adding the remaining travel-agency features would mostly repeat these learned
patterns through more entities, forms, and CRUD endpoints. The project stops
here deliberately to preserve its learning-first scope and avoid turning a
focused example into an unnecessarily large business application.

### Why the UML model is broader than the code

The first page of `diagrams/star-dust-domain-model.drawio` is a **conceptual
domain map**. It records how a fuller travel agency could eventually include
ApplicationUser, CrewMember, JourneyCrewAssignment, Ticket, FareClass,
RouteFare, Payment, Announcement, and AuditRecord. It is an architectural
blueprint and vocabulary map, not a promise that every displayed concept has a
table, API, service, or React page in this repository.

Only selected vertical slices were implemented: the operational journey core,
weekly generation and queries, identity-owned bookings, and the booking form.
Consequently, functions and endpoints implied by the broader UML model are
absent by design. In particular, ticketing, passengers, seat allocation,
pricing, payment, crew management, announcements, auditing, and administration
were not partially forgotten; they were intentionally left outside the final
learning boundary.

Runtime sequence diagrams have a different purpose from the conceptual domain
map. They document functions and request paths that actually exist in the
implemented application. When extending the project, the conceptual model must
first be refined into an approved implementation slice; only then should its
entities, endpoints, generated client methods, UI, tests, and runtime diagrams
be added together.

### How to interpret unfinished-looking items

- Checked roadmap items describe completed learning work.
- Items explicitly marked **Deferred** are possible extensions, not completion
  requirements.
- Concepts shown only in the conceptual UML overview are design possibilities,
  not missing implementation.
- Production use would require a new requirements and threat-modeling phase;
  it should not be inferred merely by filling in every UML box.

## Completed milestone: accept and validate booking form input

- [x] Define form fields, trust boundaries, and validation rules.
- [x] Define the request, success, and validation-error contracts.
- [x] Extend the Booking entity and EF Core configuration.
- [x] Generate and review the schema migration before applying it.
- [x] Extend booking creation, projection, and focused backend tests.
- [x] Regenerate and verify the OpenAPI TypeScript client.
- [x] Add the routed React form with controlled Material UI inputs.
- [x] Map backend field errors and prevent duplicate submissions.
- [x] Refresh owned-booking state and navigate after success.
- [x] Document the complete form-submission runtime flow in draw.io.

The approved contract and its trust boundaries live in
`docs/booking-form-design.md`. Work remains incremental so model, HTTP, and UI
changes can be inspected independently.

## Completed milestone: understand the handwritten HTTP boundary

- [x] Trace frontend startup and provider composition.
- [x] Trace the system-status request and response.
- [x] Trace the journey-week query and table rendering.
- [x] Trace journey-week generation and TanStack Query cache refresh.

The handwritten `fetch()` functions remained through this milestone so their
URL construction, headers, serialization, errors, and cancellation were visible
before tooling generated equivalent code.

## Completed milestone: generate a typed client from OpenAPI

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
9. [x] Complete full-stack verification without introducing a browser-test
   framework solely for the current single screen.
   - [x] Run all backend tests and the stale-client check.
   - [x] Run TypeScript checks and both production container builds.
   - [x] Smoke-test GET, POST, validation, and idempotency through nginx.
   - [x] Confirm React rendering and the Generate button in a real browser.
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

## Final milestone history and deferred extensions

- **Deferred:** Complete health-check, EF Core model, and seeding sequence
  diagrams. These would improve documentation but introduce no new runtime
  behavior.
- [x] Integrate Keycloak authentication and authorization.
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
  - [x] Add React Authorization Code + PKCE and generated-client tokens.
    - [x] Integrate the official adapter through an AuthProvider boundary.
    - [x] Refresh and attach tokens through the generated-client fetch hook.
    - [x] Deploy exact runtime identity configuration through nginx.
    - [x] Verify viewer and operator behavior through real browser logins.
- [x] Add minimal booking behavior to teach identity-owned application data.
  - [x] Design the minimal Booking-to-Journey persistence relationship.
  - [x] Add the entity, EF Core configuration, and reviewed migration.
  - [x] Create a booking using the validated token's `sub` claim.
  - [x] Query bookings belonging to the authenticated user.
  - [x] Add the smallest React interface for those two operations.
- [x] Introduce client-side routing without weakening backend authorization.
  - [x] Split the monolithic App into a layout and focused page components.
  - [x] Protect private pages through the existing AuthProvider state.
  - [x] Preserve the Keycloak callback and verify nginx history fallback.
- **Deferred:** Add administration workflows only after defining new product
  requirements and their authorization model. They are not required to
  complete this learning project.

