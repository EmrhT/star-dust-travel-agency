export interface AppConfig {
  apiBaseUrl: string
}

declare global {
  interface Window {
    __APP_CONFIG__?: Partial<AppConfig>
  }
}

export const appConfig: AppConfig = {
  apiBaseUrl: window.__APP_CONFIG__?.apiBaseUrl || '/api',
}
