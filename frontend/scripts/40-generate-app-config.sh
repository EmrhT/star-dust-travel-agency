#!/bin/sh

# Generates browser runtime settings from the container environment.
# The nginx entrypoint runs it before serving index.html and the React bundle.
set -eu

envsubst '${APP_API_BASE_URL}' \
  < /opt/star-dust/config/app-config.template.js \
  > /usr/share/nginx/html/app-config.js
