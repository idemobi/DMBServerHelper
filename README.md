# DMBServerHelper

## Purpose

`DMBServerHelper` provides the shared server-side foundation for the PageBuilder ecosystem.

It centralizes reusable infrastructure for configuration loading, URL/domain composition, localization aggregation, strongly typed cookies, strongly typed sessions, API documentation assembly registration, and custom validation adapters.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBServerHelper`
- Project folder: `DMBServerHelper`
- Project role: core server infrastructure package for the PageBuilder ecosystem.
- Primary consumers: server-side PageBuilder packages and MVC/Razor applications.
- Publication host: `labs_idemobi_com`

## Scope

This package includes:

- generic configuration bootstrapping,
- server helper configuration defaults,
- domain and URL composition helpers,
- combined localization lookup infrastructure,
- strongly typed cookie definitions,
- strongly typed session definitions,
- custom boolean validation attributes and MVC adapters,
- API documentation assembly registration helpers.

This package does not define visual components, form builders, page layout builders, or ASP.NET web middleware. Those responsibilities belong to related packages such as `DMBPageBuilder`, `DMBBootstrapBuilder`, `DMBFormBuilder`, and `DMBServerWebHelper`.

## Main entry points

- `GenericConfiguration<T>`
- `ServerHelperConfiguration`
- `IServerConfig`
- `ICombinedStringLocalizer`
- `CombinedStringLocalizer`
- `WebLocalizer`
- `WebLocalizedViewLocationExpander`
- `CookieDefinition` and typed cookie definitions
- `SessionDefinition` and typed session definitions
- `DomainComposite`
- `ApiDocumentationList`
- `BoolMustBeTrueAttribute`
- `BoolMustBeFalseAttribute`
- `CustomValidationAttributeAdapterProvider`

## Documentation strategy

Documentation must be written so it can be consumed by developers and AI assistants without private chat context.

Use the local rule files:

- [AGENTS.md](AGENTS.md)
- [AI_CONTEXT.md](AI_CONTEXT.md)
- [DOCUMENTATION_RULES.md](DOCUMENTATION_RULES.md)
- [EXAMPLES_AND_TUTORIALS_RULES.md](EXAMPLES_AND_TUTORIALS_RULES.md)
- [DRAWIO_DIAGRAM_RULES.md](DRAWIO_DIAGRAM_RULES.md)
- [PROJECT_MAP.md](PROJECT_MAP.md)
- [LOCALIZATION_NOMENCLATURE.md](LOCALIZATION_NOMENCLATURE.md)
- [DELIVERY_CHECKLIST.md](DELIVERY_CHECKLIST.md)

Documentation pages, examples, tutorials, and diagrams are published through `labs_idemobi_com` when applicable.

## Development constraints

- Keep public APIs backward compatible unless explicitly requested.
- Keep configuration loading and global registries deterministic.
- Document security-sensitive behavior such as cookies, sessions, data protection, validation messages, and localized lookup fallbacks.
- Do not run `dotnet build`, `dotnet test`, `dotnet restore`, or `dotnet format` unless explicitly requested.
