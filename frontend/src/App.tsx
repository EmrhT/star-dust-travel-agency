import { Navigate, Route, Routes } from 'react-router'
import { AppLayout } from './components/AppLayout'
import { AuthenticationCallbackPage } from './pages/AuthenticationCallbackPage'
import { BookingsPage } from './pages/BookingsPage'
import { HomePage } from './pages/HomePage'
import { JourneysPage } from './pages/JourneysPage'
import { RequireAuthentication } from './routes/RequireAuthentication'

// Maps browser URLs to page components inside the shared AppLayout.
// RequireAuthentication guards private pages without replacing API security.

// Declares public, callback, protected, and fallback browser routes.
function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route index element={<HomePage />} />
        <Route
          path="auth/callback"
          element={<AuthenticationCallbackPage />}
        />
        <Route element={<RequireAuthentication />}>
          <Route path="journeys" element={<JourneysPage />} />
          <Route path="bookings" element={<BookingsPage />} />
        </Route>
        <Route path="*" element={<Navigate replace to="/" />} />
      </Route>
    </Routes>
  )
}

export default App
