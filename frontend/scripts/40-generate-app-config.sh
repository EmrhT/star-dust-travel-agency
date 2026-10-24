#!/bin/sh

# Generates browser API and identity settings from container environment.
# The nginx entrypoint runs it before serving index.html and the React bundle.
set -eu

envsubst '${APP_API_BASE_URL} ${APP_AUTH_BASE_URL} ${APP_AUTH_REALM}
${APP_AUTH_CLIENT_ID}' \
  < /opt/star-dust/config/app-config.template.js \
  > /usr/share/nginx/html/app-config.js
