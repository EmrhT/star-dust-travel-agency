import { appConfig } from '../config'
import type { ProblemDetails } from './generated/star-dust-api'

// Centralizes runtime URL translation and errors for generated API adapters.
// system.ts and journeys.ts share these deployment-boundary conventions.

export const generatedClientBaseUrl = appConfig.apiBaseUrl.replace(
  /\/api\/?$/,
  '',
)

// Converts generated ProblemDetails rejections into UI-compatible Errors.
export function toApiError(error: unknown, fallbackMessage: string): Error {
  if (error instanceof Error) {
    return error
  }

  if (typeof error === 'object' && error !== null) {
    const problem = error as ProblemDetails

    return new Error(problem.detail ?? problem.title ?? fallbackMessage)
  }

  return new Error(fallbackMessage)
}
