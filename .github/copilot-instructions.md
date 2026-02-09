<!-- Auto-generated guidance for AI coding agents. Keep concise and actionable. -->
# Copilot / AI Agent Instructions — WroclawTheatreTickets

Repository snapshot (discoverable):

- Solution file at project root: `WroclawTheatreTickets.slnx`
- Visual Studio workspace settings in: `.vs/`

Quick goals for an agent arriving at this repo:

- Confirm what projects exist by searching for `*.csproj` / `*.fsproj` and `README.md`.
- If no source projects are present, prompt the human owner for the missing `src/` or solution content.

Fast discovery commands (Windows / PowerShell):

```powershell
Get-ChildItem -Recurse -Filter *.csproj
Get-ChildItem -Recurse -Include README.md,appsettings.json
git status --porcelain
git rev-parse --abbrev-ref HEAD
```

Standard build/test/run steps to try when projects are found:

- Restore dependencies: `dotnet restore` (run from solution root).
- Build the solution: `dotnet build WroclawTheatreTickets.slnx`.
- Run tests (if a `tests/` project exists): `dotnet test`.
- Launch a web/service project: `dotnet run --project <path-to-project>`.

Repository-specific conventions and expectations

- The single discoverable artifact is the solution file; don’t assume folder layout. Always search for `src/`, `tests/`, and `docs/`.
- Respect Visual Studio-generated files in `.vs/` — avoid modifying unless instructed.

When making changes

- Run `dotnet restore` and `dotnet build` after edits; ensure no new compiler errors are introduced.
- If adding or modifying projects, update the solution file with `dotnet sln add <project.csproj>`.
- Add or update unit tests in a `tests/` project when behaviour is changed.

Integration points & external dependencies

- No explicit external service configs are present in the repository snapshot. If you find `appsettings.{Environment}.json`, treat those as the place to document external endpoints/secrets.
- For database-backed projects, look for `Migrations/` or `*.db` files and any `ConnectionStrings` entries in `appsettings.json`.

When you are blocked or files are missing

- Ask the repository owner for the missing source tree, or for pointers to the expected branch/tag that contains the code.
- If CI/CD configuration is absent, request the preferred build runner (GitHub Actions, Azure Pipelines, etc.).

Examples & patterns to look for (if present)

- Project names like `WroclawTheatreTickets.Web` or `WroclawTheatreTickets.Api` indicate web/API boundaries.
- A `WroclawTheatreTickets.Data` or `*.EFCore` project suggests EF Core patterns (look for `DbContext` implementations and `Migrations/`).

Reporting back

- After discovery, provide a concise summary: projects found, build commands that succeed, and any missing files.
- If you make edits, include the `dotnet build` and `dotnet test` outputs, and the list of modified files.

If anything in this guidance is unclear or if you expect different conventions in this repo, please tell me what to adjust.

React Frontend (when present)

- Detect frontend: look for `package.json`, `tsconfig.json`, `src/`, `public/`, `vite.config.ts`, `next.config.js`, or `.env` files at repository root or `frontend/`/`client/` subfolders.
- Common commands (use the package manager found in `package.json` or presence of `pnpm-lock.yaml`/`yarn.lock`/`package-lock.json`):

```powershell
npm install
npm run dev         # local dev server (Vite, CRA, Next)
npm run build       # production build
npm test            # run frontend tests
npm run lint        # ESLint (if configured)
```

- Environment and backend integration:
	- Frontend typically reads API base URLs from `.env`, `.env.local`, or `process.env.NEXT_PUBLIC_API_URL` — search for `process.env` usage.
	- If a .NET backend exists, expect CORS or a reverse-proxy during development; look for `launchSettings.json` or API URL values in `appsettings.json` for the backend port.

- Files to reference when changing frontend behaviour:
	- `package.json` — scripts, dependencies, node engine hints
	- `tsconfig.json` or `jsconfig.json` — path aliases and module resolution
	- `vite.config.ts` / `next.config.js` — dev-server proxy and build customizations
	- `src/` and `public/` — UI entry points and static assets

- Testing and quality:
	- Run `npm test` and `npm run lint` after changes; prefer updating unit tests in `src/__tests__` or `tests/` adjacent to the code.
	- Respect existing ESLint/Prettier configuration files when formatting.

- When adding a frontend project to the repository:
	- Place it in a top-level `frontend/` or `client/` folder and add a short `README.md` explaining how to run it locally and connect to the backend.
	- Do NOT modify the `.slnx` Visual Studio solution unless asked; if required, update it using `dotnet sln add <project.csproj>` for backend projects only.

	Backend — Clean Architecture (.NET 10 C#)

	- Detect backend projects: search for `*.csproj` and inspect `<TargetFramework>` in each project file to confirm `net10.0`.
	- Expected project layout (common pattern agents should look for):
		- `WroclawTheatreTickets.Domain` — entities, value objects, domain services, domain events (no external deps)
		- `WroclawTheatreTickets.Application` — use-cases, interfaces, DTOs/contracts, MediatR handlers, validation
		- `WroclawTheatreTickets.Infrastructure` — EF Core `DbContext`, repository implementations, external service clients, migrations
		- `WroclawTheatreTickets.Web` or `WroclawTheatreTickets.Api` — API surface (Minimal APIs or Controllers), DI composition root, configuration
		- (optional) `WroclawTheatreTickets.SharedKernel` — cross-cutting types, exceptions, result types

	- Dependency rules (enforce inward-facing dependencies):
		- No references from `Domain` -> `Application`/`Infrastructure`/`Web`.
		- `Application` may reference `Domain` but not `Infrastructure` directly; use interfaces for persistence abstractions.
		- `Infrastructure` references `Application` and `Domain` to provide implementations.

	- Practical conventions for agents editing code:
		- Put repository and EF `DbContext` in `Infrastructure`. Keep EF migrations under `Infrastructure/Migrations`.
		- Define persistence interfaces (e.g., `IShowRepository`) in `Application` or `Application.Contracts`; implement them in `Infrastructure`.
		- Keep DTOs and mapping profiles in `Application` (or `Application.Contracts`); use `Mapster` or `AutoMapper` integrated in the composition root (`Web` project).
		- Place MediatR handlers in `Application` and register MediatR in the `Web` project's DI registration.

	- Build/test/maintenance commands (run from solution root):

	```powershell
	dotnet restore
	dotnet build WroclawTheatreTickets.slnx -c Release
	dotnet test
	```

	- Adding or modifying projects:
		- When adding a new project, run `dotnet sln add <path-to-project.csproj>` so the solution stays consistent.
		- To add EF migrations (from solution root):

	```powershell
	dotnet ef migrations add <Name> --project Infrastructure --startup-project Web
	dotnet ef database update --project Infrastructure --startup-project Web
	```

	- CI tips for agents:
		- Build the full solution and run tests as separate steps; restore first. Cache ~/.nuget packages between runs.
		- Keep migration generation reproducible: prefer explicit `--startup-project` and `--project` arguments.

	- If the repo currently has no backend source files (only the `.slnx`), ask the owner for the intended project folders or branch that contains the code; do not scaffold a full backend without instruction.

Automatic Session Logging

- **REQUIREMENT**: Every AI agent session MUST automatically log all user prompts and assistant responses to `.chatlogs/prompts/` folder.
- **Timing**: Save logs AFTER each user request is completed (not before). Create one file per message.
- **Automation**: Treat this as an automatic step in every interaction — do not ask for permission or confirmation.
- **Workflow**:
  1. After processing a user request, immediately create a prompt log file
  2. After generating a response, immediately create a response log file
  3. Use timestamps in ascending order to maintain chronological ordering
  4. Include context, goals, and decisions made
- **File Format**: Use YAML front-matter + markdown body (see Save prompts & chat logs section below)
- **Error Recovery**: If log creation fails, log the error but continue with main task — don't block user work
- **Privacy**: Redact secrets, credentials, tokens, API keys before saving; set `redacted: true` in front-matter
- **Benefits**: Creates reproducible audit trail, enables AI handoff, supports debugging, documents decision history
- **Example Trigger**: After user says "Create cleanup job", save the prompt, complete the work, then save the response.

Save prompts & chat logs

- Requirement: persist every user prompt and assistant response to separate files so humans can audit, reproduce, and continue an agent session.
- Storage location: create a repo-local folder `.chatlogs/prompts/` and add it to `.gitignore`.
- Filename format (recommended): `YYYYMMDD_HHMMSS_role_slug.md` (e.g. `20260204_143012_user_search-shows.md`).
- File contents (YAML front-matter + body):

```md
---
date: 2026-02-04T14:30:12Z
role: user
conversation_id: <conversation-id-if-available>
context: WroclawTheatreTickets repo discovery
redacted: false
---

<original prompt text>
```

- For assistant responses, use `role: assistant` and include the full reply under the body. If the assistant applied a patch, include the diff or reference to the applied patch in the file body.
- Redaction rule: never write secrets, credentials, private keys, or access tokens to disk. Replace secrets with `<REDACTED>` and set `redacted: true` in the front-matter.
- Quick PowerShell example to create the folder and write a prompt file:

```powershell
mkdir -Force .chatlogs\prompts
$ts = (Get-Date).ToString('yyyyMMdd_HHmmss')
$file = ".chatlogs\prompts\${ts}_user_input.md"
@"
---
date: $(Get-Date -Format o)
role: user
---
<paste prompt here>
"@ | Out-File -Encoding utf8 $file
```

- Why: keeps a reproducible audit trail of agent actions and makes handoff to humans straightforward.
