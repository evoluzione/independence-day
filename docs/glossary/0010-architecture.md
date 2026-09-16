# GLO-0010: Architecture

- Status: active

## API contract

**Definition.** The published, versioned promise a component makes about its interface: the shapes it accepts, the shapes it returns, and the guarantees attached to them.

- **Aliases / Acronyms**: Interface contract
- **Context**: Applies to any interface with a consumer outside its own deployment unit.
- **Related**: Contract test, Service
- **Source**: _add `path/to/file.ext:line` where this concept lives in the code._

## Bounded context

**Definition.** A boundary inside which a set of domain terms has one consistent meaning. The same word may legitimately mean something different in another context.

- **Aliases / Acronyms**: —
- **Context**: Applies to domain modelling and to deciding where a boundary belongs.
- **Related**: Ubiquitous language, Service, Aggregate
- **Source**: _add `path/to/file.ext:line` where this concept lives in the code._

## Component

**Definition.** The smallest named unit of the codebase that owns source code: a leaf of the namespace tree. Everything above it is a subdomain and holds no code, which is what makes a component's size, coupling, and owner answerable questions.

- **Aliases / Acronyms**: Leaf package, leaf namespace
- **Context**: Applies to code organisation, independently of how many things are deployed.
- **Related**: Module, Shared code, Afferent coupling
- **Source**: _add `path/to/file.ext:line` where this concept lives in the code._

## Service

**Definition.** An independently deployable unit that owns its data and is reached only through its published interface.

- **Aliases / Acronyms**: —
- **Context**: Applies to runtime topology and ownership, not to code organisation.
- **Related**: Module, API contract, Bounded context
- **Source**: _add `path/to/file.ext:line` where this concept lives in the code._

## Shared code

**Definition.** Code used by more than one component, held in a component of its own rather than in a parent namespace. Shared domain logic is business logic common to some components; shared infrastructure is operational and common to all of them. The two are kept apart.

- **Aliases / Acronyms**: Common code
- **Context**: Applies to interfaces, abstract classes, and utilities with more than one caller.
- **Related**: Component, Module
- **Source**: _add `path/to/file.ext:line` where this concept lives in the code._
