# DMBServerHelper Glossary

## Purpose

Define common terms used in `DMBServerHelper` documentation and AI instructions.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBServerHelper`
- Project folder: `DMBServerHelper`
- Project role: core server infrastructure package for the PageBuilder ecosystem.
- Publication host: `labs_idemobi_com`

## Terms

### Generic configuration

The `GenericConfiguration<T>` base class that coordinates typed configuration loading, lifecycle hooks, optional file loading, and loaded-state tracking.

### Server helper configuration

The `ServerHelperConfiguration` type that provides shared server defaults such as domain analysis, launch token behavior, supported languages, and data protection configuration.

### Typed cookie helper

A cookie abstraction binding a cookie name to metadata and a strongly typed value, such as `CookieString`, `CookieBool`, or `CookieEnum<T>`.

### Typed session helper

A session abstraction binding a session key to metadata and a strongly typed value, such as `SessionString`, `SessionInt`, or `SessionSerializable<T>`.

### Cookie registry

The `CookieGlobal.KDictionary` registry that stores cookie definitions by sanitized cookie name.

### Session registry

The `SessionGlobal.KDictionary` registry that stores session definitions by sanitized session name.

### Combined string localizer

The `CombinedStringLocalizer` implementation that resolves localized strings from multiple injected `IStringLocalizer` resources.

### Web localizer

The `WebLocalizer` static helper that exposes shared localizers for data annotation and internal package text.

### Domain composite

The `DomainComposite` parser that extracts domain, subdomain, port, localhost state, and canonical HTTPS website URL from a configured input.

### DocumentationViewer

The documentation browsing feature in `labs_idemobi_com` that displays generated API documentation for NuGet packages.

### DocumentationBuilder

The documentation generation process that extracts and renders API documentation. AI prepares content; the developer executes the generator.
