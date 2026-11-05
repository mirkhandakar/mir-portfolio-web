# Changelog

All notable changes to this project are documented here.

The format is inspired by [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added

- Deployment prep:
  - generic filesystem publish profile at
    `src/MirPortfolio.Web/Properties/PublishProfiles/FolderProfile.pubxml`;
  - `appsettings.Production.json` baseline;
  - `Docs/DEPLOYMENT.md` with publish/config/migration/proxy notes.
- SEO/offline discoverability:
  - `/robots.txt`
  - `/sitemap.xml` generated from projects and published blog posts.
- Friendly error pages:
  - 404 page for unknown routes/slug;
  - 500 page for unhandled exceptions;
  - status-code re-execution via `UseStatusCodePagesWithReExecute`.
- Automated tests:
  - xUnit test project `tests/MirPortfolio.Web.Tests`;
  - EF Core InMemory-backed service tests;
  - blog slug/publish flow tests;
  - Markdown sanitizer tests;
  - contact inbox service tests.
- Blog rendering details:
  - Markdown headings render as semantic HTML.
  - Markdown links render with safe URI schemes.
  - Inline script-like HTML is encoded as text instead of executed.

- Public blog pages:
  - `/blog` shows published posts newest-first.
  - `/blog/{slug}` shows a single post by unique slug.
- Markdown rendering and server-side HTML sanitization for blog bodies.
- Admin blog management at `/admin/Posts`:
  - create, edit, delete;
  - draft/publish flow;
  - automatic unique slug generation;
  - cover image URL;
  - comma-separated tags.
- Main navigation now links to `/blog`.
- Admin dashboard links blog post count to blog management.

### Changed

- Public and admin navigation now includes the blog.
- Admin blog listing distinguishes published and draft posts.
- Smoke tests were refined to match the sanitizer behavior precisely (raw
  JavaScript removed, inline script-like content escaped as text).

## Milestone history

These are implementation milestones rather than product releases.

- Milestone 0 — .NET 10 project scaffold and base packages.
- Milestone 1 — EF Core data model, migrations, Identity seed, baseline content.
- Milestone 2 — public layout/theme and core public pages.
- Milestone 3 — projects and skills content, public list/detail views, admin CRUD.
- Milestone 4 — experience timeline and resume upload/download.
- Milestone 5 — public contact form, persistence, rate limiting, admin inbox.
- Milestone 6 — public blog pages, Markdown rendering, admin post management.
- Milestone 7 — SEO routes (`robots.txt`, `sitemap.xml`), friendly 404/500
  pages, accessibility polish, and automated service tests.
- Milestone 8 — publish profile, production config, and deployment notes.
