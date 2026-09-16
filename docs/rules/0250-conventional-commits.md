# R-0250: Commit messages follow Conventional Commits

- Status: enforced
- Source: [ADR-0810](../adr/0810-commit-convention.md)

## Rule

Use `type(scope): summary`, with `!` or a `BREAKING CHANGE:` footer for incompatible changes.

## Why

A parseable history is what lets release notes, version bumps, and changelogs be generated instead of curated by hand.

## Enforcement

Commit-message lint in CI on the pull request.
