# GL-0040: Reduce nesting, watch complexity

- Status: active
- Source: [ADR-0400](../adr/0400-clean-code.md)

## Scope

All code.

## Guideline

Use guard clauses and early returns to keep the happy path at the outermost level. When a function grows several levels of nesting, extract the inner levels rather than reformatting them.

## Rationale

Nesting depth is what forces a reader to keep several conditions in mind at once, which is where misreadings come from.

## Examples

Prefer:

```
if (!user) return null;
if (!user.active) return null;
return charge(user);
```

Avoid:

```
if (user) {
  if (user.active) {
    return charge(user);
  }
}
```
