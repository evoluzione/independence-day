# GL-0270: Saga design

- Status: active
- Source: [ADR-0340](../adr/0340-distributed-transactions.md)

## Scope

Business processes spanning more than one service.

## Guideline

Break the process into steps that each have a compensating action, and make every step idempotent. Persist the saga's state so it can resume after a crash. Set an explicit timeout per step and decide up front what happens when it expires.

## Rationale

A saga trades atomicity for availability. Compensation and timeouts are what keep the intermediate states from becoming permanently stuck.
