## AGENTS.md

### Repo: Haiku

Blazor Server (.NET 10) social platform for haiku poetry — FluentNHibernate, SQL Server, Docker.

### Quick start

```bash
cp .env.example .env
docker compose up -d      # starts SQL Server + web app on http://localhost:5000
dotnet build              # or: dotnet build Haiku.slnx
dotnet test               # xUnit + NSubstitute, VSTest runner, no MTP
dotnet run --project src/Haiku.Web    # local dev (needs SQL Server on localhost:1433, DB: Haiku_Dev)
```

### Solution structure (`Haiku.slnx` — modern `.slnx` format)

| Project | Description |
|---|---|
| `Haiku.Domain` | Entities (virtual props for NHibernate lazy loading), enums, repository interfaces, value objects |
| `Haiku.Infrastructure` | FluentNHibernate mappings + session factory, repository implementations, email senders |
| `Haiku.Services` | Business logic: `AuthService`, `HaikuService`, `PoemEngine`, `SyllableEngine`, `DictionaryService`, `ModerationService` |
| `Haiku.Web` | Blazor Server UI — `Program.cs` wires DI, cookie auth, NHibernate |
| `Haiku.Tests` | xUnit + NSubstitute (19 tests across auth, syllable, email) |

### Architecture notes

- **NHibernate session**: Singleton `NHibernateSessionFactory` → Scoped `ScopedSession`. Mappings auto-discovered from `NHibernateSessionFactory` assembly.
- **DI wiring** in `Program.cs`: repositories → services → Blazor components. `SyllableEngine` is singleton; all services are scoped.
- **Auth cookie**: named `haiku-auth`, 7-day sliding expiry. Dev mode relaxes Secure/SameSite for HTTP.
- **Email**: `IEmailSender` interface → `SmtpEmailSender` (prod) / `ConsoleEmailSender` (dev). `FakeEmailSender` used in tests.
- **Dev config** (`appsettings.Development.json`): uses `Haiku_Dev` DB on localhost, `RequireVerification = false`, `Console` email provider.
- **Two syllable engines**: `PoemEngine` (used by `HaikuService`, full CMU dict loading, rhyme detection, generation) and `SyllableEngine` (simpler, constructor-injected dictionaries, used directly in tests).
- **Poem type detection**: `PoemType` enum with 13 types (Haiku, Tanka, Monoku, Sonnet, Limerick, etc.). `DetectPoemType()` in `HaikuService` is static and instance overloads exist.

### Testing quirks

- xUnit v2 (`[Fact]` only, no `[Theory]` used yet), VSTest runner (no MTP).
- Tests reference `Haiku.Domain` + `Haiku.Services` directly.
- `AuthService` tests use `BCrypt.Net.BCrypt.HashPassword` with work factor 12.
- No integration tests — all unit tests with NSubstitute mocks.

### Code conventions

- **Formatting**: CSharpier (`.editorconfig`: LF, 4-space indent for C#/shell, 2-space for JSON/YAML).
- **Entity properties**: all `virtual` (NHibernate requirement).
- **Build flags**: `Nullable=enable`, `ImplicitUsings=enable`, `BlazorDisableThrowNavigationException=true` in Web.csproj.
- **Env vars**: `NUGET_XMLDOC_MODE=skip` set in devcontainer (silences XML doc warnings).

### Deployment

- `deploy/docker-compose.yml` — production-style stack (SQL Server + web). Build context one level up.
- `deploy/db/init-db.sql` + `seed-data.sql` — auto-init on container start (mounted to `/docker-entrypoint-initdb.d/`).
- Dockerfile runs `dotnet test` before `dotnet publish` (tests gate the build).
- No GitHub Actions CI found.

### Gotchas

- `docker compose up` from root uses the root `docker-compose.yml` (dev); `deploy/docker-compose.yml` is for production.
- `.slnx` format requires .NET 8+ SDK (the root `Haiku.slnx` is not a legacy `.sln`).
- DB port defaults to 1433, web port defaults to 5000 — override via `.env` (`DB_PORT`, `WEB_PORT`).
- `BlazorDisableThrowNavigationException=true` suppresses Blazor navigation exceptions — do not remove.
