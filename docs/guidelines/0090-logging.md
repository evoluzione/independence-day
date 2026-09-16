# GL-0090: Logging conventions

- Status: active
- Source: [ADR-0700](../adr/0700-structured-logging.md)

## Scope

All code that emits diagnostics.

## Guideline

Log through the shared logger with stable keys (`request_id`, `correlation_id`, `user_id`). Use levels consistently: `debug` for development detail, `info` for state transitions, `warn` for recoverable anomalies, `error` for failed operations. Log the outcome of an operation once, at its boundary.

## Rationale

Consistent keys make logs queryable; logging once per operation is what keeps them readable under load.
