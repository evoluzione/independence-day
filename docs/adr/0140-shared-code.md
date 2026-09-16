# ADR-0140: Shared code placement

- Status: accepted
- Date: 2026-09-15
- Decision key: `shared-code` = `dedicated-component`

## Context

Interfaces, abstract classes, and utilities used by more than one component have to live somewhere. The instinctive answer is the nearest parent namespace — which is the one placement that makes the share invisible and the parent impossible to extract.

## Decision

**A shared component of its own.** Put shared code in its own leaf component, under a suffix reserved for that purpose and used for nothing else.

## Consequences

- The reserved suffix makes the share countable: what percentage of the codebase is shared, and across how many components.
- Shared code has an owner and a coupling budget like any other component.
- It is one more component for every domain that shares anything, and the suffix has to be defended in review.

## Alternatives considered

- **A versioned shared library** — The strongest boundary and independent upgrades, paid for with release overhead and version skew.
- **Do not share — duplicate** — Maximum independence between components, paid for with every fix applied several times over.

## Documents this decision produced

- Guidelines: [GL-0430](../guidelines/0430-shared-code-placement.md)

<!-- To change this decision, run `specframe revise shared-code`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
