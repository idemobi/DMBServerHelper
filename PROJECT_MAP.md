# DMBServerHelper Project Map

## Purpose

Map the structure of `DMBServerHelper` so AI assistants can find the right files quickly.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBServerHelper`
- Project folder: `DMBServerHelper`
- Project role: core server infrastructure package for the PageBuilder ecosystem.
- Publication host: `labs_idemobi_com`

## Root files

- `DMBServerHelper.csproj`: project file and package metadata.
- `README.md`: package overview and documentation entry point.
- `AGENTS.md`: local AI instructions.
- `AI_CONTEXT.md`: project context for AI assistants.
- `DOCUMENTATION_RULES.md`: XML and reference documentation rules.
- `EXAMPLES_AND_TUTORIALS_RULES.md`: website page, example, and tutorial rules.
- `DRAWIO_DIAGRAM_RULES.md`: editable Draw.io diagram rules.
- `DELIVERY_CHECKLIST.md`: pre-delivery checklist.
- `ARCHITECTURE_DECISIONS.md`: durable architecture decisions.
- `LOCALIZATION_NOMENCLATURE.md`: localization key rules.
- `LOCAL_DEVELOPMENT_RUNBOOK.md`: local workflow guide.
- `TROUBLESHOOTING.md`: common issue guide.
- `GLOSSARY.md`: common term definitions.

## Attributes

Folder: `Attributes`

- `BoolMustBeTrueAttribute.cs`: validation attribute requiring a boolean value to be `true`.
- `BoolMustBeFalseAttribute.cs`: validation attribute requiring a boolean value to be `false`.

## Configuration

Folder: `Configuration`

- `GenericConfiguration.cs`: generic configuration loading lifecycle and configuration file registration.
- `ServerHelperConfiguration.cs`: core server defaults, domain analysis, supported languages, data protection setup, and URL composition helpers.

## Cookies

Folder: `Cookies`

- `CookieDefinition.cs`: base metadata and read/write behavior for cookie definitions.
- `CookieGlobal.cs`: global registry and deletion helper for cookie definitions.
- `CookieDefinitionGroup.cs`: cookie grouping enum.
- `CookieDefinitionKind.cs`: cookie storage kind enum.
- `CookieBool.cs`, `CookieString.cs`, `CookieEnum.cs`, `CookieFloat.cs`, `CookieInt.cs`, `CookieLong.cs`, `CookieShort.cs`, `CookieUInt.cs`, `CookieUShort.cs`: typed cookie definitions.

## Facades

Folder: `Facades`

- `IServerConfig.cs`: configuration lifecycle contract.
- `ICombinedStringLocalizer.cs`: composed localization contract.

## Localizer

Folder: `Localizer`

- `CombinedStringLocalizer.cs`: aggregate localizer implementation.
- `WebLocalizer.cs`: shared localizer access point.
- `WebLocalizedViewLocationExpander.cs`: Razor view location expansion by culture.

## Models

Folder: `Models`

- `ApiDocumentationList.cs`: API documentation assembly registry.

## Resources

Folder: `Resources`

- `DataAnnotationLocalization.Designer.cs`: generated data annotation localization accessors.
- `InternalLocalization.Designer.cs`: generated internal localization accessors.

Do not edit generated designer files manually unless the generation workflow requires it.

## Sessions

Folder: `Sessions`

- `SessionDefinition.cs`: base metadata and read/write behavior for session definitions.
- `SessionGlobal.cs`: global registry and deletion helper for session definitions.
- `SessionDefinitionGroup.cs`: session grouping enum.
- `SessionDefinitionKind.cs`: session storage kind enum.
- `SessionBool.cs`, `SessionString.cs`, `SessionEnum.cs`, `SessionFloat.cs`, `SessionInt.cs`, `SessionLong.cs`, `SessionShort.cs`, `SessionUInt.cs`, `SessionUShort.cs`, `SessionSerializable.cs`: typed session definitions.

## Tools

Folder: `Tools`

- `DomainComposite.cs`: domain, subdomain, port, localhost, and HTTPS URL composition helper.

## Validators

Folder: `Validators`

- `BoolMustBeTrueAttributeAdapter.cs`: MVC adapter for `BoolMustBeTrueAttribute`.
- `BoolMustBeFalseAttributeAdapter.cs`: MVC adapter for `BoolMustBeFalseAttribute`.
- `CustomValidationAttributeAdapterProvider.cs`: adapter provider for custom validation attributes.

## Related projects

- `DMBServerWebHelper`: ASP.NET web services, middleware, request localization, static assets, and captcha helpers.
- `DMBPageBuilder`: low-level page and HTML builder package.
- `DMBBootstrapBuilder`: Bootstrap-oriented visual builder package.
- `DMBComponentBuilder`: reusable visual component package.
- `DMBFormBuilder`: form builder package.
- `labs_idemobi_com`: publication host for examples, tutorials, information pages, and diagrams.
