import { getAccessToken } from './keycloak'

// Adds a refreshed Keycloak access token to generated-client fetch requests.
// NSwag clients receive this object without gaining authentication knowledge.
export const authenticatedHttp = {
  // Preserves generated RequestInit values while setting Authorization.
  async fetch(url: RequestInfo, init: RequestInit = {}): Promise<Response> {
    const token = await getAccessToken()
    const headers = new Headers(init.headers)
    headers.set('Authorization', `Bearer ${token}`)

    return window.fetch(url, {
      ...init,
      headers,
    })
  },
}
