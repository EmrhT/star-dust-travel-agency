import { authenticatedHttp } from '../auth/authenticated-http'
import { generatedClientBaseUrl, toApiError } from './client-support'
import {
  type BookingListItem as GeneratedBookingListItem,
  BookingsClient,
  type CreateBookingRequest,
  type CreateBookingResponse as GeneratedCreateBookingResponse,
} from './generated/star-dust-api'

// Adapts generated booking operations for TanStack Query consumers.
// Authenticated HTTP supplies tokens while this module normalizes failures.

// Reuses the owned-booking projection generated from the backend contract.
export type BookingListItem = GeneratedBookingListItem

// Reuses the creation response generated from BookingsController metadata.
export type CreateBookingResponse = GeneratedCreateBookingResponse

// Carries TanStack Query cancellation into the generated GET request.
export interface GetMyBookingsOptions {
  signal?: AbortSignal
}

// Identifies the shared owned-booking cache across both routed pages.
export const myBookingsQueryKey = ['bookings', 'mine'] as const

const bookingsClient = new BookingsClient(
  generatedClientBaseUrl,
  authenticatedHttp,
)

// Delegates the owned GET request and JSON parsing to BookingsClient.
export async function getMyBookings(
  { signal }: GetMyBookingsOptions = {},
): Promise<BookingListItem[]> {
  try {
    return await bookingsClient.getMine(signal)
  } catch (error) {
    throw toApiError(error, 'The booking-list request failed.')
  }
}

// Sends only a Journey ID; the backend derives ownership from the JWT.
export async function createBooking(
  journeyId: string,
): Promise<CreateBookingResponse> {
  const request: CreateBookingRequest = { journeyId }

  try {
    return await bookingsClient.create(request)
  } catch (error) {
    throw toApiError(error, 'The booking request failed.')
  }
}
