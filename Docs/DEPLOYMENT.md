# Deployment — `mir-portfolio-web`

Practical deployment notes for **ASP.NET Core MVC (.NET 10)**. The app is
host-agnostic; these notes cover a generic filesystem publish plus the usual
ASP.NET Core runtime requirements.

---

## 1. Build artifacts

From repo root:

```powershell
dotnet publish src/MirPortfolio.Web/MirPortfolio.Web.csproj -c Release -o out/publish
```

A publish profile is provided at:

- `src/MirPortfolio.Web/Properties/PublishProfiles/FolderProfile.pubxml`

To use it:

```powershell
dotnet publish src/MirPortfolio.Web/MirPortfolio.Web.csproj -p:PublishProfile=FolderProfile
```

`bin/Release/net10.0/publish/` contains the framework-dependent deployment.

---

## 2. Production configuration

Configuration must come from environment variables or the host's secret
mechanism, never from source files.

Recommended:

- `ConnectionStrings__DefaultConnection`
- `SeedAdmin__Email`
- `SeedAdmin__Password`
- `ASPNETCORE_ENVIRONMENT=Production`

Example:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=...;Database=MirPortfolio;Trusted_Connection=True;MultipleActiveResultSets=true"
$env:SeedAdmin__Email = "admin@your-domain"
$env:SeedAdmin__Password = "<strong-password>"
$env:ASPNETCORE_ENVIRONMENT = "Production"
```

Notes:

- In Production, the app does not use user-secrets.
- HSTS is enabled outside Development.
- Friendly 404/500 pages are enabled in all environments; stack traces are not
  shown.
- Local `LocalDB` is useful only for development; production should use SQL
  Server or equivalent.

---

## 3. Database migrations

Important:

- `SeedData.InitializeAsync` runs at startup in Development only.
- In Production, apply migrations before the first app start.

Recommended:

```powershell
dotnet ef database update --project src/MirPortfolio.Web
```

For CI/CD, add a dedicated migration step rather than relying on app startup.

---

## 4. Reverse proxy & HTTPS (generic notes)

When hosted behind IIS, nginx, or a cloud load balancer:

- terminate TLS at the proxy and forward HTTP/HTTPS headers correctly;
- keep `app.UseHttpsRedirection()`;
- `app.UseHsts()` is enabled outside Development;
- set `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true` when using a proxy so
  `Request.Scheme`/`Request.Host` are correct for canonical links and OG tags.

---

## 5. Deployment checklist

- [ ] Run `dotnet build` locally, 0 warnings / 0 errors.
- [ ] Run `dotnet test tests/MirPortfolio.Web.Tests` locally.
- [ ] Publish using the publish profile.
- [ ] Configure secrets as environment variables on the target host.
- [ ] Apply migrations before first start.
- [ ] Verify `/`, `/projects`, `/blog`, `/contact`, `/resume`, `/robots.txt`,
      `/sitemap.xml`, and admin flow in production.

These notes are implementation-agnostic and deliberately do not include
host-specific setup.
