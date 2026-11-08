# BullMarket AI Store

An e-commerce store with a .NET 10 backend (Clean Architecture) and a React + TypeScript (Vite) frontend, backed by PostgreSQL.

## Project Structure

```
backend/                .NET 10 solution (BullMarketAiStore.slnx)
  src/
    BullMarket.Domain          Entities and value objects (no dependencies)
    BullMarket.Application     Use cases and interfaces (-> Domain)
    BullMarket.Infrastructure  Persistence and external services (-> Application)
    BullMarket.Api             ASP.NET Core web API, composition root
  tests/                xUnit projects (Domain, Application, Api)
frontend/               React + TypeScript (Vite)
docs/                   Class diagrams, DB schema, migrations, design notes
docker-compose.yml      PostgreSQL + API containers
podman-compose.yml      Podman equivalent
```

Dependencies point inward only; the Domain layer references nothing.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) and npm
- Podman (or Docker) with compose

## Getting Started

### Run with Podman (Full Stack or Services)

Start the full stack (PostgreSQL, .NET API backend, React frontend):

```bash
podman-compose up -d --build    # or: podman compose up -d --build
```

Or run services individually:

```bash
podman-compose up -d postgres   # Start Database only
podman-compose up -d api        # Start Backend API (+ postgres)
podman-compose up -d frontend   # Start Frontend storefront (+ api + postgres)
```

- Frontend is served at http://localhost:5173
- Backend API is served at http://localhost:5164 (Scalar docs: http://localhost:5164/scalar/v1)

Stop services with `podman-compose down`.

### Run Locally (Development)

1. Start database:
```bash
podman-compose up -d postgres
```

2. Run the API:
```bash
dotnet run --project backend/src/BullMarket.Api
```

3. Run the frontend:
```bash
cd frontend
npm install
npm run dev
```

## Build and Test

```bash
dotnet build backend/BullMarketAiStore.slnx
dotnet test backend/BullMarketAiStore.slnx
cd frontend && npm run build
```

## Configuration

The API reads the database connection string from `appsettings.json` (`ConnectionStrings`) and supports `Database__ApplyMigrationsOnStartup` and `Frontend__AllowedOrigins__0` via environment variables. The credentials in the compose files are for local development only.

## Documentation

See the [docs](docs/) folder:

- [Class diagrams](docs/class-diagrams.md)
- [Database schema](docs/database-schema.md)
- [Migrations](docs/migrations.md)
- [Design pattern recommendations](docs/design-pattern-recommendations.md)
