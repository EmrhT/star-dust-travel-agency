# Compiles the React application and serves its static output through nginx.
# Runtime templates connect browser API requests to the backend container.
FROM node:24-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginxinc/nginx-unprivileged:1.29-alpine AS runtime
COPY nginx/default.conf.template /etc/nginx/templates/default.conf.template
COPY config/app-config.template.js /opt/star-dust/config/app-config.template.js
COPY scripts/40-generate-app-config.sh \
    /docker-entrypoint.d/40-generate-app-config.sh
COPY --from=build /app/dist /usr/share/nginx/html
USER root
RUN chmod +x /docker-entrypoint.d/40-generate-app-config.sh \
    && chown -R 101:101 /usr/share/nginx/html
USER 101
EXPOSE 8080
