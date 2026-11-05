# Testing — `mir-portfolio-web`

Testing today combines:

1. automated xUnit tests for core services (Milestone 7);
2. manual milestone smoke tests for public/admin behavior.

## Automated tests

The test project lives at `tests/MirPortfolio.Web.Tests` and is referenced by
`mir-portfolio-web.slnx`. It uses xUnit plus EF Core InMemory for service-level
tests.

Run with:

```powershell
dotnet test tests/MirPortfolio.Web.Tests/MirPortfolio.Web.Tests.csproj --nologo
```

Covered areas:

- blog post publish/draft flow, timestamps, and unique slug generation;
- `MarkdownSanitizer` (headings, links, script removal, event handler removal);
- contact message persistence, unread counts, read toggling, deletion.

## Smoke test scope

For each milestone, verify:

- the app builds clean (0 warnings, 0 errors);
- startup works with the configured LocalDB connection;
- relevant public pages render;
- relevant admin CRUD flows work;
- routes/authorization behave as intended.

Current smoke route set:

- public: `/`, `/Home/About`, `/projects`, `/projects/<slug>`,
  `/blog`, `/blog/<slug>`, `/contact`, `/resume`, `/robots.txt`, `/sitemap.xml`
- admin: `/admin`, `/admin/Projects`, `/admin/Skills`,
  `/admin/Experiences`, `/admin/Posts`, `/admin/Messages`, `/admin/Settings`
- auth: `/Identity/Account/Login`, admin access denied when signed out
- errors: unknown route shows the friendly 404 page; unhandled exceptions show
  the friendly 500 page

## Milestone 6 checklist

1. Build:
   ```powershell
   dotnet build src/MirPortfolio.Web/MirPortfolio.Web.csproj -v minimal
   ```
2. Start:
   ```powershell
   dotnet run --project src/MirPortfolio.Web --launch-profile http
   ```
3. Verify the seeded post appears on `/blog`.
4. Open the post detail and confirm Markdown winds up as sanitized HTML
   (headings, links, code formatting, etc. all render as expected).
5. In `/admin/Posts`:
   - create a new draft post; confirm it is hidden on `/blog`;
   - publish the draft; confirm it appears publicly;
   - edit slug/content; verify updated public link and content;
   - delete the test post; verify it no longer appears publicly or in admin.
6. Verify unique slugs are enforced.
7. Confirm Markdown containing suspicious HTML/script is neutralized in the
   rendered view. Inline script-like content should remain encoded as text
   rather than interpreted.

## Milestone 7 checklist

1. Build:
   ```powershell
   dotnet build src/MirPortfolio.Web/MirPortfolio.Web.csproj -v minimal
   dotnet test tests/MirPortfolio.Web.Tests/MirPortfolio.Web.Tests.csproj --nologo
   ```
2. Start:
   ```powershell
   dotnet run --project src/MirPortfolio.Web --launch-profile http
   ```
3. Public:
   - visit `/robots.txt` and confirm admin paths are disallowed;
   - visit `/sitemap.xml` and confirm public pages/entries appear;
   - visit an unknown path and confirm the friendly 404 page;
   - confirm per-page titles/description and Open Graph document properties.
4. Admin/regression:
   - verify `/admin` and admin CRUD pages still work when logged in;
   - verify `/projects`, `/blog`, and `/contact` still render.
5. Shutdown:
   ```powershell
   Get-Process MirPortfolio.Web -ErrorAction SilentlyContinue | Stop-Process -Force
   ```

## Milestone 8 checklist

1. Run local checks:
   ```powershell
   dotnet build src/MirPortfolio.Web/MirPortfolio.Web.csproj -v minimal
   dotnet test tests/MirPortfolio.Web.Tests/MirPortfolio.Web.Tests.csproj --nologo
   ```
2. Publish smoke check:
   ```powershell
   dotnet publish src/MirPortfolio.Web/MirPortfolio.Web.csproj -p:PublishProfile=FolderProfile
   ```
3. Confirm publish artifacts contain the app binaries, `appsettings*.json`,
   `wwwroot`, and no local user-secret content.
4. Verify `Docs/DEPLOYMENT.md` matches the actual publish profile.

## Planned automated tests

Automated tests now exist for service-level behavior. Additional future tests
may cover more controller routing, validation, and admin flows.
