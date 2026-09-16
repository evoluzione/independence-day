# GL-0410: Persistence conventions

- Status: active
- Source: [ADR-0300](../adr/0300-persistence.md)

## Scope

Data access code.

## Guideline

Name the authoritative store for every piece of state, and derive the rest from it. Keep queries behind repositories named for the domain operation they serve. Set explicit transaction boundaries in the application layer, never in the domain. Index for the queries you actually run, and treat an unbounded query as a bug.

## Rationale

Repository boundaries keep the storage choice replaceable, and explicit transaction scope is what makes concurrent behaviour reviewable.
