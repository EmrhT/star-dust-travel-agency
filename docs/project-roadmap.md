# Project roadmap

This roadmap keeps the learning sequence explicit. Each milestone is reviewed
before implementation, and application changes remain small enough to inspect.

## Current milestone: understand the handwritten HTTP boundary

- [x] Trace frontend startup and provider composition.
- [x] Trace the system-status request and response.
- [x] Trace the journey-week query and table rendering.
- [ ] Trace journey-week generation and TanStack Query cache refresh.

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

1. Freeze and inspect the current OpenAPI document as the language-neutral API
   contract.
2. Compare TypeScript generators for browser `fetch()` support, abort signals,
   enum handling, generated-code readability, and deterministic builds.
3. Select and pin one generator version; record the choice and tradeoffs in an
   architecture decision record.
4. Improve operation identifiers, response declarations, validation errors,
   and schemas where generation exposes ambiguity.
5. Add a repeatable generation command and a clearly marked generated output
   directory. Generated files must not be edited manually.
6. Replace the handwritten transport details in `api/system.ts` and
   `api/journeys.ts` while retaining small application-facing adapters where
   they improve naming or TanStack Query integration.
7. Preserve `AbortSignal` propagation, runtime API-base configuration, JSON
   enum behavior, and ASP.NET Core `ProblemDetails` errors.
8. Add a stale-client check so CI fails when the committed generated client no
   longer matches the OpenAPI contract.
9. Run backend tests, TypeScript checks, the production frontend build, and
   browser-level GET and POST smoke tests.
10. Update affected comments and draw.io sequence diagrams to distinguish
    generated client calls from browser `fetch()` and TanStack Query behavior.

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
- [ ] Integrate Keycloak authentication and authorization with default-deny API
  policies.
- [ ] Add booking behavior in small domain, API, and frontend slices.
- [ ] Add administration workflows only after their authorization model is
  defined.

