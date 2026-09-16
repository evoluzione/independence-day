# ADR-0540: Mutation testing

- Status: accepted
- Date: 2026-09-15
- Decision key: `mutation-testing` = `no`

## Context

Coverage proves a line ran. Mutation testing proves something checked what it did, by introducing faults and seeing whether the suite notices.

## Decision

**No.** Do not run mutation testing; rely on review to judge assertion quality.

## Consequences

- No additional pipeline time or tooling.
- Tests that assert nothing meaningful are found only by a reader.

## Alternatives considered

- **Yes, on core logic** — The strongest available signal about test quality, and the most expensive to run.

<!-- To change this decision, run `specframe revise mutation-testing`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
