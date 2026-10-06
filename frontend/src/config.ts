// Reads browser runtime configuration written by the nginx startup script.
// API clients use the result without baking environment URLs into the bundle.
// Defines the runtime settings shared by window configuration and API clients.
export interface AppConfig {
  apiBaseUrl: string
}

declare global {
  // Extends the browser Window type with the injected application settings.
  interface Window {
    __APP_CONFIG__?: Partial<AppConfig>
  }
}

export const appConfig: AppConfig = {
  apiBaseUrl: window.__APP_CONFIG__?.apiBaseUrl || '/api',
}
