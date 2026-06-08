# DMBServerHelper

## Purpose

`DMBServerHelper` provides the shared server-side foundation for the PageBuilder ecosystem.

It centralizes reusable infrastructure for configuration loading, URL/domain composition, localization aggregation, strongly typed cookies, strongly typed sessions, and API documentation assembly registration.

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
- secret lookup, redaction, validation, and setup diagnostics,
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
- `IServerHelperLogger`
- `SecretManager`
- `SecretDefinition`
- `ISecretLogger`
- `DomainComposite`
- `ApiDocumentationList`

## Logging

`DMBServerHelper` routes its infrastructure diagnostics through `ServerHelperConfiguration.Logger`.

Host applications can replace the default console logger:

```csharp
ServerHelperConfiguration.UseLogger(myServerHelperLogger);
```

The default console logger writes informational messages to standard output and warnings/errors to standard error. Exception diagnostics stay concise by default and do not dump stack traces.

## Secret management

`DMBServerHelper` provides the generic secret consumption mechanism through `ServerHelperConfiguration.Config.Secrets`.

Feature packages declare their own logical secret keys:

```csharp
ServerHelperConfiguration.Config.Secrets.Require(new SecretDefinition
{
    Key = "DMB:Stripe:WebhookSecret",
    Owner = "DMBStripe",
    DisplayName = "Stripe webhook signing secret"
});
```

Then they consume values through the same manager:

```csharp
string webhookSecret = ServerHelperConfiguration.Config.Secrets.GetRequired("DMB:Stripe:WebhookSecret");
```

`DMBServerHelper` does not know package-specific keys. It only knows how to read values from the active ASP.NET Core configuration pipeline, redact values for diagnostics, validate registered requirements, and explain how to configure a missing secret for the selected store.

`SecretManager.RequiredSecrets` is exposed as a read-only collection. Package modules and host applications must register or replace required secrets through `SecretManager.Require(...)`.

Secret diagnostics are routed through `ISecretLogger`. The default secret logger delegates warnings to `ServerHelperConfiguration.Logger`; host applications can still provide a secret-specific logger:

```csharp
ServerHelperConfiguration.Config.Secrets.UseLogger(mySecretLogger);
```

Supported diagnostic stores are:

- `UserSecrets`
- `EnvironmentVariables`
- `AzureKeyVault`
- `Configuration`
- `External`

Configure only the strategy in `ServerHelperConfiguration`, never the secret values:

```json
{
  "ServerHelperConfiguration": {
    "Secrets": {
      "Environment": "PreProduction",
      "Store": "AzureKeyVault",
      "AzureKeyVaultUri": "https://example-vault.vault.azure.net/",
      "FailFast": false,
      "LogMissingSecrets": true
    }
  }
}
```

`PreProduction` and `Production` always fail closed when a required secret is missing. The configured `FailFast` flag can make local environments stricter, but it cannot disable fail-fast behavior in protected environments. Package modules should register missing secrets and let the central validation point decide whether startup must stop.

When `Secrets.Environment` is `Auto`, common host names such as `Development`, `Testing`, `Staging`, `PreProduction`, and `Production` are inferred. Empty or unknown host environment names are reported as `Unspecified` during validation. Production and preproduction hosts must configure `ServerHelperConfiguration:Secrets:Environment` explicitly instead of relying on custom host names.

After all package configuration calls have completed, validate all registered secrets from one central point:

```csharp
ServerHelperConfiguration.LoadCommonConfig(builder);
DMBServerEmailHelperConfiguration.LoadCommonConfig(builder);
DMBStripeConfiguration.LoadCommonConfig(builder);
DMBPennylaneConfiguration.LoadCommonConfig(builder);

ServerHelperConfiguration.ValidateRequiredSecrets(builder);
```

This final validation reports every registered missing secret in one pass. In `PreProduction` and `Production`, it throws when at least one required secret is missing.

The manager never logs secret values. Missing-secret diagnostics show setup instructions such as the environment variable name `DMB__Stripe__WebhookSecret` or the Azure Key Vault name `DMB--Stripe--WebhookSecret`.

## Secret rotation

`DMBServerHelper` supports secret rotation without timer or polling. Packages that keep long-lived clients register an `ISecretRotationHandler` with `ServerHelperConfiguration.RegisterSecretRotationHandler(...)`.

After the host configuration has changed, trigger rotation explicitly:

```csharp
ServerHelperConfiguration.RotateResolvedSecrets(builder);
```

This call reloads the secret manager from the current host configuration, validates every registered required secret, and then asks registered handlers to rebuild components that hold resolved secret values.

Hosts can also register a configuration reload callback:

```csharp
IDisposable registration = ServerHelperConfiguration.RegisterSecretReload(builder);
```

This reacts only when an ASP.NET Core configuration provider publishes a reload token. It does not poll secret stores. Providers such as environment variables generally require a process restart because they do not publish runtime reload events.

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
