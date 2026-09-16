# R-0160: No breaking change to a published interface without a version

- Status: enforced
- Source: [ADR-0100](../adr/0100-architecture-style.md), [ADR-0120](../adr/0120-api-style.md)

## Rule

Removing a field, narrowing a type, or changing a meaning requires a new version served alongside the old one until consumers have migrated.

## Why

Consumers deploy on their own schedule. A breaking change without a version is an outage scheduled for someone else.

## Enforcement

Contract diff in CI plus code review.
