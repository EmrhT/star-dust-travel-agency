import {
  Alert,
  Button,
  CircularProgress,
  Paper,
  Stack,
  Typography,
} from '@mui/material'
import { useQuery } from '@tanstack/react-query'
import { getMyBookings, myBookingsQueryKey } from '../api/bookings'
import { BookingTable } from '../components/BookingTable'

// Owns the identity-filtered booking query rendered at /bookings.
// It reuses cache updates produced by booking mutations on JourneysPage.

// Renders loading, failure, empty, and populated owned-booking states.
export function BookingsPage() {
  const bookingsQuery = useQuery({
    queryKey: myBookingsQueryKey,
    queryFn: ({ signal }) => getMyBookings({ signal }),
  })

  return (
    <Paper variant="outlined" sx={{ p: { xs: 3, md: 4 } }}>
      <Stack spacing={2.5}>
        <Typography component="h2" variant="h4">
          My bookings
        </Typography>

        {bookingsQuery.isPending && (
          <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
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
              severity={bookingsQuery.data.length === 0 ? 'info' : 'success'}
            >
              <strong>{bookingsQuery.data.length}</strong> bookings are owned by
              your authenticated identity.
            </Alert>
            {bookingsQuery.data.length > 0 && (
              <BookingTable bookings={bookingsQuery.data} />
            )}
          </Stack>
        )}
      </Stack>
    </Paper>
  )
}
