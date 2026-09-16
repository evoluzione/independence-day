# Decisions

The decision backlog for independence-day. Everything below comes from the
specframe catalog: each entry is a choice this repository will eventually make,
whether or not it has been made yet.

An open decision is not a defect. It is a decision that has not been forced yet
— but it is one an agent will otherwise make silently, on your behalf, the first
time it writes code that depends on it.

**To record one:** run `specframe decide`, or write the ADR by hand using
`adr/0000-template.md` and the reserved number below, then tick it off here.

**Already implemented?** If this repository was built before the log existed,
most of the decisions below have answers sitting in the code. Run
`/specframe-bootstrap` with an agent that has it installed: it looks for the
evidence, records only what it can prove, cites `path:line`, and leaves the rest
open. Recording one by hand works the same way —
`specframe decide --set <id>=<value> --detected` marks the ADR as documenting an
existing implementation rather than a new choice.

**Will never apply here?** Not every decision belongs to every repository —
every frontend decision in a backend-only service, event sourcing in a plain
CRUD app. Where a gate question has a `none`-shaped option (`persistence: none`,
`ui-surface: none`), answer that instead: it retires the whole group at once
and produces a real ADR. For the cases no gate covers, dismiss the decision
directly: `specframe dismiss <id>` — optionally `--reason "..."`, and
`--group <name>` for a whole section in one call. A dismissal is a claim about
this repository's *shape*, not a way to clear the backlog: it leaves no ADR,
only the record below, and `specframe restore <id>` reopens it if that changes.

## Decisions taken

| Decision | Choice | ADR |
| --- | --- | --- |
| Architecture style | Microservices | [ADR-0100](adr/0100-architecture-style.md) |
| Inter-component communication | Asynchronous messaging | [ADR-0110](adr/0110-inter-component-communication.md) |
| External API style | REST over HTTP | [ADR-0120](adr/0120-api-style.md) |
| Component structure | Domain namespaces, code only in leaves | [ADR-0130](adr/0130-component-structure.md) |
| Shared code placement | A shared component of its own | [ADR-0140](adr/0140-shared-code.md) |
| Domain-driven design | Full DDD | [ADR-0210](adr/0210-domain-driven-design.md) |
| Design patterns | Yes, as vocabulary | [ADR-0220](adr/0220-design-patterns.md) |
| Dependency injection | DI container | [ADR-0230](adr/0230-dependency-injection.md) |
| Primary persistence | Mixed (polyglot) | [ADR-0300](adr/0300-persistence.md) |
| Data ownership across services | Database per service | [ADR-0310](adr/0310-data-ownership.md) |
| Event sourcing | Yes, events are the system of record | [ADR-0320](adr/0320-event-sourcing.md) |
| Command-query separation of models | Full CQRS | [ADR-0330](adr/0330-cqrs.md) |
| Consistency across boundaries | Orchestrated saga | [ADR-0340](adr/0340-distributed-transactions.md) |
| Clean Code practices | Yes | [ADR-0400](adr/0400-clean-code.md) |
| SOLID principles | Yes | [ADR-0410](adr/0410-solid-principles.md) |
| Test-driven development | Pragmatic | [ADR-0500](adr/0500-test-driven-development.md) |
| Mutation testing | No | [ADR-0540](adr/0540-mutation-testing.md) |
| Compliance regime | None specific | [ADR-0650](adr/0650-compliance-regime.md) |
| Structured logging | Yes | [ADR-0700](adr/0700-structured-logging.md) |
| Service level objectives | No | [ADR-0730](adr/0730-service-level-objectives.md) |
| Branching strategy | Trunk-based | [ADR-0800](adr/0800-branching-strategy.md) |
| Commit convention | Conventional Commits | [ADR-0810](adr/0810-commit-convention.md) |

## Open decisions

### Architecture

- [ ] **Structural governance** — `architecture-governance`, ADR-0150 reserved
  - How is structural erosion detected?
  - Options: `fitness-functions` _(recommended)_ · `review` · `none`
  - Why it matters: Decides whether the structure recorded above describes the repository, or only the day it was scaffolded.

### Design and modelling

- [ ] **Layering approach** — `layering`, ADR-0200 reserved
  - How is the code layered?
  - Options: `clean` _(recommended)_ · `hexagonal` · `onion` · `layered` · `none`
  - Why it matters: Decides whether business rules can be tested and changed without the framework around them.

- [ ] **Error handling strategy** — `error-handling`, ADR-0240 reserved
  - How are errors represented?
  - Options: `typed-errors` _(recommended)_ · `error-codes` · `exceptions`
  - Why it matters: Decides whether a caller can handle a failure or can only log it.

### Data and consistency

- [ ] **Schema migration policy** — `migrations`, ADR-0350 reserved
  - How are schema changes managed?
  - Options: `versioned-forward-only` _(recommended)_ · `up-down` · `none`
  - Why it matters: Decides whether a deploy can be rolled back after the schema has moved.

### Code quality

- [ ] **Complexity budget** — `complexity-budget`, ADR-0420 reserved
  - Do you enforce a complexity limit?
  - Options: `strict` · `moderate` _(recommended)_ · `none`
  - Why it matters: Turns "this function feels tangled" into a number CI can check.

- [ ] **Lint and format enforcement** — `lint-format`, ADR-0430 reserved
  - How are formatting and linting enforced?
  - Options: `ci-enforced` _(recommended)_ · `local-only` · `none`
  - Why it matters: Decides whether style is a machine's job or a reviewer's.

### Testing

- [ ] **Test distribution** — `test-pyramid`, ADR-0510 reserved
  - How are tests distributed across levels?
  - Options: `pyramid` _(recommended)_ · `trophy` · `e2e-heavy`
  - Why it matters: Decides how fast the suite is and how much of the real system it exercises.

- [ ] **Coverage gate** — `coverage-gate`, ADR-0520 reserved
  - Do you gate the build on test coverage?
  - Options: `high` · `moderate` _(recommended)_ · `none`
  - Why it matters: A ratchet against untested code landing quietly. It measures execution, not correctness.

- [ ] **Contract testing** — `contract-testing`, ADR-0530 reserved
  - Do you use consumer-driven contract tests?
  - Options: `yes` _(recommended)_ · `no`
  - Why it matters: Moves integration breakage from the consumer's runtime to the provider's build.

### Security and compliance

- [ ] **Secret management** — `secret-management`, ADR-0600 reserved
  - Where do secrets come from at runtime?
  - Options: `secret-manager` _(recommended)_ · `env-vars`
  - Why it matters: Decides whether rotating a credential needs a deploy.

- [ ] **Input validation** — `boundary-validation`, ADR-0610 reserved
  - How is external input validated?
  - Options: `schema` _(recommended)_ · `manual`
  - Why it matters: Decides whether the core can trust its inputs.

- [ ] **Authentication model** — `authn`, ADR-0620 reserved
  - How are callers authenticated?
  - Options: `oauth2-oidc` _(recommended)_ · `jwt` · `sessions` · `none`
  - Why it matters: Decides where identity comes from and how revocation works.

- [ ] **Personal data in logs** — `pii-logging`, ADR-0630 reserved
  - How strictly is personal data kept out of logs?
  - Options: `strict` _(recommended)_ · `advisory`
  - Why it matters: Logs are copied further, kept longer, and read more widely than the database they came from.

- [ ] **Dependency scanning** — `dependency-scanning`, ADR-0640 reserved
  - How do you handle vulnerable dependencies?
  - Options: `ci-blocking` _(recommended)_ · `advisory` · `none`
  - Why it matters: Most exploited vulnerabilities are already public and already fixed upstream.

### Observability

- [ ] **Metrics** — `metrics`, ADR-0710 reserved
  - Do you instrument metrics?
  - Options: `yes` _(recommended)_ · `no`
  - Why it matters: Decides whether you learn about degradation from a dashboard or from a user.

- [ ] **Distributed tracing** — `tracing`, ADR-0720 reserved
  - Do you use distributed tracing?
  - Options: `yes` _(recommended)_ · `no`
  - Why it matters: The only way to answer "where did the time go" across a call chain.

### Delivery

- [ ] **Review policy** — `pr-policy`, ADR-0820 reserved
  - What review is required before merge?
  - Options: `one-review` _(recommended)_ · `two-reviews` · `none`
  - Why it matters: Review is where context spreads. It is also the main source of delivery latency.

- [ ] **CI gates** — `ci-gates`, ADR-0830 reserved
  - What must pass before a change can merge?
  - Options: `full` _(recommended)_ · `minimal` · `none`
  - Why it matters: The pipeline is the only check that never gets tired or skipped.

- [ ] **Versioning and releases** — `versioning`, ADR-0840 reserved
  - How are versions and releases produced?
  - Options: `semver-automated` _(recommended)_ · `semver-manual` · `none`
  - Why it matters: Decides whether a consumer can tell from a version number that something will break.

- [ ] **Environments** — `environments`, ADR-0850 reserved
  - What environments does a change pass through?
  - Options: `dev-staging-prod` _(recommended)_ · `dev-prod` · `ephemeral-preview`
  - Why it matters: Decides where a change is verified before it reaches users.

## Decisions that do not apply

### User interface

| Decision | Why not | Since |
| --- | --- | --- |
| User interface surface | Non c'e' un'applicazione frontend: l'unica pagina e' la sala operativa, un file HTML statico servito dal servizio Terra. | 2026-09-15 |
| Rendering strategy | Non c'e' un'applicazione frontend: l'unica pagina e' la sala operativa, un file HTML statico servito dal servizio Terra. | 2026-09-15 |
| UI composition | Non c'e' un'applicazione frontend: l'unica pagina e' la sala operativa, un file HTML statico servito dal servizio Terra. | 2026-09-15 |
| Design system source | Non c'e' un'applicazione frontend: l'unica pagina e' la sala operativa, un file HTML statico servito dal servizio Terra. | 2026-09-15 |
| Client and server state | Non c'e' un'applicazione frontend: l'unica pagina e' la sala operativa, un file HTML statico servito dal servizio Terra. | 2026-09-15 |
| Styling strategy | Non c'e' un'applicazione frontend: l'unica pagina e' la sala operativa, un file HTML statico servito dal servizio Terra. | 2026-09-15 |
| Accessibility baseline | Non c'e' un'applicazione frontend: l'unica pagina e' la sala operativa, un file HTML statico servito dal servizio Terra. | 2026-09-15 |
| Internationalisation | Non c'e' un'applicazione frontend: l'unica pagina e' la sala operativa, un file HTML statico servito dal servizio Terra. | 2026-09-15 |
| User interface testing | Non c'e' un'applicazione frontend: l'unica pagina e' la sala operativa, un file HTML statico servito dal servizio Terra. | 2026-09-15 |
| Frontend performance budget | Non c'e' un'applicazione frontend: l'unica pagina e' la sala operativa, un file HTML statico servito dal servizio Terra. | 2026-09-15 |

---

<!-- This file is generated at scaffold time and refreshed by `specframe decide`.
     It is yours to edit: add decisions specific to this project that the
     catalog does not cover, using the same shape. -->
