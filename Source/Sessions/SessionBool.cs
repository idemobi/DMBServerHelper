#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents a boolean session variable definition.
    /// </summary>
    public class SessionBool : SessionDefinition
    {
        #region Static methods

        /// <summary>
        ///     Returns the session definition for the specified name.
        /// </summary>
        /// <param name="name">The name of the session definition.</param>
        /// <returns>The session definition with the specified name, or null if not found.</returns>
        public static SessionBool? GetSessionDefinition(string name)
        {
            SessionBool? result = null;
            if (SessionGlobal.KDictionary.ContainsKey(name))
            {
                result = (SessionBool)SessionGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Represents a boolean session variable definition.
        /// </summary>
        /// <param name="name">The session key. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable session title.</param>
        /// <param name="description">The human-readable session explanation.</param>
        /// <param name="group">The session purpose group.</param>
        /// <param name="defaultValue">The default boolean value used when the session value is absent or invalid.</param>
        /// <param name="deletable">A value indicating whether the session entry may be deleted by bulk deletion helpers.</param>
        /// <param name="manualEditable">A value indicating whether diagnostics may expose a manual editor.</param>
        /// <remarks>
        ///     The definition is registered in <see cref="SessionGlobal.KDictionary" /> under the sanitized name.
        /// </remarks>
        public SessionBool(
            string name,
            string title,
            string description,
            SessionDefinitionGroup group,
            bool defaultValue,
            bool deletable = false,
            bool manualEditable = false
        )
        {
            Kind = SessionDefinitionKind.BoolKind;
            Name = SpaceCleaner(name);
            Title = title;
            Explication = description;
            Group = group;
            DefaultValue = defaultValue.ToString();
            Deletable = deletable;
            ManualEditable = manualEditable;
            if (SessionGlobal.KDictionary.ContainsKey(Name))
            {
                SessionGlobal.KDictionary[Name] = this;
            }
            else
            {
                SessionGlobal.KDictionary.TryAdd(Name, this);
            }
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Gets the value of the session variable.
        /// </summary>
        /// <param name="httpContext">The HttpContext object to access the session.</param>
        /// <returns>The boolean value of the session variable.</returns>
        public bool GetValue(HttpContext? httpContext)
        {
            bool.TryParse(DefaultValue, out bool result);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false && bool.TryParse(value, out bool parsedValue))
            {
                result = parsedValue;
            }

            return result;
        }

        /// <summary>
        ///     Sets the value of the session to the specified boolean value.
        /// </summary>
        /// <param name="httpContext">The HttpContext object representing the current HTTP request.</param>
        /// <param name="value">The boolean value to set.</param>
        public void SetValue(HttpContext? httpContext, bool value)
        {
            _SetValue(httpContext, value.ToString());
        }

        #endregion
    }
}