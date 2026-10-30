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
import { useState } from 'react'
import {
  createBooking,
  getMyBookings,
  myBookingsQueryKey,
} from '../api/bookings'
import {
  generateJourneysForWeek,
  getJourneysForWeek,
} from '../api/journeys'
import { useAuth } from '../auth/AuthProvider'
import { JourneyTable } from '../components/JourneyTable'

// Owns journey queries, generation, and booking mutations for /journeys.
// TanStack cache invalidation synchronizes this page with /bookings.
const initialJourneyWeek = '2026-10-12'

// Checks the selected date before the page requests a schedule week.
function isMonday(dateValue: string) {
  const date = new Date(`${dateValue}T00:00:00Z`)

  return !Number.isNaN(date.getTime()) && date.getUTCDay() === 1
}

// Coordinates journey server state and reports booking intents to the API.
export function JourneysPage() {
  const auth = useAuth()
  const queryClient = useQueryClient()
  const [journeyWeekStarting, setJourneyWeekStarting] =
    useState(initialJourneyWeek)
  const journeyWeekIsValid = isMonday(journeyWeekStarting)
  const journeysQuery = useQuery({
    queryKey: ['journeys', 'week', journeyWeekStarting],
    queryFn: ({ signal }) =>
      getJourneysForWeek(journeyWeekStarting, { signal }),
    enabled: journeyWeekIsValid,
  })
  const bookingsQuery = useQuery({
    queryKey: myBookingsQueryKey,
    queryFn: ({ signal }) => getMyBookings({ signal }),
  })
  const generateWeekMutation = useMutation({
    mutationFn: generateJourneysForWeek,
    onSuccess: async (result) => {
      await queryClient.invalidateQueries({
        queryKey: ['journeys', 'week', result.weekStarting],
      })
    },
  })
  const createBookingMutation = useMutation({
    mutationFn: createBooking,
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: myBookingsQueryKey,
      })
    },
  })
  const bookedJourneyIds = new Set(
    bookingsQuery.data?.map((booking) => booking.journeyId) ?? [],
  )

  return (
    <Paper variant="outlined" sx={{ p: { xs: 3, md: 4 } }}>
      <Stack spacing={2.5}>
        <Typography component="h2" variant="h4">
          Journeys
        </Typography>

        <TextField
          label="Week starting"
          type="date"
          value={journeyWeekStarting}
          onChange={(event) => {
            setJourneyWeekStarting(event.target.value)
            generateWeekMutation.reset()
          }}
          error={!journeyWeekIsValid}
          helperText={
            journeyWeekIsValid
              ? 'Select a Monday to load that schedule week.'
              : 'The selected date must be a Monday.'
          }
          slotProps={{ inputLabel: { shrink: true } }}
          sx={{ maxWidth: 280 }}
        />

        <Box>
          <Button
            variant="contained"
            disabled={
              !auth.canGenerateJourneys ||
              !journeyWeekIsValid ||
              generateWeekMutation.isPending
            }
            onClick={() =>
              generateWeekMutation.mutate(journeyWeekStarting)
            }
          >
            {generateWeekMutation.isPending ? 'Generating…' : 'Generate week'}
          </Button>
        </Box>

        {!auth.canGenerateJourneys && (
          <Alert severity="info">
            Your account has read-only journey access. Generation needs the
            journey-operator role.
          </Alert>
        )}

        {generateWeekMutation.isError && (
          <Alert severity="error">
            {generateWeekMutation.error.message}
          </Alert>
        )}

        {generateWeekMutation.isSuccess && (
          <Alert severity="success">
            Generation finished:{' '}
            <strong>{generateWeekMutation.data.created}</strong> created
            {' and '}
            <strong>{generateWeekMutation.data.alreadyExisted}</strong> already
            existed.
          </Alert>
        )}

        {!journeyWeekIsValid && (
          <Alert severity="warning">
            Select a Monday before loading journeys.
          </Alert>
        )}

        {journeyWeekIsValid && journeysQuery.isPending && (
          <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
            <CircularProgress size={22} />
            <Typography>Loading the generated week…</Typography>
          </Stack>
        )}

        {journeyWeekIsValid && journeysQuery.isError && (
          <Alert
            severity="error"
            action={
              <Button
                color="inherit"
                size="small"
                onClick={() => journeysQuery.refetch()}
              >
                Retry
              </Button>
            }
          >
            {journeysQuery.error.message}
          </Alert>
        )}

        {journeyWeekIsValid && journeysQuery.isSuccess && (
          <Stack spacing={2}>
            <Alert
              severity={journeysQuery.data.length === 0 ? 'info' : 'success'}
            >
              <strong>{journeysQuery.data.length}</strong> journeys were
              returned for the week starting {journeyWeekStarting}.
            </Alert>
            {journeysQuery.data.length > 0 && (
              <JourneyTable
                bookedJourneyIds={bookedJourneyIds}
                bookingActionsDisabled={!bookingsQuery.isSuccess}
                bookingJourneyId={
                  createBookingMutation.isPending
                    ? createBookingMutation.variables
                    : undefined
                }
                journeys={journeysQuery.data}
                onBook={(journeyId) =>
                  createBookingMutation.mutate(journeyId)
                }
              />
            )}
          </Stack>
        )}

        {createBookingMutation.isError && (
          <Alert severity="error">
            {createBookingMutation.error.message}
          </Alert>
        )}

        {createBookingMutation.isSuccess && (
          <Alert severity="success">
            Booking created. The owned-booking cache was refreshed.
          </Alert>
        )}
      </Stack>
    </Paper>
  )
}
