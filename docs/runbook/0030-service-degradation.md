# RB-0030: Diagnose a degraded or unavailable service

- Source: [ADR-0100](../adr/0100-architecture-style.md)

## When to use

A service is failing, slow, or reporting elevated errors.

## Prerequisites

- Read access to logs, metrics, and traces for the affected service.
- The service's dependency list and its owner.

## Steps

1. Establish the blast radius: which callers are affected, and since when.
2. Check whether the service is failing itself or propagating a dependency failure — follow the trace, not the alert.
3. Look for a recent deploy or configuration change as the first suspect.
4. If a dependency is at fault, confirm the caller degrades gracefully (timeout, fallback, shed load) rather than queueing indefinitely.
5. Mitigate first (roll back, scale, disable the failing path), then diagnose the root cause.

## Verification

Error rate and latency return to baseline for every affected caller, not just the service itself.

## Rollback

Roll back the most recent change to this service; if that does not help, escalate to the owner of the failing dependency.

<!-- Replace the placeholder commands above with the real ones for this repository. -->
