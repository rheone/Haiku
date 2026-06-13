# Haiku

A web-based social platform for composing, sharing, and discovering messages in haiku and related poetry forms. Built with [Blazor Server](https://learn.microsoft.com/aspnet/core/blazor/?view=aspnetcore-10.0) (.NET 10) and [SQL Server 2022](https://www.microsoft.com/sql-server).

See the [Product Requirements Document](./prd.md) for full specification.

---

## Quick Start

```bash
cp .env.example .env
docker compose up -d
```

The app is available at `http://localhost:5000`. The SQL Server database is auto-initialized with the schema and seed data.

## Environment Variables

| Variable | Default | Description |
|---|---|---|
| `SA_PASSWORD` | `Haiku@Dev2024!` (_default_) | SQL Server SA password (min 8 chars with uppercase, lowercase, and digit/special). Required. |
| `DB_PORT` | `1433` | Host port for SQL Server |
| `WEB_PORT` | `5000` | Host port for the web application |
| `ASPNETCORE_ENVIRONMENT` | `Development` | .NET environment (`Debug`, `Dev`, `Qual`, `Prod`) |
| `BUILD_CONFIG` | `Release` | .NET build configuration (`Debug`, `Release`) |

Copy `.env.example` to `.env` and adjust as needed. The `.env` file is git-ignored.

## Build

```bash
dotnet build
```

### Platform-specific notes

| Platform | Note |
|---|---|
| **Windows** | Use `dotnet build` from any terminal (cmd, PowerShell, Developer Command Prompt). |
| **Linux** | Install the .NET 10 SDK via [official packages](https://learn.microsoft.com/dotnet/core/install/linux). Ensure `System.Drawing` dependencies (libgdiplus) are installed if running tests that involve image processing. |

---

## Test

The solution contains 6 test projects using **xUnit v3** and **NSubstitute**. Tests run before every Docker build (gating the publish step).

```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test tests/Haiku.Tests

# Run tests with verbose output
dotnet test --verbosity detailed

# Run a specific test class or method (MTP syntax)
dotnet test --filter-class AuthServiceTests

# Run tests matching a trait
dotnet test --filter-trait "Category=Unit"

# Run tests in parallel across multiple projects
dotnet test --parallel
```

### Platform-specific test notes

| Platform | Note |
|---|---|
| **Windows** | Tests run natively with no extra setup. Use `dotnet test` from any terminal or run via Visual Studio Test Explorer. |
| **Linux** | Ensure the `System.Security.Cryptography` native libraries are available (pre-installed on most distros). The CMU pronunciation dictionary (`dictionary.dic`) is loaded relative to the working directory — run tests from the solution root. |

---

## Run (local, without Docker)

Requires SQL Server on `localhost:1433` with the `Haiku_Dev` database created and schema applied.

### 1. Start SQL Server

Choose one option:

<details>
<summary><b>Option A: SQL Server via Docker (recommended for Linux & macOS)</b></summary>

```bash
# Start only the database container
docker compose up -d haiku-db

# Wait for it to be healthy (may take 30-60s on first run)
docker compose logs haiku-db --tail 5
```
</details>

<details>
<summary><b>Option B: SQL Server on Windows (native install)</b></summary>

1. Install [SQL Server 2022 Developer Edition](https://www.microsoft.com/sql-server/sql-server-downloads)
2. Enable TCP/IP in SQL Server Configuration Manager
3. Create the database:

```sql
CREATE DATABASE [Haiku_Dev];
```

4. Run the schema and seed scripts:

```bash
sqlcmd -S localhost -U sa -P "Haiku@Dev2024!" -d Haiku_Dev -i deploy/db/init-db.sql
sqlcmd -S localhost -U sa -P "Haiku@Dev2024!" -d Haiku_Dev -i deploy/db/seed-data.sql
```
</details>

<details>
<summary><b>Option C: SQL Server on Linux (native install)</b></summary>

```bash
# Install SQL Server (Ubuntu/Debian)
curl -s https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
curl -s https://packages.microsoft.com/config/ubuntu/22.04/mssql-server-2022.list | sudo tee /etc/apt/sources.list.d/mssql-server-2022.list
sudo apt-get update && sudo apt-get install -y mssql-server
sudo /opt/mssql/bin/mssql-conf setup

# Create database
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C -Q "CREATE DATABASE [Haiku_Dev]"

# Run schema and seed
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C -d Haiku_Dev -i deploy/db/init-db.sql
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C -d Haiku_Dev -i deploy/db/seed-data.sql
```
</details>

### 2. Run the web app

```bash
dotnet run --project src/Haiku.Web
```

The app uses `appsettings.Development.json` (connects to `localhost:1433`, database `Haiku_Dev`) and is available at the URL shown in the console output (typically `http://localhost:5000`).

---

## Run (containerized, Docker)

### Development stack (root `docker-compose.yml`)

Uses `.env` for configuration. Starts both SQL Server and the web app.

```bash
cp .env.example .env        # one-time setup
docker compose up -d        # start both services
docker compose logs -f      # follow logs until startup completes
```

The app is available at `http://localhost:5000`. SQL Server is auto-initialized with schema and seed data.

### Production stack (`deploy/docker-compose.yml`)

Uses hardcoded values suitable for deployment. The build context is one level up.

```bash
docker compose -f deploy/docker-compose.yml up -d --build
```

### Container lifecycle

```bash
# Rebuild and restart after code changes
docker compose up -d --build

# Stop without losing data
docker compose down

# Full teardown (destroys volumes, including database data)
docker compose down -v
```

## Deployment

### Deploy the Docker stack

#### Linux (Docker Engine)

```bash
# Install Docker Engine on Ubuntu/Debian
curl -fsSL https://get.docker.com | sh
sudo usermod -aG docker $USER

# Clone and deploy
git clone <repo-url> haiku
cd haiku
cp .env.example .env
# edit .env with production values
docker compose -f deploy/docker-compose.yml up -d --build
```

#### Windows (Docker Desktop)

```powershell
# Install Docker Desktop from https://www.docker.com/products/docker-desktop/
git clone <repo-url> haiku
cd haiku
copy .env.example .env
# edit .env with production values
docker compose -f deploy/docker-compose.yml up -d --build
```

#### Production considerations

| Concern | Recommendation |
|---|---|
| **Reverse proxy** | Place behind nginx, Caddy, or Traefik for TLS termination and domain routing |
| **TLS** | Use Let's Encrypt via the reverse proxy |
| **Connection string** | Use a strong `SA_PASSWORD` and consider Azure Key Vault or Docker secrets |
| **Data persistence** | SQL Server data lives in a Docker volume (`haiku-db-data`); back up regularly |
| **Health checks** | The web container waits for the DB health check before starting |
| **Logging** | Use `docker compose logs` or a log aggregator (Loki, ELK, Datadog) |

### Deploy as a standalone .NET application

Build and publish, then run behind a reverse proxy:

```bash
dotnet publish src/Haiku.Web -c Release -o publish
# Copy publish/ to target server, then:
ASPNETCORE_URLS=http://0.0.0.0:5000 \
ASPNETCORE_ENVIRONMENT=Production \
ConnectionStrings__DefaultConnection="Server=...;Database=Haiku;..." \
dotnet publish/Haiku.Web.dll
```

### Platform-specific deployment notes

| Platform | Notes |
|---|---|
| **Windows Server** | Install the [.NET 10 Hosting Bundle](https://learn.microsoft.com/aspnet/core/host-and-deploy/iis) and deploy to IIS, or run as a Windows Service. |
| **Linux** | Deploy as a systemd service. Use nginx or Caddy as a reverse proxy. See [Microsoft docs](https://learn.microsoft.com/aspnet/core/host-and-deploy/linux-nginx). |

---

## Troubleshooting

| Symptom | Likely cause | Solution |
|---|---|---|
| SQL Server won't start | SA_PASSWORD doesn't meet complexity requirements | Use a password with uppercase, lowercase, and a digit or special character |
| Web app can't connect to SQL Server | SQL Server still initializing | Wait 30-60s for SQL Server to be ready; check `docker compose logs haiku-db` |
| Port already in use | `DB_PORT` or `WEB_PORT` conflicts | Change the port in `.env` |
| `docker compose` not found | Docker not installed | Install [Docker Desktop](https://www.docker.com/products/docker-desktop/) or [Docker Engine](https://docs.docker.com/engine/install/) |
| `docker compose ps` shows no containers | Docker Engine not running | Start Docker Desktop / Docker Engine and wait for it to be ready |
| EF Core DbContext connection issue | Connection string mismatch or SQL Server not ready | Verify SQL Server is running and connection string in `appsettings.*.json` is correct |
| Build fails with MSB3374 | File permission issue on `obj/` files | Run `sudo rm -rf src/*/obj tests/*/obj` (Linux) or clean the solution in Visual Studio (Windows) |

## Poem Types

The platform detects and supports 15 poem types based on syllable patterns, plus Freeform for unconstrained poetry.
Types are matched in priority order — the first matching pattern wins.

| Priority | Type | Lines | Syllable pattern | Description |
|---|---|---|---|---|
| 1 | **Monoku** | 1 | 4–17 total | Single-line poem. Total syllables must be between 4 and 17 inclusive. |
| 2 | **Haiku** | 3 | 5-7-5 | Traditional Japanese form, three lines of 5, 7, and 5 syllables. |
| 3 | **Katauta** | 3 | 5-7-7 | Classical Japanese form, three lines of 5, 7, and 7 syllables. |
| 4 | **American Lune** | 3 | 3-5-3 | Modern American adaptation of haiku. Formerly called Minimalist. |
| 5 | **Kelly Lune** | 3 | 5-3-5 | Created by Robert Kelly. Three lines of 5, 3, and 5 syllables. |
| 6 | **Compressed** | 3 | 2-3-2 | Nonstandard haiku-inspired ultra-short form. |
| 7 | **Near-Traditional** | 3 | 4-6-4 | Nonstandard approximation of haiku. |
| 8 | **Tanka** | 5 | 5-7-5-7-7 | Extended Japanese form, 31 syllables across 5 lines. |
| 9 | **American Cinquain** | 5 | 2-4-6-8-2 | Invented by Adelaide Crapsey. Five lines ascending then descending. |
| 10 | **Reverse Cinquain** | 5 | 2-8-6-4-2 | Reverse of the American cinquain. |
| 11 | **Sedoka** | 6 | 5-7-7-5-7-7 | Equivalent to two joined katauta. |
| 12 | **Butterfly Cinquain** | 9 | 2-4-6-8-2-8-6-4-2 | American cinquain merged with its reverse, center line dropped. |
| 13 | **Mirror Cinquain** | 10 | 2-4-6-8-2-2-8-6-4-2 | American cinquain concatenated with a Reverse cinquain. |
| 14 | **Choka** | odd ≥ 7 | (5-7)ⁿ + 5-7-7 | Long poem alternating 5 and 7 syllables, ending with 5-7-7. Minimum valid pattern: 5-7-5-7-7 (5 lines, detected as Tanka). |
| 15 | **Isosyllabic** | ≥ 2 | n-n-n... | All lines have the same syllable count. n is any positive integer. |
| — | **Freeform** | any | any | No formal constraints. Must be explicitly chosen by the poet. |

Auto-detection checks each type in priority order (1 → 15). If no pattern matches and Freeform was not explicitly selected, the poem defaults to Freeform with a notification.

---

## Input Validation

All poems are automatically cleaned and validated before storage. The system returns all errors at once so you can fix everything in one pass.

### Auto-cleanup (silent fixes)

These issues are corrected automatically — you won't see an error:

| Issue | How it's fixed |
|---|---|
| Carriage returns (`\r`) | Converted to newlines (`\n`) |
| Trailing whitespace | Right-trimmed |
| Consecutive control/whitespace chars | Any run containing `\n` → single `\n`; otherwise → single space |
| Zero-width characters (non-emoji) | Stripped |
| Empty lines after trimming | Removed |
| Lines with only whitespace | Removed |
| Unicode non-NFC forms | Normalized to NFC |

### Validation rules (errors you must fix)

| Rule | Error message |
|---|---|
| Poem is empty after cleanup | "Poem is empty" |
| Exceeds 200 characters (NFC-normalized) | "Exceeds 200 character limit" |
| Contains no letters or digits | "Poem must contain at least one letter or number" |
| Contains emoji but no letter/digit | "Poem with emoji must also contain letters or numbers" |
| Fewer than 2 distinct words | "Poem must contain at least two distinct words" |
| Fewer than 4 syllables total | "Poem must have at least 4 syllables total" |
| Any line has 0 syllables | "Line {N} has no syllables" |
| Duplicate content (case-insensitive) | "A poem with identical content already exists" |

**Words**: A "word" is any whitespace-delimited token. Hashtags (e.g. `#haiku`) count as words (the `#` is stripped for comparison purposes).

### Input guidance

- Write one line per line (use `Enter` / `\n` to separate)
- The syllable engine is authoritative — what it counts is what the platform uses
- Each line must have at least 1 syllable
- Each poem must have at least 2 distinct words
- Total syllables across all lines must be at least 4
- Maximum 200 characters (after normalization, before storage)
- No zero-width characters (except ZWJ used in emoji sequences)
- Content is case-insensitive for duplicate detection

## Slices

The project follows a feature-oriented **Slice/CQRS** architecture. Business logic is organized into slices — each slice contains commands, queries, handlers, and validators grouped by domain feature:

| Slice | Description |
|---|---|
| **Auth** | User registration and login via `RegisterUserCommand`/`LoginUserQuery`. Password hashing with BCrypt. |
| **Poems** | Create, delete, and detect poem type via `CreatePoemCommand`/`DeletePoemCommand`/`DetectPoemTypeQuery`. |
| **Votes** | Upvote/downvote poems via `CastVoteCommand`; query scores via `GetVoteScoreQuery`. |
| **Loves** | Love/unlove poems via `AddLoveCommand`/`RemoveLoveCommand`; query counts via `GetLoveCountQuery`. |
| **Bookmarks** | Bookmark/unbookmark poems via `AddBookmarkCommand`/`RemoveBookmarkCommand`. |
| **Tags** | Tag lookup and creation via `GetOrCreateTagCommand`. |
| **PoetProfile** | Retrieve poet profile data via `GetPoetProfileQuery`. |
| **Dictionary** | Manage the custom dictionary — `AddWordCommand`, `RemoveWordCommand`, `GetAllWordsQuery`, `SubmitSuggestionCommand`, `ApproveSuggestionCommand`, `RejectSuggestionCommand`. |
| **Moderation** | Admin actions — `HidePoemCommand`/`UnhidePoemCommand`, `DisableUserCommand`/`ReinstateUserCommand`, `HasPrivilegeQuery`. |
| **Email** | Send verification and password-reset emails via `SendVerificationEmailCommand`/`SendPasswordResetEmailCommand`. |
| **WordSearch** | Search poems by word via `SearchPoemsByWordQuery`. |

## Formatting

The project uses automatic formatting enforced by pre-commit hooks (via [.NET Husk](https://github.com/alexaigor/husk)) and CI. Three tools run on each commit:

```bash
# Style formatting (whitespace, brace placement, etc.)
dotnet format style

# Roslyn analyzer fixes
dotnet format analyzers

# CSharpier (opinionated code formatter)
dotnet csharpier format .
```

To check formatting without making changes:

```bash
dotnet format --verify-no-changes
```

Configuration files:

- [.editorconfig](./.editorconfig) — coding style and conventions
- [.csharpierrc.json](./.csharpierrc.json) — CSharpier options (128 col width, 4-space tabs)
- [stylecop.json](./stylecop.json) — StyleCop analyzer settings
- [.husky/task-runner.json](./.husky/task-runner.json) — pre-commit hook tasks

## Solution Overview

| Project | Description |
|---|---|
| `Haiku.Domain` | Entities (User, Poem, Vote, Love, Follow, Bookmark, Tag, etc.), enums, repository interfaces |
| `Haiku.Infrastructure` | EF Core `HaikuDbContext`, repository implementations, email senders |
| `Haiku.Services` | Business logic: AuthService, HaikuService, SyllableEngine, EmailService, ModerationService, DictionaryService |
| `Haiku.Web` | Blazor Server UI with Bootstrap 5, FontAwesome, 9 pages (Feed, PoetPage, TagPage, WordPage, Login, Register, Bookmarks, Loves, Admin) |
| `MicroMediator` | Lightweight CQRS mediator library (`ICommand<>`, `IQuery<>`, `ICommandHandler<>`, `IQueryHandler<>`, `IMediator`). This is the only project packaged as a NuGet distribution package. |
| `Haiku.Tests` | xUnit + NSubstitute |
| `MicroMediator.Tests` | xUnit |
| `Haiku.Services.Tests` | Services Test scaffolding |
| `Haiku.Domain.Tests` | Domain Test scaffolding|
| `Haiku.Infrastructure.Tests` | Infrastructure Test scaffolding|
| `Haiku.Web.Tests` | Web UI Test scaffolding|

## Technology Stack

- **UI:** [Blazor Server](https://learn.microsoft.com/aspnet/core/blazor/?view=aspnetcore-10.0) (Interactive Server), [Bootstrap 5](https://getbootstrap.com/), [FontAwesome](https://fontawesome.com/)
- **ORM:** [Entity Framework Core](https://learn.microsoft.com/ef/core/) 10.x
- **Database:** [SQL Server 2022](https://www.microsoft.com/sql-server)
- **Auth:** [ASP.NET Core Cookie Authentication](https://learn.microsoft.com/aspnet/core/security/authentication/cookie), [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net)
- **CQRS:** MicroMediator (custom lightweight mediator)
- **Validation:** [FluentValidation](https://docs.fluentvalidation.net/)
- **Testing:** [xUnit.net v3](https://xunit.net/), [NSubstitute](https://nsubstitute.github.io/), MTP runner
- **Style/Formatting:** [CSharpier](https://csharpier.com/), [StyleCop](https://github.com/DotNetAnalyzers/StyleCopAnalyzers), [EditorConfig](https://editorconfig.org/)
- **Git hooks:** [.NET Husk](https://github.com/alexaigor/husk)
- **Containerization:** [Docker Compose](https://docs.docker.com/compose/) (SQL Server + .NET web app)

## Project Structure

```
src/
├── Haiku.Domain/         # Entities, enums, interfaces, value objects
├── Haiku.Infrastructure/ # EF Core DbContext, repositories, email senders
├── Haiku.Services/       # Business logic services + Slices (CQRS)
│   └── Slices/           # Feature slices: Auth, Poems, Votes, Loves, Bookmarks,
│                         #   Tags, PoetProfile, Dictionary, Moderation, Email,
│                         #   WordSearch
├── MicroMediator/        # Lightweight CQRS mediator library (distributable)
└── Haiku.Web/            # Blazor Server app (Components, Pages, Layout)
tests/
├── Haiku.Tests/          # Unit tests (Slices/ subdirectories by feature)
├── MicroMediator.Tests/
├── Haiku.Services.Tests/ # Slice handler and service tests
├── Haiku.Domain.Tests/   # Entity and value object tests
├── Haiku.Infrastructure.Tests/ # Mapping, repository, and integration tests
└── Haiku.Web.Tests/      # Web UI scaffolding tests
deploy/
├── docker-compose.yml    # Production-style deployment
└── db/
    ├── init-db.sql       # Schema (15 tables, indexes, constraints)
    └── seed-data.sql     # Idempotent seed data (users, poems, tags, votes, loves)
_manifest/
└── manifest.spdx.json    # SPDX SBOM (Software Bill of Materials)
```

## Entity Framework Core

The project uses **Entity Framework Core 10.x** with **SQL Server 2022**. The `HaikuDbContext` (`src/Haiku.Infrastructure/HaikuDbContext.cs`) defines the data model via Fluent API in `OnModelCreating`.

### Connection string resolution

The connection string is resolved in `Program.cs` with this priority:

1. `ConnectionStrings:DefaultConnection` from `appsettings.*.json`
2. Fallback: `Server=localhost;Database=Haiku;Trusted_Connection=True;TrustServerCertificate=True;`

Three config files cover different environments:

| File | Environment | Database |
|------|-------------|----------|
| `appsettings.json` | Production (default) | `Haiku` |
| `appsettings.Development.json` | `Development` | `Haiku_Dev` |
| `appsettings.Debug.json` | `Debug` | `Haiku_Dev` |

### Entity configuration conventions

- **Table names**: Each entity uses `[Table]` data annotation or Fluent API `ToTable()` to define the table name.
- **Primary keys**: `Guid` primary keys; composite keys use Fluent API `HasKey()`.
- **Indexes**: Unique indexes on natural keys (Email, Username, Tag.Name) and on composite uniqueness constraints (PoemId + UserId, FollowerId + FolloweeId, etc.).
- **Foreign keys**: `[ForeignKey]` on navigation properties; `OnDelete` behavior explicitly configured:
  - `Cascade` — owned/child relationships (Poem → Vote/Love/Bookmark, Tag → HaikuTag/TagDailyCount, User → PasswordResetToken/UserVerificationToken)
  - `Restrict` — shared-entity references (User → Poem/Vote/Love/Bookmark/ModerationAction/DictionaryWord)
  - `SetNull` — optional relationships (CustomDictionarySuggestion.ReviewedBy)

### Query patterns

- All repository methods accept `CancellationToken` and call `ThrowIfCancellationRequested()` at entry.
- EF Core retry on failure is enabled (`EnableRetryOnFailure(3)`) to handle transient SQL Server errors.
- Navigation properties use `= null!` with nullable reference types enabled, relying on EF Core's `!` postfix to suppress compiler warnings.

### Adding a new entity

1. Create the entity class in `Haiku.Domain/Entities/` with DataAnnotations (`[Table]`, `[Key]`, `[ForeignKey]`, `[MaxLength]`, etc.)
2. Add a `DbSet<T>` property in `HaikuDbContext`
3. Configure relationships, indexes, and cascade behavior in `OnModelCreating` via Fluent API
4. Update the repository interface in `Haiku.Domain/Interfaces/` and implementation in `Haiku.Infrastructure/Repositories/`
5. Add a migration (run from solution root):
   ```bash
   dotnet ef migrations add MigrationName --project src/Haiku.Infrastructure --startup-project src/Haiku.Web
   ```

### DbContext lifetime

`HaikuDbContext` is registered as **scoped** (`AddDbContext<>`), one instance per HTTP request. Context pooling is not enabled (single-server deployment).

## Debugging and impersonation

When `ASPNETCORE_Environment=Debug` is set, the app loads `appsettings.Debug.json` and enables automatic user impersonation.

### Debug impersonation

In the `Debug` environment, the `DebugImpersonationMiddleware` automatically signs in a configurable debug user for all unauthenticated requests. Configure impersonation in `appsettings.Debug.json`:

```json
{
  "Impersonation": {
    "Enabled": true,
    "DefaultUserId": "00000000-0000-0000-0000-000000000001",
    "DefaultUsername": "debug_user",
    "DefaultEmail": "debug@haiku.local"
  }
}
```

Run with the `Debug` launch profile:

```bash
dotnet run --project src/Haiku.Web --launch-profile Debug
```

The app starts on `http://localhost:5001` and all authenticated pages are accessible immediately without login. To disable impersonation, switch to the `Development` or `Production` profile.

## Architecture

The project follows a **Slice/CQRS** pattern for feature organization. Service logic under `src/Haiku.Services/Slices/` is organized into feature-specific command/query handlers dispatched through the `MicroMediator` mediator. Tests under `tests/` mirror this structure.

The root `Dockerfile` gates the build on tests passing — `dotnet test` runs before `dotnet publish`.
