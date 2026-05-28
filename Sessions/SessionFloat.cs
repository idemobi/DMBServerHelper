#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj SessionFloat.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Globalization;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents a session definition for float values.
    /// </summary>
    public class SessionFloat : SessionDefinition
    {
        #region Constants

        /// <summary>
        ///     Represents the record format used for a <see cref="SessionFloat" /> session definition.
        /// </summary>
        private const string K_RecordFormat = "0.00000";

        #endregion

        #region Static methods

        /// <summary>
        ///     Gets a registered floating-point session definition by name.
        /// </summary>
        /// <param name="name">The name of the session definition.</param>
        /// <returns>The session definition object if found; otherwise null.</returns>
        public static SessionFloat? GetSessionDefinition(string name)
        {
            SessionFloat? result = null;
            if (SessionGlobal.KDictionary.ContainsKey(name))
            {
                result = (SessionFloat)SessionGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     A session definition for float values.
        /// </summary>
        /// <param name="name">The session key. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable session title.</param>
        /// <param name="description">The human-readable session explanation.</param>
        /// <param name="group">The session purpose group.</param>
        /// <param name="defaultValue">The default floating-point value used when the session value is absent or invalid.</param>
        /// <param name="deletable">A value indicating whether the session entry may be deleted by bulk deletion helpers.</param>
        /// <param name="manualEditable">A value indicating whether diagnostics may expose a manual editor.</param>
        /// <remarks>
        ///     The definition is registered in <see cref="SessionGlobal.KDictionary"/> under the sanitized name.
        /// </remarks>
        public SessionFloat(
            string name,
            string title,
            string description,
            SessionDefinitionGroup group,
            float defaultValue,
            bool deletable = false,
            bool manualEditable = false
        )
        {
            Kind = SessionDefinitionKind.FloatKind;
            Name = SpaceCleaner(name);
            Title = title;
            Explication = description;
            Group = group;
            DefaultValue = defaultValue.ToString(K_RecordFormat, CultureInfo.InvariantCulture);
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
        ///     Retrieves the value of the session property as a float.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>
        ///     The value of the session property as a float. Returns the default value if the property is not found or cannot
        ///     be parsed as a float.
        /// </returns>
        public float GetValue(HttpContext? httpContext)
        {
            float result = float.Parse(DefaultValue);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false)
            {
                float.TryParse(value, out result);
            }

            return result;
        }

        /// <summary>
        ///     Increments the value of the session variable by the specified increment.
        /// </summary>
        /// <param name="httpContext">The HTTP context of the session.</param>
        /// <param name="increment">The increment value. Default value is 1.0.</param>
        public void IncrementValue(HttpContext? httpContext, float increment = 1F)
        {
            _SetValue(httpContext, (GetValue(httpContext) + increment).ToString(K_RecordFormat, CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Sets the value of the session variable.
        /// </summary>
        /// <param name="httpContext">The HTTP context of the session.</param>
        /// <param name="value">The value to be set.</param>
        public void SetValue(HttpContext? httpContext, float value)
        {
            _SetValue(httpContext, value.ToString(K_RecordFormat, CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
