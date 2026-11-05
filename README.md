# mir-portfolio-web

A personal developer portfolio built with **ASP.NET Core MVC (.NET 10)**, EF Core,
SQL Server LocalDB, and ASP.NET Core Identity. It includes a public portfolio site
and a protected admin area for managing projects, skills, experience, blog posts,
and contact messages.

## Documentation

- [Product Requirements](Docs/PRD.md)
- [Software Architecture](Docs/SOFTWARE-ARCHITECTURE.md)
- [Tech Stack](Docs/TECH-STACK.md)
- [Operations](Docs/OPERATIONS.md)
- [Testing](Docs/TESTING.md)
- [Security](Docs/SECURITY.md)
- [Deployment](Docs/DEPLOYMENT.md)
- [AI Workflow](Docs/AI-WORKFLOW.md)
- [Changelog](Docs/CHANGELOG.md)
- [Contributing](CONTRIBUTING.md)
- [AI agent notes](AGENTS.md)

## Tech stack

- .NET 10 / ASP.NET Core MVC (Razor views)
- Entity Framework Core 10 + SQL Server LocalDB
- ASP.NET Core Identity (admin-only access)
- Bootstrap 5 + vanilla JavaScript
- Markdown blog rendering with server-side sanitization. Inline script-like
  HTML is encoded as text rather than executed.
- Automated tests with xUnit and EF Core InMemory.

## Prerequisites

- .NET SDK 10.0.x
- SQL Server LocalDB (`MSSQLLocalDB`)
- `dotnet-ef` global tool (`dotnet tool install --global dotnet-ef`)

## Getting started

```powershell
# 1. Restore and build
dotnet build

# 2. Configure local secrets (connection string + seed admin)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=MirPortfolio;Trusted_Connection=True;MultipleActiveResultSets=true" --project src/MirPortfolio.Web
dotnet user-secrets set "SeedAdmin:Email" "admin@mirportfolio.local" --project src/MirPortfolio.Web
dotnet user-secrets set "SeedAdmin:Password" "<strong-password>" --project src/MirPortfolio.Web

# 3. Run (migrations + seed data are applied on startup in Development)
dotnet run --project src/MirPortfolio.Web
```

The site runs at `http://localhost:5110` with the `http` launch profile.

## Admin access

- Login: `/Identity/Account/Login`
- Admin area: `/admin` (dashboard), `/admin/Projects`, `/admin/Skills`, `/admin/Experiences`, `/admin/Posts`, `/admin/Messages`, `/admin/Settings`
- The seeded admin email/password come from the `SeedAdmin` user-secrets in
  Development, or equivalent environment variables in production.
- There is no public registration.
- The public contact form at `/contact` stores messages in the admin inbox. It includes a
  honeypot field and is rate limited to 5 submissions per 10 minutes per IP address.
- Resumes are uploaded in `/admin/Settings` (PDF, max 5 MB). The public link is `/resume`
  and is hidden until a file is configured.
- View configured secrets with:

```powershell
dotnet user-secrets list --project src/MirPortfolio.Web
```

## Project structure

```
Docs/                     PRD, architecture, deployment and ops docs
src/MirPortfolio.Web/     ASP.NET Core MVC application
  Areas/Admin/            Protected admin area (dashboard, content, settings)
  Controllers/            Public controllers
  Data/                   DbContext, migrations, seed data
  Models/                 Entities + Identity user
  Services/               Portfolio, content, resume, contact, and dashboard services
  ViewModels/             Public and admin view models
  Views/                  Public Razor views
  wwwroot/                Static assets
tests/                    xUnit tests (service-level)
mir-portfolio-web.slnx    Solution file
```

## Public routes

- `/`, `/Home/About`, `/Home/Privacy`
- `/projects`, `/projects/{slug}`
- `/blog`, `/blog/{slug}`
- `/contact`
- `/resume`
- `/robots.txt`, `/sitemap.xml`
- Friendly `404` and `500` pages (e.g. visit a missing slug).

## Status

- [x] Milestone 0 — solution scaffolded, EF Core + Identity configured
- [x] Milestone 1 — entities, DbContext, initial migration, seed data, admin user
- [x] Milestone 2 — public layout, light theme, Home, About, Privacy
- [x] Milestone 3 — public projects list/detail plus admin project and skill CRUD
- [x] Milestone 4 — experience CRUD, timeline, and resume upload/download
- [x] Milestone 5 — contact form, persistence, rate limiting, and admin inbox
- [x] Milestone 6 — blog
- [x] Milestone 7 — SEO, accessibility, friendly error pages, tests
- [x] Milestone 8 — deployment prep and publish profile
