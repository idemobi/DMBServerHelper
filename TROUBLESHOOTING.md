# DMBServerHelper Troubleshooting

## Purpose

Collect common issues and investigation paths for `DMBServerHelper`.

## Project-specific section

When copying this file to another PageBuilder ecosystem project, update this section first.

- Project name: `DMBServerHelper`
- Project folder: `DMBServerHelper`
- Project role: core server infrastructure package for the PageBuilder ecosystem.
- Publication host: `labs_idemobi_com`

## Configuration does not load

Check:

- the expected section name matches the concrete configuration type name,
- `NeedsConfigFileOrAppSettings()` returns the expected value,
- `{ConfigurationType}.json` or `{ConfigurationType}.{Environment}.json` exists when required,
- `ConfigIsOptional()` and `ConfigHotReload()` return the intended values,
- `Loaded` was not already set before the expected loading path ran.

## Generated URL is wrong

Check:

- `ServerHelperConfiguration.DomainName` is set before `AfterConfiguration` runs,
- `DomainComposite` parsed the input as expected,
- localhost values include the intended port,
- production domains include the expected subdomain or `www` prefix,
- route parameter values were encoded when using the dictionary overload.

## Localization fallback text appears

Check:

- the key exists in the expected resource family,
- resource injection happened before lookup,
- the expected localizer was injected into `CombinedStringLocalizer`,
- duplicate resources did not change lookup order,
- debug fallback behavior is not being mistaken for a production translation.

## Validation messages are not localized

Check:

- `CustomValidationAttributeAdapterProvider` is registered by the consuming web project,
- `BoolMustBeTrueAttributeAdapter` or `BoolMustBeFalseAttributeAdapter` is selected,
- data annotation localization resources contain the expected key,
- the attribute `ErrorMessage` matches the resource key convention.

## Session helper returns unexpected default values

Check:

- the typed session definition name was sanitized with `SpaceCleaner`,
- the session value exists in the current `HttpContext`,
- ASP.NET Core session is configured and available in the consuming web project,
- parsing succeeds for numeric and enum session definitions,
- nullable context behavior is expected by the caller.

## Cookie helper behavior mismatch

Check:

- the cookie definition is registered in `CookieGlobal.KDictionary`,
- the cookie name was sanitized with `SpaceCleaner`,
- SameSite, Secure, duration, and auto-renewal settings match the definition,
- the browser accepts the cookie,
- generated JavaScript cookie writes match the server-side cookie options,
- parsing succeeds for numeric and enum cookie definitions.

## Documentation registration is incomplete

Check:

- the configuration returns `true` from `ApiDescription()` when API documentation should include its assembly,
- `ApiDocumentationList.AddApiAssembly(...)` or `AddApiType(...)` was called,
- duplicate registrations are expected and harmless.

## Documentation page issues

When pages in `labs_idemobi_com` are wrong or inconsistent:

- read `EXAMPLES_AND_TUTORIALS_RULES.md`,
- use `CodeBlockBuilder` or `Html.CodeBlock(...)` for code examples,
- use `ActionItem` with `ButtonRender` for action links,
- use `DRAWIO_DIAGRAM_RULES.md` for editable diagrams,
- keep DocumentationViewer links targeting `DMBServerHelper`.
