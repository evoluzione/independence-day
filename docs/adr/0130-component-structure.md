# ADR-0130: Component structure

- Status: accepted
- Date: 2026-09-15
- Decision key: `component-structure` = `domain-leaf`

## Context

The namespace tree is the only structure a reader, a reviewer, and a tool can all see without running anything. Whether it expresses business domains or technical roles decides whether a feature lives in one place or crosses the whole tree — and whether "component" is defined precisely enough to be measured at all.

## Decision

**Domain namespaces, code only in leaves.** Name namespaces after business domains and subdomains, and keep all source code in leaf nodes. An intermediate node is a container: extending it means moving its code down into a leaf of its own, in the same change.

## Consequences

- "Component" gets a mechanical definition, so its size, its coupling, and its owner become answerable without a debate each time.
- A feature's change surface is one path in the tree — which is also what an agent needs in order to place a change correctly.
- Every extension of the tree forces a placement decision up front, which is friction exactly when someone would rather not think about it.

## Alternatives considered

- **Domain namespaces, no leaf constraint** — Keeps the domain-first tree with less ceremony, but gives up the ability to measure a component.
- **Technical layers at the top** — Lowest thinking cost per file, at the price of a structure that says nothing about the business and cannot be decomposed later.

## Documents this decision produced

- Rules: [R-0310](../rules/0310-no-source-in-non-leaf-namespace.md), [R-0320](../rules/0320-approved-domains-only.md), [R-0330](../rules/0330-declared-component-dependencies.md)
- Guidelines: [GL-0420](../guidelines/0420-component-naming.md)

<!-- To change this decision, run `specframe revise component-structure`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
