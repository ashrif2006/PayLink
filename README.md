# PayLink

A bill-payment and wallet backend. Year work project — Phase 1 (requirements, design, Docker and CI foundation).

> **Team size:** 4 · **Phase 1 due:** Wed 25 Nov · **Phase 2 due:** Fri 18 Dec

## What this repository contains

Phase 1 delivers the **foundation**, not the finished payment system:

| Area | Deliverable | Where |
| --- | --- | --- |
| Requirements | 25 user stories, acceptance criteria, 6 NFRs, priorities | `docs/requirements.md` |
| Design | 6 diagrams in Mermaid source | `docs/design/` |
| DevOps | Dockerfile, compose with database, CI workflow | root, `.github/workflows/` |
| AI documentation | Prompt log, 5 entries incl. 2 corrected answers | `docs/prompt-log.md` |

## Quick start

Requires .NET 8 SDK, or just Docker.

```bash
# Local
dotnet build
dotnet test
dotnet run --project src/PayLink.Api

# Everything, including the database
docker compose up --build
```

Then open:

- API health: <http://localhost:8080/health>
- Swagger (development only): <http://localhost:8080/swagger>

## Layout

```
docs/            requirements, design diagrams, prompt log, report
docs/design/     Mermaid sources (.mmd) — render in any Mermaid viewer
src/PayLink.Api/ the backend (Controllers + Program.cs + Domain)
tests/           xUnit tests, run by CI on every push
.github/         CI workflow (restore, build, test)
```

## Conventions

- **Branching:** `main` is protected. Work on `feature/<short-name>`, open a pull request, need one review and green CI to merge.
- **Commits:** `area: what changed` — e.g. `wallet: add hold and release`.
- **Money:** always `decimal`, never `double`. Currency is stored with the amount.
- **No secrets** in the repository. Local configuration goes in `.env` (ignored) or environment variables.

## The core problem

A customer pays a bill from their wallet. The interesting part is not the happy path — it is what happens when the biller received the money but the reply arrived late (`Unknown`), when the customer taps **Pay** twice (idempotency), and when the callback arrives twice.

Those cases drive the requirements, the state machine, and the tests.

## Status

- [x] Repository skeleton, Docker, CI
- [ ] Requirements (25 stories)
- [ ] Design diagrams
- [ ] Prompt log
- [ ] Phase 1 report
