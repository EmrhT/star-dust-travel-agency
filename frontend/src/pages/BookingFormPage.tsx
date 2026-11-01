import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Paper,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { type FormEvent, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router'
import {
  type BookingFieldErrors,
  BookingValidationError,
  createBooking,
  myBookingsQueryKey,
} from '../api/bookings'
import { getJourneysForWeek } from '../api/journeys'
import { isMonday } from '../utils/journey-week'

// Owns controlled form state for one route-selected Journey booking.
// TanStack Query reloads its week and submits through the generated client.
const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
const uuidPattern =
  /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i

// Groups raw controlled-input strings before validation and normalization.
interface BookingFormValues {
  numberOfTravelers: string
  contactEmail: string
  specialRequests: string
}

// Reports whether the route parameter is safe to send to the backend.
function isUuid(value: string): boolean {
  return uuidPattern.test(value)
}

// Mirrors backend field rules to provide immediate browser feedback.
function validateBookingForm(
  values: BookingFormValues,
): BookingFieldErrors {
  const errors: BookingFieldErrors = {}
  const numberOfTravelers = Number(values.numberOfTravelers)
  const contactEmail = values.contactEmail.trim()
  const specialRequests = values.specialRequests.trim()

  if (
    !Number.isInteger(numberOfTravelers)
    || numberOfTravelers < 1
    || numberOfTravelers > 6
  ) {
    errors.numberOfTravelers =
      'Number of travelers must be an integer from 1 through 6.'
  }

  if (contactEmail.length === 0) {
    errors.contactEmail = 'Contact email is required.'
  } else if (contactEmail.length > 254) {
    errors.contactEmail = 'Contact email cannot exceed 254 characters.'
  } else if (!emailPattern.test(contactEmail)) {
    errors.contactEmail = 'Enter a valid contact email address.'
  }

  if (specialRequests.length > 500) {
    errors.specialRequests =
      'Special requests cannot exceed 500 characters.'
  }

  return errors
}

// Coordinates journey loading, form validation, submission, and navigation.
export function BookingFormPage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const { journeyId = '', weekStarting = '' } = useParams()
  const [numberOfTravelers, setNumberOfTravelers] = useState('1')
  const [contactEmail, setContactEmail] = useState('')
  const [specialRequests, setSpecialRequests] = useState('')
  const [fieldErrors, setFieldErrors] = useState<BookingFieldErrors>({})
  const routeIsValid = isMonday(weekStarting) && isUuid(journeyId)
  const journeysQuery = useQuery({
    queryKey: ['journeys', 'week', weekStarting],
    queryFn: ({ signal }) =>
      getJourneysForWeek(weekStarting, { signal }),
    enabled: routeIsValid,
  })
  const journey = journeysQuery.data?.find(
    (candidate) => candidate.id === journeyId,
  )
  const createBookingMutation = useMutation({
    mutationFn: createBooking,
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: myBookingsQueryKey,
      })
      navigate('/bookings', { replace: true })
    },
    onError: (error) => {
      if (error instanceof BookingValidationError) {
        setFieldErrors(error.fields)
      }
    },
  })

  // Validates controlled fields before starting the booking mutation.
  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!journey || journey.status !== 'Scheduled') {
      return
    }

    const values = {
      numberOfTravelers,
      contactEmail,
      specialRequests,
    }
    const errors = validateBookingForm(values)
    setFieldErrors(errors)

    if (Object.keys(errors).length > 0) {
      return
    }

    const normalizedSpecialRequests = specialRequests.trim()
    createBookingMutation.mutate({
      journeyId,
      numberOfTravelers: Number(numberOfTravelers),
      contactEmail: contactEmail.trim(),
      specialRequests: normalizedSpecialRequests || undefined,
    })
  }

  if (!routeIsValid) {
    return (
      <Alert severity="error">
        The booking URL does not contain a valid Monday and Journey ID.{' '}
        <Link to="/journeys">Return to journeys.</Link>
      </Alert>
    )
  }

  if (journeysQuery.isPending) {
    return (
      <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
        <CircularProgress size={22} />
        <Typography>Loading the selected journey…</Typography>
      </Stack>
    )
  }

  if (journeysQuery.isError) {
    return (
      <Alert severity="error">
        {journeysQuery.error.message}{' '}
        <Link to="/journeys">Return to journeys.</Link>
      </Alert>
    )
  }

  if (!journey) {
    return (
      <Alert severity="error">
        The selected journey does not belong to this schedule week.{' '}
        <Link to="/journeys">Return to journeys.</Link>
      </Alert>
    )
  }

  return (
    <Paper variant="outlined" sx={{ p: { xs: 3, md: 4 } }}>
      <Stack spacing={3}>
        <Box>
          <Typography component="h2" variant="h4">
            Complete your booking
          </Typography>
          <Typography color="text.secondary" sx={{ mt: 1 }}>
            {journey.launchStation} → {journey.destination} ·{' '}
            {journey.departureAtUtc}
          </Typography>
        </Box>

        {journey.status !== 'Scheduled' && (
          <Alert severity="warning">
            This journey is no longer available for booking.
          </Alert>
        )}

        {createBookingMutation.isError && (
          <Alert severity="error">
            {createBookingMutation.error.message}
          </Alert>
        )}

        <Box component="form" noValidate onSubmit={handleSubmit}>
          <Stack spacing={2.5}>
            <TextField
              required
              error={fieldErrors.numberOfTravelers !== undefined}
              helperText={
                fieldErrors.numberOfTravelers
                ?? 'Choose between 1 and 6 travelers.'
              }
              label="Number of travelers"
              type="number"
              value={numberOfTravelers}
              onChange={(event) =>
                setNumberOfTravelers(event.target.value)
              }
              slotProps={{
                htmlInput: { min: 1, max: 6, step: 1 },
              }}
            />

            <TextField
              required
              error={fieldErrors.contactEmail !== undefined}
              helperText={
                fieldErrors.contactEmail
                ?? 'We will use this address for booking communication.'
              }
              label="Contact email"
              type="email"
              value={contactEmail}
              onChange={(event) => setContactEmail(event.target.value)}
              slotProps={{ htmlInput: { maxLength: 254 } }}
            />

            <TextField
              multiline
              error={fieldErrors.specialRequests !== undefined}
              helperText={
                fieldErrors.specialRequests
                ?? `${specialRequests.trim().length}/500 characters`
              }
              label="Special requests"
              minRows={4}
              value={specialRequests}
              onChange={(event) => setSpecialRequests(event.target.value)}
              slotProps={{ htmlInput: { maxLength: 500 } }}
            />

            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <Button
                disabled={
                  journey.status !== 'Scheduled'
                  || createBookingMutation.isPending
                }
                type="submit"
                variant="contained"
              >
                {createBookingMutation.isPending
                  ? 'Creating booking…'
                  : 'Confirm booking'}
              </Button>
              <Button component={Link} to="/journeys" variant="outlined">
                Cancel
              </Button>
            </Stack>
          </Stack>
        </Box>
      </Stack>
    </Paper>
  )
}
