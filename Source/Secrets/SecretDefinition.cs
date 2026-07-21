#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Describes one logical secret required by a DMB package or host application.
    /// </summary>
    /// <remarks>
    ///     Package modules such as DMBStripe or DMBPennylane should declare their own
    ///     <see cref="SecretDefinition" /> values and register them through
    ///     <see cref="SecretManager.Require(SecretDefinition)" />. DMBServerHelper keeps
    ///     the generic storage and diagnostic behavior, but does not own module-specific keys.
    /// </remarks>
    public sealed class SecretDefinition
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the accepted display values for this secret when the value belongs to a constrained set.
        /// </summary>
        /// <remarks>
        ///     Typical examples are enum names such as <c>Development</c>, <c>Preproduction</c>, or
        ///     <c>Production</c>. The values are used only for diagnostics and setup guidance; they are never used
        ///     as secret values.
        /// </remarks>
        public List<string> AcceptedValues { get; set; } = new List<string>();

        /// <summary>
        ///     Gets or sets a human-readable description of the secret purpose.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the display name shown in diagnostics.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets an optional documentation URL explaining how to create the secret.
        /// </summary>
        public string DocumentationUrl { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets an optional example placeholder showing the expected value shape.
        /// </summary>
        /// <remarks>
        ///     Do not put real secret values here. Use safe examples such as <c>Preproduction</c>,
        ///     <c>123456789</c>, or <c>range:account</c>.
        /// </remarks>
        public string ExampleValue { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets an optional format hint explaining how to write the value.
        /// </summary>
        public string FormatHint { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the logical ASP.NET Core configuration key, for example <c>DMB:Stripe:WebhookSecret</c>.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the package, module, or host feature that declared the secret.
        /// </summary>
        public string Owner { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the application should report this secret as missing.
        /// </summary>
        public bool Required { get; set; } = true;

        /// <summary>
        ///     Gets or sets the expected value type shown in missing or invalid secret diagnostics.
        /// </summary>
        public string ValueType { get; set; } = string.Empty;

        #endregion

        #region Instance methods

        /// <summary>
        ///     Creates a copy of the current secret definition.
        /// </summary>
        /// <returns>A detached copy of the current <see cref="SecretDefinition" />.</returns>
        public SecretDefinition Clone()
        {
            return new SecretDefinition
            {
                AcceptedValues = AcceptedValues?.ToList() ?? new List<string>(),
                Description = Description,
                DisplayName = DisplayName,
                DocumentationUrl = DocumentationUrl,
                ExampleValue = ExampleValue,
                FormatHint = FormatHint,
                Key = Key,
                Owner = Owner,
                Required = Required,
                ValueType = ValueType
            };
        }

        #endregion
    }
}
