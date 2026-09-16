# R-0290: No direct pushes to the default branch

- Status: enforced
- Source: [ADR-0800](../adr/0800-branching-strategy.md)

## Rule

The default branch accepts merges from pull requests only. Force-pushes and branch deletion are disabled.

## Why

It keeps the branch releasable at every commit and makes history auditable.

## Enforcement

Branch protection.
