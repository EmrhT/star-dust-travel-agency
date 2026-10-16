import { appConfig } from '../config'
import {
  type GenerateJourneyWeekRequest,
  type GenerateJourneyWeekResponse as GeneratedJourneyWeekResponse,
  JourneysClient,
  type JourneyListItem as GeneratedJourneyListItem,
  type JourneyStatus as GeneratedJourneyStatus,
  type ProblemDetails,
  type TravelDirection as GeneratedTravelDirection,
} from './generated/star-dust-api'

// Adapts generated journey transport operations for TanStack Query consumers.
// Runtime config supplies their deployment path and adapters normalize errors.

// Re-exports NSwag's TravelDirection contract for JourneyTable consumers.
export type TravelDirection = GeneratedTravelDirection

// Re-exports NSwag's JourneyStatus contract for JourneyTable consumers.
export type JourneyStatus = GeneratedJourneyStatus

// Reuses the JourneyListItem generated from JourneyQueryService's response.
export type JourneyListItem = GeneratedJourneyListItem

// Carries TanStack Query's AbortSignal into the browser fetch request.
export interface GetJourneysOptions {
  signal?: AbortSignal
}

// Reuses the generation response produced from JourneysController's contract.
export type GenerateJourneyWeekResult = GeneratedJourneyWeekResponse

const generatedClientBaseUrl = appConfig.apiBaseUrl.replace(/\/api\/?$/, '')
const journeysClient = new JourneysClient(generatedClientBaseUrl)

// Delegates the GET and JSON work to JourneysClient and normalizes failures.
export async function getJourneysForWeek(
  weekStarting: string,
  { signal }: GetJourneysOptions = {},
): Promise<JourneyListItem[]> {
  try {
    return await journeysClient.getWeek(weekStarting, signal)
  } catch (error) {
    if (error instanceof Error) {
      throw error
    }

    if (typeof error === 'object' && error !== null) {
      const problem = error as ProblemDetails

      throw new Error(
        problem.detail ?? problem.title ?? 'The journey-week request failed.',
      )
    }

    throw new Error('The journey-week request failed.')
  }
}

// Builds the generated request DTO and delegates POST serialization to NSwag.
export async function generateJourneysForWeek(
  weekStarting: string,
): Promise<GenerateJourneyWeekResult> {
  const request: GenerateJourneyWeekRequest = { weekStarting }

  try {
    return await journeysClient.generateWeek(request)
  } catch (error) {
    if (error instanceof Error) {
      throw error
    }

    if (typeof error === 'object' && error !== null) {
      const problem = error as ProblemDetails

      throw new Error(
        problem.detail ??
          problem.title ??
          'The journey-generation request failed.',
      )
    }

    throw new Error('The journey-generation request failed.')
  }
}
