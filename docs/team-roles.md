# Team roles — Phase 1

The brief asks for ownership **and** for documenting who led each part. Ownership
means accountable for the deliverable, not the only person allowed to touch it:
everyone reviews the requirements and the diagrams before submission.

## Ownership

| | Person 1 | Person 2 | Person 3 | Person 4 |
| --- | --- | --- | --- | --- |
| **Role** | Requirements & Product Lead | Architecture & Design Lead | DevOps & Infrastructure Lead | Integration, QA & Documentation Lead |
| **Owns** | `docs/requirements.md` | `docs/design/` | `Dockerfile`, `docker-compose.yml`, `.github/` | `docs/prompt-log.md`, report, reviews |
| **Stories** | 7 — Accounts & Wallet | 6 — Billers & Payments | 6 — Fees, Fraud & Notifications | 6 — Admin, Support & Reconciliation |
| **Also accountable for** | Story format consistency across all 25 | Diagram consistency with requirements | Reproducible `docker compose up` on a clean machine | Real technical review, not file collection |

## Lead log

The doctor wants to know who led what and when. Fill one row per phase.

| Phase | Area | Led by | Dates | Notes |
| --- | --- | --- | --- | --- |
| Phase 1 | Requirements | Person 1 | 22–25 Nov | |
| Phase 1 | Architecture & design | Person 2 | 22–25 Nov | |
| Phase 1 | DevOps & CI | Person 3 | 22–25 Nov | |
| Phase 1 | QA, docs, submission | Person 4 | 22–25 Nov | |
| Phase 2 | Requirements | *rotate* | | |
| Phase 2 | Architecture & design | *rotate* | | |
| Phase 2 | DevOps & CI | *rotate* | | |
| Phase 2 | QA, docs, submission | *rotate* | | |

## Rotation for Phase 2

Roles rotate so nobody is graded on one area only. The person who led DevOps in
Phase 1 should not lead DevOps in Phase 2; whoever takes an area inherits its
documents and the previous lead's notes.

Suggested rotation (decide as a team and record it above):

| Person | Phase 1 | Phase 2 |
| --- | --- | --- |
| 1 | Requirements | Architecture |
| 2 | Architecture | DevOps |
| 3 | DevOps | QA & docs |
| 4 | QA & docs | Requirements |

## Working agreements

- One owner per task. Never "we will all do the diagrams".
- Review is real: a reviewer must be able to explain the change in one sentence.
- Blocked task → say so in the same day, in the repository (issue), not only in chat.
- Everything that matters ends up in the repository. Chat is not documentation.
