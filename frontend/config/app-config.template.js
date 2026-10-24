// Provides runtime API and identity settings substituted at container startup.
// config.ts reads this global before clients or Keycloak are constructed.
window.__APP_CONFIG__ = {
  apiBaseUrl: '${APP_API_BASE_URL}',
  authBaseUrl: '${APP_AUTH_BASE_URL}',
  authRealm: '${APP_AUTH_REALM}',
  authClientId: '${APP_AUTH_CLIENT_ID}',
}
