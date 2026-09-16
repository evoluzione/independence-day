# GL-0370: Branching workflow

- Status: active
- Source: [ADR-0800](../adr/0800-branching-strategy.md)

## Scope

All contributions.

## Guideline

Integrate into the default branch at least daily; hold unfinished work behind a feature flag, never on a long-lived branch. Branches are short-lived and rebased or merged before they age. Feature flags carry unfinished work, not long-running branches.

## Rationale

Branch lifetime is the single biggest driver of merge pain; a flag makes integration continuous while the feature stays incomplete.
