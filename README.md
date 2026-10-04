# Star Dust Travel Agency

A learning-first, containerized travel-agency application built with React,
TypeScript, ASP.NET Core, and PostgreSQL.

The current milestone is the application foundation. It proves that the three
containers can start and that a browser request can travel through the complete
stack:

```text
Browser -> nginx/React -> ASP.NET Core -> PostgreSQL
```

The travel domain, Keycloak authentication, bookings, and administration pages
will be added incrementally after this foundation is verified.

## Prerequisites

- Docker Engine with Docker Compose v2
- Git

Node.js and the .NET SDK are not required on the host for the containerized
workflow. The build images supply those toolchains.

## Start the application

1. Create the local environment file:

   ```bash
   cp .env.example .env
   ```

2. Replace the development database password in `.env`.

3. Build and start the stack:

   ```bash
   docker compose up --build
   ```

4. Open:

   - Frontend: <http://localhost:3000>
   - Swagger UI: <http://localhost:8080/swagger>
   - Backend liveness: <http://localhost:8080/health/live>
   - Backend readiness: <http://localhost:8080/health/ready>

Run `docker compose ps` to inspect container health and
`docker compose logs -f backend` to follow structured backend logs.

Stop the containers without deleting PostgreSQL data:

```bash
docker compose down
```

Deleting the named volume is intentionally a separate, destructive operation.
Do not use `docker compose down --volumes` if the database contents matter.

## How container networking works

Compose creates a private network and publishes only the declared host ports.
Within that network, service discovery resolves the service names `frontend`,
`backend`, and `postgres`. Those names belong to Compose configuration, not to
application code:

- The backend receives `Host=postgres` in its connection string.
- nginx receives `http://backend:8080` as its upstream.
- The browser calls the relative URL `/api`, so it never needs to know a
  container or Kubernetes Service name.

In Kubernetes, configuration can replace those two internal DNS values while
the same images continue to run unchanged.

## Health semantics

`GET /health/live` checks only whether the ASP.NET process can answer requests.
It deliberately does not query PostgreSQL. This is suitable for a Kubernetes
liveness probe.

`GET /health/ready` runs a lightweight PostgreSQL query. It reports whether the
API is currently able to perform useful application work and is suitable for a
readiness probe.

## Configuration

ASP.NET Core converts double underscores in environment-variable names into
configuration sections. For example:

```text
ConnectionStrings__Postgres -> ConnectionStrings:Postgres
Swagger__Enabled            -> Swagger:Enabled
```

The frontend image generates `/app-config.js` when its container starts. This
is runtime configuration; it avoids baking environment-specific addresses into
the Vite bundle.

Do not commit `.env`. It is ignored by Git. `.env.example` contains names and
safe placeholders, not deployable secrets.

## Developer checks

When the matching SDKs are installed locally, run:

```bash
dotnet test backend/StarDustTravelAgency.slnx --configuration Release
npm --prefix frontend ci
npm --prefix frontend run build
```

The normal container build runs the frontend and backend production builds even
when those SDKs are not installed on the host.

## Repository layout

```text
backend/      ASP.NET Core API and backend container build
frontend/     React SPA, nginx configuration, and frontend container build
docs/         Architectural decisions and learning notes
docker-compose.yml
```

## Keycloak direction

A later milestone will use the existing Keycloak deployment at
`keycloak.no-name.win`. It will use a separate `star-dust` realm, a public SPA
client with Authorization Code + PKCE, and an API audience with `Admin` and
`Customer` client roles. Keycloak is intentionally not required for this first
anonymous connectivity milestone.
