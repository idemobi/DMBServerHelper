#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Globalization;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents a session definition of type int.
    /// </summary>
    /// <remarks>
    ///     This class is used to define a session variable of type int.
    ///     It inherits from the base <see cref="SessionDefinition" /> class.
    /// </remarks>
    public class SessionInt : SessionDefinition
    {
        #region Static methods

        /// <summary>
        ///     Retrieves the session definition with the specified name.
        /// </summary>
        /// <param name="name">The name of the session definition to retrieve.</param>
        /// <returns>The session definition with the specified name, or null if not found.</returns>
        public static SessionInt? GetSessionDefinition(string name)
        {
            SessionInt? result = null;
            if (SessionGlobal.KDictionary.ContainsKey(name) == true)
            {
                result = (SessionInt)SessionGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a strongly typed integer session definition.
        /// </summary>
        /// <param name="name">The session key. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable session title.</param>
        /// <param name="description">The human-readable session explanation.</param>
        /// <param name="group">The session purpose group.</param>
        /// <param name="defaultValue">The default integer value used when the session value is absent or invalid.</param>
        /// <param name="deletable">A value indicating whether the session entry may be deleted by bulk deletion helpers.</param>
        /// <param name="manualEditable">A value indicating whether diagnostics may expose a manual editor.</param>
        /// <remarks>
        ///     The definition is registered in <see cref="SessionGlobal.KDictionary" /> under the sanitized name.
        /// </remarks>
        public SessionInt(
            string name,
            string title,
            string description,
            SessionDefinitionGroup group,
            int defaultValue,
            bool deletable = false,
            bool manualEditable = false
        )
        {
            Kind = SessionDefinitionKind.IntKind;
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
        ///     Returns the value of the session definition.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The value of the session definition as an integer.</returns>
        public int GetValue(HttpContext? httpContext)
        {
            int result = int.Parse(DefaultValue);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false)
            {
                int.TryParse(value, out result);
            }

            return result;
        }

        /// <summary>
        ///     Increment the value of the session variable by a specified increment.
        /// </summary>
        /// <param name="httpContext">The HTTP context of the session.</param>
        /// <param name="increment">The increment value. Default is 1.</param>
        public void IncrementValue(HttpContext? httpContext, int increment = 1)
        {
            _SetValue(httpContext, (GetValue(httpContext) + increment).ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Sets the value of the session definition.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(HttpContext? httpContext, int value)
        {
            _SetValue(httpContext, value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}