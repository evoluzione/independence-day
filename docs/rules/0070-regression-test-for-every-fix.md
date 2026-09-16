# R-0070: Every bug fix ships with a regression test

- Status: enforced
- Source: [ADR-0500](../adr/0500-test-driven-development.md)

## Rule

A fix is incomplete without a test that fails before the change and passes after it.

## Why

Without one there is no evidence the bug is understood, and nothing stops it returning silently in a later refactor.

## Enforcement

Code review; the PR states which test covers the regression.
