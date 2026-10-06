import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Configures React compilation and proxies local API calls to ASP.NET Core.
// Production uses nginx instead, with the same browser-facing /api path.
export default defineConfig({
  plugins: [react()],
  server: {
    host: '0.0.0.0',
    port: 5173,
    proxy: {
      '/api': {
        target:
          process.env.VITE_DEV_API_PROXY_TARGET ??
          'http://localhost:8080',
        changeOrigin: true,
      },
    },
  },
})
