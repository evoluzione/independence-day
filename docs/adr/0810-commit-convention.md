# ADR-0810: Commit convention

- Status: accepted
- Date: 2026-09-15
- Decision key: `commit-convention` = `conventional`

## Context

Commit messages are read by people during archaeology and by tools during release. A convention serves both.

## Decision

**Conventional Commits.** Use `type(scope): summary`, marking incompatible changes with `!` or a `BREAKING CHANGE:` footer.

## Consequences

- Changelogs and version bumps can be generated rather than curated.
- The history filters by area and by kind of change.
- One more thing to learn, and it needs linting to stay consistent.

## Alternatives considered

- **No fixed convention** — No friction; no automation.

## Documents this decision produced

- Rules: [R-0250](../rules/0250-conventional-commits.md)
- Guidelines: [GL-0120](../guidelines/0120-git-and-prs.md)

<!-- To change this decision, run `specframe revise commit-convention`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
