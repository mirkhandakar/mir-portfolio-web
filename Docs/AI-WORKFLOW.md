# AI Workflow — `mir-portfolio-web`

Practical process for collaborating with AI coding agents on this repository.
This is the process layer that complements the other agent-facing docs:

- `AGENTS.md` — hard **rules** (architecture, security, migrations, commands).
- `CONTRIBUTING.md` — repo **conventions** (commits, docs, review checklist).
- This file — the **collaboration loop** a human and an agent should follow.

If any statement here conflicts with `AGENTS.md` or `CONTRIBUTING.md`, those
documents win.

---

## 1. What an agent actually is

Treat the agent as a capable collaborator with these properties:

- It has **no memory between sessions**. Every session starts fresh.
- It can **read files, search code, and run shell commands** (build, tests,
  git, migrations) and make edits with your approval.
- It works in a small loop: **observe → reason → act → verify → repeat**.
- It does **not watch your app run**; it relies on command output, code, and
  explicit confirmation.

Act accordingly: give it concrete tasks, verify claims with commands, and keep
durable facts in files rather than in chat.

---

## 2. The collaboration loop

Use this rhythm for every non-trivial change:

1. **State the task and constraints** — what to build/fix, the scope, and any
   hard rules ("keep 0 warnings / 0 errors", "do not commit yet").
2. **Ask for a plan first** — for anything beyond a one-line fix, have the
   agent propose a short plan before editing.
3. **Approve or adjust the plan** — small course-corrections are far cheaper
   now than after implementation.
4. **Implement** — let the agent make the agreed edits.
5. **Verify via commands** — the agent runs `dotnet build`, `dotnet test`,
   and, when behavior changed, the relevant smoke test. Do not accept
   results without command output.
6. **Review the diff together** — check files, routes, and docs for
   consistency.
7. **Commit only when you ask** — commits are an explicit human decision
   (see `CONTRIBUTING.md` for commit style).

Docs-only changes skip steps 2 and 5 partially: they usually need only a final
clean build, not a runtime smoke test.

---

## 3. What to read before each task type

Point the agent at the smallest useful context instead of pasting files.

| Task type | Read first |
| --- | --- |
| New public/admin feature | Relevant section of `Docs/PRD.md`, `Docs/SOFTWARE-ARCHITECTURE.md`, and a similar existing feature (e.g. mirror `IProjectService` for new content). |
| Data model change | `Docs/PRD.md` model section, `Docs/OPERATIONS.md` migration rules (`AGENTS.md` §3 also covers this). |
| Bugfix | Reproduce first (route/error message), then read the smallest relevant controller/service. Prefer the smallest fix. |
| Deployment/config change | `Docs/DEPLOYMENT.md`, `Docs/OPERATIONS.md`, and the publish profile. |
| Security-relevant change | `Docs/SECURITY.md` and `AGENTS.md` §4. |
| Docs only | The relevant subset of `README.md`, `Docs/OPERATIONS.md`, `Docs/TESTING.md`, `Docs/SECURITY.md`, `Docs/CHANGELOG.md`, `CONTRIBUTING.md`. |

---

## 4. Verification contract

A change is only "done" when all relevant items pass:

- [ ] `dotnet build src/MirPortfolio.Web/MirPortfolio.Web.csproj -v minimal`
      — **0 warnings, 0 errors**.
- [ ] `dotnet test tests/MirPortfolio.Web.Tests/MirPortfolio.Web.Tests.csproj --nologo`
      — all tests pass (currently 13/13).
- [ ] Relevant smoke test from `Docs/TESTING.md` for milestone or
      feature-level changes (public pages, admin flows, routes).
- [ ] Documentation updated when behavior, routes, security posture,
      deployment, or testing steps change (per `AGENTS.md` §5).
- [ ] No leftover `MirPortfolio.Web` process after smoke tests:
      `Get-Process MirPortfolio.Web -ErrorAction SilentlyContinue | Stop-Process -Force`.
- [ ] No secrets committed, no credentials echoed into logs or docs.

Never disable antiforgery, auth, authorization, or rate limiting to make a
test pass. If a security feature blocks a test, stop and ask.

---

## 5. Context and session hygiene

- **Start every session fresh** by having the agent read (or re-read):
  1. `AGENTS.md`
  2. `Docs/PRD.md` (the specific milestone/acceptance criteria in play)
  3. The most relevant feature area (views/controllers/services)
- **Durable facts live in files** (`AGENTS.md`, docs, `CHANGELOG.md`), not in
  chat. If something important is only in a session, it is effectively lost
  once the session ends.
- **Keep prompts lean**. Reference file paths (`Program.cs`,
  `Docs/OPERATIONS.md`) instead of pasting long contents.
- **Prefer compact, focused sessions**: one milestone or one coherent task per
  session. Long sessions risk older context being summarized away.
- **End milestones with accurate docs** — the next session should be able to
  resume from `README.md` + `Docs/` alone without needing the old chat.

---

## 6. When to stop and ask first

Have the agent pause for explicit human approval before:

- changing the database schema or `ApplicationDbContextModelSnapshot`;
- adding/removing a NuGet package or multiple dependencies;
- changing `Program.cs` middleware order or security configuration;
- touching authentication, authorization, antiforgery, or rate limiting;
- creating a new public/admin route or admin view;
- large refactors or broad renaming;
- committing, amending, or pushing to Git;
- anything ambiguous or not clearly covered by the PRD.

For smaller tasks, saying just "tell me your plan first, don't edit yet" is
usually enough.

---

## 7. Commit and PR notes

- Commit and PR work is human-driven. The agent should propose, not decide.
- Keep commit titles descriptive, in the style used in this repo:
  - `feat: blog list and detail pages`
  - `fix: unique blog slug handling`
  - `chore: update operations doc`
- Do not commit generated binaries (`bin/`, `obj/`), uploaded resumes, or user
  secrets; see `CONTRIBUTING.md`.
- Before committing, review the actual diff (`git diff`, `git status`) — trust
  only what is there, not the summary.

---

## 8. Example prompt

A good request is specific, constrained, verifiable, and points the agent at
the right files:

> "Using `AGENTS.md` and `Docs/PRD.md`, implement <X> following existing
> conventions. Steps: (1) first propose a plan, don't edit yet; (2) implement;
> (3) verify with `dotnet build src/MirPortfolio.Web/MirPortfolio.Web.csproj -v
> minimal` and `dotnet test tests/MirPortfolio.Web.Tests/MirPortfolio.Web.Tests.csproj
> --nologo`; (4) update `README.md`, `Docs/TESTING.md`, and
> `Docs/CHANGELOG.md`; (5) report the results. Do not commit without me asking."

That single prompt encodes the whole loop from this file: plan → implement →
verify → docs → report, with verification built in.
