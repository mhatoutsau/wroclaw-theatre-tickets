import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import path from "path";
import https from "https";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    port: 5173,
    proxy: {
      "/api": {
        target: "https://localhost:50398",
        changeOrigin: true,
        secure: false, // ← This disables SSL verification for the proxy
        agent: new https.Agent({
          rejectUnauthorized: false, // ← Accept self-signed certificates
        }),
        xfwd: true,
        headers: {
          "X-Proxied-By": "Vite",
        },
        // Middleware to log and verify headers are being passed
        configure: (proxy, options) => {
          proxy.on("proxyReq", (proxyReq, req) => {
            // Force copy Authorization header from incoming request
            const authHeader = req.headers["authorization"];
            if (authHeader) {
              proxyReq.setHeader("Authorization", authHeader);
              console.log(
                `[Proxy] ✓ Authorization header copied: ${authHeader.substring(0, 30)}...`,
              );
            } else {
              console.warn(
                `[Proxy] ⚠️ No Authorization header in request to ${req.url}`,
              );
            }

            // Copy all headers explicitly
            Object.keys(req.headers).forEach((key) => {
              const value = req.headers[key];
              if (value && !proxyReq.getHeader(key)) {
                proxyReq.setHeader(key, value);
              }
            });

            console.log(
              `[Proxy] ${req.method} ${req.url} → ${options.target}${req.url}`,
            );
          });
        },
      },
    },
  },
});
