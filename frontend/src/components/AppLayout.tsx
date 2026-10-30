import {
  Alert,
  Box,
  Button,
  Chip,
  Container,
  Stack,
  Typography,
} from '@mui/material'
import { useQueryClient } from '@tanstack/react-query'
import { Link, Outlet } from 'react-router'
import { useAuth } from '../auth/AuthProvider'

// Provides navigation, identity controls, and spacing around routed pages.
// Outlet renders the page selected by App's route configuration.

// Keeps shared page chrome mounted while Outlet changes with the browser URL.
export function AppLayout() {
  const auth = useAuth()
  const queryClient = useQueryClient()

  // Clears identity-specific cache entries before Keycloak signs the user out.
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
              <Chip color="secondary" label="Star Dust" size="small" />
              <Typography component="h1" variant="h1" sx={{ mt: 2 }}>
                Star Dust Travel Agency
              </Typography>
              <Typography color="text.secondary" sx={{ mt: 2, maxWidth: 660 }}>
                Explore scheduled journeys and review bookings owned by your
                authenticated identity.
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

          <Stack direction="row" spacing={1} sx={{ flexWrap: 'wrap' }}>
            <Button component={Link} to="/">
              Home
            </Button>
            {auth.isAuthenticated && (
              <>
                <Button component={Link} to="/journeys">
                  Journeys
                </Button>
                <Button component={Link} to="/bookings">
                  My bookings
                </Button>
              </>
            )}
          </Stack>

          {auth.error && <Alert severity="error">{auth.error}</Alert>}

          <Outlet />

          <Typography color="text.secondary" variant="body2">
            Browser → Keycloak token → nginx → ASP.NET Core → PostgreSQL
          </Typography>
        </Stack>
      </Container>
    </Box>
  )
}
