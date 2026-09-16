# RB-0040: Rebuild a projection from the event log

- Source: [ADR-0320](../adr/0320-event-sourcing.md)

## When to use

A read model is inconsistent, a projection has a bug, or a new projection needs backfilling.

## Prerequisites

- Read access to the event store and write access to the projection store.
- The projection's current checkpoint or position.
- An estimate of replay duration, from the event count.

## Steps

1. Stop the projection worker so it cannot write while you rebuild.
2. Build into a new store or table rather than truncating the live one.
3. Replay from the beginning of the stream, recording the position reached.
4. Compare the rebuilt projection against the live one on a sample before switching.
5. Point readers at the rebuilt store, then restart the worker from the recorded position.

## Verification

The rebuilt projection matches on the sampled entities and its checkpoint has caught up to the head of the stream.

## Rollback

Point readers back at the original store — it was never truncated — and restart the worker from its old checkpoint.

<!-- Replace the placeholder commands above with the real ones for this repository. -->
