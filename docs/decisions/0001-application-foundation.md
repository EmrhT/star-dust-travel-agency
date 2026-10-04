# ADR 0001: Application foundation

- Status: Accepted
- Date: 2026-10-03

## Context

Star Dust Travel Agency needs independently deployable frontend and backend
images for Docker Compose now and Kubernetes later. The browser needs a stable
way to reach the API without embedding environment-specific hostnames in the
JavaScript bundle.

## Decision

- Build a React single-page application into static assets and serve them with
  unprivileged nginx.
- Route browser requests under `/api` through nginx to the ASP.NET Core API.
- Supply the nginx upstream and browser configuration at container startup.
- Keep the ASP.NET Core API stateless and store durable application state in
  PostgreSQL.
- Keep liveness independent of external services; let readiness verify
  PostgreSQL connectivity.
- Send application logs to stdout/stderr.
- Use UTC for stored instants and `Europe/Istanbul` for business-facing time.

## Consequences

- The browser uses one origin, which avoids environment-specific API URLs and
  simplifies future OIDC and CORS behavior.
- Docker Compose and Kubernetes may supply different DNS names without changing
  or rebuilding application code.
- PostgreSQL failure makes the backend unready but does not falsely report that
  the ASP.NET process is dead.
- The frontend image contains no Node.js runtime.
