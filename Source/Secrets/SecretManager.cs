#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Globalization;
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
        #region Static methods

        private static string BuildAzureKeyVaultSecretName(string key)
        {
            return key.Replace(":", "--");
        }

        private static string BuildEnvironmentVariableName(string key)
        {
            return key.Replace(":", "__");
        }

        private static string BuildLoggedMissingSecretExceptionMessage(SecretDefinition definition)
        {
            return $"Missing secret: {definition.Key}. The full setup diagnostic was already written to the secret logger.";
        }

        private static string BuildLoggedInvalidSecretExceptionMessage(SecretDefinition definition)
        {
            return $"Invalid secret: {definition.Key}. The full format diagnostic was already written to the secret logger.";
        }

        private static string BuildCurrentProcessEnvironmentVariableCommand(string environmentVariableName)
        {
            if (OperatingSystem.IsWindows())
            {
                return $"""
                        Windows PowerShell:
                        $env:{environmentVariableName}="<secret-value>"
                        """;
            }

            if (OperatingSystem.IsMacOS())
            {
                return $"""
                        macOS terminal:
                        export {environmentVariableName}="<secret-value>"
                        """;
            }

            if (OperatingSystem.IsLinux())
            {
                return $"""
                        Linux terminal:
                        export {environmentVariableName}="<secret-value>"
                        """;
            }

            return $"""
                    POSIX terminal:
                    export {environmentVariableName}="<secret-value>"
                    """;
        }

        private static List<string> BuildEnumAcceptedValues<TEnum>() where TEnum : struct, Enum
        {
            List<string> values = new List<string>();
            foreach (TEnum value in Enum.GetValues<TEnum>())
            {
                values.Add($"{value} ({Convert.ToInt64(value, CultureInfo.InvariantCulture)})");
            }

            return values;
        }

        #endregion

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
        ///     Gets a value indicating whether missing required secrets must stop the application startup or configuration flow.
        /// </summary>
        /// <remarks>
        ///     This value is always <see langword="true" /> for <see cref="SecretUsageEnvironment.PreProduction" />
        ///     and <see cref="SecretUsageEnvironment.Production" />, even when <see cref="FailFast" /> is
        ///     <see langword="false" />.
        /// </remarks>
        public bool EffectiveFailFast => FailFast || IsProtectedRuntimeEnvironment();

        /// <summary>
        ///     Gets or sets the configured usage environment.
        /// </summary>
        public SecretUsageEnvironment Environment { get; set; } = SecretUsageEnvironment.Auto;

        /// <summary>
        ///     Gets or sets a value determining whether missing required secrets should halt the application startup
        ///     or configuration process immediately.
        /// </summary>
        /// <remarks>
        ///     When enabled, the application will fail to start if any required secret is missing. This behavior is
        ///     enforced regardless of this property's value in <see cref="SecretUsageEnvironment.PreProduction" />
        ///     and <see cref="SecretUsageEnvironment.Production" />.
        /// </remarks>
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

        private static string BuildEnvironmentVariableInstruction(string key)
        {
            string environmentVariableName = BuildEnvironmentVariableName(key);

            return $"""
                    Set this environment variable before starting the application:
                    {environmentVariableName}=<secret-value>

                    Current process:
                    {BuildCurrentProcessEnvironmentVariableCommand(environmentVariableName)}

                    Persistent local setup:
                    macOS zsh:
                    echo 'export {environmentVariableName}="<secret-value>"' >> ~/.zshrc

                    macOS GUI applications launched from Finder, Dock, or an IDE:
                    launchctl setenv {environmentVariableName} "<secret-value>"
                    Fully quit and restart the application and/or the IDE that launches it, such as Rider, Visual Studio Code, Visual Studio, or Terminal.app, before starting the application again.

                    Linux bash:
                    echo 'export {environmentVariableName}="<secret-value>"' >> ~/.bashrc

                    Windows PowerShell user profile:
                    [Environment]::SetEnvironmentVariable("{environmentVariableName}", "<secret-value>", "User")

                    Azure App Service application setting:
                    az webapp config appsettings set --resource-group <resource-group> --name <app-name> --settings {environmentVariableName}="<secret-value>"

                    Azure Portal:
                    App Service > Settings > Environment variables > App settings > Add.
                    Name: {environmentVariableName}
                    Value: <secret-value>
                    Save, then restart the application.
                    """;
        }

        private static void AppendValueExpectations(StringBuilder builder, SecretDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(definition.ValueType) == false)
            {
                builder.AppendLine($"Expected type: {definition.ValueType.Trim()}");
            }

            if (string.IsNullOrWhiteSpace(definition.FormatHint) == false)
            {
                builder.AppendLine($"Format: {definition.FormatHint.Trim()}");
            }

            if (string.IsNullOrWhiteSpace(definition.ExampleValue) == false)
            {
                builder.AppendLine($"Example value: {definition.ExampleValue.Trim()}");
            }

            List<string>? acceptedValues = definition.AcceptedValues;
            if (acceptedValues != null && acceptedValues.Count > 0)
            {
                builder.AppendLine("Accepted values:");
                foreach (string acceptedValue in acceptedValues.Where(value => string.IsNullOrWhiteSpace(value) == false))
                {
                    builder.AppendLine($"- {acceptedValue.Trim()}");
                }
            }
        }

        private SecretDefinition BuildDefinitionForType(string key, string expectedValueType, IEnumerable<string>? acceptedValues, string formatHint)
        {
            SecretDefinition definition = FindDefinition(key) ?? new SecretDefinition
            {
                Key = key,
                DisplayName = key,
                Owner = "Unregistered consumer"
            };

            SecretDefinition copy = definition.Clone();
            if (string.IsNullOrWhiteSpace(copy.ValueType))
            {
                copy.ValueType = expectedValueType;
            }

            if (string.IsNullOrWhiteSpace(copy.FormatHint))
            {
                copy.FormatHint = formatHint;
            }

            if ((copy.AcceptedValues == null || copy.AcceptedValues.Count == 0) && acceptedValues != null)
            {
                copy.AcceptedValues = acceptedValues
                    .Where(value => string.IsNullOrWhiteSpace(value) == false)
                    .Select(value => value.Trim())
                    .ToList();
            }

            return copy;
        }

        private string BuildInvalidSecretMessage(SecretDefinition definition)
        {
            SecretStoreKind store = ResolveStoreKind();
            SecretUsageEnvironment environment = ResolveUsageEnvironment();
            string owner = string.IsNullOrWhiteSpace(definition.Owner) ? "Unspecified" : definition.Owner.Trim();
            string displayName = string.IsNullOrWhiteSpace(definition.DisplayName) ? definition.Key : definition.DisplayName.Trim();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Invalid secret: {definition.Key}");
            builder.AppendLine($"Requested by: {owner}");
            builder.AppendLine($"Display name: {displayName}");
            builder.AppendLine($"Environment: {environment}");
            builder.AppendLine($"Secret store: {store}");
            AppendValueExpectations(builder, definition);
            builder.AppendLine("Configured value: hidden");
            builder.AppendLine("Secret diagnostics never print configured secret values.");

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

            AppendValueExpectations(builder, definition);

            builder.AppendLine();
            builder.AppendLine(BuildStoreInstruction(definition.Key, store, environment));

            if (string.IsNullOrWhiteSpace(definition.DocumentationUrl) == false)
            {
                builder.AppendLine();
                builder.AppendLine($"Documentation: {definition.DocumentationUrl.Trim()}");
            }

            return builder.ToString().TrimEnd();
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
                SecretStoreKind.EnvironmentVariables => BuildEnvironmentVariableInstruction(key),
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

        /// <summary>
        ///     Configures the manager with the active ASP.NET Core configuration pipeline.
        /// </summary>
        /// <param name="configuration">The configuration used to resolve secret values.</param>
        /// <param name="hostEnvironmentName">
        ///     The host environment name used when <see cref="Environment" /> is
        ///     <see cref="SecretUsageEnvironment.Auto" />.
        /// </param>
        public void Configure(IConfiguration configuration, string? hostEnvironmentName)
        {
            _configuration = configuration;
            _hostEnvironmentName = hostEnvironmentName ?? string.Empty;
        }

        private SecretDefinition? FindDefinition(string key)
        {
            return _requiredSecrets.FirstOrDefault(secret =>
                string.Equals(secret.Key, key, StringComparison.OrdinalIgnoreCase));
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
        ///     Gets a required secret value or throws when the value is missing.
        /// </summary>
        /// <param name="key">The logical secret key.</param>
        /// <returns>The resolved secret value.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the secret value is missing. The exception contains the complete setup diagnostic only
        ///     when missing-secret logging is disabled; otherwise, the complete diagnostic is written through the
        ///     configured secret logger and the exception message stays concise.
        /// </exception>
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
                throw new InvalidOperationException(BuildLoggedMissingSecretExceptionMessage(definition));
            }

            throw new InvalidOperationException(message);
        }

        /// <summary>
        ///     Gets a required secret value parsed as a defined enum value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type expected by the consumer.</typeparam>
        /// <param name="key">The logical secret key.</param>
        /// <returns>The parsed enum value.</returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the secret is missing or when the configured value is not a defined enum value.
        /// </exception>
        public TEnum GetRequiredEnum<TEnum>(string key) where TEnum : struct, Enum
        {
            string value = GetRequired(key).Trim();
            if (Enum.TryParse(value, true, out TEnum enumValue) && Enum.IsDefined<TEnum>(enumValue))
            {
                return enumValue;
            }

            throw CreateInvalidSecretException(
                key,
                $"{typeof(TEnum).Name} enum",
                "Use the textual enum name. Numeric enum values are accepted only when they match a defined value.",
                BuildEnumAcceptedValues<TEnum>());
        }

        /// <summary>
        ///     Creates a secret-format exception with a diagnostic explaining the expected type and format.
        /// </summary>
        /// <param name="key">The logical secret key.</param>
        /// <param name="expectedValueType">The expected value type shown in diagnostics.</param>
        /// <param name="formatHint">A short explanation of the expected value format.</param>
        /// <param name="acceptedValues">Optional accepted values shown when the value belongs to a constrained set.</param>
        /// <returns>An <see cref="InvalidOperationException" /> ready to throw.</returns>
        /// <remarks>
        ///     The configured value is never printed. When <see cref="LogMissingSecrets" /> is enabled, the full
        ///     diagnostic is written through the secret logger and the returned exception contains only a concise
        ///     summary to avoid duplicate unhandled-exception output.
        /// </remarks>
        public InvalidOperationException CreateInvalidSecretException(string key, string expectedValueType, string formatHint, IEnumerable<string>? acceptedValues = null)
        {
            SecretDefinition definition = BuildDefinitionForType(key, expectedValueType, acceptedValues, formatHint);
            string message = BuildInvalidSecretMessage(definition);
            if (LogMissingSecrets)
            {
                _logger.Warning(message);
                return new InvalidOperationException(BuildLoggedInvalidSecretExceptionMessage(definition));
            }

            return new InvalidOperationException(message);
        }

        /// <summary>
        ///     Gets a required secret value parsed as a 16-bit signed whole number.
        /// </summary>
        /// <param name="key">The logical secret key.</param>
        /// <returns>The parsed <see cref="short" /> value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the secret is missing or has an invalid number format.</exception>
        public short GetRequiredInt16(string key)
        {
            string value = GetRequired(key).Trim();
            if (short.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out short parsedValue))
            {
                return parsedValue;
            }

            throw CreateInvalidSecretException(key, "Int16 whole number", "Use digits only, for example 12.");
        }

        /// <summary>
        ///     Gets a required secret value parsed as a 32-bit signed whole number.
        /// </summary>
        /// <param name="key">The logical secret key.</param>
        /// <returns>The parsed <see cref="int" /> value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the secret is missing or has an invalid number format.</exception>
        public int GetRequiredInt32(string key)
        {
            string value = GetRequired(key).Trim();
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedValue))
            {
                return parsedValue;
            }

            throw CreateInvalidSecretException(key, "Int32 whole number", "Use digits only, for example 123456.");
        }

        /// <summary>
        ///     Gets a required secret value parsed as a 64-bit signed whole number.
        /// </summary>
        /// <param name="key">The logical secret key.</param>
        /// <returns>The parsed <see cref="long" /> value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the secret is missing or has an invalid number format.</exception>
        public long GetRequiredInt64(string key)
        {
            string value = GetRequired(key).Trim();
            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long parsedValue))
            {
                return parsedValue;
            }

            throw CreateInvalidSecretException(key, "Int64 whole number", "Use digits only, for example 68456707772.");
        }

        /// <summary>
        ///     Gets a required secret value parsed as a single-precision floating-point number.
        /// </summary>
        /// <param name="key">The logical secret key.</param>
        /// <returns>The parsed <see cref="float" /> value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the secret is missing or has an invalid number format.</exception>
        public float GetRequiredFloat(string key)
        {
            string value = GetRequired(key).Trim();
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedValue))
            {
                return parsedValue;
            }

            throw CreateInvalidSecretException(key, "Single floating-point number", "Use invariant culture with . as decimal separator, for example 12.5.");
        }

        /// <summary>
        ///     Gets a required secret value parsed as a double-precision floating-point number.
        /// </summary>
        /// <param name="key">The logical secret key.</param>
        /// <returns>The parsed <see cref="double" /> value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the secret is missing or has an invalid number format.</exception>
        public double GetRequiredDouble(string key)
        {
            string value = GetRequired(key).Trim();
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue))
            {
                return parsedValue;
            }

            throw CreateInvalidSecretException(key, "Double floating-point number", "Use invariant culture with . as decimal separator, for example 12.5.");
        }

        /// <summary>
        ///     Gets a required secret value parsed as a decimal number.
        /// </summary>
        /// <param name="key">The logical secret key.</param>
        /// <returns>The parsed <see cref="decimal" /> value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the secret is missing or has an invalid number format.</exception>
        public decimal GetRequiredDecimal(string key)
        {
            string value = GetRequired(key).Trim();
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedValue))
            {
                return parsedValue;
            }

            throw CreateInvalidSecretException(key, "Decimal number", "Use invariant culture with . as decimal separator, for example 12.5.");
        }

        private bool IsProtectedRuntimeEnvironment()
        {
            SecretUsageEnvironment environment = ResolveUsageEnvironment();
            return environment is SecretUsageEnvironment.PreProduction or SecretUsageEnvironment.Production;
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
        /// <exception cref="InvalidOperationException">
        ///     Thrown when <see cref="EffectiveFailFast" /> is enabled and at least one
        ///     required secret is missing.
        /// </exception>
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
                if (LogMissingSecrets)
                {
                    IEnumerable<string> missingSecretKeys = result.Issues.Select(issue => issue.Definition.Key);
                    throw new InvalidOperationException($"Missing required secrets: {string.Join(", ", missingSecretKeys)}. The full setup diagnostics were already written to the secret logger.");
                }

                throw new InvalidOperationException(string.Join(System.Environment.NewLine + System.Environment.NewLine, result.Issues.Select(issue => issue.Message)));
            }

            return result;
        }

        #endregion
    }
}
