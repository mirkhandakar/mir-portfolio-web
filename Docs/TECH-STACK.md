# Tech Stack — `mir-portfolio-web`

Companion to [PRD.md](PRD.md) and [SOFTWARE-ARCHITECTURE.md](SOFTWARE-ARCHITECTURE.md).

---

## 1. Summary

| Area | Technology | Version | Why |
| --- | --- | --- | --- |
| Language | C# | 14 | Modern C# bundled with .NET 10 |
| Runtime / SDK | .NET | 10.0 (LTS) | Current LTS; local SDK 10.0.401 |
| Web framework | ASP.NET Core MVC | 10.0 | Server-rendered, SEO-friendly, familiar |
| ORM | Entity Framework Core | 10.0 | Code-first, migrations, strong tooling |
| Database | SQL Server LocalDB | 2022+ | Zero-config on Windows; upgrade path to SQL Server |
| Auth | ASP.NET Core Identity | 10.0 | Cookie auth, hashing, roles/policies |
| Views | Razor + Bootstrap | Bootstrap 5.x | Responsive UI with minimal custom CSS |
| Client JS | Vanilla JS | ES2022 | Small UX enhancements only |
| Markdown | Markdig | latest | Blog post rendering |
| HTML sanitizing | HtmlSanitizer | latest | Sanitize rendered Markdown |
| Testing | xUnit | latest | Unit tests for services/validation |
| Tooling | .NET CLI, EF Core CLI | matching SDK | Scaffold, migrate, run |

> Pin exact package versions in `MirPortfolio.Web.csproj`; upgrade deliberately.
> Only add Markdig/HtmlSanitizer when Milestone 6 (Blog) begins.

---

## 2. Backend

- **C# / .NET 10** — target framework `net10.0`, nullable reference types enabled.
- **ASP.NET Core MVC** — controllers + Razor views, areas for admin.
- **Dependency injection** — built-in container; services registered in `Program.cs`.
- **Middleware** — HTTPS redirection, HSTS, exception handling, static files,
  routing, authentication, authorization, rate limiting.
- **Configuration** — `appsettings.json` / `appsettings.Development.json` plus
  `dotnet user-secrets` for connection string and seed admin credentials.
- **Logging** — `ILogger<T>` (console in dev; provider-agnostic for hosting).

### NuGet packages (planned / added)

| Package | Milestone | Purpose |
| --- | --- | --- |
| `Microsoft.EntityFrameworkCore.SqlServer` | M1 | SQL Server provider |
| `Microsoft.EntityFrameworkCore.Design` | M1 | Migrations/tooling |
| `Microsoft.EntityFrameworkCore.Tools` | M1 | `dotnet ef` usage |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | M1 | Identity stores |
| `Microsoft.AspNetCore.Identity.UI` | M1 | Login/logout default UI |
| `Markdig` | M6 | Markdown → HTML |
| `HtmlSanitizer` | M6 | Sanitize rendered HTML |
| `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk` | M7 | Tests |

---

## 3. Data

- **SQL Server LocalDB** in development (`(localdb)\MSSQLLocalDB`).
- **EF Core code-first** with checked-in migrations.
- **Identity tables** + application tables in the same database via
  `IdentityDbContext<ApplicationUser>`.
- Production can point the same provider at SQL Server; PostgreSQL is a possible
  later swap (provider + connection string only).

Suggested local connection string (in user-secrets, not source):

```
Server=(localdb)\MSSQLLocalDB;Database=MirPortfolio;Trusted_Connection=True;MultipleActiveResultSets=true
```

---

## 4. Frontend

- **Razor views** with a shared layout, partials, and tag helpers.
- **Bootstrap 5** for grid, components, and responsive utilities.
- **Custom CSS** for a clean, light, professional theme.
- **Vanilla JavaScript** for the mobile nav and small interactions; no SPA framework.
- **Accessibility** — semantic HTML, labels, visible focus, AA contrast targets.
- **SEO** — per-page metadata, Open Graph tags, `sitemap.xml`, `robots.txt`.

---

## 5. Development Environment

Prerequisites:

- Windows with **.NET SDK 10.0.401** (or compatible .NET 10 SDK)
- **SQL Server LocalDB** (installed with Visual Studio / SQL Server Express)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`
- Optional: Visual Studio 2022+ or VS Code with C# Dev Kit

Common commands (run from repo root):

```powershell
dotnet new sln -n mir-portfolio-web
dotnet new mvc -n MirPortfolio.Web -o src/MirPortfolio.Web -f net10.0
dotnet sln add src/MirPortfolio.Web/MirPortfolio.Web.csproj

dotnet build
dotnet run --project src/MirPortfolio.Web
dotnet test

dotnet ef migrations add <Name> --project src/MirPortfolio.Web
dotnet ef database update --project src/MirPortfolio.Web
```

Secrets (development):

```powershell
dotnet user-secrets init --project src/MirPortfolio.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<localdb connection string>" --project src/MirPortfolio.Web
dotnet user-secrets set "SeedAdmin:Email" "<admin email>" --project src/MirPortfolio.Web
dotnet user-secrets set "SeedAdmin:Password" "<strong password>" --project src/MirPortfolio.Web
```

---

## 6. Testing & Quality

- **xUnit** for unit tests of services and validation logic (M7).
- Integration tests for admin CRUD and contact submission (optional, later).
- `dotnet build` warnings treated as signals; keep the build clean.
- Manual smoke test per milestone: public pages, admin login, CRUD round-trip.

---

## 7. Version Control & Delivery

- **Git** (repository already initialized).
- Convention: feature work per milestone, small commits with clear messages.
- `.gitignore` excludes `bin/`, `obj/`, user secrets, and local databases.
- Hosting target **undecided**; app stays standard ASP.NET Core with no
  host-specific dependencies. Deployment notes added in Milestone 8.

---

## 8. Explicit Non-Choices

To keep the MVP focused, these are intentionally **not** used:

- SPA frameworks (React/Vue/Angular)
- Custom identity/auth implementation
- Docker / Kubernetes (can be added later)
- External CMS or headless CMS
- Client-side state libraries
