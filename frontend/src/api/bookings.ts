import { authenticatedHttp } from '../auth/authenticated-http'
import { generatedClientBaseUrl, toApiError } from './client-support'
import {
  type BookingListItem as GeneratedBookingListItem,
  BookingsClient,
  type CreateBookingRequest,
  type CreateBookingResponse as GeneratedCreateBookingResponse,
  type ValidationProblemDetails,
} from './generated/star-dust-api'

// Adapts generated booking operations for TanStack Query consumers.
// Authenticated HTTP supplies tokens while this module normalizes failures.

// Reuses the owned-booking projection generated from the backend contract.
export type BookingListItem = GeneratedBookingListItem

// Reuses the creation response generated from BookingsController metadata.
export type CreateBookingResponse = GeneratedCreateBookingResponse

// Reuses the generated request contract as the booking form's mutation input.
export type CreateBookingInput = CreateBookingRequest

// Names backend validation messages that the form can attach to its fields.
export interface BookingFieldErrors {
  numberOfTravelers?: string
  contactEmail?: string
  specialRequests?: string
}

// Carries ASP.NET field errors without reducing them to one generic message.
export class BookingValidationError extends Error {
  // Retains normalized field messages for BookingFormPage to display.
  constructor(public readonly fields: BookingFieldErrors) {
    super('Please correct the highlighted booking fields.')
    this.name = 'BookingValidationError'
  }
}

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

// Selects one backend error regardless of PascalCase or camelCase keys.
function getFieldError(
  errors: Record<string, string[]>,
  fieldName: string,
): string | undefined {
  const entry = Object.entries(errors).find(
    ([key]) => key.toLowerCase() === fieldName.toLowerCase(),
  )

  return entry?.[1][0]
}

// Converts ValidationProblemDetails into the form's three known fields.
function toBookingFieldErrors(
  problem: ValidationProblemDetails,
): BookingFieldErrors {
  return {
    numberOfTravelers: getFieldError(
      problem.errors,
      'numberOfTravelers',
    ),
    contactEmail: getFieldError(problem.errors, 'contactEmail'),
    specialRequests: getFieldError(problem.errors, 'specialRequests'),
  }
}

// Recognizes generated ASP.NET validation payloads thrown by BookingsClient.
function isValidationProblem(
  error: unknown,
): error is ValidationProblemDetails {
  return typeof error === 'object'
    && error !== null
    && 'errors' in error
    && typeof error.errors === 'object'
    && error.errors !== null
}

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

// Sends validated form values; the backend still derives ownership from JWT.
export async function createBooking(
  request: CreateBookingInput,
): Promise<CreateBookingResponse> {
  try {
    return await bookingsClient.create(request)
  } catch (error) {
    if (isValidationProblem(error)) {
      throw new BookingValidationError(toBookingFieldErrors(error))
    }

    throw toApiError(error, 'The booking request failed.')
  }
}
