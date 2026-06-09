#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Contains the result of a secret validation pass.
    /// </summary>
    public sealed class SecretValidationResult
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the detected secret validation issues.
        /// </summary>
        public List<SecretValidationIssue> Issues { get; set; } = new List<SecretValidationIssue>();

        /// <summary>
        ///     Gets a value indicating whether all required secrets were found.
        /// </summary>
        public bool Success => Issues.Any() == false;

        #endregion
    }
}