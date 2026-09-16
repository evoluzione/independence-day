# GL-0250: Projections and rebuilds

- Status: active
- Source: [ADR-0320](../adr/0320-event-sourcing.md)

## Scope

Read models derived from the event log.

## Guideline

A projection is disposable: it can be dropped and rebuilt from the log at any time. Keep projection logic free of side effects and of calls to other services, and store an explicit checkpoint per projection.

## Rationale

Rebuildability is the property that makes event sourcing worth its cost. A projection with side effects cannot be replayed, which forfeits it.
