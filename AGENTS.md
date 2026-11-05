# AGENTS.md

Operational guidance for AI coding agents working on **mir-portfolio-web**.

## Project picture

A personal developer portfolio built with .NET 10, ASP.NET Core MVC (Razor),
EF Core 10, SQL Server LocalDB, and ASP.NET Core Identity. The project has:

- public pages: home, about, projects, blog, contact, and resume download;
- an admin area for projects, skills, experience, blog posts, contact messages,
  and site settings;
- Markdown-based blog posts with sanitized rendering;
- rate-limited contact form submissions.

Key docs:

- `Docs/PRD.md` — product scope and acceptance criteria.
- `Docs/SOFTWARE-ARCHITECTURE.md` — architecture and invariants.
- `Docs/TECH-STACK.md` — stack choices and tooling.
- `Docs/OPERATIONS.md` — run/build/migration commands and troubleshooting.
- `Docs/TESTING.md` — smoke-testing approach.
- `Docs/SECURITY.md` — security posture and controls.
- `Docs/CHANGELOG.md` — milestone history.
- `CONTRIBUTING.md` — repo conventions.
- `README.md` — practical quickstart.

## Working rules

### 1. Read before writing

- Inspect the relevant PRD/architecture section and surrounding code before
  changing anything.
- Follow ASP.NET Core MVC conventions rather than inventing alternate patterns.
- Do not commit secrets or output secrets into source files or logs.
- Keep the build clean: 0 warnings, 0 errors.

### 2. Use the intended architecture

- Controllers are thin (routing, model binding, auth, `ModelState`, calls to
  services).
- Services own domain logic and DB access. They should not depend on
  `HttpContext`.
- Data access uses EF Core and the `ApplicationDbContext`, not raw SQL inside
  controllers.
- Admin controllers live in `Areas/Admin/Controllers` and are decorated with
  `[Authorize(Roles = "Admin")]`.
- Each new/changed admin page sets `ViewData["Title"]` and
  `ViewData["Section"]`.

### 3. Validation and migrations

- Validate input through data annotations, `ModelState`, and unique-slug
  rules in services.
- Prefer using existing base classes and helper patterns. For example, mirror
  `IProjectService`/`IExperienceService` for new content entities.
- When entity shapes change, add a migration with a clear name and include it
  in the same change. Do not edit existing migrations.
- Do not write raw migration SQL into views/services.

### 4. Security

- Never disable antiforgery, authentication, authorization, or rate limiting
  to make a test pass.
- Keep phone/HTML/Markdown content sanitized server-side.
- Applied validation happens in services when possible; client-side checks are
  convenience only.
- Do not copy seeded admin credentials out of user-secrets into source files.
  Local credentials exist in user-secrets only.

### 5. Docs

- Update `README.md`, `Docs/OPERATIONS.md`, `Docs/TESTING.md`,
  `Docs/SECURITY.md`, and `Docs/CHANGELOG.md` whenever behavior, routes,
  deployment, security posture, testing steps, or milestone progress changes.
- Keep docs factual and implementation-accurate; avoid aspirational details.
- Use `Docs/` for long-form documents and root README for quickstart.

### 6. Milestones

- The project follows `PRD.md` milestones. Complete one milestone at a time and
  end with build + smoke tests.
- Milestone 7 is likely to add SEO/accessibility polish plus automated tests.
- Milestone 8 is deployment configuration documentation.

### 7. Smoke tests

- Every milestone should conclude with a manual smoke test covering relevant
  public pages and admin screens.
- Coordinate changes with other smoke tests so they can be run together.
- Prefer a fresh DB state or cleaned-up test data so milestones are repeatable.

## Typical commands

Run from the repository root:

```powershell
dotnet build src/MirPortfolio.Web/MirPortfolio.Web.csproj -v minimal
dotnet run --project src/MirPortfolio.Web --launch-profile http
dotnet ef migrations add <MigrationName> --project src/MirPortfolio.Web
dotnet ef database update --project src/MirPortfolio.Web
```

.NET 10/ASP.NET Core specific notes:

- Use `dotnet run --project src/MirPortfolio.Web --launch-profile http` to run
  on `http://localhost:5110`.
- Use user-secrets for local database and seed admin credentials.
- The app runs in Development mode; user-secrets load automatically.
- SQL Server LocalDB is required.

## Key folders

```text
Docs/                              docs index and deeper guidance
src/MirPortfolio.Web/              ASP.NET Core MVC app
  Areas/Admin/                     protected admin area
    Controllers/
    Views/
  Controllers/                     public controllers
  Data/                           DbContext, migrations, seed data
  Models/                         entities + Identity user
  Services/                       application services and interfaces
  ViewModels/                     public and admin view models
  Views/                          public Razor views
  wwwroot/                        static assets
```

## Forbidden

- Committing secrets, keys, or credentials.
- Disabling security features to make a call easier.
- Creating another ORM/data access abstraction unless requested.
- Adding framework-level infrastructure not requested by PRD.
- Editing generated migration files after they are applied.
- Treating unchecked smoke notes as implementation complete.
