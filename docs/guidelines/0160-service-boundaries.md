# GL-0160: Drawing service boundaries

- Status: active
- Source: [ADR-0100](../adr/0100-architecture-style.md)

## Scope

Service decomposition.

## Guideline

A service boundary follows a business capability and its data. Prefer fewer, larger services over many chatty ones. If two services must be deployed together to ship a feature, they are one service.

## Rationale

Boundaries drawn along technical layers produce distributed coupling: all the operational cost of a network with none of the independence that justifies it.
