// Provides the runtime API URL that nginx substitutes during container startup.
// config.ts reads this browser global before any API request is created.
window.__APP_CONFIG__ = {
  apiBaseUrl: '${APP_API_BASE_URL}',
}
