# Operations — `mir-portfolio-web`

Practical commands for running, migrating, and troubleshooting the project.

## Quickstart

```powershell
dotnet restore
dotnet build src/MirPortfolio.Web/MirPortfolio.Web.csproj -v minimal
dotnet run --project src/MirPortfolio.Web --launch-profile http
```

The site runs on `http://localhost:5110` when using the `http` profile.

---

## Publishing

For deployment, see [DEPLOYMENT.md](DEPLOYMENT.md). Quick publish from repo root:

```powershell
dotnet publish src/MirPortfolio.Web/MirPortfolio.Web.csproj -p:PublishProfile=FolderProfile
```

Production configuration is environment-variable based; user-secrets are
development-only.

---

## Configuration

Configure these once per machine with user-secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=MirPortfolio;Trusted_Connection=True;MultipleActiveResultSets=true" --project src/MirPortfolio.Web
dotnet user-secrets set "SeedAdmin:Email" "admin@mirportfolio.local" --project src/MirPortfolio.Web
dotnet user-secrets set "SeedAdmin:Password" "<strong-password>" --project src/MirPortfolio.Web
```

- The connection string points at SQL Server LocalDB in Development.
- The seed admin is created at startup in Development when credentials exist.
- In Production, use environment variables/secret store rather than user secrets.
- Never write credentials into `appsettings*.json`, source files, logs, or docs.

## Database & migrations

```powershell
dotnet ef migrations add <MigrationName> --project src/MirPortfolio.Web
dotnet ef database update --project src/MirPortfolio.Web
```

Rules:

- Do not edit applied migrations.
- Add a new migration when the entity model changes.
- On startup in Development, `SeedData.InitializeAsync` applies pending
  migrations and seeds baseline content/admin.

## Common maintenance

- Delete the local `MirPortfolio` database only when you intentionally want a
  clean database and an empty content state.
- Uploaded resumes live under `src/MirPortfolio.Web/wwwroot/resume/`.
- Contact messages and blog posts are application data, not configuration.

## Shutdown/cleanup

After smoke tests or long-running debugging:

```powershell
Get-Process MirPortfolio.Web -ErrorAction SilentlyContinue | Stop-Process -Force
```

## Troubleshooting

### Build fails on locked files

- Stop leftover `MirPortfolio.Web` processes.
- Close long-running test processes and rebuild.

### Login or admin access fails

- Confirm `SeedAdmin:Email` and `SeedAdmin:Password` exist in user-secrets.
- Confirm the LocalDB database exists.
- Confirm the account has the `Admin` role after seeding.

### Contact/rate limiting fails

- The contact policy allows 5 submissions per IP per 10 minutes.
- During manual testing, avoid rapid repeated submissions or restart the app
  to reset the limiter.

### Blog post invisible publicly

- Confirm the post is checked as published.
- Confirm `PublishedAt` is populated.
- Draft posts intentionally do not appear on `/blog`.

### Sitemap or robots seems stale

- `/sitemap.xml` is generated from live projects and published blog posts.
- `/robots.txt` intentionally disallows `/admin` and `/Identity`.
- After content changes, re-fetch/refresh externally; the app does not keep a
  cached copy.
