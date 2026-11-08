# BullMarket AI Store – Agent Guide

## Layout
- `backend/` – .NET 10 solution (`BullMarketAiStore.slnx`), Clean Architecture
  - `src/BullMarket.Domain` – entities, value objects (no dependencies)
  - `src/BullMarket.Application` – use cases, interfaces (→ Domain)
  - `src/BullMarket.Infrastructure` – persistence, external services (→ Application)
  - `src/BullMarket.Api` – ASP.NET Core web API, composition root (→ Application, Infrastructure)
  - `tests/` – xUnit projects for backend (`Domain.Tests`, `Application.Tests`, `Api.Tests`)
- `frontend/` – React + TypeScript (Vite)

## Rules
- Dependencies point inward only; Domain references nothing.
- Add tests in `backend/tests/` for new backend behavior.

## Commands
- Start Database: `podman-compose up -d` (or `podman compose up -d`)
- Stop Database: `podman-compose down`
- Backend build: `dotnet build backend/BullMarketAiStore.slnx`
- Backend tests: `dotnet test backend/BullMarketAiStore.slnx`
- Run API: `dotnet run --project backend/src/BullMarket.Api`
- Frontend: `cd frontend && npm install && npm run dev` (build: `npm run build`)
