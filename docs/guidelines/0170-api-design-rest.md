# GL-0170: REST API conventions

- Status: active
- Source: [ADR-0120](../adr/0120-api-style.md)

## Scope

HTTP interfaces.

## Guideline

Resources are plural nouns; behaviour comes from the method, not the path. Use status codes for outcome and a consistent error body for detail. Paginate every collection from the start. Additive changes only within a version.

## Rationale

Predictable shape is most of an API's usability, and an unpaginated collection is an outage waiting for its first large customer.

## Examples

Prefer:

```
GET /invoices?status=overdue&limit=50
```

Avoid:

```
GET /getOverdueInvoices
```
