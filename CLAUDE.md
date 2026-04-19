# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run API
dotnet run --project ZeroBudget.Api

# Run tests
dotnet test ZeroBudget.UnitTests

# Run a single test class
dotnet test ZeroBudget.UnitTests --filter "FullyQualifiedName~LoginCommandHandlerTests"

# Add EF Core migration
dotnet ef migrations add <MigrationName> --project ZeroBudget.Infrastructure --startup-project ZeroBudget.Api

# Start full stack (API + PostgreSQL)
docker-compose up
```

## Architecture

ASP.NET Core 8 REST API for zero-based budgeting, following **Clean Architecture + CQRS**:

- **ZeroBudget.Domain** — Entities, value objects, domain exceptions. No external dependencies.
- **ZeroBudget.Application** — Use cases as MediatR commands/queries, DTOs, repository interfaces, FluentValidation validators. `ValidationBehavior` auto-validates all MediatR requests.
- **ZeroBudget.Infrastructure** — EF Core + PostgreSQL (Npgsql), repository implementations, JWT token service, BCrypt password hasher.
- **ZeroBudget.Api** — Controllers, `ExceptionHandlingMiddleware` (maps domain exceptions to HTTP status codes), CORS configuration.

### Key patterns

- **CQRS via MediatR**: Every operation is a `Command` or `Query` in `Application/Features/<Domain>/`. Controllers call `_mediator.Send(...)`.
- **Repository + Unit of Work**: `IUnitOfWork.SaveChangesAsync()` must be called after mutations. Repositories are injected into handlers.
- **ApiControllerBase**: All controllers inherit from it; exposes `CurrentUserId` (parsed from JWT claim) and applies `[Authorize]`.
- **Domain exceptions**: Throw from domain/application layers (`NotFoundException`, `UnauthorizedException`, `BudgetMonthLockedException`, `InsufficientFundsException`). The middleware translates them to appropriate HTTP responses.
- **Single-user system**: Only one user registration is allowed (`RegistrationClosedException`).

### Domain concepts

- **YearMonth**: Value object representing a budget month (Year + Month).
- **BudgetEntry**: The assigned amount for a `Category` in a given `YearMonth`.
- **Transaction**: Income (positive, no category) or expense (negative, with category).
- **LockPastMonths** (`BudgetOptions`): When `true`, mutations to past budget months are rejected.
- **Rollover**: Unspent category balance carries forward automatically.

## Configuration

| Setting | Location |
|---|---|
| DB connection string | `appsettings.json` → `ConnectionStrings:Default` |
| JWT secret/issuer/audience/expiry | `appsettings.json` → `Jwt` |
| Lock past months | `appsettings.json` → `BudgetOptions:LockPastMonths` |
| CORS allowed origins | `appsettings.Development.json` → `CorsAllowedDomains` (semicolon-separated) |

Development uses Docker Compose with PostgreSQL on host port `5434`.

## Frontend client

There is a companion Next.js 14 frontend (Pages Router). Key integration points relevant to backend changes:

- **Swagger codegen**: The frontend generates its typed API client from the backend's Swagger spec via `yarn codegen`. Regenerate after any API contract change (new endpoints, renamed DTOs, changed response shapes).
- **JWT**: The frontend stores the JWT in `localStorage` and injects it via an Axios interceptor. A 401 response clears storage and redirects to `/login`.
- **Month format**: Frontend routes use `YYYY-MM` (e.g., `/budget/2026-04`), which maps to the `YearMonth` domain value object.
- **CORS**: Frontend dev origin must be listed in `appsettings.Development.json` → `CorsAllowedDomains`.
