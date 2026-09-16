# GL-0210: Tactical DDD patterns

- Status: active
- Source: [ADR-0210](../adr/0210-domain-driven-design.md)

## Scope

Domain model.

## Guideline

Model with entities, value objects, aggregates, repositories, and domain services. An aggregate is the consistency boundary: load it whole, change it through its root, and keep it small. Prefer value objects over primitives for domain concepts.

## Rationale

The aggregate boundary is the design decision that determines transaction scope and, later, service boundaries.

## Examples

Prefer:

```
order.addLine(item, Quantity.of(3))
```

Avoid:

```
orderRepository.updateLineQuantity(orderId, lineId, 3)
```
