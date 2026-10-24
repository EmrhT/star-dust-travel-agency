import Keycloak from 'keycloak-js'
import { appConfig } from '../config'

// Owns the single Keycloak adapter and its browser redirect configuration.
// AuthProvider and authenticatedHttp share this in-memory token authority.
const apiClientId = 'star-dust-api'
const callbackUrl = `${window.location.origin}/auth/callback`
const logoutUrl = `${window.location.origin}/`

export const keycloak = new Keycloak({
  url: appConfig.authBaseUrl,
  realm: appConfig.authRealm,
  clientId: appConfig.authClientId,
})

let initialization: Promise<boolean> | undefined

// Initializes Authorization Code + PKCE once, including StrictMode remounts.
export function initializeAuthentication(): Promise<boolean> {
  initialization ??= keycloak.init({
    onLoad: 'check-sso',
    flow: 'standard',
    pkceMethod: 'S256',
    responseMode: 'fragment',
    redirectUri: callbackUrl,
  })

  return initialization
}

// Redirects the browser to Keycloak and back through the registered callback.
export function login(): Promise<void> {
  return keycloak.login({ redirectUri: callbackUrl })
}

// Ends the Keycloak session and returns to the application root.
export function logout(): Promise<void> {
  return keycloak.logout({ redirectUri: logoutUrl })
}

// Refreshes a nearly expired token before an API request reads it.
export async function getAccessToken(): Promise<string> {
  if (!keycloak.authenticated) {
    throw new Error('Authentication is required before calling the API.')
  }

  try {
    await keycloak.updateToken(30)
  } catch {
    keycloak.clearToken()
    throw new Error('The login session expired. Please sign in again.')
  }

  if (!keycloak.token) {
    throw new Error('Keycloak did not provide an access token.')
  }

  return keycloak.token
}

// Reports whether the current API token carries journey generation permission.
export function canGenerateJourneys(): boolean {
  return keycloak.hasResourceRole('journey-operator', apiClientId)
}

// Reads the display identity supplied by Keycloak's parsed access token.
export function getUsername(): string | undefined {
  const username = keycloak.tokenParsed?.preferred_username

  return typeof username === 'string' ? username : undefined
}
