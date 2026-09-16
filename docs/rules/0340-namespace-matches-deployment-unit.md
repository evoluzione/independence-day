# R-0340: Every deployment unit owns one root namespace

- Status: enforced
- Source: [ADR-0100](../adr/0100-architecture-style.md)

## Rule

All code in a deployment unit lives under that unit's own root namespace, and no other unit uses it.

## Why

The namespace is the only place a boundary is visible while reading code. When it does not match what ships, the deployment unit becomes an unstructured container and moving code between units stops being mechanical.

## Enforcement

Per-unit CI check asserting its sources reside under its declared root package.
