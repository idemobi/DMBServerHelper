#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Identifies the configured storage strategy used to resolve application secrets.
    /// </summary>
    public enum SecretStoreKind
    {
        /// <summary>
        ///     Lets <see cref="SecretManager" /> infer the store from the configured usage environment.
        /// </summary>
        Auto,

        /// <summary>
        ///     Indicates that the secret store cannot be resolved until the usage environment is explicit.
        /// </summary>
        Unspecified,

        /// <summary>
        ///     Reads secrets from the ASP.NET Core configuration pipeline, typically developer user secrets.
        /// </summary>
        UserSecrets,

        /// <summary>
        ///     Reads secrets from environment variables exposed through the ASP.NET Core configuration pipeline.
        /// </summary>
        EnvironmentVariables,

        /// <summary>
        ///     Reads secrets from Azure Key Vault exposed through the ASP.NET Core configuration pipeline.
        /// </summary>
        AzureKeyVault,

        /// <summary>
        ///     Reads secrets from already configured application configuration providers.
        /// </summary>
        Configuration,

        /// <summary>
        ///     Indicates that an external host-specific provider is responsible for supplying secrets.
        /// </summary>
        External
    }
}