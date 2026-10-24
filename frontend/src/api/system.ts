import {
  SystemClient,
  type SystemStatusResponse,
} from './generated/star-dust-api'
import { authenticatedHttp } from '../auth/authenticated-http'
import { generatedClientBaseUrl, toApiError } from './client-support'

// Adapts generated SystemClient transport behavior to React application needs.
// App supplies cancellation; runtime config supplies the deployment API path.
// Reuses the generated OpenAPI type instead of duplicating its DTO fields.
export type SystemStatus = SystemStatusResponse

const systemClient = new SystemClient(generatedClientBaseUrl, authenticatedHttp)

// Delegates HTTP and JSON work to SystemClient and normalizes API failures.
export async function getSystemStatus({
  signal,
}: {
  signal?: AbortSignal
} = {}): Promise<SystemStatus> {
  try {
    return await systemClient.getStatus(signal)
  } catch (error) {
    throw toApiError(error, 'The system-status request failed.')
  }
}
