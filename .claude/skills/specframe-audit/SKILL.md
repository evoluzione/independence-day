---
name: specframe-audit
description: Auto-trigger when asked whether the docs are compliant: judge every existing document against its section gate. Reviews the standing log, not a diff.
---

# specframe-audit

Read every document under `docs/` and judge each one against the gate its own section publishes. Not a diff review — `specframe-conform` does that, and it can only see what a change touches. This reads the log that is already there, which is where a document that should never have been written goes on sitting.

Report first, always. Nothing is moved or removed until the user has seen the verdicts and said which ones to act on.

Run these as `specframe <args>`. If the shell reports the command is not found,
this repository was set up with `npx specframe` and never installed it — use
`npx --yes specframe <args>` instead, same behaviour, no install required. Never
substitute writing the file yourself for a command that fails to run.

## Trigger

Invoke when:
- The user asks whether the docs are compliant, tidy, or worth trusting.
- ADRs have been accumulating from agent sessions and nobody has read them since.
- Before adopting this repository's log as the source of truth for a new team.

## The gates

Each section README states its own test, and those are the criteria — not your judgement of what a good document looks like. Read them, do not recall them:

- `docs/adr/README.md` — the three-part ADR gate, all of which must hold.
- `docs/rules/README.md` — one imperative sentence, always wrong to violate, and something that checks it.
- `docs/guidelines/README.md`, `docs/runbook/README.md`, `docs/glossary/README.md` — their any-of triggers.

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

## Do

1. Read the five section READMEs first and quote each gate into your working notes. If a README has been rewritten by the user, the user's version wins — it is their log.
2. Run `specframe review --json` to learn which ADRs are catalog decisions with reserved numbers. Those are architectural by construction: do not audit them against the gate. Audit the rest — the `9000+` band, and any file whose number the manifest does not know about.
3. Read every document in every section. For each one report a single row: `path | verdict | which condition failed | where it belongs instead`.
   - `pass` — it meets its section's gate.
   - `misfiled` — it is a real, useful document in the wrong section. Name the right one.
   - `unwarranted` — it does not belong anywhere. Say which gate condition failed.
   - `unverifiable` — you cannot tell without asking. Say what you would need. Never guess a verdict.
4. Group the report by verdict, `unwarranted` first, and stop. Do not act yet.
5. When the user names the documents to act on, act on those only:
   - `misfiled` → write the document in its proper section (delegate to `doc-writer`), carrying a `Source: ADR-NNNN` line when it derives from a decision, then withdraw the original.
   - `unwarranted`, local band → `specframe adr rm <number>`. It removes the file, the index row and the manifest entry in one step, and the number is never reissued.
   - `unwarranted`, catalog ADR → never remove it. `specframe dismiss <id>` if the decision can never apply here, `specframe revise <id>` if the answer changed.
   - `unwarranted`, written by hand at a number the manifest never allocated → remove the file and its index row yourself, and say so explicitly in the summary: specframe did not allocate it and cannot account for it.
   - A rule or guideline that fails its gate → say so and let the user decide. Demoting a rule to a guideline is a change in what the repository enforces, not housekeeping.
6. Report what was done, per document, and what was left alone.

## Do not

- Do not remove or rewrite anything before the user has picked from the report. An audit that edits as it reads is not an audit.
- Do not audit a catalog ADR against the ADR gate. It was reserved by the catalog because it is architectural; a `pass` there is meaningless and a `fail` is wrong.
- Do not apply your own standard of quality. A thin document that clears its section's gate is a pass, and a beautiful one that does not is not.
- Do not mark `unwarranted` because a document is short, stale, or badly written. Those are reasons to improve it, not to withdraw it.
- Do not touch a document authored by the user unless they name it. A log someone has been writing in by hand is theirs.
- Do not renumber anything, ever. Numbers appear in links, commits and agent output; withdrawing one spends it permanently.
