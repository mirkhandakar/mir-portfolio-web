# Contributing

Small conventions so this repo stays readable and testable.

## Repository layout

- `Docs/` holds project documentation (architecture, operations, testing,
  security, requirements, tech stack).
- `src/MirPortfolio.Web/` is the single web application.
- `tests/` is reserved for automated tests when Milestone 7 starts.

## Workflow

1. Work in small, milestone-sized changes.
2. Keep one milestone at a time. Prefer ending with:
   - clean build (`dotnet build`);
   - passing local smoke tests;
   - updated documentation.
3. Use descriptive commit titles:
   - `feat: blog list and detail pages`
   - `fix: unique blog slug handling`
   - `chore: update operations doc`
4. For AI collaboration, follow `Docs/AI-WORKFLOW.md`.
5. Do not commit generated binaries. These include:
   - `bin/`
   - `obj/`
   - `wwwroot/resume/*.pdf`
   - user secrets.

## Coding standards

- Follow existing ASP.NET Core MVC patterns.
- Controllers stay thin and route only.
- Services own domain rules and database access.
- Adhere to the architecture doc.
- Keep nullable reference types enabled.
- Prefer explicit C# types and factories over ad-hoc duplication.
- Avoid duplicating existing service interfaces and helpers; reuse them.

## Documentation standards

- Docs live under `Docs/` when long-form.
- `README.md` stays a practical quickstart.
- User-facing behavior changes must update README.
- Security behavior changes must update `Docs/SECURITY.md`.
- New public/admin routes should be reflected in the README when created.
- Avoid aspirational claims; describe the current implementation.

## Security

Before asking for review:

- no secrets in source;
- no credentials pasted into logs;
- no security guardrails disabled to get a test to pass;
- all admin routes still protected;
- contact form still rate limited.

## Test expectations

Each functional milestone should include:

- manual smoke steps under `Docs/TESTING.md`;
- a build with 0 warnings and 0 errors;
- relevant admin and public routes verified;
- updated testing notes if behavior changes.

## Review checklist

Ask yourself before opening a PR:

- Do I understand the PRD acceptance criteria?
- Did I update docs?
- Is the build clean?
- Did I break any authentication/authorization assumptions?
- Did I regenerate or add migrations when the data model changed?
- Are all relevant public/admin routes smoke-tested?
