# Rules

*What is non-negotiable?*

A rule is a constraint with no acceptable exception, and something that can
check it. If a reviewer could reasonably wave a violation through, it is not a
rule — it is a [guideline](../guidelines/README.md).

## When to write one

Write a rule when **all** of these hold:

- it can be stated as one imperative sentence;
- a violation is always wrong, not usually wrong;
- you can name what checks it — CI, a linter, a permission, code review.

A rule with no enforcement is a wish. Write the enforcement down even when it is
"code review": naming it is what makes it someone's job.

## Conventions

- **Filename**: `NNNN-slug.md`, numbered in steps of 10.
- **Identifier**: `R-NNNN`, matching the filename. Cite it in review comments.
- **Status**: `enforced` (checked and blocking) · `advisory` (agreed, not yet
  automated). Advisory is a staging area, not a permanent home.
- Rules that exist because of an architectural choice carry a
  `Source: ADR-NNNN` line. When that ADR is superseded, revisit the rule.
- **One constraint per rule.** A rule covering three things cannot be relaxed
  one third at a time.

Start from [`0000-template.md`](./0000-template.md).

## Index

| ID | Rule | Status | Source |
| --- | --- | --- | --- |
| [R-0070](./0070-regression-test-for-every-fix.md) | Every bug fix ships with a regression test | enforced | ADR-0500 |
| [R-0080](./0080-timeouts-and-backoff.md) | Every outbound call has a timeout and bounded retries | enforced | ADR-0100 |
| [R-0090](./0090-no-cross-service-db.md) | No service reads another service's database | enforced | ADR-0100, ADR-0310 |
| [R-0100](./0100-service-owns-its-data.md) | A service owns its data | enforced | ADR-0100, ADR-0310 |
| [R-0130](./0130-events-are-immutable.md) | Recorded events are immutable | enforced | ADR-0320 |
| [R-0140](./0140-no-writes-through-read-models.md) | Read models are never written through | enforced | ADR-0330 |
| [R-0160](./0160-no-breaking-api-change-without-version.md) | No breaking change to a published interface without a version | enforced | ADR-0100, ADR-0120 |
| [R-0230](./0230-structured-logs-only.md) | Logs are structured, with stable keys | enforced | ADR-0700 |
| [R-0250](./0250-conventional-commits.md) | Commit messages follow Conventional Commits | enforced | ADR-0810 |
| [R-0290](./0290-no-direct-push-to-main.md) | No direct pushes to the default branch | enforced | ADR-0800 |
| [R-0300](./0300-outbox-for-cross-service-writes.md) | Cross-service effects are published through the outbox | enforced | ADR-0340 |
| [R-0310](./0310-no-source-in-non-leaf-namespace.md) | Source code lives only in leaf namespaces | enforced | ADR-0130 |
| [R-0320](./0320-approved-domains-only.md) | Only approved domains exist below the root namespace | enforced | ADR-0130 |
| [R-0330](./0330-declared-component-dependencies.md) | Forbidden dependencies between components stay forbidden | enforced | ADR-0130 |
| [R-0340](./0340-namespace-matches-deployment-unit.md) | Every deployment unit owns one root namespace | enforced | ADR-0100 |

## Regole locali

Le regole specifiche di questo dominio, fuori dal catalogo. Non compaiono nell'indice qui sopra, che
specframe rigenera dalle decisioni del catalogo.

| ID | Regola | Fonte |
| --- | --- | --- |
| [R-9000](./9000-cross-service-communication.md) | L'unica interfaccia fra due servizi e' il bus | ADR-9000, ADR-9020 |
| [R-9010](./9010-aggregates-do-not-throw.md) | Una regola di dominio si rifiuta con un evento, non con un'eccezione | ADR-9020, ADR-9030 |
| [R-9020](./9020-projections-never-rethrow.md) | Una proiezione registra l'errore e prosegue | ADR-9010 |
| [R-9030](./9030-domain-events-carry-their-aggregate.md) | Un evento di dominio porta il nome del proprio aggregato | ADR-9000 |
| [R-9040](./9040-commit-id-is-always-new.md) | Il commitId di un comando e' sempre nuovo | ADR-9020 |
| [R-9050](./9050-apply-is-unconditional.md) | Un Apply non contiene condizioni | R-9010 |
| [R-9060](./9060-saga-does-not-decide.md) | Una saga non decide, e non ospita dominio | ADR-9020, ADR-9070 |
| [R-9070](./9070-compensation-is-confirmed.md) | Una compensazione va confermata prima di chiudere | ADR-9070, ADR-9080 |
