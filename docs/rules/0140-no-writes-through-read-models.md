# R-0140: Read models are never written through

- Status: enforced
- Source: [ADR-0330](../adr/0330-cqrs.md)

## Rule

State changes go through commands on the write model. Query-side stores are derived, disposable, and rebuildable from their source.

## Why

A write that lands only in a projection cannot be replayed, so the next rebuild silently deletes it.

## Enforcement

Code review; the query side runs with read-only credentials where the store allows it.
