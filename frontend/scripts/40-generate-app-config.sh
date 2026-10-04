#!/bin/sh
set -eu

envsubst '${APP_API_BASE_URL}' \
  < /opt/star-dust/config/app-config.template.js \
  > /usr/share/nginx/html/app-config.js
