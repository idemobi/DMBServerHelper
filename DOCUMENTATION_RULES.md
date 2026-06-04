# DMBServerHelper Documentation Rules

## Language

- Documentation must be written in English.
- XML documentation comments must be written in English.

## Target audience

- Primary: developers maintaining or integrating `DMBServerHelper`.
- Secondary: developers building server-side packages in the PageBuilder ecosystem.
- Tertiary: AI assistants consuming structured project rules and technical context.

Documentation must be useful without private chat context. A reader should understand how configuration loading works, how global registries are populated, how cookie/session helpers serialize values, how localization lookup is composed, and what constraints apply before reading the implementation.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBServerHelper`
- Primary API families: configuration helpers, server defaults, domain and URL helpers, localization composition, typed cookie definitions, typed session definitions, and API documentation registration.
- Important types to reference when relevant: `GenericConfiguration<T>`, `ServerHelperConfiguration`, `IServerConfig`, `ICombinedStringLocalizer`, `CombinedStringLocalizer`, `WebLocalizer`, `WebLocalizedViewLocationExpander`, `CookieDefinition`, `CookieGlobal`, typed cookie definitions, `SessionDefinition`, `SessionGlobal`, typed session definitions, `DomainComposite`, and `ApiDocumentationList`.
- Publication host: `labs_idemobi_com`
- Documentation generation strategy: DocumentationBuilder-first; AI prepares content, the developer executes generation.

## Strict C# XML documentation policy

- Always write XML HeaderDoc for:
  - public classes,
  - public interfaces,
  - public structs,
  - public methods,
  - public constructors,
  - public properties,
  - public fields,
  - public constants,
  - public events,
  - public delegates,
  - public enums,
  - public enum values,
  - public extension methods.
- Also write XML HeaderDoc for protected members when they are part of the inheritance contract or are expected to be overridden by derived configurations.
- Internal and private members do not require XML HeaderDoc unless they explain complex configuration, security, localization, cookie, or session behavior that would otherwise be difficult to maintain.
- XML documentation must use valid C# XML syntax.
- Prefer these tags:
  - `<summary>`
  - `<param>`
  - `<typeparam>`
  - `<returns>`
  - `<value>`
  - `<remarks>`
  - `<exception>`
  - `<see cref="..."/>`
  - `<seealso cref="..."/>`
- Use `<inheritdoc/>` only when the inherited documentation is accurate for the current member.

## XML documentation quality standard

XML documentation must explain the public contract, not repeat the member name.

For classes and interfaces, document:

- the role in server-side application composition,
- the configuration, cookie, session, localization, or utility responsibility,
- the relationship with important types such as `GenericConfiguration<T>`, `ServerHelperConfiguration`, `CookieDefinition`, `SessionDefinition`, `CombinedStringLocalizer`, or `ICombinedStringLocalizer`,
- lifecycle expectations, including whether the type is used directly, by static registries, by MVC validation, or by application startup.

For methods and constructors, document:

- what the member registers, loads, resolves, serializes, parses, or returns,
- every parameter and the expected format when relevant,
- returned values and fallback behavior,
- side effects such as adding configuration files, writing cookies, writing session state, registering localizers, or registering API documentation assemblies,
- validation rules and exceptions,
- whether `null`, empty strings, invalid formats, duplicate keys, repeated calls, or missing HTTP context values have special behavior.

For properties, fields, and constants, document:

- the meaning of the value,
- the default value when meaningful,
- whether consumers may set it directly,
- how it affects configuration loading, URL generation, cookies, sessions, localization, or documentation registration.

For enums and enum values, document:

- where the enum is used,
- how each value maps to behavior, storage, UI grouping, serialization kind, or fallback behavior.

## Project API documentation requirements

- Configuration APIs must identify file names, section names, lifecycle hooks, optional/hot reload behavior, and loaded-state behavior.
- Cookie APIs must document names, default values, security flags, SameSite behavior, duration units, auto-renewal, deletion, JavaScript generation, and raw HTML output.
- Session APIs must document names, default values, deletion behavior, parsing fallback, nullable HTTP context handling, and serialization format.
- Localization APIs must document lookup order, duplicate injection behavior, debug fallback behavior, and special sentinel values such as `__EMPTY__` and `__SPACE__`.
- Domain APIs must document accepted input formats, localhost handling, subdomain handling, and generated HTTPS URL format.
- Security-sensitive APIs must mention risks related to user-controlled values, cookies, sessions, URLs, data protection, or generated JavaScript.

## Examples in XML documentation

Use `<example>` when it materially improves understanding of:

- configuration startup,
- cookie and session declaration,
- localization resource injection,
- domain composition.

Examples must be short, realistic, and compile-oriented.

## Markdown documentation policy

- Follow PageBuilder markdown conventions in:
  - `../MARKDOWN_GUIDELINES.md`
- Keep this structure where applicable:
  1. Context
  2. Explanation
  3. Example
  4. Notes / constraints

## Draw.io diagrams for conceptual documentation

Information pages, instruction pages, concept pages, architecture pages, and configuration pages may use Draw.io diagrams when they clarify a real model or flow.

Draw.io diagrams must follow:

- `DRAWIO_DIAGRAM_RULES.md`

Do not use Draw.io diagrams in XML documentation comments. XML documentation may reference concepts that are diagrammed on pages, but the diagram artifact belongs to the website documentation layer.

## DocumentationBuilder-first rule

Documentation in this module must be authored with a DocumentationBuilder-first objective.

- Write docs so they can be extracted and rendered without manual rewrite.
- Keep headings deterministic and stable.
- Keep examples self-contained and realistically useful.
- Avoid implicit references to chat history or hidden context.
- Prefer stable type and member names that DocumentationBuilder can cross-reference.
- Use `<see cref="..."/>` and `<seealso cref="..."/>` for related PageBuilder types whenever it improves navigation.

## Separation from examples and tutorials

`EXAMPLES_AND_TUTORIALS_RULES.md` is not a general documentation rule source.

- Use this file for API documentation, XML HeaderDoc, README updates, reference pages, and DocumentationBuilder-ready documentation.
- Use `../MARKDOWN_GUIDELINES.md` for general Markdown formatting rules.
- Use `EXAMPLES_AND_TUTORIALS_RULES.md` only when the task explicitly creates or updates example pages, demo pages, information pages, instruction pages, concept pages, tutorials, or tutorial-like walkthroughs.
- Do not import example-page requirements into XML documentation or reference documentation unless the task also changes examples or tutorials.

### Target publication project

- `../labs_idemobi_com` from PageBuilder root.

### Execution responsibility

- AI prepares documentation content, structure, and metadata.
- The developer executes DocumentationBuilder.
- AI must not claim DocumentationBuilder execution unless it was actually run.

## Minimum update policy

If public configuration behavior, cookie behavior, session behavior, localization behavior, validation behavior, domain composition, or documentation registration behavior changes, update in the same change set:

- local `Source/README.md`,
- relevant XML docs,
- impacted guidance/examples when the task includes pages.

## Review checklist for documentation changes

- The documentation names the real ServerHelper concept, not a copied source project concept.
- All public and protected-contract API members touched by the change have valid XML documentation.
- Summaries are specific enough to help IntelliSense users choose the right API.
- Parameters, return values, generic parameters, exceptions, and side effects are documented where applicable.
- Examples reflect current code behavior and realistic server-side usage.
- Draw.io diagrams, when added, follow `DRAWIO_DIAGRAM_RULES.md`.
- DocumentationBuilder can extract the content without needing hidden context or manual rewrite.
