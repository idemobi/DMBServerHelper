#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Provides the base metadata and storage behavior for strongly typed session definitions.
    /// </summary>
    /// <remarks>
    ///     Derived session definitions sanitize the configured name, register themselves in
    ///     <see cref="SessionGlobal.KDictionary" />, and use this base class to read and write serialized
    ///     values through ASP.NET Core session state.
    /// </remarks>
    public abstract class SessionDefinition
    {
        #region Static fields and properties

        private static readonly Regex SpaceCleanerRgx = new Regex(@"\s+", RegexOptions.Compiled);

        #endregion

        #region Static methods

        /// <summary>
        ///     Removes all whitespace characters from a session definition name.
        /// </summary>
        /// <param name="sString">
        ///     The string to normalize.
        /// </param>
        /// <returns>
        ///     The input string without whitespace.
        /// </returns>
        public static string SpaceCleaner(string sString)
        {
            return SpaceCleanerRgx.Replace(sString, string.Empty);
        }

        #endregion

        #region Instance fields and properties

        /// <summary>
        ///     Represents a session definition with a default value.
        /// </summary>
        public string DefaultValue = string.Empty;

        /// <summary>
        ///     Represents a flag indicating whether an object is deletable.
        /// </summary>
        public bool Deletable = true;

        /// <summary>
        ///     The description of the session bool.
        /// </summary>
        public string Explication = string.Empty;

        /// <summary>
        ///     Represents a session definition group.
        /// </summary>
        public SessionDefinitionGroup Group = SessionDefinitionGroup.Navigation;

        /// <summary>
        ///     Represents the kind of a session definition.
        /// </summary>
        public SessionDefinitionKind Kind = SessionDefinitionKind.StringKind;

        /// <summary>
        ///     Represents a session definition with manual editability.
        /// </summary>
        public bool ManualEditable = false;

        /// <summary>
        ///     Represents a session variable definition.
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        ///     Represents a session variable title.
        /// </summary>
        public string Title = string.Empty;

        #endregion

        #region Instance methods

        /// <summary>
        ///     Retrieves the value of the session property stored in the HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>The value of the session property. Returns null if the property is not found in the session.</returns>
        protected string? _GetValue(HttpContext? httpContext)
        {
            string? result = null;
            if (httpContext != null)
            {
                if (httpContext.Session.GetString(Name) != null)
                {
                    result = httpContext.Session.GetString(Name);
                }
                else
                {
                    result = DefaultValue;
                }
            }

            return result;
        }

        /// <summary>
        ///     Writes a serialized value into the current HTTP session.
        /// </summary>
        /// <param name="httpContext">
        ///     The current HTTP context, or <see langword="null" /> to skip writing.
        /// </param>
        /// <param name="value">
        ///     The serialized value to store under <see cref="Name" />.
        /// </param>
        protected void _SetValue(HttpContext? httpContext, string value)
        {
            if (httpContext != null && httpContext.Session != null)
            {
                httpContext.Session.SetString(Name, value);
            }
        }

        /// <summary>
        ///     Deletes a session variable with the given name from the provided HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext from which the session variable should be deleted.</param>
        public void DeleteFrom(HttpContext? httpContext)
        {
            if (httpContext != null)
            {
                _SetValue(httpContext, DefaultValue);
                httpContext.Session.Remove(Name);
            }
        }

        /// <summary>
        ///     Checks if the session variable exists in the provided HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext to check for the session variable.</param>
        /// <returns>Returns true if the session variable exists, otherwise false.</returns>
        public bool Exists(HttpContext? httpContext)
        {
            bool result = false;
            if (httpContext != null)
            {
                if (httpContext.Session.GetString(Name) != null)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        ///     Returns the value stored in the session as a string for the given HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext from which to retrieve the session value.</param>
        /// <returns>
        ///     The session value as a string, or an empty string if the HttpContext is null,
        /// </returns>
        public string? GetValueAsString(HttpContext? httpContext)
        {
            if (httpContext != null)
            {
                return _GetValue(httpContext);
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}