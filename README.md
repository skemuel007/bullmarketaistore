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

### 1. Start the database

```bash
podman-compose up -d      # or: podman compose up -d
```

Stop it with `podman-compose down`.

### 2. Run the API

```bash
dotnet run --project backend/src/BullMarket.Api
```

### 3. Run the frontend

```bash
cd frontend
npm install
npm run dev
```

The frontend is served at http://localhost:5173.

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
