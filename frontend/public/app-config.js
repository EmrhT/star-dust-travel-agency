// Supplies local defaults for API and Keycloak settings read by config.ts.
// The production container replaces it from app-config.template.js.
window.__APP_CONFIG__ = {
  apiBaseUrl: '/api',
  authBaseUrl: 'https://auth-stardust.no-name.win',
  authRealm: 'star-dust',
  authClientId: 'star-dust-web',
}
