# BullMarket AI Store – Frontend

The BullMarket AI Store customer storefront, built with React, TypeScript, Vite, Tailwind CSS, TanStack Query, React Router, Axios, and Vitest.

## Run locally

The Vite development server proxies `/api` and `/health` to the backend at http://localhost:5164.

```bash
npm install
npm run dev
```

Copy `.env.example` to `.env.local` when the API or demo customer differs from the defaults.

## Quality commands

```bash
npm run typecheck
npm run lint
npm test
npm run build
```

## Structure

- `src/app` — application providers and routing
- `src/components` — shared layout and UI primitives
- `src/features` — feature-owned catalog, cart, order, and home modules
- `src/config` — runtime configuration boundary
- `src/lib` — HTTP and formatting utilities
- `src/pages` — cross-feature route pages
- `src/test` — shared test setup

The customer ID defaults to the deterministic development customer seeded by the API. Authentication will replace this development bridge in a later phase.

## Podman / Docker

From the repository root, start the complete stack with:

```bash
podman-compose up -d --build   # or: podman compose up -d --build
```

To run individual services:

```bash
podman-compose up -d api        # Backend API only
podman-compose up -d frontend   # Frontend storefront only
```

The production frontend is served by Nginx at http://localhost:5173. Nginx handles SPA route fallback and proxies `/api` and `/health` to the API container over the internal compose network.
