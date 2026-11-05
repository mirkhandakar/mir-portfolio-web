# Security — `mir-portfolio-web`

Focus areas for this project, aligned with the architecture and PRD.

## Threat posture

The site is a single self-hosted ASP.NET Core MVC app with:

- one seeded `Admin` user;
- public contact submissions;
- content CRUD via admin;
- Markdown posts rendered server-side.

Main attack surfaces:

1. admin authentication/authorization;
2. contact spam and rate-limit abuse;
3. Markdown/HTML injection;
4. uploaded files;
5. secrets leakage.

## Implemented safeguards

### Admin area

- `[Authorize(Roles = "Admin")]` on all admin controllers.
- Login only through ASP.NET Core Identity; no public registration.
- Admin credentials come from configuration/user-secrets, never source.
- Identity password requirements and lockout are configured.

### Contact form

- Anti-forgery token required.
- Hidden honeypot field to discourage bots.
- Per-IP rate limiting: 5 submissions per 10 minutes.
- No email delivery; database is the source of truth.

### Blog rendering

- Markdown -> HTML is rendered server-side.
- HTML output is sanitized before display using `HtmlSanitizer`.
- Allowed HTML elements/attributes and URI schemes are explicitly limited.
- Inline script/event-style HTML is not interpreted; it is encoded as text
  rather than executed.

### Uploaded resumes

- Stored under `src/MirPortfolio.Web/wwwroot/resume/`.
- Only PDF files are accepted.
- Maximum size is enforced (5 MB).
- Unique generated filenames avoid overwriting existing files.
- Download uses a file disposition to avoid accidental rendering.

### Web application fundamentals

- Antiforgery tokens on state-changing forms.
- HTTPS redirection, plus HSTS outside Development.
- Razor default output encoding on model values.
- No secrets written into logs or docs.
- Admin area pages set `noindex, nofollow`.
- Friendly error pages avoid leaking stack traces; unhandled exceptions are
  handled outside Development.
- `/robots.txt` disallows `/admin` and `/Identity` (defense in depth, not a
  substitute for authorization).

## Secrets

Use user-secrets locally:

```powershell
dotnet user-secrets list --project src/MirPortfolio.Web
```

Do not:

- commit connection strings;
- paste admin passwords into documentation;
- store secrets in environment variables inside generated source;
- copy seeded admin credentials into code or test fixtures.

Do:

- configure admin credentials on a per-machine basis;
- avoid running seed content/API keys through `appsettings.json` defaults;
- in production, use environment variables or the host secret store;
- see [DEPLOYMENT.md](DEPLOYMENT.md) for environment-variable settings.

## Known current limitations

- Markdown output sanitizer is intentionally conservative but not a WYSIWYG
  editor; admins should review rendered output before publishing.
- Upload validation exists but is not yet intended for bulk or programmatic
  client-side uploads.
- Rate limiting is per IP; shared infrastructures/proxies may need
  configuration in later milestones.
