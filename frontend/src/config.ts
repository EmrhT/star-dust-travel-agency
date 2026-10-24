// Reads browser runtime configuration written by the nginx startup script.
// API and authentication modules consume it without build-time URLs.
// Defines the runtime settings shared by window configuration and API clients.
export interface AppConfig {
  apiBaseUrl: string
  authBaseUrl: string
  authRealm: string
  authClientId: string
}

declare global {
  // Extends the browser Window type with the injected application settings.
  interface Window {
    __APP_CONFIG__?: Partial<AppConfig>
  }
}

export const appConfig: AppConfig = {
  apiBaseUrl: window.__APP_CONFIG__?.apiBaseUrl || '/api',
  authBaseUrl:
    window.__APP_CONFIG__?.authBaseUrl || 'https://auth-stardust.no-name.win',
  authRealm: window.__APP_CONFIG__?.authRealm || 'star-dust',
  authClientId: window.__APP_CONFIG__?.authClientId || 'star-dust-web',
}
