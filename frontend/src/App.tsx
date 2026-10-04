import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  Container,
  Paper,
  Stack,
  Typography,
} from '@mui/material'
import { useQuery } from '@tanstack/react-query'
import { getSystemStatus } from './api/system'

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

function App() {
  const statusQuery = useQuery({
    queryKey: ['system', 'status'],
    queryFn: ({ signal }) => getSystemStatus({ signal }),
  })

  return (
    <Box component="main" sx={{ minHeight: '100vh', py: { xs: 6, md: 12 } }}>
      <Container maxWidth="md">
        <Stack spacing={4}>
          <Box>
            <Chip color="secondary" label="Foundation milestone" size="small" />
            <Typography component="h1" variant="h1" sx={{ mt: 2 }}>
              Star Dust Travel Agency
            </Typography>
            <Typography color="text.secondary" sx={{ mt: 2, maxWidth: 660 }}>
              The customer and operations portal is under construction. This page
              verifies the production frontend, API, and database connection.
            </Typography>
          </Box>

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
                    <Button color="inherit" size="small" onClick={() => statusQuery.refetch()}>
                      Retry
                    </Button>
                  }
                >
                  {statusQuery.error.message}
                </Alert>
              )}

              {statusQuery.isSuccess && (
                <Stack spacing={1.5}>
                  <Alert severity="success">The complete request path is working.</Alert>
                  <Typography>
                    API: <strong>{statusQuery.data.service}</strong>
                  </Typography>
                  <Typography>
                    PostgreSQL: <strong>{statusQuery.data.database}</strong>
                  </Typography>
                  <Typography color="text.secondary" variant="body2">
                    Checked at {istanbulDateTime.format(new Date(statusQuery.data.checkedAtUtc))}
                  </Typography>
                </Stack>
              )}
            </Stack>
          </Paper>

          <Typography color="text.secondary" variant="body2">
            Browser → nginx → ASP.NET Core → PostgreSQL
          </Typography>
        </Stack>
      </Container>
    </Box>
  )
}

export default App
