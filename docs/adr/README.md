# Architecture Decision Records

*What did we decide, and why?*

An ADR records one choice between real alternatives, at the moment it was made,
with the reasoning still attached. Its value is not the decision — that is
visible in the code — but the alternatives that were rejected and the reason
they were.

## When to write one

Write an ADR when **all** of these hold:

- there was more than one credible option;
- reversing the choice later would be expensive;
- someone reading the code in six months would ask "why is it like this?".

If any one of the three fails, there is no ADR. Where it goes instead:

| What it is | Where it goes |
| --- | --- |
| A default with room for judgement | a guideline |
| A constraint with no acceptable exception, and something checks it | a rule |
| A procedure with steps and a way to verify it worked | a runbook |
| A term that means something specific here | the glossary |
| One credible option only — the obvious way, or the only way | nowhere |
| Cheaply reversible inside the module it lives in | nowhere |

**Nowhere is a legitimate outcome, and the most common one.** Naming, file
layout, which helper to call, how a function is structured, a library used in one
place and swappable in an afternoon: those are code, not decisions. An ADR for
one of them costs more than it records — it dilutes the sections that matter
until an ADR stops meaning anything, which is worse than the gap it was meant to
fill.

If the choice constrains future code, the ADR is the right home; if it merely
describes today's style, it is a guideline.

## Conventions

- **Filename**: `NNNN-slug.md`, numbered in steps of 10.
- **Status**: `proposed` · `accepted` · `superseded` · `deprecated`.
- **Never edit a decision after it is accepted.** Write a new ADR that
  supersedes it, and set the old one's status with a link forward. The record of
  what you believed at the time is the whole point.
- Generated rules and guidelines link back with a `Source: ADR-NNNN` line. When
  an ADR is superseded, revisit everything that cites it.

Start from [`0000-template.md`](./0000-template.md).
Decisions not yet taken are listed in [`../DECISIONS.md`](../DECISIONS.md).

## Index

### Architecture

| ADR | Decision | Choice |
| --- | --- | --- |
| [ADR-0100](./0100-architecture-style.md) | Architecture style | Microservices |
| [ADR-0110](./0110-inter-component-communication.md) | Inter-component communication | Asynchronous messaging |
| [ADR-0120](./0120-api-style.md) | External API style | REST over HTTP |
| [ADR-0130](./0130-component-structure.md) | Component structure | Domain namespaces, code only in leaves |
| [ADR-0140](./0140-shared-code.md) | Shared code placement | A shared component of its own |

### Design and modelling

| ADR | Decision | Choice |
| --- | --- | --- |
| [ADR-0210](./0210-domain-driven-design.md) | Domain-driven design | Full DDD |
| [ADR-0220](./0220-design-patterns.md) | Design patterns | Yes, as vocabulary |
| [ADR-0230](./0230-dependency-injection.md) | Dependency injection | DI container |

### Data and consistency

| ADR | Decision | Choice |
| --- | --- | --- |
| [ADR-0300](./0300-persistence.md) | Primary persistence | Mixed (polyglot) |
| [ADR-0310](./0310-data-ownership.md) | Data ownership across services | Database per service |
| [ADR-0320](./0320-event-sourcing.md) | Event sourcing | Yes, events are the system of record |
| [ADR-0330](./0330-cqrs.md) | Command-query separation of models | Full CQRS |
| [ADR-0340](./0340-distributed-transactions.md) | Consistency across boundaries | Orchestrated saga |

### Code quality

| ADR | Decision | Choice |
| --- | --- | --- |
| [ADR-0400](./0400-clean-code.md) | Clean Code practices | Yes |
| [ADR-0410](./0410-solid-principles.md) | SOLID principles | Yes |

### Testing

| ADR | Decision | Choice |
| --- | --- | --- |
| [ADR-0500](./0500-test-driven-development.md) | Test-driven development | Pragmatic |
| [ADR-0540](./0540-mutation-testing.md) | Mutation testing | No |

### Security and compliance

| ADR | Decision | Choice |
| --- | --- | --- |
| [ADR-0650](./0650-compliance-regime.md) | Compliance regime | None specific |

### Observability

| ADR | Decision | Choice |
| --- | --- | --- |
| [ADR-0700](./0700-structured-logging.md) | Structured logging | Yes |
| [ADR-0730](./0730-service-level-objectives.md) | Service level objectives | No |

### Delivery

| ADR | Decision | Choice |
| --- | --- | --- |
| [ADR-0800](./0800-branching-strategy.md) | Branching strategy | Trunk-based |
| [ADR-0810](./0810-commit-convention.md) | Commit convention | Conventional Commits |

## Decisions outside the catalog

Decisions this repository has recorded that specframe's own catalog never asks
about — a project-specific choice like which payment provider to use. Recorded
with `specframe adr new <slug> --title "..."`, not by hand: that command
reserves a number the catalog will never allocate, so the two can never
collide.

| ADR | Title |
| --- | --- |
| [ADR-9000](./9000-service-boundaries.md) | Tre servizi e non un monolite modulare |
| [ADR-9010](./9010-event-store-per-service.md) | Un event store per servizio |
| [ADR-9020](./9020-saga-owns-the-coordination.md) | La saga e' l'unico coordinatore della difesa |
| [ADR-9030](./9030-defense-escalation-ladder.md) | La scala di escalation delle difese |
| [ADR-9040](./9040-battle-outcome-outside-the-aggregate.md) | L'esito della battaglia deciso fuori dall'aggregato |
| [ADR-9050](./9050-everything-runs-in-containers.md) | Tutto gira in container, compose e' l'unico comando d'avvio |
| [ADR-9060](./9060-deterministic-combat.md) | Combattimento deterministico a forze contabili |
| [ADR-9070](./9070-the-owner-decides.md) | Chi possiede la risorsa prende la decisione |
| [ADR-9080](./9080-failure-is-the-game.md) | Il guasto e' il gioco, ed e' deterministico |
