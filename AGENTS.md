## AGENTS.md

### Repo: Haiku

Blazor Web App (.NET 10, Interactive Server render mode) social platform for haiku poetry — EF Core 10, SQL Server 2022, Docker. See `prd/prd.md` and `prd/prd.UiStylingAddendum.md` for the full spec (consult before adding features that affect UX or domain rules).

### Quick start

```bash
cp .env.example .env
docker compose up -d          # SQL Server + web app on :5000
dotnet build Haiku.slnx
dotnet test                    # xUnit v3 + NSubstitute, MTP runner (106 tests)
dotnet run --project src/Haiku.Web    # local dev, needs SQL on :1433, DB: Haiku_Dev
```

### Key architecture

- **Blazor Web App**, not legacy Blazor Server — `Program.cs` uses `AddRazorComponents().AddInteractiveServerComponents()` / `MapRazorComponents<App>().AddInteractiveServerRenderMode()`. Cookie auth (`haiku-auth`, 7-day sliding).
- **Slice/CQRS**: 11 feature slices under `src/Haiku.Services/Slices/` dispatched via `MicroMediator` (custom lightweight mediator in `src/MicroMediator/`, the only packable project). Handlers + FluentValidation validators registered via `builder.Services.AddApplication()`.
- **`TargetFramework`** is set in `src/Directory.Build.props` and `tests/Directory.Build.props` (both `<TargetFramework>net10.0</TargetFramework>`), **not** in individual `.csproj` files — `grep` for TFM there will miss it.
- **`Directory.Build.targets`** auto-installs [Husky](https://github.com/alexaigor/husk) pre-commit hooks on restore/build. Disable in CI with env var `HUSKY: 0`.
- **Singleton** engines: `SyllableEngine` and `PoemEngine` hold the CMU pronunciation dictionary (`dictionary.dic`, loaded relative to working directory). All other services are **scoped**.
- **Poem type detection**: `PoemType` enum (15 types), `DetectPoemType()` in `HaikuService`, matcher chain `PoemMatcherChain` with priority-order matchers.

### Testing

- **xUnit v3 + Microsoft.Testing.Platform (MTP)** runner. Filter syntax: `--filter-class AuthServiceTests`, `--filter-trait "Category=Unit"`.
- 3 test projects with actual tests (106 total): `Haiku.Tests` (54), `Haiku.Services.Tests` (47), `MicroMediator.Tests` (5). 3 remaining test projects are empty scaffolding.
- Run tests from solution root (`dictionary.dic` is resolved relative to CWD).
- `AuthService` tests call `BCrypt.Net.BCrypt.HashPassword` with work factor 12 (~300ms/hash).
- Cancellation tokens from `TestContext.Current.CancellationToken`.

### Debug impersonation

```bash
dotnet run --project src/Haiku.Web --launch-profile Debug   # :5001, auto-login as debug_user
```

Active when `ASPNETCORE_ENVIRONMENT=Debug` — `DebugImpersonationMiddleware` auto-signs unauthenticated requests. Configured in `appsettings.Debug.json`.

### CI

Two GitHub Actions workflows under `.github/workflows/`:
- **`build.yml`** — on push/PR to `main`: restore → build → `dotnet format --verify-no-changes` → test. Sets `HUSKY: 0`.
- **`publish.yml`** — on `v*` tag: build + test + push container image to `ghcr.io`.

Docker build (`Dockerfile`) gates publish on `dotnet test` passing.

### Code conventions

- CSharpier (`.csharpierrc.json`: 128 col, 4-space tabs) + StyleCop + Roslynator. Enforced via pre-commit. Run: `dotnet format style && dotnet format analyzers && dotnet csharpier format .`
- `.editorconfig`: CRLF line endings for `.cs` files, braces on new lines, `var` preferred, `_camelCase` private fields.
- `Nullable=enable`, `ImplicitUsings=enable`, `Deterministic=true`. `NUGET_XMLDOC_MODE=skip` in devcontainer.
- `BlazorDisableThrowNavigationException=true` in `Haiku.Web.csproj` — do not remove.
- `InternalsVisibleTo`: `Haiku.Services` → `Haiku.Services.Tests`, `Haiku.Web`.
