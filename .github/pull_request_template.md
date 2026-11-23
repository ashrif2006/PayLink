## What does this change?

<!-- One or two sentences, in the imperative. -->

## Why?

<!-- Link the story: US-xx from docs/requirements.md, or the problem it fixes. -->

Closes #

## How was it verified?

- [ ] `dotnet build --configuration Release` passes
- [ ] `dotnet test --configuration Release` passes
- [ ] Tried it by hand (local run or `docker compose up`)

## Checklist

- [ ] Money uses `decimal`, not `double`
- [ ] No negative balances possible on any path
- [ ] Idempotent: repeating the request does not charge twice
- [ ] `Unknown` is not auto-refunded
- [ ] Acceptance criteria in `docs/requirements.md` match the behaviour
- [ ] Prompt log updated if AI produced part of this change
- [ ] Diagram updated if this changes a documented flow
