#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj CookieDefinitionGroup.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

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
