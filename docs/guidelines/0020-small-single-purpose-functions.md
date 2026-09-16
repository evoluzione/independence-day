# GL-0020: Small functions with a single purpose

- Status: active
- Source: [ADR-0400](../adr/0400-clean-code.md)

## Scope

All code.

## Guideline

A function does one thing at one level of abstraction. Keep the parameter list short, and avoid boolean parameters that switch behaviour — pass a value or split the function.

## Rationale

A function you can hold in your head is one you can test at its edges and change without reading its callers.

## Examples

Prefer:

```
render(user)
renderCompact(user)
```

Avoid:

```
render(user, { compact: true, escape: false, legacy: true })
```
