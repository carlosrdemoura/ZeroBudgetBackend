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

# Reset dev database (drop + recreate via startup migration)
dotnet ef database drop --project ZeroBudget.Infrastructure --startup-project ZeroBudget.Api --force

# Start full stack (API + PostgreSQL), dev
docker-compose up

# Start full stack, prod (requires POSTGRES_PASSWORD, JWT_SECRET, AUTH_EMAIL, AUTH_PASSWORD in env / .env)
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## Architecture

ASP.NET Core 8 REST API for a simple **accounts payable / accounts receivable** ledger, following **Clean Architecture + CQRS**:

- **ZeroBudget.Domain** — `Transaction` entity, domain exceptions. No external dependencies.
- **ZeroBudget.Application** — Use cases as MediatR commands/queries, DTOs, repository interfaces, FluentValidation validators. `ValidationBehavior` auto-validates all MediatR requests.
- **ZeroBudget.Infrastructure** — EF Core + PostgreSQL (Npgsql), `TransactionRepository`, JWT token service, config-based `IAuthSettings`.
- **ZeroBudget.Api** — `TransactionsController`, `AuthController`, `ExceptionHandlingMiddleware` (maps domain exceptions to HTTP status codes), CORS configuration.

### Key patterns

- **CQRS via MediatR**: Every operation is a `Command` or `Query` in `Application/Features/<Domain>/`. Controllers call `_mediator.Send(...)`.
- **Repository + Unit of Work**: `IUnitOfWork.SaveChangesAsync()` must be called after mutations. Repositories are injected into handlers.
- **ApiControllerBase**: All controllers inherit from it; applies `[Authorize]`.
- **Domain exceptions**: `NotFoundException`, `UnauthorizedException`, `DomainException`. The middleware translates them to HTTP responses.
- **Credentials from config, not DB**: There is no user table. `LoginCommandHandler` compares `Auth:Email` / `Auth:Password` from configuration using `FixedTimeEquals`, then issues a JWT. There is no registration flow.

### Domain concepts

- **Transaction**: The only domain entity. A credit/debit line in the ledger.
  - `Amount` (decimal, signed): positive = receivable (income), negative = payable (expense).
  - `Date` (DateOnly): when the transaction is dated.
  - `Description` (string?): free-form text shown in the UI.
  - `IsConsolidated` (bool): whether the transaction has been actually paid/received. Pending = `false`, settled = `true`. The frontend toggles this inline via `PATCH /api/transactions/{id}/consolidate`.
- **No categories, no accounts, no monthly budget.** The list is filtered by month at query time (`GET /api/transactions?year=YYYY&month=MM`).
- **Negative totals are allowed.** The backend does not block transactions that would leave the projected balance negative — the frontend renders that state as informational only.

## Configuration

| Setting | Location |
|---|---|
| DB connection string | `appsettings.json` → `ConnectionStrings:Default` |
| JWT secret/issuer/audience/expiry | `appsettings.json` → `Jwt` |
| Login credentials | `appsettings.json` → `Auth:Email`, `Auth:Password` (required — startup fails without them) |
| CORS allowed origins | `appsettings.json` → `CorsAllowedDomains` (semicolon-separated hostnames, no scheme) |

Development runs PostgreSQL via Docker Compose on host port `5434`. Production (`docker-compose.prod.yml`) binds the API to `127.0.0.1:8080` and pulls secrets from env (`POSTGRES_PASSWORD`, `JWT_SECRET`, `AUTH_EMAIL`, `AUTH_PASSWORD`).

## Frontend client

Companion Next.js 14 frontend (Pages Router). Integration points relevant to backend changes:

- **Swagger codegen**: `yarn codegen` regenerates the typed client from the backend's Swagger spec. Run after any API contract change. (The current frontend was rewritten to use hand-typed wrappers in `src/lib/api/transactions.ts` after the refactor — `yarn codegen` will recreate `src/lib/api/generated/` if desired.)
- **JWT**: The frontend stores the JWT in `localStorage` and injects it via an Axios interceptor. A 401 response clears storage and redirects to `/login`.
- **Month format**: Frontend routes use `YYYY-MM` (e.g., `/transactions/2026-05`); the backend takes `year`/`month` as separate query params.
- **CORS**: Frontend dev origin must be listed in `appsettings.Development.json` → `CorsAllowedDomains`.
