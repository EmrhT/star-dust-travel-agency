import { appConfig } from '../config'
import {
  SystemClient,
  type ProblemDetails,
  type SystemStatusResponse,
} from './generated/star-dust-api'

// Adapts generated SystemClient transport behavior to React application needs.
// App supplies cancellation; runtime config supplies the deployment API path.
// Reuses the generated OpenAPI type instead of duplicating its DTO fields.
export type SystemStatus = SystemStatusResponse

const generatedClientBaseUrl = appConfig.apiBaseUrl.replace(/\/api\/?$/, '')
const systemClient = new SystemClient(generatedClientBaseUrl)

// Delegates HTTP and JSON work to SystemClient and normalizes API failures.
export async function getSystemStatus({
  signal,
}: {
  signal?: AbortSignal
} = {}): Promise<SystemStatus> {
  try {
    return await systemClient.getStatus(signal)
  } catch (error) {
    if (error instanceof Error) {
      throw error
    }

    if (typeof error === 'object' && error !== null) {
      const problem = error as ProblemDetails

      throw new Error(
        problem.detail ?? problem.title ?? 'The system-status request failed.',
      )
    }

    throw new Error('The system-status request failed.')
  }
}
