# GL-0130: AI agents modifying this repository

- Status: active
- Source: [ADR-0400](../adr/0400-clean-code.md)

## Scope

Any change authored or assisted by an agent.

## Guideline

Prefer minimal, targeted diffs. Match the surrounding style rather than an idiomatic ideal. Do not add a dependency without justification, and do not make cosmetic refactors that were not requested. Before changing code: find the right extension point, find the related tests, and respect the boundaries in docs/adr/.

## Rationale

An agent can produce a large plausible diff quickly, which shifts the whole cost of the change onto review. Keeping diffs narrow keeps that cost proportional.
