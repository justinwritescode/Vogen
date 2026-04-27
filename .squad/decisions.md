# Team Decisions

**Canonical decision ledger.** Append-only. Agents read this before starting work. New decisions written to `.squad/decisions/inbox/` by agents; Scribe merges them here.

## 2026-04-26: Team Initialization

**By:** Squad (Coordinator)

**What:** Initial Squad team cast for Vogen project.
- Keaton (Lead) — Architecture, Roslyn versioning, decisions
- Fenster (Backend) — Generator templates, conversions, code emission
- Dallas (Frontend) — Analyzer diagnostics, CodeFixer logic
- Hockney (Tester) — Snapshot tests, AnalyzerTests, consumer validation
- Scribe (Session Logger) — Memory, decisions, orchestration logs
- Ralph (Work Monitor) — Issue triage, work queue management

**Why:** Vogen is a source-generator project requiring deep Roslyn expertise, multi-framework testing, and careful architectural oversight. This team size and shape matches the project's scope and technical depth.

---

## End of decisions

New decisions will be merged here by Scribe from `.squad/decisions/inbox/`.
