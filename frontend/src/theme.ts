import { createTheme } from '@mui/material/styles'

// Defines Material UI design tokens consumed by ThemeProvider in main.tsx.
// App components inherit this palette, typography, and shape configuration.
export const theme = createTheme({
  palette: {
    mode: 'dark',
    primary: {
      main: '#8da7ff',
    },
    secondary: {
      main: '#6ee7d8',
    },
    background: {
      default: '#081225',
      paper: '#101d35',
    },
  },
  shape: {
    borderRadius: 12,
  },
  typography: {
    fontFamily:
      'Inter, ui-sans-serif, system-ui, -apple-system, ' +
      'BlinkMacSystemFont, "Segoe UI", sans-serif',
    h1: {
      fontSize: 'clamp(2rem, 5vw, 3.5rem)',
      fontWeight: 700,
      letterSpacing: '-0.04em',
    },
  },
})
