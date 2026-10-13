# ADR 0002: Generate the TypeScript API client with NSwag

- Status: accepted
- Date: 2026-10-13

## Context

The frontend currently repeats backend contracts and HTTP details in
`frontend/src/api/journeys.ts` and `frontend/src/api/system.ts`. This was useful
while learning the request path, but it allows the C# API and TypeScript client
to drift apart as the application grows.

The current `/swagger/v1/swagger.json` document was inspected before choosing a
generator. Its paths, JSON enums, date formats, response bodies, and XML
summaries are usable, but the following details need correction first:

- Operations have no stable `operationId`, so generated method names are not
  yet part of a deliberate contract.
- The GET query parameter and POST request body appear optional even though
  each operation requires them.
- Several non-nullable response properties appear nullable or optional.
- Operations advertise extra media types and do not yet declare a consistent
  error-response set.

Generating from this document now would automate its ambiguities as well as its
correct parts. Contract quality therefore precedes client generation.

## Decision

Use the NSwag Fetch TypeScript generator, pinned as the local .NET tool
`NSwag.ConsoleCore` version `14.7.1` when implementation begins. Keep
Swashbuckle as the ASP.NET Core OpenAPI producer; replacing it is unnecessary
because NSwag can consume its OpenAPI document.

The future generator configuration will:

- generate named methods and DTO types from stable OpenAPI operation IDs;
- use the browser Fetch API and enable `AbortSignal` parameters;
- accept the runtime API base URL instead of embedding an environment URL;
- place generated code under `frontend/src/api/generated/`;
- mark generated files as machine-owned and never edit them manually; and
- run from one repository command using an exact, committed tool version.

Small handwritten adapters will remain between React/TanStack Query and the
generated client. They will configure the base URL, preserve application-level
names, and provide one place for authentication and error translation. TanStack
Query will continue to own server-state caching and request orchestration; the
generated client will own HTTP transport and contract types.

## Alternatives considered

- `openapi-typescript` with `openapi-fetch` has small output and typed Fetch
  calls. Calls remain path-based, such as `client.GET("/api/journeys")`, unless
  we add wrappers.
- Microsoft Kiota provides rich request adapters and multi-language SDK
  support. Its runtime bundle and abstractions are excessive for this small
  browser client.
- Hey API generates named SDK functions and can integrate with TanStack Query.
  Its pre-1.0 generator has frequent breaking changes and adds concepts we do
  not need yet.

NSwag is not universally the best generator. It is the best fit here because
the backend is .NET, the desired output is a direct Fetch client, and its
generated request path remains recognizable while we learn the framework.

## Consequences

- The OpenAPI document becomes an executable contract, not only Swagger UI
  documentation.
- C# DTO or endpoint changes can regenerate corresponding TypeScript types and
  methods.
- Generated code will be verbose, but it will be isolated from handwritten
  application code.
- Stable operation IDs become compatibility-sensitive API names.
- Keycloak token attachment can later be implemented once in the handwritten
  client boundary instead of in every request function.
- CI must eventually regenerate the client and reject uncommitted differences.

## Implementation order

1. Correct operation IDs, requiredness, nullability, media types, and declared
   error responses in the backend OpenAPI document.
2. Inspect the resulting OpenAPI diff before adding generation tooling.
3. Add the pinned NSwag tool and committed generator configuration.
4. Generate and review the client without changing React behavior.
5. Replace one handwritten endpoint adapter at a time and run GET/POST tests.
6. Update the draw.io request diagrams after the runtime path changes.
