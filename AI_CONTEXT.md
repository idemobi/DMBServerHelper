# DMBServerHelper AI Context

## Purpose

This file gives AI assistants the minimum project context required to work safely in `DMBServerHelper`.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBServerHelper`
- Project folder: `DMBServerHelper`
- Project role: core server infrastructure package for the PageBuilder ecosystem.
- Publication host: `labs_idemobi_com`
- Primary documentation audience: maintainers of PageBuilder server-side packages and MVC/Razor applications.

## What this project is

`DMBServerHelper` is the shared server-side foundation layer used by multiple PageBuilder packages.

It provides:

- generic configuration loading and lifecycle hooks,
- core server helper configuration,
- URL and domain composition,
- API documentation assembly registration,
- combined localization lookup,
- strongly typed cookie definitions,
- strongly typed session definitions.

## What this project is not

This project is not:

- a visual component library,
- a form builder,
- a page layout builder,
- an ASP.NET middleware package,
- a product-specific feature package,
- a documentation website.

Visual examples and tutorial pages belong in `labs_idemobi_com` when requested.

## Main concepts

- `GenericConfiguration<T>` coordinates configuration loading for typed server configuration objects.
- `ServerHelperConfiguration` provides shared server defaults such as domain analysis, supported languages, and data protection persistence.
- `CookieDefinition` and typed cookie classes define strongly typed cookie metadata, read/write behavior, and raw form rendering hooks.
- `SessionDefinition` and typed session classes define strongly typed session metadata and read/write behavior.
- `CombinedStringLocalizer` aggregates multiple `IStringLocalizer` sources with deterministic lookup order.
- `WebLocalizer` exposes shared localizers for data annotations and internal package text.
- Validation attributes and adapters integrate boolean requirements with ASP.NET Core model validation.

## Change strategy

- Keep changes localized to the relevant feature family.
- Preserve public API names and behavior unless the request explicitly asks for a breaking change.
- Document public API behavior in XML comments when the code is touched.
- Update README and local rule files when project behavior or documentation strategy changes.

## Documentation strategy

- Use `DOCUMENTATION_RULES.md` for XML docs, README/reference docs, and DocumentationBuilder-ready documentation.
- Use `EXAMPLES_AND_TUTORIALS_RULES.md` only for pages, examples, tutorials, and walkthroughs.
- Use `DRAWIO_DIAGRAM_RULES.md` when diagrams clarify configuration loading, localization lookup, cookie definitions, session definitions, or API documentation registration.
- Keep all generated documentation in English unless the user explicitly requests another language for user-facing website content.
