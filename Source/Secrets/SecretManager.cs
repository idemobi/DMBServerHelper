#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Provides centralized secret lookup, registration, redaction, validation, and setup diagnostics.
    /// </summary>
    /// <remarks>
    ///     This manager does not know module-specific secret keys. Feature packages register their own
    ///     <see cref="SecretDefinition" /> values, then use this manager to read values from the
    ///     configured ASP.NET Core configuration pipeline.
    /// </remarks>
    public sealed class SecretManager
    {
        #region Instance fields and properties

        [JsonIgnore] private IConfiguration? _configuration;

        [JsonIgnore] private string _hostEnvironmentName = string.Empty;

        [JsonIgnore] private ISecretLogger _logger = new ConsoleSecretLogger();

        private readonly List<SecretDefinition> _requiredSecrets = new List<SecretDefinition>();

        /// <summary>
        ///     Gets or sets the Azure Key Vault URI used only for diagnostics and setup guidance.
        /// </summary>
        public string AzureKeyVaultUri { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the configured usage environment.
        /// </summary>
        public SecretUsageEnvironment Environment { get; set; } = SecretUsageEnvironment.Auto;

        /// <summary>
        ///     Gets a value indicating whether missing required secrets must stop the application startup or configuration flow.
        /// </summary>
        /// <remarks>
        ///     This value is always <see langword="true" /> for <see cref="SecretUsageEnvironment.PreProduction" />
        ///     and <see cref="SecretUsageEnvironment.Production" />, even when <see cref="FailFast" /> is
        ///     <see langword="false" />.
        /// </remarks>
        public bool EffectiveFailFast => FailFast || IsProtectedRuntimeEnvironment();

        /// <summary>
        ///     Gets or sets a value indicating whether missing required secrets should throw during validation in local environments.
        /// </summary>
        /// <remarks>
        ///     <see cref="SecretUsageEnvironment.PreProduction" /> and <see cref="SecretUsageEnvironment.Production" />
        ///     always fail closed through <see cref="EffectiveFailFast" />.
        /// </summary>
        public bool FailFast { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether missing required secrets should be written to console diagnostics.
        /// </summary>
        public bool LogMissingSecrets { get; set; } = true;

        /// <summary>
        ///     Gets the secret definitions registered by package modules or host applications.
        /// </summary>
        /// <remarks>
        ///     The returned collection is read-only. Register or replace definitions through
        ///     <see cref="Require(SecretDefinition)" />. Items are detached copies so callers cannot mutate
        ///     the internal registry through returned <see cref="SecretDefinition" /> instances.
        /// </remarks>
        public IReadOnlyList<SecretDefinition> RequiredSecrets => _requiredSecrets
            .Select(secret => secret.Clone())
            .ToList()
            .AsReadOnly();

        /// <summary>
        ///     Gets or sets the configured secret storage strategy.
        /// </summary>
        public SecretStoreKind Store { get; set; } = SecretStoreKind.Auto;

        #endregion

        #region Instance methods

        /// <summary>
        ///     Builds a human-readable diagnostic for a missing secret.
        /// </summary>
        /// <param name="definition">The missing secret definition.</param>
        /// <returns>A setup message adapted to the configured secret store.</returns>
        public string BuildMissingSecretMessage(SecretDefinition definition)
        {
            SecretStoreKind store = ResolveStoreKind();
            SecretUsageEnvironment environment = ResolveUsageEnvironment();
            string owner = string.IsNullOrWhiteSpace(definition.Owner) ? "Unspecified" : definition.Owner.Trim();
            string displayName = string.IsNullOrWhiteSpace(definition.DisplayName) ? definition.Key : definition.DisplayName.Trim();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Missing secret: {definition.Key}");
            builder.AppendLine($"Requested by: {owner}");
            builder.AppendLine($"Display name: {displayName}");
            builder.AppendLine($"Environment: {environment}");
            builder.AppendLine($"Secret store: {store}");

            if (string.IsNullOrWhiteSpace(definition.Description) == false)
            {
                builder.AppendLine($"Description: {definition.Description.Trim()}");
            }

            builder.AppendLine();
            builder.AppendLine(BuildStoreInstruction(definition.Key, store, environment));

            if (string.IsNullOrWhiteSpace(definition.DocumentationUrl) == false)
            {
                builder.AppendLine();
                builder.AppendLine($"Documentation: {definition.DocumentationUrl.Trim()}");
            }

            return builder.ToString().TrimEnd();
        }

        /// <summary>
        ///     Configures the manager with the active ASP.NET Core configuration pipeline.
        /// </summary>
        /// <param name="configuration">The configuration used to resolve secret values.</param>
        /// <param name="hostEnvironmentName">The host environment name used when <see cref="Environment" /> is <see cref="SecretUsageEnvironment.Auto" />.</param>
        public void Configure(IConfiguration configuration, string? hostEnvironmentName)
        {
            _configuration = configuration;
            _hostEnvironmentName = hostEnvironmentName ?? string.Empty;
        }

        /// <summary>
        ///     Gets a secret value from the configured ASP.NET Core configuration pipeline.
        /// </summary>
        /// <param name="key">The logical secret key, for example <c>DMB:Stripe:SecretKey</c>.</param>
        /// <returns>The resolved value, or <see langword="null" /> when it is missing.</returns>
        public string? Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || _configuration == null)
            {
                return null;
            }

            string? value = _configuration[key];
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        /// <summary>
        ///     Gets a required secret value or throws with a setup message when the value is missing.
        /// </summary>
        /// <param name="key">The logical secret key.</param>
        /// <returns>The resolved secret value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the secret value is missing.</exception>
        public string GetRequired(string key)
        {
            string? value = Get(key);
            if (value != null)
            {
                return value;
            }

            SecretDefinition definition = FindDefinition(key) ?? new SecretDefinition
            {
                Key = key,
                DisplayName = key,
                Owner = "Unregistered consumer"
            };

            string message = BuildMissingSecretMessage(definition);
            if (LogMissingSecrets)
            {
                _logger.Warning(message);
            }

            throw new InvalidOperationException(message);
        }

        /// <summary>
        ///     Registers a required secret definition declared by a package module or host application.
        /// </summary>
        /// <param name="definition">The secret definition to register.</param>
        /// <returns>The current <see cref="SecretManager" /> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="definition" /> does not contain a key.</exception>
        public SecretManager Require(SecretDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            if (string.IsNullOrWhiteSpace(definition.Key))
            {
                throw new ArgumentException("A secret definition must provide a non-empty key.", nameof(definition));
            }

            int existingIndex = _requiredSecrets.FindIndex(secret =>
                string.Equals(secret.Key, definition.Key, StringComparison.OrdinalIgnoreCase));

            SecretDefinition copy = definition.Clone();
            if (existingIndex >= 0)
            {
                _requiredSecrets[existingIndex] = copy;
            }
            else
            {
                _requiredSecrets.Add(copy);
            }

            return this;
        }

        /// <summary>
        ///     Registers a required secret declared by a package module or host application.
        /// </summary>
        /// <param name="key">The logical secret key.</param>
        /// <param name="owner">The package, module, or host feature that requires the secret.</param>
        /// <param name="displayName">The human-readable display name shown in diagnostics.</param>
        /// <returns>The current <see cref="SecretManager" /> instance.</returns>
        public SecretManager Require(string key, string owner, string displayName)
        {
            return Require(new SecretDefinition
            {
                Key = key,
                Owner = owner,
                DisplayName = displayName
            });
        }

        /// <summary>
        ///     Masks a secret value for logs, diagnostics, pages, and MCP responses.
        /// </summary>
        /// <param name="value">The secret value to mask.</param>
        /// <returns>A redacted placeholder that never exposes the original value.</returns>
        public string Redact(string? value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : "********";
        }

        /// <summary>
        ///     Sets the logger used for missing-secret diagnostics.
        /// </summary>
        /// <param name="logger">The logger that receives secret management warnings.</param>
        /// <returns>The current <see cref="SecretManager" /> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger" /> is <see langword="null" />.</exception>
        public SecretManager UseLogger(ISecretLogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
            return this;
        }

        /// <summary>
        ///     Validates registered required secrets and writes store-specific setup diagnostics when values are missing.
        /// </summary>
        /// <returns>The validation result containing all missing required secrets.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="EffectiveFailFast" /> is enabled and at least one required secret is missing.</exception>
        public SecretValidationResult ValidateRequiredSecrets()
        {
            ThrowIfUsageEnvironmentIsUnspecified();

            SecretValidationResult result = new SecretValidationResult();

            foreach (SecretDefinition definition in _requiredSecrets
                         .Where(definition => definition.Required)
                         .Where(definition => string.IsNullOrWhiteSpace(definition.Key) == false))
            {
                if (Get(definition.Key) != null)
                {
                    continue;
                }

                string message = BuildMissingSecretMessage(definition);
                result.Issues.Add(new SecretValidationIssue
                {
                    Definition = definition.Clone(),
                    Message = message
                });

                if (LogMissingSecrets)
                {
                    _logger.Warning(message);
                }
            }

            if (EffectiveFailFast && result.Success == false)
            {
                throw new InvalidOperationException(string.Join(System.Environment.NewLine + System.Environment.NewLine, result.Issues.Select(issue => issue.Message)));
            }

            return result;
        }

        private static string BuildAzureKeyVaultSecretName(string key)
        {
            return key.Replace(":", "--");
        }

        private static string BuildEnvironmentVariableName(string key)
        {
            return key.Replace(":", "__");
        }

        private SecretDefinition? FindDefinition(string key)
        {
            return _requiredSecrets.FirstOrDefault(secret =>
                string.Equals(secret.Key, key, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsProtectedRuntimeEnvironment()
        {
            SecretUsageEnvironment environment = ResolveUsageEnvironment();
            return environment is SecretUsageEnvironment.PreProduction or SecretUsageEnvironment.Production;
        }

        private string BuildStoreInstruction(string key, SecretStoreKind store, SecretUsageEnvironment environment)
        {
            if (environment == SecretUsageEnvironment.Unspecified)
            {
                string hostEnvironmentName = string.IsNullOrWhiteSpace(_hostEnvironmentName)
                    ? "<empty>"
                    : _hostEnvironmentName.Trim();
                return $"""
                        Configure ServerHelperConfiguration:Secrets:Environment explicitly before storing this secret.
                        Host environment: {hostEnvironmentName}
                        Accepted values: LocalUnitTests, LocalWebsite, PreProduction, Production.
                        """;
            }

            return store switch
            {
                SecretStoreKind.UserSecrets => $"""
                                                   Store the value in the user secrets of the startup or test project:
                                                   dotnet user-secrets set "{key}" "<secret-value>"
                                                   """,
                SecretStoreKind.EnvironmentVariables => $"""
                                                            Set this environment variable before starting the application:
                                                            {BuildEnvironmentVariableName(key)}=<secret-value>
                                                            """,
                SecretStoreKind.AzureKeyVault => BuildAzureKeyVaultInstruction(key),
                SecretStoreKind.Configuration => $"""
                                                     Add the value through a secure ASP.NET Core configuration provider:
                                                     "{key}": "<secret-value>"
                                                     Do not commit real secret values to source control.
                                                     """,
                SecretStoreKind.External => $"""
                                                Register a host-specific secret provider that exposes this key through IConfiguration:
                                                {key}
                                                """,
                _ => $"""
                     Configure this secret in the resolved store for {environment}.
                     Environment variable name: {BuildEnvironmentVariableName(key)}
                     Azure Key Vault name: {BuildAzureKeyVaultSecretName(key)}
                     """
            };
        }

        private string BuildAzureKeyVaultInstruction(string key)
        {
            string vault = string.IsNullOrWhiteSpace(AzureKeyVaultUri)
                ? "the configured Azure Key Vault"
                : AzureKeyVaultUri.Trim();

            return $"""
                    Create this Azure Key Vault secret in {vault}:
                    {BuildAzureKeyVaultSecretName(key)}

                    Grant the application identity permission to read secrets, then expose Azure Key Vault through the ASP.NET Core configuration pipeline.
                    """;
        }

        private SecretStoreKind ResolveStoreKind()
        {
            if (Store != SecretStoreKind.Auto)
            {
                return Store;
            }

            SecretUsageEnvironment environment = ResolveUsageEnvironment();
            if (environment == SecretUsageEnvironment.Unspecified)
            {
                return SecretStoreKind.Unspecified;
            }

            return environment switch
            {
                SecretUsageEnvironment.LocalUnitTests => SecretStoreKind.UserSecrets,
                SecretUsageEnvironment.LocalWebsite => SecretStoreKind.UserSecrets,
                SecretUsageEnvironment.PreProduction when string.IsNullOrWhiteSpace(AzureKeyVaultUri) == false => SecretStoreKind.AzureKeyVault,
                SecretUsageEnvironment.Production when string.IsNullOrWhiteSpace(AzureKeyVaultUri) == false => SecretStoreKind.AzureKeyVault,
                SecretUsageEnvironment.PreProduction => SecretStoreKind.EnvironmentVariables,
                SecretUsageEnvironment.Production => SecretStoreKind.EnvironmentVariables,
                _ => SecretStoreKind.Configuration
            };
        }

        private SecretUsageEnvironment ResolveUsageEnvironment()
        {
            if (Environment != SecretUsageEnvironment.Auto)
            {
                return Environment;
            }

            if (_hostEnvironmentName.Contains("test", StringComparison.OrdinalIgnoreCase))
            {
                return SecretUsageEnvironment.LocalUnitTests;
            }

            if (_hostEnvironmentName.Contains("stag", StringComparison.OrdinalIgnoreCase) ||
                _hostEnvironmentName.Contains("preprod", StringComparison.OrdinalIgnoreCase) ||
                _hostEnvironmentName.Contains("pre-prod", StringComparison.OrdinalIgnoreCase))
            {
                return SecretUsageEnvironment.PreProduction;
            }

            if (_hostEnvironmentName.Contains("prod", StringComparison.OrdinalIgnoreCase))
            {
                return SecretUsageEnvironment.Production;
            }

            if (string.IsNullOrWhiteSpace(_hostEnvironmentName))
            {
                return SecretUsageEnvironment.Unspecified;
            }

            if (_hostEnvironmentName.Contains("dev", StringComparison.OrdinalIgnoreCase) ||
                _hostEnvironmentName.Contains("local", StringComparison.OrdinalIgnoreCase))
            {
                return SecretUsageEnvironment.LocalWebsite;
            }

            return SecretUsageEnvironment.Unspecified;
        }

        private void ThrowIfUsageEnvironmentIsUnspecified()
        {
            SecretUsageEnvironment environment = ResolveUsageEnvironment();
            if (environment != SecretUsageEnvironment.Unspecified)
            {
                return;
            }

            string hostEnvironmentName = string.IsNullOrWhiteSpace(_hostEnvironmentName)
                ? "<empty>"
                : _hostEnvironmentName.Trim();
            throw new InvalidOperationException($"""
                                                 Secret usage environment is not explicit and the host environment name is not recognized.
                                                 Host environment: {hostEnvironmentName}
                                                 Configure ServerHelperConfiguration:Secrets:Environment with one of these values: LocalUnitTests, LocalWebsite, PreProduction, Production.
                                                 """);
        }

        #endregion
    }
}
