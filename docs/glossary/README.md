# Glossary

*What do words mean here?*

Domain and architecture terms as they are used in independence-day — not as they
are used generally. The entries that matter most are the ones where this
repository's meaning differs from the obvious one.

## When to write one

Add a term when:

- it means something specific here, narrower or different from common usage;
- two people have used the same word for different things;
- the code and the business use different words for the same concept — record
  the business one and rename the code to match.

## Conventions

- **One file per domain area**, not per term: `NNNN-slug.md` (`0010-billing.md`,
  `0020-identity.md`), numbered in steps of 10.
- **Identifier**: `GLO-NNNN`, matching the filename.
- **One `## Term` section per term**, with a definition of one or two sentences
  that says what it *is*, never how it is implemented — an implementation
  changes and the definition should survive it.
- Include a **`Source: path/to/file.ext:line`** pointing at where the concept
  lives in the code. It is what stops the glossary drifting into fiction.
- If the same word means different things in two areas, define it in both and
  say so in each. That collision is worth documenting, not resolving by fiat.

Use this README as the index only. Start from
[`0000-template.md`](./0000-template.md).

## Index

| File | Domain area | Terms |
| --- | --- | --- |
| [0010-architecture.md](./0010-architecture.md) | Architecture | API contract, Bounded context, Component, Service, Shared code |
| [0020-data.md](./0020-data.md) | Data and consistency | Aggregate, Command, Event store, Eventual consistency, Outbox, Projection, Read model, Saga |
| [0050-delivery.md](./0050-delivery.md) | Delivery and operations | Trunk |

## Aree locali

| File | Area | Termini |
| --- | --- | --- |
| [GLO-9000](./9000-invasione.md) | L'invasione | Atomica, Campagna, Flotta, Forza, Ingaggio, Integrita', Livello, Minaccia, Ondata, Riserva, Tempo di avvicinamento |
