import { CircularProgress, Stack, Typography } from '@mui/material'
import { Navigate, Outlet } from 'react-router'
import { useAuth } from '../auth/AuthProvider'

// Protects routed UI while AuthProvider initializes or lacks a user session.
// ASP.NET Core authorization remains the authoritative API security boundary.

// Renders protected child routes only for an authenticated Keycloak session.
export function RequireAuthentication() {
  const auth = useAuth()

  if (auth.isInitializing) {
    return (
      <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
        <CircularProgress size={22} />
        <Typography>Checking your authentication session…</Typography>
      </Stack>
    )
  }

  if (!auth.isAuthenticated) {
    return <Navigate replace to="/" />
  }

  return <Outlet />
}
