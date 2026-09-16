# GL-0110: Performance work

- Status: active
- Source: [ADR-0400](../adr/0400-clean-code.md)

## Scope

All code.

## Guideline

Measure before optimising, and keep the measurement. Watch for N+1 access patterns, repeated queries in a loop, and allocations on hot paths. Introduce a cache only with a stated invalidation strategy.

## Rationale

Un-measured optimisation trades readability for an unverified gain, and a cache without invalidation trades a slow answer for a wrong one.
