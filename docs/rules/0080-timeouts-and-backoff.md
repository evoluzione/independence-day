# R-0080: Every outbound call has a timeout and bounded retries

- Status: enforced
- Source: [ADR-0100](../adr/0100-architecture-style.md)

## Rule

Set an explicit timeout on every network call. Retries use bounded exponential backoff with jitter and a maximum attempt count.

## Why

A call without a timeout converts a slow dependency into an exhausted connection pool; unbounded retries convert a brief outage into a self-inflicted denial of service.

## Enforcement

Code review; a shared client wrapper carries the defaults.
