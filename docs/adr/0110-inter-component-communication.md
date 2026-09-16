# ADR-0110: Inter-component communication

- Status: accepted
- Date: 2026-09-15
- Decision key: `inter-component-comm` = `async-messaging`

## Context

With more than one deployment unit, the transport between them determines the failure modes: a synchronous chain fails together, while an asynchronous one stays available but converges late.

## Decision

**Asynchronous messaging.** Components communicate by publishing and consuming messages through a broker.

## Consequences

- A consumer being down stops nothing: the producer keeps working and the backlog drains later.
- All cross-component state becomes eventually consistent, and consumers must be idempotent.
- Debugging moves from a stack trace to correlated traces and queue inspection.

## Alternatives considered

- **Synchronous HTTP/REST** — Easiest to reason about, but couples the availability of every component in a call chain.
- **gRPC** — Strong typed contracts, but adds a schema toolchain and stays availability-coupled.
- **Hybrid** — Fits the two interaction shapes properly, but doubles the transport surface to operate.

## Documents this decision produced

- Guidelines: [GL-0200](../guidelines/0200-async-messaging.md)
- Runbooks: [RB-0050](../runbook/0050-replay-failed-messages.md)

<!-- To change this decision, run `specframe revise inter-component-comm`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
