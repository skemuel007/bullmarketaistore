# EF Core Migrations Guide

This guide details how to create, apply, roll back, and script database migrations for the BullMarket AI Store backend.

---

## Prerequisites

Ensure the EF Core command-line tool is installed:

```bash
dotnet tool install --global dotnet-ef
# or update existing tool
dotnet tool update --global dotnet-ef
```

---

## Projects Structure

In accordance with Clean Architecture:
- **Migrations Assembly (Target Project)**: `backend/src/BullMarket.Infrastructure` (contains `BulkMarketAiStoreDbContext` and the `Migrations/` directory)
- **Startup Project**: `backend/src/BullMarket.Api` (contains `appsettings.json`, connection strings, and DI configuration)

---

## Commands Reference

Run commands from the repository root:

### 1. Create a New Migration
```bash
dotnet ef migrations add <MigrationName> \
  --project backend/src/BullMarket.Infrastructure \
  --startup-project backend/src/BullMarket.Api \
  --output-dir Migrations
```

*Example:*
```bash
dotnet ef migrations add AddOrderTrackingNumber \
  --project backend/src/BullMarket.Infrastructure \
  --startup-project backend/src/BullMarket.Api
```

### 2. Apply Migrations to the Database
```bash
dotnet ef database update \
  --project backend/src/BullMarket.Infrastructure \
  --startup-project backend/src/BullMarket.Api
```

To target a specific migration:
```bash
dotnet ef database update <MigrationName> \
  --project backend/src/BullMarket.Infrastructure \
  --startup-project backend/src/BullMarket.Api
```

### 3. Remove the Last Unapplied Migration
```bash
dotnet ef migrations remove \
  --project backend/src/BullMarket.Infrastructure \
  --startup-project backend/src/BullMarket.Api
```

### 4. Generate an Idempotent SQL Script (Production Deployments)
Generates an idempotent SQL script that can be executed against any environment (e.g., CI/CD pipeline):
```bash
dotnet ef migrations script \
  --idempotent \
  --project backend/src/BullMarket.Infrastructure \
  --startup-project backend/src/BullMarket.Api \
  --output migrations.sql
```

---

## Automatic Migrations on Startup

In development environments, you can enable automatic migrations on application startup in `appsettings.json`:

```json
{
  "Database": {
    "ApplyMigrationsOnStartup": true
  }
}
```

When set to `true`, `Program.cs` automatically executes:
1. `dbContext.Database.MigrateAsync()`
2. `DevelopmentDataSeeder.SeedAsync()` to populate initial categories, products, inventory, and demo customer data.

---

## Database Connection & Local Podman Setup

Start PostgreSQL via Podman Compose:

```bash
podman-compose up -d
# or using built-in compose in Podman CLI
podman compose up -d
```

Configure the database connection string in user secrets (recommended for local development) or `appsettings.json`:

```bash
dotnet user-secrets set "ConnectionStrings:BullMarketAiStore" \
  "Host=localhost;Port=5432;Database=bullmarket;Username=bullmarket;Password=BullMarket_Local_2026!" \
  --project backend/src/BullMarket.Api
```
