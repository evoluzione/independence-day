# GL-0030: Comments explain why, not what

- Status: active
- Source: [ADR-0400](../adr/0400-clean-code.md)

## Scope

All code.

## Guideline

Prefer code that needs no comment. Comment non-obvious intent, constraints, and the reason an unusual approach was chosen. Delete a comment rather than let it drift out of date.

## Rationale

A comment restating the code duplicates a fact that will diverge; a comment recording intent carries information the code cannot.

## Examples

Prefer:

```
// The provider rate-limits per account, not per key, so a shared bucket is required.
```

Avoid:

```
// increment the counter
counter += 1;
```
