# R-0320: Only approved domains exist below the root namespace

- Status: enforced
- Source: [ADR-0130](../adr/0130-component-structure.md)

## Rule

The set of top-level domains is declared in one place and every source file resides under one of them. Adding a domain is a decision taken with the product owner, not a side effect of a merge.

## Why

Domains added by inertia, one package at a time, produce a tree that stops describing the business. A component that fits no domain is evidence that the domain model is wrong, not that the tree needs another folder.

## Enforcement

CI check asserting every source file resides in one of the declared domain packages.
