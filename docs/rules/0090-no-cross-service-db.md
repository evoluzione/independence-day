# R-0090: No service reads another service's database

- Status: enforced
- Source: [ADR-0100](../adr/0100-architecture-style.md), [ADR-0310](../adr/0310-data-ownership.md)

## Rule

A service reaches another service only through its published interface. Direct connections to a database it does not own are forbidden, including read-only ones.

## Why

A shared schema is a shared deploy: the moment two services read the same tables, neither can migrate independently and the service boundary is decorative.

## Enforcement

Code review plus per-service database credentials that cannot reach other schemas.
