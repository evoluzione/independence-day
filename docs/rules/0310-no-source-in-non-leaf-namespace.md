# R-0310: Source code lives only in leaf namespaces

- Status: enforced
- Source: [ADR-0130](../adr/0130-component-structure.md)

## Rule

A namespace with children holds no source files: it is a subdomain, a container. All code belongs to a leaf. Adding a child to a namespace that holds code means moving that code down into a leaf of its own, in the same change.

## Why

It is what makes "component" a definition instead of an opinion. Code stranded in an intermediate node belongs to no component, so no question about its size, coupling, or ownership has a single answer.

## Enforcement

CI check walking the source tree: a non-leaf node containing code fails the build.
