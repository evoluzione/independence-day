# ADR-0220: Design patterns

- Status: accepted
- Date: 2026-09-15
- Decision key: `design-patterns` = `yes`

## Context

Patterns are a naming convention for recurring structures. The decision is whether the team commits to that vocabulary, not whether the structures exist.

## Decision

**Yes, as vocabulary.** Use the classic pattern names when a structure genuinely matches one, and name it in the code. Do not design pattern-first.

## Consequences

- Review and onboarding get shorter: one word replaces a paragraph of explanation.
- Requires judgement about when a pattern is genuinely present rather than imposed.

## Alternatives considered

- **No, prefer plain structures** — Keeps code direct, loses a compact shared vocabulary.

## Documents this decision produced

- Guidelines: [GL-0070](../guidelines/0070-design-patterns-vocabulary.md)

<!-- To change this decision, run `specframe revise design-patterns`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
