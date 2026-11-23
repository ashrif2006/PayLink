# Changelog

All notable changes to this repository. Format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), versions follow
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

Phase milestones are also tagged: `phase-1`, `phase-2`.

## [Unreleased]

### Added — Phase 1 foundation

- Repository skeleton: `docs/`, `src/`, `tests/`, CI, Docker.
- `docs/requirements.md`: problem summary, stakeholders, 25 user stories with
  priorities, 6 measurable NFRs, 6 recorded ambiguities.
- Design sources (Mermaid): use case, domain class, pay sequence, refund sequence,
  payment state, ER diagram.
- `docs/prompt-log.md`: AI usage with verdicts, including two corrected answers.
- `Wallet` domain type with available/held balances and hold-release-capture rules,
  covered by 11 xUnit tests.
- `HealthController` at `/health` so the container has a probe.
- Multi-stage `Dockerfile` (non-root, port 8080) and `docker-compose.yml` with PostgreSQL.
- GitHub Actions CI: restore, build, test, and a Docker image build.
- Contribution workflow: protected `main`, PR template, CODEOWNERS, issue templates,
  Dependabot.

### Known gaps

- 22 user stories still need acceptance criteria.
- Prompt log needs two more entries.
- Phase 1 report PDF not yet produced.

## [0.1.0] - 2026-11-23

Initial scaffold committed.
