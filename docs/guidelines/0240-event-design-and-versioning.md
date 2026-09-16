# GL-0240: Event design and versioning

- Status: active
- Source: [ADR-0320](../adr/0320-event-sourcing.md)

## Scope

Domain events in the event store.

## Guideline

Name events in the past tense after what happened in the domain, not after the table that changed. Include everything a consumer needs to act without a callback. Never change a published event shape: add a new version and upcast old ones on read.

## Rationale

Events are permanent, so their schema is a forever-decision. Upcasting on read is what keeps a years-old event readable by today's code.

## Examples

Prefer:

```
InvoiceSettled { invoiceId, amount, settledAt, version: 2 }
```

Avoid:

```
InvoiceUpdated { id, changedFields }
```
