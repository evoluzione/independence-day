# ADR-0120: External API style

- Status: accepted
- Date: 2026-09-15
- Decision key: `api-style` = `rest`

## Context

The external interface is the part of the system that is hardest to change, because its consumers deploy on their own schedule. Its style determines how additive change and deprecation work.

## Decision

**REST over HTTP.** Expose resource-oriented HTTP endpoints with conventional methods and status codes.

## Consequences

- Universally understood, cacheable at the HTTP layer, debuggable with ordinary tools.
- Clients often need several round trips, or endpoints grow bespoke shapes for specific screens.

## Alternatives considered

- **GraphQL** — Removes over-fetching, but hands query cost to the server and gives up HTTP caching.
- **gRPC** — Best performance and typing, worst reach for browser and third-party consumers.
- **Typed RPC (tRPC or equivalent)** — Fastest to build inside one stack, but not an option for external or polyglot consumers.

## Documents this decision produced

- Rules: [R-0160](../rules/0160-no-breaking-api-change-without-version.md)
- Guidelines: [GL-0170](../guidelines/0170-api-design-rest.md)

<!-- To change this decision, run `specframe revise api-style`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
