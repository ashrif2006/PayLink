# Contributing

Phase 1 is a **four-person** deliverable. Treat `main` as always-working and
submittable; nothing lands there without a pull request.

## Branching

| Branch | Purpose |
| --- | --- |
| `main` | Protected. Always green, always submittable. |
| `feature/<area>-<short-name>` | One story or one task. e.g. `feature/wallet-hold-release` |
| `docs/<short-name>` | Documentation and diagrams only |

## Before you push

```bash
dotnet build --configuration Release
dotnet test  --configuration Release
```

Both must pass locally. CI runs the same commands and is required on `main`.

## Pull requests

1. One logical change per PR — one story, one diagram set, one fix.
2. Title: `area: what changed` (e.g. `payments: hold funds before biller call`).
3. Fill the PR template: what, why, how you verified it.
4. **One approval required**, and CI must be green.
5. Squash-merge only. Delete the branch after merging.

## Definition of done

A story is done when **all** of these are true:

- [ ] Code builds with no warnings
- [ ] Unit tests cover the new rule, and pass
- [ ] Acceptance criteria written in `docs/requirements.md`
- [ ] Behaviour verified manually as well (Docker or local run)
- [ ] Docs/diagram updated if the behaviour changed a document
- [ ] If AI was used, the prompt log has an entry with a verdict

## Money rules (non-negotiable)

- `decimal` for money. **Never** `double` or `float`.
- Every balance change writes a ledger entry.
- No code path may produce a negative `availableBalance`.
- A payment with an existing `idempotencyKey` returns the existing payment — it never charges twice.
- `Unknown` is never auto-refunded.

## Commit messages

```
type: short imperative summary
```

`feat` · `fix` · `docs` · `test` · `chore` · `refactor` · `ci`

## Roles

Each area has an owner for Phase 1 — see `docs/team-roles.md`. Ownership means the
person is accountable for the deliverable, not the only person allowed to touch it.
