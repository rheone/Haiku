# Haiku

A web-based social platform for composing, sharing, and discovering messages in haiku and related poetry forms. Built with Blazor Server (.NET 10) and SQL Server.

See the [Product Requirements Document](./prd.md) for full specification.

---

## Quick Start

```bash
cp .env.example .env
docker compose up -d
```

The app is available at `http://localhost:5000`. The SQL Server database is auto-initialized with the schema and seed data.

## Build

```bash
dotnet build
```

## Test

```bash
dotnet test
```

## Run (local, without Docker)

Requires SQL Server on `localhost:1433` with the `Haiku_Dev` database created.

```bash
dotnet run --project src/Haiku.Web
```

## Solution Overview

| Project | Description |
|---|---|
| `Haiku.Domain` | Entities (User, Poem, Vote, Love, Follow, Bookmark, Tag, etc.), enums, repository interfaces |
| `Haiku.Infrastructure` | FluentNHibernate mappings, NHibernate session factory, repository implementations, email senders |
| `Haiku.Services` | Business logic: AuthService, HaikuService, SyllableEngine, EmailService, ModerationService, DictionaryService |
| `Haiku.Web` | Blazor Server UI with Bootstrap 5, FontAwesome, 9 pages (Feed, PoetPage, TagPage, WordPage, Login, Register, Bookmarks, Loves, Admin) |
| `Haiku.Tests` | xUnit + NSubstitute tests (19 tests covering auth, syllable counting, poem type detection, email sending) |

## Technology Stack

- **UI:** Blazor Server (Interactive Server), Bootstrap 5, FontAwesome
- **ORM:** FluentNHibernate 3.x
- **Database:** SQL Server 2022 (T-SQL, NEWSEQUENTIALID PKs)
- **Auth:** ASP.NET Core Cookie Authentication, BCrypt (work factor 12)
- **Testing:** xUnit, NSubstitute
- **Containerization:** Docker Compose (SQL Server + .NET web app)

## Project Structure

```
src/
└── Haiku.Web/          # Blazor Server app
src/
├── Haiku.Domain/       # Entities, enums, interfaces
├── Haiku.Infrastructure/ # NHibernate, repositories, email
└── Haiku.Services/     # Business logic
tests/
└── Haiku.Tests/        # Unit tests
deploy/
├── docker-compose.yml  # Production-style deployment
└── db/
    ├── init-db.sql     # Schema (15 tables, indexes, constraints)
    └── seed-data.sql   # Idempotent seed data (users, poems, tags, votes, loves)
```
