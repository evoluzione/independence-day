# GL-0290: The test-driven loop

- Status: active
- Source: [ADR-0500](../adr/0500-test-driven-development.md)

## Scope

Feature and fix development.

## Guideline

Red, green, refactor, in small steps. Write the smallest failing test that expresses the next behaviour, make it pass plainly, then improve the design while it stays green. Tests name behaviour, not implementation, so a refactor does not rewrite them.

## Rationale

The discipline's real output is design pressure: code that is hard to test first is usually code with too many dependencies.
