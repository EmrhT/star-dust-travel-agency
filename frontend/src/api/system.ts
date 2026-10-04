import { appConfig } from '../config'

export interface SystemStatus {
  service: string
  database: string
  checkedAtUtc: string
}

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
