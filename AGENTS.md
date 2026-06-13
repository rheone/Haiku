## AGENTS.md

### Repo: Haiku

Blazor Server (.NET 10) social platform for haiku poetry — EF Core, SQL Server, Docker.

### Quick start

```bash
cp .env.example .env
docker compose up -d      # starts SQL Server + web app on http://localhost:5000
dotnet build              # or: dotnet build Haiku.slnx
dotnet test               # xUnit v3 + NSubstitute, MTP runner
dotnet run --project src/Haiku.Web    # local dev (needs SQL Server on localhost:1433, DB: Haiku_Dev)
```

### Solution structure (`Haiku.slnx` — modern `.slnx` format)

| Project | Description |
|---|---|
| `Haiku.Domain` | Entities (DataAnnotations for EF Core), enums, repository interfaces, value objects |
| `Haiku.Infrastructure` | EF Core `HaikuDbContext`, repository implementations, email senders |
| `Haiku.Services` | Business logic + 11 CQRS Slices, `PoemEngine`, `SyllableEngine`, `DictionaryService` |
| `Haiku.Web` | Blazor Server UI — `Program.cs` wires DI, cookie auth, EF Core |
| `MicroMediator` | Custom lightweight CQRS mediator (the only packable project) |
| `Haiku.Tests` | xUnit + NSubstitute (32 tests — auth, syllable, email, slices) |
| `Haiku.Services.Tests` | Slice handler tests (47 tests) |
| `MicroMediator.Tests` | Mediator unit tests (5 tests) |
| 3 other test projects | Empty scaffolding (Domain, Infrastructure, Web) |

84 total passing tests. All packages locked via `packages.lock.json`.

### Testing quirks

- **xUnit v3** — tests use `TestContext.Current.CancellationToken` for cancellation tokens.
- **MTP runner** — filters use MTP syntax: `--filter-class AuthServiceTests`, `--filter-trait "Category=Unit"`.
- `dotnet test tests/Haiku.Tests` to run a single project.
- `AuthService` tests use `BCrypt.Net.BCrypt.HashPassword` with work factor 12 (~300ms per hash).
- No integration tests — all unit tests with NSubstitute mocks.
- CMU pronunciation dictionary (`dictionary.dic`) loaded relative to working directory — run tests from solution root.

### Architecture notes

- **EF Core**: Scoped `HaikuDbContext` per request via `AddDbContext`. Fluent API in `OnModelCreating` for relationships and indexes; DataAnnotations on entities for table/column/key mapping. No `virtual` on entity properties.
- **Slice/CQRS**: 11 feature slices under `src/Haiku.Services/Slices/` (Auth, Poems, Votes, Loves, Bookmarks, Tags, PoetProfile, Dictionary, Moderation, Email, WordSearch). Each slice has Command/Query + Handler pairs dispatched via `MicroMediator`.
- **DI wiring** in `Program.cs`: repositories → services → Blazor components. `SyllableEngine` and `PoemEngine` are **singletons** (hold CMU dictionary in memory); all other services are **scoped**.
- **Auth cookie**: named `haiku-auth`, 7-day sliding expiry. Dev mode relaxes Secure/SameSite for HTTP.
- **Email**: `IEmailSender` → `SmtpEmailSender` (prod) / `ConsoleEmailSender` (dev). Configured via `Email:Sender:Provider` setting.
- **Dev config** (`appsettings.Development.json`): `Haiku_Dev` DB, `RequireVerification = false`, `Console` email.
- **Two syllable engines**: `PoemEngine` (full CMU dict, rhyme, generation) and `SyllableEngine` (simpler, constructor-injected dictionaries, used in tests).
- **Poem type detection**: `PoemType` enum with 15 types. `DetectPoemType()` in `HaikuService`. Matcher chain in `PoemMatcherChain` with priority-order matchers.
- **InternalsVisibleTo**: `Haiku.Services` exposes internals to `Haiku.Services.Tests` and `Haiku.Web`.

### Docker

- **Root `docker-compose.yml`** — dev stack, uses `.env` for config.
- **`deploy/docker-compose.yml`** — production stack, hardcoded values, build context is `..` (one level up).
- SQL Server auto-initialized via `deploy/db/init-db.sql` + `seed-data.sql` mounted to `/docker-entrypoint-initdb.d/`.
- Web container health-check: waits for `haiku-db` to be healthy.
- `BlazorDisableThrowNavigationException=true` in `Haiku.Web.csproj` — do not remove.

### Code conventions

- **Formatting**: CSharpier (`.csharpierrc.json`: 128 col, 4-space tabs) + StyleCop + Roslynator. Enforced via pre-commit (`dotnet husky run`). Run manually: `dotnet format style && dotnet format analyzers && dotnet csharpier format .`
- **Line endings**: CRLF for `.cs` files per `.editorconfig`.
- **Build flags**: `Nullable=enable`, `ImplicitUsings=enable`, `Deterministic=true`, SourceLink with GitHub.
- **Env vars**: `NUGET_XMLDOC_MODE=skip` set in devcontainer (silences XML doc warnings).

### Deployment

- Dockerfile runs `dotnet test` before `dotnet publish` (tests gate the build).
- No GitHub Actions CI found.
- `.slnx` format requires .NET 8+ SDK.
- DB port defaults to 1433, web port to 5000 — override via `.env` (`DB_PORT`, `WEB_PORT`).
