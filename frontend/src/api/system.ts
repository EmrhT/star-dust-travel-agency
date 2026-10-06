import { appConfig } from '../config'

// Defines and retrieves the system status contract exposed by SystemController.
// App consumes the typed result while TanStack Query controls cancellation.
// Describes the JSON fields serialized by the backend status response record.
export interface SystemStatus {
  service: string
  database: string
  checkedAtUtc: string
}

// Calls the configured API and converts its JSON response into SystemStatus.
export async function getSystemStatus({
  signal,
}: {
  signal?: AbortSignal
} = {}): Promise<SystemStatus> {
  const response = await fetch(`${appConfig.apiBaseUrl}/system/status`, {
    headers: {
      Accept: 'application/json',
    },
    signal,
  })

  if (!response.ok) {
    throw new Error(`The API returned HTTP ${response.status}.`)
  }

  return response.json() as Promise<SystemStatus>
}
