# R-0230: Logs are structured, with stable keys

- Status: enforced
- Source: [ADR-0700](../adr/0700-structured-logging.md)

## Rule

Emit machine-readable records with stable field names. Do not interpolate variables into free-text messages.

## Why

Interpolated text cannot be aggregated or alerted on; stable keys are what make a log store queryable during an incident.

## Enforcement

Shared logger is the only permitted sink; code review rejects direct writes to stdout.
