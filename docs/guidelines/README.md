# Guidelines

*How do we usually build this?*

A guideline is the default way to do something here. It carries judgement: a
reviewer can accept a deviation when there is a reason, and the reason is worth
hearing. That is exactly what separates it from a [rule](../rules/README.md).

## When to write one

Write a guideline when:

- the same feedback has come up in review more than twice;
- there is a default worth agreeing on, but exceptions are legitimate;
- a newcomer or an agent would otherwise have to infer the convention from
  reading existing code — and would infer it wrong.

## Conventions

- **Filename**: `NNNN-slug.md`, numbered in steps of 10.
- **Identifier**: `GL-NNNN`, matching the filename.
- **Status**: `active` · `deprecated`. Deprecate rather than delete, so code
  written under the old convention still explains itself.
- Show, do not only tell: a *prefer* and an *avoid* example is worth more than
  another paragraph, and is what an agent will actually pattern-match on.
- Guidelines that follow from an architectural choice carry a
  `Source: ADR-NNNN` line.
- If a guideline hardens into something with no acceptable exception, promote it
  to a rule and deprecate it here.

Start from [`0000-template.md`](./0000-template.md).

## Index

| ID | Guideline | Status | Source |
| --- | --- | --- | --- |
| [GL-0010](./0010-naming-conventions.md) | Naming conventions | active | ADR-0400 |
| [GL-0020](./0020-small-single-purpose-functions.md) | Small functions with a single purpose | active | ADR-0400 |
| [GL-0030](./0030-comments-explain-why.md) | Comments explain why, not what | active | ADR-0400 |
| [GL-0040](./0040-reduce-nesting.md) | Reduce nesting, watch complexity | active | ADR-0400 |
| [GL-0050](./0050-solid-principles.md) | SOLID principles applied | active | ADR-0410 |
| [GL-0070](./0070-design-patterns-vocabulary.md) | Design patterns as shared vocabulary | active | ADR-0220 |
| [GL-0090](./0090-logging.md) | Logging conventions | active | ADR-0700 |
| [GL-0110](./0110-performance.md) | Performance work | active | ADR-0400 |
| [GL-0120](./0120-git-and-prs.md) | Commits and pull requests | active | ADR-0810 |
| [GL-0130](./0130-ai-agent-changes.md) | AI agents modifying this repository | active | ADR-0400 |
| [GL-0160](./0160-service-boundaries.md) | Drawing service boundaries | active | ADR-0100 |
| [GL-0170](./0170-api-design-rest.md) | REST API conventions | active | ADR-0120 |
| [GL-0200](./0200-async-messaging.md) | Asynchronous messaging conventions | active | ADR-0110 |
| [GL-0210](./0210-ddd-tactical-patterns.md) | Tactical DDD patterns | active | ADR-0210 |
| [GL-0220](./0220-ubiquitous-language.md) | Ubiquitous language | active | ADR-0210 |
| [GL-0230](./0230-dependency-injection.md) | Dependency injection | active | ADR-0230 |
| [GL-0240](./0240-event-design-and-versioning.md) | Event design and versioning | active | ADR-0320 |
| [GL-0250](./0250-projection-rebuilds.md) | Projections and rebuilds | active | ADR-0320 |
| [GL-0260](./0260-command-query-separation.md) | Command and query separation | active | ADR-0330 |
| [GL-0270](./0270-saga-design.md) | Saga design | active | ADR-0340 |
| [GL-0290](./0290-tdd-loop.md) | The test-driven loop | active | ADR-0500 |
| [GL-0370](./0370-branching-workflow.md) | Branching workflow | active | ADR-0800 |
| [GL-0410](./0410-persistence-conventions.md) | Persistence conventions | active | ADR-0300 |
| [GL-0420](./0420-component-naming.md) | Components name what they do | active | ADR-0130 |
| [GL-0430](./0430-shared-code-placement.md) | Shared code is a component, not a parent node | active | ADR-0140 |

## Convenzioni locali

Fuori dal catalogo, quindi fuori dall'indice rigenerato qui sopra.

| ID | Convenzione | Fonte |
| --- | --- | --- |
| [GL-9000](./9000-context-project-layout.md) | Un contesto, cinque progetti | ADR-9000 |
| [GL-9010](./9010-domain-test-naming.md) | Un file per scenario, il nome dice il caso | ADR-9040 |
