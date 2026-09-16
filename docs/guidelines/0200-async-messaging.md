# GL-0200: Asynchronous messaging conventions

- Status: active
- Source: [ADR-0110](../adr/0110-inter-component-communication.md)

## Scope

Producers and consumers of messages.

## Guideline

Consumers are idempotent and tolerate redelivery and out-of-order arrival. Every message carries an identifier, a type, a version, and its trace context. Configure a dead-letter queue before going to production.

## Rationale

At-least-once delivery is the norm, not the exception. Idempotency is what turns a redelivery from a bug into a no-op.
