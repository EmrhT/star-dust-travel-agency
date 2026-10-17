#!/bin/sh

# Verifies that committed OpenAPI and TypeScript artifacts are reproducible.
# The generator may overwrite them temporarily; cleanup restores exact inputs.
set -eu

script_directory=$(
  CDPATH= cd -- "$(dirname -- "$0")" && pwd
)
repository_root=$(
  CDPATH= cd -- "$script_directory/.." && pwd
)
openapi_file="$repository_root/openapi/star-dust-v1.json"
generated_client="$repository_root/frontend/src/api/generated/star-dust-api.ts"
temporary_directory=$(mktemp -d)
openapi_backup="$temporary_directory/star-dust-v1.json"
client_backup="$temporary_directory/star-dust-api.ts"

# Restores both tracked artifacts after success, drift, interruption, or error.
cleanup() {
  if [ -f "$openapi_backup" ]; then
    cp "$openapi_backup" "$openapi_file"
  fi

  if [ -f "$client_backup" ]; then
    cp "$client_backup" "$generated_client"
  fi

  rm -f "$openapi_backup" "$client_backup"
  rmdir "$temporary_directory" 2>/dev/null || true
}

trap cleanup EXIT INT TERM

if [ ! -f "$openapi_file" ] || [ ! -f "$generated_client" ]; then
  echo "Generated API artifacts are missing; run the generator." >&2
  exit 1
fi

cp "$openapi_file" "$openapi_backup"
cp "$generated_client" "$client_backup"

"$script_directory/generate-api-client.sh"

openapi_changed=false
client_changed=false

if ! cmp -s "$openapi_backup" "$openapi_file"; then
  openapi_changed=true
fi

if ! cmp -s "$client_backup" "$generated_client"; then
  client_changed=true
fi

if [ "$openapi_changed" = false ] && [ "$client_changed" = false ]; then
  echo "OpenAPI and generated TypeScript client are current."
  exit 0
fi

echo "Generated API artifacts are stale." >&2

if [ "$openapi_changed" = true ]; then
  echo "- openapi/star-dust-v1.json differs from backend output." >&2
fi

if [ "$client_changed" = true ]; then
  echo "- frontend generated TypeScript client differs." >&2
fi

echo "Run ./scripts/generate-api-client.sh and commit both artifacts." >&2
exit 1
