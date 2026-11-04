# Software Architecture — `mir-portfolio-web`

Companion to [PRD.md](PRD.md). Describes how the portfolio is structured and why.

---

## 1. Architectural Style

A **layered, server-rendered MVC monolith**. A single ASP.NET Core application
serves both the public portfolio and a protected admin area.

Why this style:

- The domain is small and read-heavy; a monolith keeps deployment and debugging simple.
- Server-side rendering gives fast first paint and good SEO with minimal tooling.
- Clear layer boundaries still allow future extraction (e.g. an API) if needed.

```
                    ┌───────────────────────────────┐
                    │           Browser             │
                    └───────────────┬───────────────┘
                                    │ HTTPS
                    ┌───────────────▼───────────────┐
                    │        ASP.NET Core           │
                    │  ┌─────────────────────────┐  │
                    │  │ Presentation            │  │
                    │  │ Controllers, Razor,     │  │
                    │  │ ViewModels, Middleware  │  │
                    │  └───────────┬─────────────┘  │
                    │  ┌───────────▼─────────────┐  │
                    │  │ Application             │  │
                    │  │ Services, Validation    │  │
                    │  └───────────┬─────────────┘  │
                    │  ┌───────────▼─────────────┐  │
                    │  │ Domain / Data           │  │
                    │  │ Entities, DbContext,    │  │
                    │  │ EF Core, Identity       │  │
                    │  └───────────┬─────────────┘  │
                    └──────────────┼────────────────┘
                                   │
                    ┌──────────────▼────────────────┐
                    │      SQL Server LocalDB       │
                    └───────────────────────────────┘
```

---

## 2. Solution Layout

```
mir-portfolio-web/
├── Docs/                            # PRD, architecture, tech stack
├── src/
│   └── MirPortfolio.Web/            # single MVC web app
│       ├── Areas/Admin/             # admin controllers + views (M3+)
│       ├── Controllers/             # public controllers
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   ├── Migrations/          # checked-in EF migrations
│       │   └── Seed/                # seed data + admin user
│       ├── Models/                  # entities + Identity user
│       ├── Services/                # application services/interfaces
│       ├── ViewModels/              # view-specific DTOs
│       ├── Views/                   # Razor views + shared layout
│       ├── wwwroot/                 # css, js, images, resume/
│       ├── Program.cs               # composition root
│       └── appsettings*.json
├── tests/
│   └── MirPortfolio.Web.Tests/      # xUnit (M7)
├── mir-portfolio-web.sln
└── README.md
```

Paths follow ASP.NET Core conventions; `Areas/Admin` keeps admin routing and
authorization isolated from public pages.

---

## 3. Layer Responsibilities

| Layer | Contains | Must not |
| --- | --- | --- |
| Presentation | Controllers, Razor views, ViewModels, middleware, filters | Contain business rules or EF queries directly |
| Application | Services (e.g. `ProjectService`, `ContactService`), validation, mapping | Reference `HttpContext` or Razor |
| Domain / Data | Entities, `ApplicationDbContext`, seed logic, Identity | Depend on MVC types |

Controllers stay thin: validate `ModelState`, call a service, return a view/redirect.
Services own queries and invariants (e.g. unique slugs, publish rules).

---

## 4. Key Flows

### 4.1 Page request (public)

```
Request → Routing → Controller → Service → DbContext → LocalDB
                          ↓
                    ViewModel → Razor View → HTML response
```

Read-only pages use `AsNoTracking()` queries and project directly to ViewModels.

### 4.2 Authentication & authorization

- ASP.NET Core Identity with cookie authentication.
- One seeded `Admin` user; **no public registration**.
- Admin controllers live under `Areas/Admin` and are protected with
  `[Authorize(Roles = "Admin")]` (policy `AdminOnly`).
- Login/logout use Identity endpoints; return URLs are honored safely.
- Seed credentials come from configuration (`SeedAdmin` section) supplied via
  user-secrets — never committed.

### 4.3 Content management (admin CRUD)

```
Admin form → POST → [Authorize] → ModelState validation
           → Service (slug uniqueness, sanitize) → DbContext → redirect to list
```

- Slugs are generated from titles, de-duplicated, and validated.
- Rich text/Markdown is sanitized before persistence or render.
- Uploads (images, resume) are validated by extension + size and stored under
  `wwwroot/` (MVP); a storage abstraction allows moving to blob storage later.

### 4.4 Contact submission

```
POST /contact → honeypot check → rate limiter → validation
             → ContactService → ContactMessage saved → confirmation
             → optional email (no-op if SMTP not configured)
```

- Database is the source of truth; email is best-effort.
- Admin inbox reads messages ordered by `CreatedAt` desc.

---

## 5. Cross-Cutting Concerns

| Concern | Approach |
| --- | --- |
| Validation | Data annotations on ViewModels/entities; `ModelState` checks in controllers |
| Error handling | `UseExceptionHandler` for production; friendly 404/500 pages; structured logging |
| Security | HTTPS + HSTS, antiforgery tokens, authorization policies, rate limiting, sanitized HTML, secrets via user-secrets |
| Logging | Built-in `ILogger<T>`; no PII or secrets logged |
| Configuration | `appsettings.json` for non-secrets; environment/user-secrets for secrets |
| SEO | Per-page title/meta/OpenGraph via a shared ViewModel; `sitemap.xml`, `robots.txt` |
| Accessibility | Semantic landmarks, labelled inputs, focus states, WCAG 2.1 AA contrast |
| File uploads | Extension + size allow-list; unique stored names; serve with download disposition for resume |

---

## 6. Data & Persistence

- EF Core code-first with SQL Server LocalDB in development.
- Migrations are versioned in source and applied with `dotnet ef database update`.
- One `ApplicationDbContext` deriving from `IdentityDbContext<ApplicationUser>`.
- Relationships: `SiteProfile` is a singleton row; other entities are independent
  content records. Unique indexes on `Project.Slug` and `BlogPost.Slug`.
- Seed data provides an initial profile, sample projects/skills, and the admin user.
- No raw SQL in controllers; all data access goes through services or the context.

---

## 7. Key Decisions (ADR summary)

| Decision | Choice | Rationale |
| --- | --- | --- |
| App style | Server-rendered MVC monolith | Small domain, SEO, simple ops |
| Database | SQL Server LocalDB | Windows-friendly, EF tooling, upgrade path to full SQL Server |
| Auth | ASP.NET Core Identity, admin-only | Secure, built-in, no custom auth code |
| Content authoring | Admin CRUD + Markdown | Editable without redeploy; no CMS overhead |
| Contact | Persist to DB (email optional) | No external provider required for MVP |
| Frontend | Razor + Bootstrap, minimal JS | Fast, accessible, low maintenance |
| Deployment | Undecided; standard ASP.NET Core | Keeps hosting options open |

---

## 8. Evolution Path

If the project grows beyond MVP:

1. Extract `Application` services into a separate project/assembly.
2. Add a JSON API and a JS frontend if interactivity demands it.
3. Replace local file uploads with blob storage behind the existing abstraction.
4. Add caching (response/output cache) for public read-heavy pages.
5. Introduce CI/CD with build, test, migration, and deploy stages.

These are intentionally **not** part of the MVP to avoid premature complexity.
