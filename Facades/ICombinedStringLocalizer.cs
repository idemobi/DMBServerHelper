#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj ICombinedStringLocalizer.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using Microsoft.Extensions.Localization;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Defines a localizer that can aggregate multiple named localization resources.
    /// </summary>
    /// <remarks>
    ///     Implementations resolve localized strings through the <see cref="IStringLocalizer"/> contract
    ///     while allowing additional resources to be injected at runtime.
    /// </remarks>
    public interface ICombinedStringLocalizer : IStringLocalizer
    {
        #region Instance methods
        /// <summary>
        ///     Adds a named string localizer to the aggregate lookup collection.
        /// </summary>
        /// <param name="name">
        ///     The stable resource name used to identify the injected localizer.
        /// </param>
        /// <param name="localizer">
        ///     The string localizer to include in future lookups.
        /// </param>
        public void InjectResource(string name, IStringLocalizer localizer);

        #endregion
    }
}
