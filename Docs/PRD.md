# PRD — Personal Developer Portfolio (`mir-portfolio-web`)

## 1. Overview

A personal portfolio website for a job-seeking software developer. It presents
who the developer is, showcases projects and experience, publishes technical
blog posts, and lets recruiters get in touch. Content is managed through a
protected admin area rather than hardcoded into views.

**Stack:** ASP.NET Core MVC (.NET 10), Entity Framework Core, SQL Server LocalDB,
ASP.NET Core Identity.
**Visual direction:** Clean, light, professional.
**Status:** Draft v1 — MVP scope.

---

## 2. Goals

- Give recruiters/hiring managers a fast, credible overview of the developer.
- Showcase projects with enough detail (problem, tech, links) to be convincing.
- Provide a way to contact the developer and download a resume.
- Make content editable without redeploying (admin CRUD).
- Demonstrate solid ASP.NET Core MVC practices end-to-end.

### Non-goals (out of scope for MVP)

- Multi-user authoring / public registration.
- Comments, likes, or social features.
- Full CMS features (media library, versioning, scheduling).
- E-commerce, payments, analytics dashboards.
- Native mobile app.

---

## 3. Target User & Personas

| Persona | Need |
| --- | --- |
| Recruiter / hiring manager | Quickly assess skills, projects, and fit; contact or download CV. |
| Fellow developer / client | Read project details and articles; find source links. |
| Site owner (admin) | Add/edit projects, skills, experience, posts; read contact messages. |

---

## 4. Information Architecture

### Public routes

| Route | Page |
| --- | --- |
| `/` | Home — hero, about preview, featured projects, skills preview, experience preview, contact CTA |
| `/about` | Full bio, photo, skills, experience timeline |
| `/projects` | Grid/list of all projects (filter by tech tag optional) |
| `/projects/{slug}` | Project detail (description, tech, repo/live links, image) |
| `/blog` | Published posts, newest first |
| `/blog/{slug}` | Single post |
| `/contact` | Contact form |
| `/resume` | Downloads the configured resume file (served from `wwwroot`) |

### Admin routes (`/admin`, `[Authorize(Roles = "Admin")]`)

| Route | Function |
| --- | --- |
| `/admin` | Dashboard: counts, unread messages |
| `/admin/projects` | List / create / edit / delete projects |
| `/admin/skills` | Manage skills |
| `/admin/experiences` | Manage work & education entries |
| `/admin/posts` | Manage blog posts (draft/publish) |
| `/admin/messages` | Contact inbox (read / mark read / delete) |
| `/admin/settings` | Edit `SiteProfile` and resume file |

---

## 5. Functional Requirements

### 5.1 Home & About

- Hero shows `FullName`, `Headline`, short `Bio`, and primary CTA (Contact / Resume).
- About page shows full bio, avatar, skill list grouped by category, and an
  experience timeline (newest first).
- All personal content comes from the `SiteProfile` record — no hardcoded copy.

### 5.2 Projects

- Admin can CRUD projects with: title, slug (auto from title, editable), summary,
  rich description, tech tags, image URL/upload, repo URL, live URL, featured flag,
  display order.
- Public list shows an image, title, summary, and up to N tech tags.
- Featured projects appear on the home page.
- Slug is unique; invalid/duplicate slugs are rejected with validation errors.
- Project detail page includes SEO title/description and Open Graph tags.

### 5.3 Skills & Experience

- Skills have name, category, optional proficiency (1–5), and display order.
- Experience entries have role/title, organization, location, type
  (Work | Education), start date, optional end date, and description.
- Entries render as a timeline; `EndDate == null` displays "Present".

### 5.4 Blog

- Admin can create/edit posts with title, slug, excerpt, body, cover image, tags,
  and publish flag.
- Only published posts are visible publicly, sorted by `PublishedAt` desc.
- Draft posts are accessible only in admin.
- Body supports Markdown. Rendered HTML is sanitized before display.

### 5.5 Contact

- Fields: name, email, subject, message (all required, validated).
- On success: persist `ContactMessage`, show confirmation, and (optionally)
  send an email if SMTP is configured.
- Basic anti-spam: hidden honeypot field + per-IP rate limiting; reject obvious spam.
- Admin inbox lists messages, newest first, with read/unread state.

### 5.6 Resume

- Resume file (PDF) is uploaded/configured in admin settings and stored under
  `wwwroot/resume/`.
- `/resume` returns the file with a download disposition; a friendly 404 is shown
  if none is configured.

### 5.7 Authentication

- ASP.NET Core Identity with a single seeded `Admin` user.
- No public registration. Login at `/account/login`.
- Credentials for the seed user come from configuration/user-secrets, never source.
- Logout, access-denied, and "return URL" behavior follow Identity defaults.

---

## 6. Data Model

EF Core entities (snake-free, conventional names):

**SiteProfile** (singleton)
`Id`, `FullName`, `Headline`, `Bio`, `AvatarUrl`, `Email`, `GitHubUrl`,
`LinkedInUrl`, `Location`, `ResumePath`, `UpdatedAt`

**Project**
`Id`, `Title`, `Slug`, `Summary`, `Description`, `TechTags` (string, comma or JSON),
`ImageUrl`, `RepoUrl`, `LiveUrl`, `IsFeatured`, `DisplayOrder`, `CreatedAt`, `UpdatedAt`

**Skill**
`Id`, `Name`, `Category`, `Proficiency` (int? 1–5), `DisplayOrder`

**Experience**
`Id`, `Title`, `Organization`, `Location`, `Type` (enum Work/Education),
`StartDate`, `EndDate` (nullable), `Description`, `DisplayOrder`

**BlogPost**
`Id`, `Title`, `Slug`, `Excerpt`, `Body`, `CoverImageUrl`, `Tags`,
`IsPublished`, `PublishedAt` (nullable until published), `CreatedAt`, `UpdatedAt`

**ContactMessage**
`Id`, `Name`, `Email`, `Subject`, `Body`, `IsRead`, `CreatedAt`

**ApplicationUser : IdentityUser** — admin account only.

Constraints: unique slugs on `Project` and `BlogPost`; required fields enforced
with data annotations + `ModelState`; EF migrations drive schema.

---

## 7. Technical Design

- **Project type:** ASP.NET Core MVC, `net10.0`.
- **Data access:** EF Core with SQL Server LocalDB; migrations checked in.
- **Auth:** ASP.NET Core Identity (cookie auth, `Admin` role/policy).
- **Frontend:** Razor views, Bootstrap 5 (or existing template), responsive layout,
  small amount of vanilla JS for nav/UX. Light, professional theme.
- **Validation:** server-side data annotations; unobtrusive client validation.
- **Rich content:** Markdig for Markdown; HTML sanitized (e.g. HtmlSanitizer).
- **Configuration:** `appsettings.json` for non-secrets; `dotnet user-secrets` for
  connection string and seed admin credentials.
- **Testing:** xUnit unit tests for services/validation; optional integration tests
  for admin CRUD endpoints.

### Security requirements

- HTTPS redirection + HSTS in non-development.
- Antiforgery tokens on all state-changing forms.
- Authorization on every admin action (`[Authorize(Roles = "Admin")]`).
- Input validation and output encoding; sanitize any rendered Markdown/HTML.
- Rate limiting on `/contact` (ASP.NET Core rate limiter).
- Secrets never committed; no credentials in source or repo.
- Safe file-upload checks (extension + size) for images and resume.

### Non-functional requirements

- Responsive on mobile, tablet, desktop.
- Accessibility: semantic HTML, labels, keyboard navigable, WCAG 2.1 AA contrast.
- SEO: per-page title/meta description, Open Graph tags, `sitemap.xml`, `robots.txt`.
- Performance: optimized images, minified assets, Lighthouse performance ≥ 90 on key pages.
- Friendly 404/500 pages; unhandled exceptions logged.
- Code follows MVC conventions; nullable reference types enabled.

---

## 8. Milestones

| # | Milestone | Outcome |
| --- | --- | --- |
| 0 | Scaffold | `dotnet new mvc`, Identity + EF packages, solution builds. |
| 1 | Data layer | DbContext, entities, migration, seed data, seeded admin. |
| 2 | Public shell | Layout, theme, nav/footer, Home, About, static pages. |
| 3 | Projects & skills | Public list/detail + admin CRUD. |
| 4 | Experience & resume | Timeline, admin CRUD, resume upload/download. |
| 5 | Contact | Form, persistence, rate limiting, admin inbox. |
| 6 | Blog | Markdown posts, publish flow, public list/detail. |
| 7 | Polish | SEO, accessibility, error pages, tests, docs. |
| 8 | Deploy prep | Config, publish profile notes, README run instructions. |

Each milestone ends with a build + runtime smoke test.

---

## 9. Acceptance Criteria (MVP)

- [ ] Site builds with `dotnet build` and runs with `dotnet run` (LocalDB configured).
- [ ] Admin can log in; anonymous users are redirected on admin routes.
- [ ] Admin can create/edit/delete a project and see it on the public site.
- [ ] Featured projects render on the home page.
- [ ] Admin can manage skills and experience; timeline renders correctly incl. "Present".
- [ ] Admin can create a draft post that stays hidden, then publish it publicly.
- [ ] Contact form validates, saves a message, and it appears in the admin inbox.
- [ ] Contact form is rate-limited and rejects honeypot spam.
- [ ] Resume uploads via admin and downloads via `/resume`.
- [ ] No secrets in source; connection string/admin credentials via user-secrets.
- [ ] Pages are responsive and pass basic keyboard/accessibility checks.
- [ ] Per-page SEO metadata present; `sitemap.xml` and `robots.txt` served.

---

## 10. Assumptions & Open Questions

- Personal details are **placeholders** (`FullName`, bio, links) to be filled later.
- Hosting target is undecided; app stays standard ASP.NET Core, deployment-agnostic.
- Email sending is optional and off by default (DB inbox is the source of truth).
- Blog is Markdown-based; no WYSIWYG editor in MVP.
- Single language (English) unless requested otherwise.
- Open: custom domain, social preview image, and analytics are post-MVP.

---

## 11. Out-of-Scope / Future Ideas

- Tag filtering and search.
- Newsletter signup and post scheduling.
- Dark-mode toggle.
- Media library and drag-and-drop uploads.
- Comments.
- CI/CD pipeline and automated deployment.
