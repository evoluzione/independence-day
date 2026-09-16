# ADR-0730: Service level objectives

- Status: accepted
- Date: 2026-09-15
- Decision key: `slo` = `no`

## Context

Without a stated target, reliability is negotiated per incident. An objective plus a budget makes the trade-off against feature work explicit.

## Decision

**No.** Do not define formal objectives; respond to incidents as they arise.

## Consequences

- No measurement or process overhead.
- Every reliability-versus-features decision is re-argued from scratch.

## Alternatives considered

- **Yes** — The clearest reliability decision-making tool; worthless unless the budget is actually respected.

<!-- To change this decision, run `specframe revise slo`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
