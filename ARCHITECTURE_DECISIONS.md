# DMBServerHelper Architecture Decisions

## Purpose

Record durable architecture decisions that AI assistants and maintainers must preserve unless a change request explicitly supersedes them.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBServerHelper`
- Project folder: `DMBServerHelper`
- Project role: core server infrastructure package for the PageBuilder ecosystem.
- Publication host: `labs_idemobi_com`

## Decisions

### Keep helper behavior stable

`DMBServerHelper` is consumed by multiple packages. Prefer backward-compatible additive changes for public helpers.

### Keep configuration lifecycle centralized

Configuration loading belongs in `GenericConfiguration<T>` and package-level defaults belong in `ServerHelperConfiguration`.

New configuration lifecycle behavior must document when it runs: before file loading, during file loading, after file loading, or during application middleware registration.

### Keep global registries deterministic

Cookie definitions, session definitions, localizer instances, and API documentation assembly registration use shared registries.

Do not change registration keys, insertion order, or replacement behavior without documenting downstream impact.

### Treat cookies and sessions as public contracts

Cookie and session names, default values, groups, duration, security flags, serialization formats, and parsing behavior are externally observable.

Changing them requires documentation updates and compatibility review.

### Keep localization composition explicit

`CombinedStringLocalizer` resolves registered resources in a defined order and has debug fallback behavior.

Do not hide lookup order or fallback changes behind generic wording.

### Keep examples outside the package

Example pages, tutorials, diagrams, and explanatory pages are published through `labs_idemobi_com` when requested.

The package should not embed documentation website pages directly.
