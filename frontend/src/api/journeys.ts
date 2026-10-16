import { appConfig } from '../config'
import {
  JourneysClient,
  type JourneyListItem as GeneratedJourneyListItem,
  type JourneyStatus as GeneratedJourneyStatus,
  type ProblemDetails,
  type TravelDirection as GeneratedTravelDirection,
} from './generated/star-dust-api'

// Adapts generated journey reads and handwritten writes for TanStack Query.
// Runtime config supplies the deployment path shared by both implementations.

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

// Matches the generation result serialized by JourneysController.
export interface GenerateJourneyWeekResult {
  weekStarting: string
  created: number
  alreadyExisted: number
}

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

// Serializes one Monday for JourneysController's generation endpoint.
export async function generateJourneysForWeek(
  weekStarting: string,
): Promise<GenerateJourneyWeekResult> {
  const response = await fetch(
    `${appConfig.apiBaseUrl}/journeys/generate-week`,
    {
      method: 'POST',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ weekStarting }),
    },
  )

  if (!response.ok) {
    throw new Error(`The API returned HTTP ${response.status}.`)
  }

  return response.json() as Promise<GenerateJourneyWeekResult>
}
