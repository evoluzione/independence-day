# RB-0050: Replay messages from the dead-letter queue

- Source: [ADR-0110](../adr/0110-inter-component-communication.md)

## When to use

Messages failed processing and accumulated in a dead-letter queue.

## Prerequisites

- Read and write access to both the dead-letter and the primary queue.
- Confirmation that the bug which caused the failures is deployed and fixed.

## Steps

1. Inspect a sample first: understand why they failed before moving any of them.
2. Confirm the consumer is idempotent — replay will redeliver messages that may have partially succeeded.
3. Verify the fix is live in the consumer that will handle the replay.
4. Replay in small batches, watching the failure rate between batches.
5. Stop immediately if messages return to the dead-letter queue, and re-diagnose.

## Verification

The dead-letter queue drains without refilling, and the downstream state reflects the replayed messages exactly once.

## Rollback

Pause the replay. Messages not yet moved stay in the dead-letter queue, which is durable — there is no data to recover.

<!-- Replace the placeholder commands above with the real ones for this repository. -->
