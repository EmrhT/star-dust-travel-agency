import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  Container,
  Paper,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { createBooking, getMyBookings } from './api/bookings'
import { generateJourneysForWeek, getJourneysForWeek } from './api/journeys'
import { getSystemStatus } from './api/system'
import { useAuth } from './auth/AuthProvider'
import { BookingTable } from './components/BookingTable'
import { JourneyTable } from './components/JourneyTable'

// Renders identity, connectivity, journey, and owned-booking interactions.
// Auth gates TanStack operations; tables present their cached server state.
const initialJourneyWeek = '2026-10-12'

const istanbulDateTime = new Intl.DateTimeFormat('en-GB', {
  year: 'numeric',
  month: 'short',
  day: '2-digit',
  hour: '2-digit',
  minute: '2-digit',
  second: '2-digit',
  timeZone: 'Europe/Istanbul',
  timeZoneName: 'short',
})

// Checks the selected date before App asks the API for a schedule week.
function isMonday(dateValue: string) {
  const date = new Date(`${dateValue}T00:00:00Z`)

  return !Number.isNaN(date.getTime()) && date.getUTCDay() === 1
}

// Coordinates API server state and renders JourneyTable and BookingTable.
function App() {
  const auth = useAuth()
  const [journeyWeekStarting, setJourneyWeekStarting] =
    useState(initialJourneyWeek)
  const journeyWeekIsValid = isMonday(journeyWeekStarting)
  const queryClient = useQueryClient()
  const statusQuery = useQuery({
    queryKey: ['system', 'status'],
    queryFn: ({ signal }) => getSystemStatus({ signal }),
    enabled: auth.isAuthenticated,
  })
  const journeysQuery = useQuery({
    queryKey: ['journeys', 'week', journeyWeekStarting],
    queryFn: ({ signal }) =>
      getJourneysForWeek(journeyWeekStarting, { signal }),
    enabled: auth.isAuthenticated && journeyWeekIsValid,
  })
  const bookingsQuery = useQuery({
    queryKey: ['bookings', 'mine'],
    queryFn: ({ signal }) => getMyBookings({ signal }),
    enabled: auth.isAuthenticated,
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
        queryKey: ['bookings', 'mine'],
      })
    },
  })
  const bookedJourneyIds = new Set(
    bookingsQuery.data?.map((booking) => booking.journeyId) ?? [],
  )

  // Clears user-specific server state before redirecting through logout.
  async function handleLogout() {
    queryClient.clear()
    await auth.logout()
  }

  return (
    <Box component="main" sx={{ minHeight: '100vh', py: { xs: 6, md: 12 } }}>
      <Container maxWidth="md">
        <Stack spacing={4}>
          <Stack
            direction={{ xs: 'column', sm: 'row' }}
            spacing={3}
            sx={{ justifyContent: 'space-between' }}
          >
            <Box>
              <Chip
                color="secondary"
                label="Authentication milestone"
                size="small"
              />
              <Typography component="h1" variant="h1" sx={{ mt: 2 }}>
                Star Dust Travel Agency
              </Typography>
              <Typography color="text.secondary" sx={{ mt: 2, maxWidth: 660 }}>
                Sign in through Keycloak to read journey data. Operators can
                also generate a schedule week.
              </Typography>
            </Box>

            <Stack
              direction={{ xs: 'row', sm: 'column' }}
              spacing={1}
              sx={{ alignItems: { xs: 'center', sm: 'flex-end' } }}
            >
              {auth.isAuthenticated && (
                <Chip
                  color="primary"
                  label={auth.username ?? 'Authenticated user'}
                />
              )}
              <Button
                disabled={auth.isInitializing}
                onClick={() => {
                  if (auth.isAuthenticated) {
                    void handleLogout()
                  } else {
                    void auth.login()
                  }
                }}
                variant="outlined"
              >
                {auth.isInitializing
                  ? 'Checking session…'
                  : auth.isAuthenticated
                    ? 'Sign out'
                    : 'Sign in'}
              </Button>
            </Stack>
          </Stack>

          {auth.error && <Alert severity="error">{auth.error}</Alert>}

          {!auth.isInitializing && !auth.isAuthenticated && (
            <Paper variant="outlined" sx={{ p: { xs: 3, md: 4 } }}>
              <Stack spacing={2}>
                <Typography component="h2" variant="h5">
                  Authentication required
                </Typography>
                <Typography color="text.secondary">
                  Keycloak establishes your identity before the browser calls
                  the protected ASP.NET Core API.
                </Typography>
                <Box>
                  <Button onClick={() => void auth.login()} variant="contained">
                    Sign in with Keycloak
                  </Button>
                </Box>
              </Stack>
            </Paper>
          )}

          {auth.isAuthenticated && (
            <Paper variant="outlined" sx={{ p: { xs: 3, md: 4 } }}>
              <Stack spacing={2.5}>
                <Typography component="h2" variant="h5">
                  System connectivity
                </Typography>

                {statusQuery.isPending && (
                  <Stack
                    direction="row"
                    spacing={2}
                    sx={{ alignItems: 'center' }}
                  >
                    <CircularProgress size={22} />
                    <Typography>Checking the API and PostgreSQL…</Typography>
                  </Stack>
                )}

                {statusQuery.isError && (
                  <Alert
                    severity="error"
                    action={
                      <Button
                        color="inherit"
                        size="small"
                        onClick={() => statusQuery.refetch()}
                      >
                        Retry
                      </Button>
                    }
                  >
                    {statusQuery.error.message}
                  </Alert>
                )}

                {statusQuery.isSuccess && (
                  <Stack spacing={1.5}>
                    <Alert severity="success">
                      The complete request path is working.
                    </Alert>
                    <Typography>
                      API: <strong>{statusQuery.data.service}</strong>
                    </Typography>
                    <Typography>
                      PostgreSQL: <strong>{statusQuery.data.database}</strong>
                    </Typography>
                    <Typography color="text.secondary" variant="body2">
                      Checked at{' '}
                      {istanbulDateTime.format(
                        new Date(statusQuery.data.checkedAtUtc),
                      )}
                    </Typography>
                  </Stack>
                )}
              </Stack>
            </Paper>
          )}

          {auth.isAuthenticated && (
            <Paper variant="outlined" sx={{ p: { xs: 3, md: 4 } }}>
              <Stack spacing={2.5}>
                <Typography component="h2" variant="h5">
                  Journey data
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
                    {generateWeekMutation.isPending
                      ? 'Generating…'
                      : 'Generate week'}
                  </Button>
                </Box>

                {!auth.canGenerateJourneys && (
                  <Alert severity="info">
                    Your account has read-only journey access. Generation needs
                    the journey-operator role.
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
                    and{' '}
                    <strong>
                      {generateWeekMutation.data.alreadyExisted}
                    </strong>{' '}
                    already existed.
                  </Alert>
                )}

                {!journeyWeekIsValid && (
                  <Alert severity="warning">
                    Select a Monday before loading journeys.
                  </Alert>
                )}

                {journeyWeekIsValid && journeysQuery.isPending && (
                  <Stack
                    direction="row"
                    spacing={2}
                    sx={{ alignItems: 'center' }}
                  >
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
                      severity={
                        journeysQuery.data.length === 0 ? 'info' : 'success'
                      }
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
                    Booking created. The owned-booking list was refreshed.
                  </Alert>
                )}
              </Stack>
            </Paper>
          )}

          {auth.isAuthenticated && (
            <Paper variant="outlined" sx={{ p: { xs: 3, md: 4 } }}>
              <Stack spacing={2.5}>
                <Typography component="h2" variant="h5">
                  My bookings
                </Typography>

                {bookingsQuery.isPending && (
                  <Stack
                    direction="row"
                    spacing={2}
                    sx={{ alignItems: 'center' }}
                  >
                    <CircularProgress size={22} />
                    <Typography>Loading your bookings…</Typography>
                  </Stack>
                )}

                {bookingsQuery.isError && (
                  <Alert
                    severity="error"
                    action={
                      <Button
                        color="inherit"
                        size="small"
                        onClick={() => bookingsQuery.refetch()}
                      >
                        Retry
                      </Button>
                    }
                  >
                    {bookingsQuery.error.message}
                  </Alert>
                )}

                {bookingsQuery.isSuccess && (
                  <Stack spacing={2}>
                    <Alert
                      severity={
                        bookingsQuery.data.length === 0 ? 'info' : 'success'
                      }
                    >
                      <strong>{bookingsQuery.data.length}</strong> bookings are
                      owned by your authenticated identity.
                    </Alert>
                    {bookingsQuery.data.length > 0 && (
                      <BookingTable bookings={bookingsQuery.data} />
                    )}
                  </Stack>
                )}
              </Stack>
            </Paper>
          )}

          <Typography color="text.secondary" variant="body2">
            Browser → Keycloak token → nginx → ASP.NET Core → PostgreSQL
          </Typography>
        </Stack>
      </Container>
    </Box>
  )
}

export default App
