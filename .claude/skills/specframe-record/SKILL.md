---
name: specframe-record
description: Auto-trigger when a decision outside the catalog needs an ADR — a project-specific choice the guided pass never asked about.
---

# specframe-record

Auto-trigger when a decision passes the gate below and is **not** one of the questions the specframe catalog already asks — a project-specific choice like which payment provider to integrate, or which third-party service to depend on. For anything the catalog already covers, use `specframe-decide` instead: it carries the reserved number, the pre-written alternatives, and the derived rules.

Run these as `specframe <args>`. If the shell reports the command is not found,
this repository was set up with `npx specframe` and never installed it — use
`npx --yes specframe <args>` instead, same behaviour, no install required. Never
substitute writing the file yourself for a command that fails to run.

## The gate

Before recording an ADR, answer all three. An ADR is warranted only if every
answer is yes:

1. Were there **two or more credible options** — ones a competent team would
   argue about, not one real option and some bad ones?
2. Would **reversing it later be expensive** — does it shape code that does not
   exist yet, or does unwinding it reach past the module it lives in?
3. Would someone reading this code in six months **ask "why is it like this?"**
   and not find the answer in the code?

If any answer is no, do not record an ADR. Say which question failed, then route
it: a default with room for judgement is a guideline, a constraint with no
acceptable exception is a rule, a procedure is a runbook, a term belongs in the
glossary — and a reversible implementation detail is none of them, so **writing
nothing is the correct outcome.** Naming, file layout, which helper to call, a
library used in one place and swappable in an afternoon: that is code, not a
decision. An ADR for one of those costs more than it records — it dilutes the
log until an ADR stops meaning anything.

## Trigger

Invoke when **all** of these hold:
- `specframe review --json` does not list the decision — it is outside the catalog.
- All three gate questions above answer yes.
- The user has actually made the choice, or is asking to make it now. A choice still being explored is not a decision.

## Do not invoke when

- The choice is a name, a file location, a directory layout, or a module boundary inside one package.
- Two options were equivalent and either would have worked — picking one is code, not a decision.
- It is a formatting, style, or ergonomics preference.
- A library or helper is used in exactly one place and could be swapped in an afternoon.
- The change is a refactor with no consequence visible outside the module it touches.
- A guideline or rule under `docs/` already covers it. Update that instead.
- The decision is listed as open in `docs/DECISIONS.md`, or as dismissed. Those belong to `specframe-decide`.

## Do

1. State the gate answers first, in one line each, before touching a file. If any answer is no, stop: say which question failed and where it belongs instead — a guideline, a rule, or nowhere. Do not continue with the rest of this skill.
2. Confirm it is really outside the catalog: run `specframe review --json` (or `specframe explain <candidate-id> --json` if you have a guess at the id) first. If it *is* a catalog decision, stop and hand off to `specframe-decide`.
3. Decide the content with the user: Status (`proposed` unless they confirm adoption), Decision, Consequences, Alternatives considered — with the reason each lost. If you cannot name an alternative and why it lost, question 1 failed after all; go back to step 1.
4. Allocate the file and the number: `specframe adr new <slug> --title "<title>"`. Never pick a number yourself — this command reserves one outside the catalog's range so it can never collide with a decision a future specframe version adds.
5. Delegate to `doc-writer` to fill in Context, Decision, Consequences and Alternatives in the file `specframe adr new` created.
6. If the decision implies a constraint or a default, follow up with a matching rule or guideline in `docs/rules/` or `docs/guidelines/`, carrying a `Source: ADR-NNNN` line back to this ADR.

## Do not

- Do not record an ADR to be safe. An unwarranted ADR is not a harmless extra document: it dilutes the log, and the next reader stops trusting that an ADR marks something that matters.
- Do not invent a number. `specframe adr new` is the only thing that allocates one.
- Do not invent context — cite the source discussion or file paths.
- Do not invent an alternative to satisfy the gate. If the rejected options are not real, there is no decision here.
- Do not mark `accepted` without explicit confirmation.
- Do not use this for a decision the catalog already asks — that duplicates a reserved ADR under a different number.
