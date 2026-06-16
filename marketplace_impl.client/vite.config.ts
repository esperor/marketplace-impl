import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';
import { TanStackRouterVite } from '@tanstack/router-vite-plugin'

let certFilePath;
let keyFilePath;
const inContainer = process.env.CI === 'true' || process.env.CONTAINER === '1' || process.env.DOCKER === '1';
if (!inContainer) {
  const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
      ? `${env.APPDATA}/ASP.NET/https`
      : `${env.HOME}/.aspnet/https`;

  const certificateName = 'marketplace_impl.client';
  certFilePath = path.join(baseFolder, `${certificateName}.pem`);
  keyFilePath = path.join(baseFolder, `${certificateName}.key`);

  if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (
      0 !==
      child_process.spawnSync(
        'dotnet',
        [
          'dev-certs',
          'https',
          '--export-path',
          certFilePath,
          '--format',
          'Pem',
          '--no-password',
        ],
        { stdio: 'inherit' },
      ).status
    ) {
      throw new Error('Could not create certificate.');
    }
  }
}

const httpsPort = env.ASPNETCORE_HTTPS_PORT;
const urlsEnv = env.ASPNETCORE_URLS;
let target = null;

if (httpsPort) {
  target = `https://localhost:${httpsPort}`;
} else if (urlsEnv) {
  target = urlsEnv.split(';')[0];
} else {
  // fallback to HTTP in CI/containers where dev certs are not present
  const defaultHttps = 'https://localhost:7047';
  const defaultHttp = 'http://localhost:5000';
  target = process.env.CI === 'true' || process.env.DOCKER === '1' ? defaultHttp : defaultHttps;
}

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin(), TanStackRouterVite(),],
  resolve: {
    alias: {
      // SYNC PATH ALIASES
      '#': fileURLToPath(new URL('./src', import.meta.url)),
      '#business': fileURLToPath(new URL('./src/components/routes/business', import.meta.url)),
    },
  },
  server: {
    host: '127.0.0.1',
    proxy: {
      '/api': {
        target,
        changeOrigin: true,
        secure: false,
      },
    },
    port: 5173,
    https: (keyFilePath && certFilePath) ? {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath),
    } : undefined,
  },
});
