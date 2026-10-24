import {
  createContext,
  type ReactNode,
  useContext,
  useEffect,
  useMemo,
  useState,
} from 'react'
import {
  canGenerateJourneys,
  getUsername,
  initializeAuthentication,
  keycloak,
  login,
  logout,
} from './keycloak'

// Bridges Keycloak adapter events into stable React authentication state.
// App consumes this context without reading tokens or adapter internals.
interface AuthContextValue {
  isInitializing: boolean
  isAuthenticated: boolean
  canGenerateJourneys: boolean
  username?: string
  error?: string
  login: () => Promise<void>
  logout: () => Promise<void>
}

// Describes children whose components may consume authentication state.
interface AuthProviderProps {
  children: ReactNode
}

// Holds the mutable adapter snapshot published through AuthContext.
interface AuthState {
  isInitializing: boolean
  isAuthenticated: boolean
  canGenerateJourneys: boolean
  username?: string
  error?: string
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

// Builds a React snapshot from the adapter's current in-memory token state.
function readAuthState(isInitializing = false): AuthState {
  return {
    isInitializing,
    isAuthenticated: keycloak.authenticated === true,
    canGenerateJourneys: canGenerateJourneys(),
    username: getUsername(),
  }
}

// Initializes Keycloak and publishes adapter changes to descendant components.
export function AuthProvider({ children }: AuthProviderProps) {
  const [state, setState] = useState<AuthState>(() => readAuthState(true))

  useEffect(() => {
    let isActive = true

    // Copies adapter changes only while this provider instance remains mounted.
    const synchronize = () => {
      if (isActive) {
        setState(readAuthState())
      }
    }

    keycloak.onAuthSuccess = synchronize
    keycloak.onAuthRefreshSuccess = synchronize
    keycloak.onAuthLogout = synchronize
    keycloak.onTokenExpired = () => {
      void keycloak
        .updateToken(30)
        .then(synchronize)
        .catch(() => {
          keycloak.clearToken()
          synchronize()
        })
    }

    void initializeAuthentication()
      .then(synchronize)
      .catch(() => {
        if (isActive) {
          setState({
            ...readAuthState(),
            error: 'Authentication service initialization failed.',
          })
        }
      })

    return () => {
      isActive = false
    }
  }, [])

  const value = useMemo<AuthContextValue>(
    () => ({
      ...state,
      login,
      logout,
    }),
    [state],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

// Returns the nearest AuthProvider state and rejects missing composition.
export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be called inside AuthProvider.')
  }

  return context
}
