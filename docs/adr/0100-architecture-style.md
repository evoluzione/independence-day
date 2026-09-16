# ADR-0100: Architecture style

- Status: accepted
- Date: 2026-09-15
- Decision key: `architecture-style` = `microservices`

## Context

The number and granularity of deployment units shapes every later decision: transaction scope, failure modes, testing strategy, and the operational cost of a release. It is the most expensive decision to reverse, so it is recorded first.

## Decision

**Microservices.** Split the system into fine-grained services, each owning its data and deployed independently.

## Consequences

- Teams deploy and scale independently, and a failure can be contained to one capability.
- Requires real investment in platform work: service discovery, tracing, contract testing, and deployment automation are no longer optional.
- Any workflow crossing services is eventually consistent and needs explicit compensation.

## Alternatives considered

- **Monolith** — Cheapest to build and operate, but nothing prevents the structure from eroding into a single tangle.
- **Modular monolith** — Requires enforcing boundaries that the runtime does not, which is discipline rather than architecture.
- **Service-based** — Most of the independence of microservices at a fraction of the operational cost, but boundaries are coarse and harder to move later.
- **Serverless / functions** — Removes infrastructure work at the cost of platform constraints leaking into application design.

## Documents this decision produced

- Rules: [R-0080](../rules/0080-timeouts-and-backoff.md), [R-0090](../rules/0090-no-cross-service-db.md), [R-0100](../rules/0100-service-owns-its-data.md), [R-0160](../rules/0160-no-breaking-api-change-without-version.md), [R-0340](../rules/0340-namespace-matches-deployment-unit.md)
- Guidelines: [GL-0160](../guidelines/0160-service-boundaries.md)
- Runbooks: [RB-0030](../runbook/0030-service-degradation.md)

<!-- To change this decision, run `specframe revise architecture-style`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
