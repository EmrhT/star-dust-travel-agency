# Generated API client

Files in this directory are generated from `openapi/star-dust-v1.json` by the
pinned NSwag tool. The generation script normalizes the header and formats the
result with Prettier. Do not edit generated TypeScript manually.

Run this command from the repository root after changing the backend contract:

```bash
./scripts/generate-api-client.sh
```

Use `./scripts/check-api-client.sh` for a read-only local or CI drift check.

Handwritten API adapters outside this directory connect the generated client
to runtime configuration, authentication, and TanStack Query.
