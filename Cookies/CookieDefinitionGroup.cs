#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Defines purpose groups for registered cookie definitions.
    /// </summary>
    public enum CookieDefinitionGroup
    {
        /// <summary>
        ///     Identifies a cookie required for core application functionality.
        /// </summary>
        Functional,

        /// <summary>
        ///     Identifies a cookie used to store consent state.
        /// </summary>
        Consent,

        /// <summary>
        ///     Identifies a cookie used for advertising features.
        /// </summary>
        /// <remarks>
        ///     The advertisement group is used to categorize cookies related to advertisements.
        /// </remarks>
        Advertisement,

        /// <summary>
        ///     Identifies a cookie used for analytics features.
        /// </summary>
        Analytics,

        /// <summary>
        ///     Identifies an optional cookie that can usually be deleted or disabled.
        /// </summary>
        Optional,

        /// <summary>
        ///     Identifies a cookie used for visual design or theme preferences.
        /// </summary>
        Design,

        /// <summary>
        ///     Identifies a cookie used for user preferences.
        /// </summary>
        Preference,
    }
}