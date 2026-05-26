#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj SessionDefinitionGroup.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Defines purpose groups for registered session definitions.
    /// </summary>
    public enum SessionDefinitionGroup
    {
        /// <summary>
        ///     Identifies session data related to consent state.
        /// </summary>
        Consent,

        /// <summary>
        ///     Identifies session data required for core application functionality.
        /// </summary>
        Functional,

        /// <summary>
        ///     Identifies session data used for advertising features.
        /// </summary>
        Advertisement,

        /// <summary>
        ///     Identifies session data used for analytics features.
        /// </summary>
        Analytics,

        /// <summary>
        ///     Identifies optional session data.
        /// </summary>
        Optional,

        /// <summary>
        ///     Identifies session data used for visual design or theme preferences.
        /// </summary>
        Design,

        /// <summary>
        ///     Identifies session data used for user preferences.
        /// </summary>
        Preference,

        /// <summary>
        ///     Identifies session data used for navigation state.
        /// </summary>
        Navigation,

        /// <summary>
        ///     Identifies implementation details that should normally remain invisible to users.
        /// </summary>
        Invisible,
    }
}
