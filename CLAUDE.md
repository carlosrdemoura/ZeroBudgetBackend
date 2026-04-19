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

# Start full stack (API + PostgreSQL), dev
docker-compose up

# Start full stack, prod (requires POSTGRES_PASSWORD, JWT_SECRET, AUTH_EMAIL, AUTH_PASSWORD in env / .env)
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## Architecture

ASP.NET Core 8 REST API for zero-based budgeting, following **Clean Architecture + CQRS**:

- **ZeroBudget.Domain** — Entities, value objects, domain exceptions, domain services (e.g. `BudgetCalculationService`). No external dependencies.
- **ZeroBudget.Application** — Use cases as MediatR commands/queries, DTOs, repository interfaces, FluentValidation validators. `ValidationBehavior` auto-validates all MediatR requests.
- **ZeroBudget.Infrastructure** — EF Core + PostgreSQL (Npgsql), repository implementations, JWT token service, config-based `IAuthSettings`.
- **ZeroBudget.Api** — Controllers, `ExceptionHandlingMiddleware` (maps domain exceptions to HTTP status codes), CORS configuration.

### Key patterns

- **CQRS via MediatR**: Every operation is a `Command` or `Query` in `Application/Features/<Domain>/`. Controllers call `_mediator.Send(...)`.
- **Repository + Unit of Work**: `IUnitOfWork.SaveChangesAsync()` must be called after mutations. Repositories are injected into handlers.
- **ApiControllerBase**: All controllers inherit from it; exposes `CurrentUserId` (parsed from JWT claim) and applies `[Authorize]`.
- **Domain exceptions**: Throw from domain/application layers (`NotFoundException`, `UnauthorizedException`, `AccountHasTransactionsException`, `InsufficientFundsException`). The middleware translates them to appropriate HTTP responses.
- **Credentials from config, not DB**: There is no user table. `LoginCommandHandler` compares `Auth:Email` / `Auth:Password` from configuration using `FixedTimeEquals`, then issues a JWT. There is no registration flow.

### Domain concepts

- **Account**: A source/destination of funds (e.g. checking, savings). Referenced by every `Transaction`. `Transaction.AccountId` is immutable after creation (update uses delete + recreate).
- **YearMonth**: Value object representing a budget month (Year + Month).
- **CategoryGroup / Category**: User-defined grouping of budget categories; reorderable.
- **BudgetEntry**: The assigned amount for a `Category` in a given `YearMonth`.
- **Transaction**: Income (positive) or expense (negative). Has an `AffectsBudget` flag — when `false`, the transaction does not count toward category balances (e.g. transfers).
- **Rollover**: Unspent category balance carries forward automatically (see `BudgetCalculationService`).
- **Overdraft is allowed**: Assigning/spending beyond available does not throw; negative balances are surfaced to the frontend as an overdraft state.

## Configuration

| Setting | Location |
|---|---|
| DB connection string | `appsettings.json` → `ConnectionStrings:Default` |
| JWT secret/issuer/audience/expiry | `appsettings.json` → `Jwt` |
| Login credentials | `appsettings.json` → `Auth:Email`, `Auth:Password` (required — startup fails without them) |
| CORS allowed origins | `appsettings.json` → `CorsAllowedDomains` (semicolon-separated hostnames, no scheme) |

Development runs PostgreSQL via Docker Compose on host port `5434`. Production (`docker-compose.prod.yml`) binds the API to `127.0.0.1:8080` and pulls secrets from env (`POSTGRES_PASSWORD`, `JWT_SECRET`, `AUTH_EMAIL`, `AUTH_PASSWORD`).

## Frontend client

There is a companion Next.js 14 frontend (Pages Router). Key integration points relevant to backend changes:

- **Swagger codegen**: The frontend generates its typed API client from the backend's Swagger spec via `yarn codegen`. Regenerate after any API contract change (new endpoints, renamed DTOs, changed response shapes).
- **JWT**: The frontend stores the JWT in `localStorage` and injects it via an Axios interceptor. A 401 response clears storage and redirects to `/login`.
- **Month format**: Frontend routes use `YYYY-MM` (e.g., `/budget/2026-04`), which maps to the `YearMonth` domain value object.
- **CORS**: Frontend dev origin must be listed in `appsettings.Development.json` → `CorsAllowedDomains`.
