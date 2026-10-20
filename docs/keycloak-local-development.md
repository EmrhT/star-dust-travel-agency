# Local Keycloak infrastructure

This milestone runs Keycloak locally before connecting it to ASP.NET Core or
React. It proves identity infrastructure independently of application code.

## Data boundary

Keycloak and Star Dust share one PostgreSQL server process but use separate
databases, owners, credentials, and tables:

```text
PostgreSQL container
├── star_dust database  → Star Dust API
└── keycloak database   → Keycloak
```

The Keycloak role must own only the `keycloak` database. Do not give Keycloak
the application role or PostgreSQL cluster-administrator credentials.

## Local runtime

Compose pins Keycloak `26.8.1` and runs `start-dev`. Development mode uses
relaxed defaults, so this configuration must not be promoted to production.
Host-installed HAProxy publishes its stable external address as
`https://auth-stardust.no-name.win`.

Keycloak health endpoints run on its internal management port `9000`. The
Compose health check uses Bash TCP support because the hardened Keycloak image
does not include `curl`.

The one-gigabyte memory limit bounds the percentage-based JVM heap. Realm,
client, role, and user data persist in PostgreSQL rather than a Keycloak
container filesystem.

## Provisioning order

1. Add unique Keycloak passwords to the ignored `.env` file.
2. Create the dedicated PostgreSQL login role and database once.
3. Start Keycloak and wait for its health check.
4. Open the Admin Console and verify that a container restart retains data.

Realm, client, role, backend-token, and React-login configuration belong to
later slices. Keeping them out of this step makes database and container
behavior independently observable.

## HTTPS ingress and hostname boundary

Cloudflare DNS resolves the application and authentication hostnames to the
private Docker host. Host HAProxy terminates TLS and routes them to the
loopback ports published by Compose:

```text
stardust.no-name.win       → 127.0.0.1:3000 → frontend:8080
auth-stardust.no-name.win  → 127.0.0.1:8081 → keycloak:8080
```

The browser and backend resolve the authentication hostname to the Docker
host. They can therefore use one canonical Keycloak issuer:

```text
https://auth-stardust.no-name.win/realms/star-dust
```

Keycloak trusts HAProxy's overwritten `X-Forwarded-*` headers when generating
external URLs. The direct loopback port remains temporarily available for
diagnosis and should be removed after authentication is proven end to end.

## Host HAProxy deployment

The reviewed source configuration and ignored certificate bundle live at:

```text
haproxy/haproxy.cfg
.secrets/haproxy/no-name-win.pem
```

The host service reads deployed copies instead:

```text
/etc/haproxy/haproxy.cfg
/etc/haproxy/certs/site.pem
```

Changing repository source does not update the host automatically. Copy the
changed files deliberately, validate them, and reload only valid syntax:

```bash
sudo haproxy -c -f /etc/haproxy/haproxy.cfg
sudo systemctl reload haproxy
```

When Let's Encrypt renews the certificate, rebuild the combined PEM, deploy
it with mode `640` and ownership `root:haproxy`, and reload HAProxy. A deploy
hook can automate that operational step after the authentication flow works.
