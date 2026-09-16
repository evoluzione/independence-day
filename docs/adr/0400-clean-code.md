# ADR-0400: Clean Code practices

- Status: accepted
- Date: 2026-09-15
- Decision key: `clean-code` = `yes`

## Context

Readability standards are only worth anything if they are shared. Writing them down turns a matter of taste into a reviewable expectation.

## Decision

**Yes.** Adopt Clean Code practices as active guidelines: intention-revealing names, small single-purpose functions, comments that explain why, and shallow nesting.

## Consequences

- Review has a shared reference, so style feedback stops being a matter of opinion.
- Some changes cost more upfront, in extraction and renaming.
- Taken dogmatically it produces excessive fragmentation, so the guidelines stay guidelines rather than rules.

## Alternatives considered

- **No explicit standard** — Nothing to maintain, and nothing to point at in review.

## Documents this decision produced

- Guidelines: [GL-0010](../guidelines/0010-naming-conventions.md), [GL-0020](../guidelines/0020-small-single-purpose-functions.md), [GL-0030](../guidelines/0030-comments-explain-why.md), [GL-0040](../guidelines/0040-reduce-nesting.md), [GL-0110](../guidelines/0110-performance.md), [GL-0130](../guidelines/0130-ai-agent-changes.md)

<!-- To change this decision, run `specframe revise clean-code`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
