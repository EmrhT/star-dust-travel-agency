import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Paper,
  Stack,
  Typography,
} from '@mui/material'
import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router'
import { getSystemStatus } from '../api/system'
import { useAuth } from '../auth/AuthProvider'

// Renders the public sign-in prompt or authenticated connectivity summary.
// System status stays on this landing page while feature pages remain focused.
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

// Selects public or authenticated home content from AuthProvider state.
export function HomePage() {
  const auth = useAuth()
  const statusQuery = useQuery({
    queryKey: ['system', 'status'],
    queryFn: ({ signal }) => getSystemStatus({ signal }),
    enabled: auth.isAuthenticated,
  })

  if (auth.isInitializing) {
    return (
      <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
        <CircularProgress size={22} />
        <Typography>Checking your authentication session…</Typography>
      </Stack>
    )
  }

  if (!auth.isAuthenticated) {
    return (
      <Paper variant="outlined" sx={{ p: { xs: 3, md: 4 } }}>
        <Stack spacing={2}>
          <Typography component="h2" variant="h5">
            Authentication required
          </Typography>
          <Typography color="text.secondary">
            Keycloak establishes your identity before the browser calls the
            protected ASP.NET Core API.
          </Typography>
          <Box>
            <Button onClick={() => void auth.login()} variant="contained">
              Sign in with Keycloak
            </Button>
          </Box>
        </Stack>
      </Paper>
    )
  }

  return (
    <Stack spacing={4}>
      <Paper variant="outlined" sx={{ p: { xs: 3, md: 4 } }}>
        <Stack spacing={2}>
          <Typography component="h2" variant="h5">
            Choose a workspace
          </Typography>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <Button component={Link} to="/journeys" variant="contained">
              Explore journeys
            </Button>
            <Button component={Link} to="/bookings" variant="outlined">
              View my bookings
            </Button>
          </Stack>
        </Stack>
      </Paper>

      <Paper variant="outlined" sx={{ p: { xs: 3, md: 4 } }}>
        <Stack spacing={2.5}>
          <Typography component="h2" variant="h5">
            System connectivity
          </Typography>

          {statusQuery.isPending && (
            <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
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
    </Stack>
  )
}
