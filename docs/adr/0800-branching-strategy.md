# ADR-0800: Branching strategy

- Status: accepted
- Date: 2026-09-15
- Decision key: `branching` = `trunk-based`

## Context

How long work stays off the default branch determines integration cost and how quickly a change can reach production.

## Decision

**Trunk-based.** Integrate into the default branch at least daily through short-lived branches, keeping it releasable at every commit and holding unfinished behaviour behind flags.

## Consequences

- Merge conflicts stay small, and the branch is always releasable.
- Requires feature flags and a reliable test suite as prerequisites, not extras.

## Alternatives considered

- **GitHub flow** — Easiest to adopt; degrades exactly as review latency grows.
- **Git flow** — Necessary for versioned, shipped software; heavy for continuously deployed services.

## Documents this decision produced

- Rules: [R-0290](../rules/0290-no-direct-push-to-main.md)
- Guidelines: [GL-0370](../guidelines/0370-branching-workflow.md)

<!-- To change this decision, run `specframe revise branching`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
