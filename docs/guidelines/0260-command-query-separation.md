# GL-0260: Command and query separation

- Status: active
- Source: [ADR-0330](../adr/0330-cqrs.md)

## Scope

Application layer.

## Guideline

Commands change state and return nothing but an acknowledgement; queries return data and change nothing. Model each read path for its consumer rather than reusing the write model. Treat query results as eventually consistent and design the interface to say so.

## Rationale

Separating the paths lets each be shaped and scaled for its own access pattern; the cost is that the read side lags, which the interface has to admit rather than hide.
