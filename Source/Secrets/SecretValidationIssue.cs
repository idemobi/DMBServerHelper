#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents one missing or invalid secret detected by <see cref="SecretManager" />.
    /// </summary>
    public sealed class SecretValidationIssue
    {
        /// <summary>
        ///     Gets or sets the affected secret definition.
        /// </summary>
        public SecretDefinition Definition { get; set; } = new SecretDefinition();

        /// <summary>
        ///     Gets or sets the diagnostic message explaining how to configure the secret.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
