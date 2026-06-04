#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Implements a view location expander for localizing Razor views based on the current UI culture.
    ///     This class is used to dynamically add localized view paths for Razor views based on the culture information.
    /// </summary>
    /// <remarks>
    ///     This expander introduces culture-specific view locations to the default view searching mechanism in ASP.NET Core.
    ///     It integrates with the Razor view engine to enable localized view resolution by modifying view locations
    ///     dynamically.
    /// </remarks>
    /// <remarks>
    ///     The <see cref="PopulateValues" /> method ensures the current UI culture is captured and stored in the
    ///     <see cref="ViewLocationExpanderContext" /> to be utilized during the view location expansion process.
    ///     The <see cref="ExpandViewLocations" /> method appends culture-specific view locations based on the stored culture
    ///     value.
    ///     This is done by replacing placeholders in the default view locations with culture-specific identifiers.
    /// </remarks>
    /// <seealso cref="IViewLocationExpander" />
    public class WebLocalizedViewLocationExpander : IViewLocationExpander
    {
        #region Instance methods

        #region From interface IViewLocationExpander

        /// <summary>
        ///     Expands the list of view locations to include culture-specific views by appending the current culture to the
        ///     default view paths.
        /// </summary>
        /// <param name="context">
        ///     The context for view location expansion. This includes information such as the current values and
        ///     area.
        /// </param>
        /// <param name="viewLocations">A collection of default view locations to be expanded.</param>
        /// <returns>
        ///     An enumerable collection of expanded view locations that includes culture-specific variations and the default
        ///     paths.
        /// </returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var culture = context.Values["culture"];
            if (viewLocations != null)
            {
                var enumerable = viewLocations as string[] ?? viewLocations.ToArray();
                foreach (var location in enumerable)
                {
                    yield return location.Replace("{0}", "{0}." + culture);
                }

                foreach (var location in enumerable)
                {
                    yield return location;
                }
            }
        }

        /// <summary>
        ///     Populates the context values with additional parameters for view location expansion,
        ///     including the current UI culture name.
        /// </summary>
        /// <param name="context">
        ///     The <see cref="ViewLocationExpanderContext" /> containing information about the view location context,
        ///     where additional values can be added.
        /// </param>
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var tCulture = CultureInfo.CurrentUICulture.Name;
            context.Values["culture"] = tCulture;
        }

        #endregion

        #endregion
    }
}