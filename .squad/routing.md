# Squad Routing

Determines which agent handles which work.

## Primary Routing

| Work Pattern | Primary | Secondary |
|--------------|---------|-----------|
| Architecture, Roslyn versioning, decisions, code review | Keaton (Lead) | — |
| Generator templates, conversions, code emission | Fenster (Backend) | Keaton (review) |
| Analyzer diagnostics, CodeFixers, validation rules | Dallas (Frontend) | Keaton (review) |
| Snapshot tests, AnalyzerTests, consumer validation, TFM coverage | Hockney (Tester) | Keaton (gating) |
| Session logging, memory, decision merging | Scribe | — |
| Work queue monitoring, issue triage, board management | Ralph (Monitor) | — |

## Task Routing Examples

- **"Add a new conversion type"** → Fenster (Backend), Hockney writes tests in parallel
- **"Fix analyzer diagnostic for invalid construction"** → Dallas (Frontend), Keaton reviews
- **"Implement CodeFixer for VOG###"** → Dallas (Frontend)
- **"Test on net461/net8.0/net9.0"** → Hockney (Tester)
- **"Snapshot test update needed"** → Hockney (Tester), Fenster+Dallas verify output
- **"Review PR, approve or reject"** → Keaton (Lead)
- **"GitHub issue triage"** → Keaton (Lead)

## Anticipatory Work

When primary agent starts work:
- **Fenster writes generator code** → Hockney writes snapshot tests in parallel (from spec)
- **Dallas writes analyzer code** → Hockney writes AnalyzerTests in parallel
- **Keaton decides architecture** → All agents notified via decisions.md

## Multi-Agent Requests

- **"Team, add feature X"** → Keaton (architecture/decision), Fenster (code), Dallas (analyzer/fixer), Hockney (tests) — parallel fan-out
- **"Validate against all Roslyn versions"** → Hockney + Keaton (packaging validation)
