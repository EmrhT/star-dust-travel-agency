// Centralizes journey-week validation shared by list and booking pages.
// JourneysPage and BookingFormPage use it before starting API queries.

export const initialJourneyWeek = '2026-10-12'

// Reports whether an ISO calendar date represents a valid Monday in UTC.
export function isMonday(dateValue: string): boolean {
  const date = new Date(`${dateValue}T00:00:00Z`)

  return !Number.isNaN(date.getTime()) && date.getUTCDay() === 1
}
