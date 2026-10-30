import { CircularProgress, Stack, Typography } from '@mui/material'
import { Navigate } from 'react-router'
import { useAuth } from '../auth/AuthProvider'
import { takePostAuthenticationPath } from '../auth/keycloak'

// Keeps Keycloak's callback URL mounted while AuthProvider processes its code.
// Successful sessions continue to Journeys; failures return to the Home page.

// Waits for Keycloak initialization before selecting the post-login route.
export function AuthenticationCallbackPage() {
  const auth = useAuth()

  if (auth.isInitializing) {
    return (
      <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
        <CircularProgress size={22} />
        <Typography>Completing authentication…</Typography>
      </Stack>
    )
  }

  const destination = auth.isAuthenticated
    ? takePostAuthenticationPath()
    : '/'

  return <Navigate replace to={destination} />
}
