# Phase 1 report

The submitted PDF belongs here: **`phase-1-report.pdf`**.

It is assembled from the Markdown sources in this repository, so the PDF never
becomes the only copy of anything:

| Section | Source |
| --- | --- |
| Requirements | `docs/requirements.md` |
| Design + diagrams | `docs/design/*.mmd` (render, then export images) |
| AI use and corrections | `docs/prompt-log.md` |
| DevOps proof | screenshots of green CI, `docker compose up`, protected `main` |

Checklist before exporting:

- [ ] All six diagrams rendered from the `.mmd` sources and visually checked
- [ ] Every one of the 25 stories has acceptance criteria
- [ ] Prompt log has at least 5 entries, 2 of them corrected
- [ ] Screenshot of the green CI run on `main`
- [ ] Screenshot of `docker compose up` with the API answering `/health`
- [ ] Repository cloned fresh on another machine and run successfully
- [ ] `git tag phase-1` pushed

PDF filename in git: keep it exactly `phase-1-report.pdf`.
