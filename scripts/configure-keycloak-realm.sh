#!/bin/sh

# Creates the reviewed star-dust realm through Keycloak's Admin CLI.
# Docker supplies bootstrap credentials; no secret is stored by this script.
set -eu

docker compose exec -T keycloak sh -eu -c '
cli=/opt/keycloak/bin/kcadm.sh
config=/tmp/star-dust-kcadm.config

# Removes the short-lived Admin CLI token file from the Keycloak container.
cleanup() {
  rm -f "$config"
}

trap cleanup EXIT INT TERM

export KC_CLI_PASSWORD="$KC_BOOTSTRAP_ADMIN_PASSWORD"

"$cli" config credentials \
  --config "$config" \
  --server http://127.0.0.1:8080 \
  --realm master \
  --user "$KC_BOOTSTRAP_ADMIN_USERNAME"

if "$cli" get realms/star-dust \
  --config "$config" \
  >/dev/null 2>&1
then
  echo "Keycloak realm star-dust already exists."
  exit 0
fi

"$cli" create realms \
  --config "$config" \
  -s realm=star-dust \
  -s "displayName=Star Dust Travel Agency" \
  -s enabled=true \
  -s sslRequired=external

echo "Created Keycloak realm star-dust."
'
