import { appConfig } from '../config'

// Defines journey contracts and calls JourneysController read/write endpoints.
// App supplies these functions to TanStack Query for server-state management.

// Mirrors the backend TravelDirection values serialized by ASP.NET Core.
export type TravelDirection = 'Outbound' | 'Return'

// Mirrors JourneyStatus so UI code handles every backend lifecycle value.
export type JourneyStatus =
  | 'Draft'
  | 'Scheduled'
  | 'Boarding'
  | 'Departed'
  | 'Arrived'
  | 'Delayed'
  | 'Cancelled'

// Matches the JourneyListItem JSON projected by JourneyQueryService.
export interface JourneyListItem {
  id: string
  launchStation: string
  destination: string
  direction: TravelDirection
  departureAtUtc: string
  arrivalAtUtc: string
  spaceship: string
  status: JourneyStatus
}

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

// Fetches the requested Monday's journeys from the backend read endpoint.
export async function getJourneysForWeek(
  weekStarting: string,
  { signal }: GetJourneysOptions = {},
): Promise<JourneyListItem[]> {
  const query = new URLSearchParams({ weekStarting })
  const response = await fetch(
    `${appConfig.apiBaseUrl}/journeys?${query}`,
    {
      headers: {
        Accept: 'application/json',
      },
      signal,
    },
  )

  if (!response.ok) {
    throw new Error(`The API returned HTTP ${response.status}.`)
  }

  return response.json() as Promise<JourneyListItem[]>
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
