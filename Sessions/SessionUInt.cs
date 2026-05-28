#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj SessionUInt.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Globalization;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents a session definition for an unsigned integer value.
    /// </summary>
    public class SessionUInt : SessionDefinition
    {
        #region Static methods

        /// <summary>
        ///     Retrieves the session definition for a given name.
        /// </summary>
        /// <param name="name">The name of the session definition.</param>
        /// <returns>The session definition for the given name, or null if not found.</returns>
        public static SessionUInt? GetSessionDefinition(string name)
        {
            SessionUInt? result = null;
            if (SessionGlobal.KDictionary.ContainsKey(name))
            {
                result = (SessionUInt)SessionGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Represents a session definition for an unsigned integer value.
        /// </summary>
        /// <param name="name">The session key. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable session title.</param>
        /// <param name="description">The human-readable session explanation.</param>
        /// <param name="group">The session purpose group.</param>
        /// <param name="defaultValue">The default unsigned integer value used when the session value is absent or invalid.</param>
        /// <param name="deletable">A value indicating whether the session entry may be deleted by bulk deletion helpers.</param>
        /// <param name="manualEditable">A value indicating whether diagnostics may expose a manual editor.</param>
        /// <remarks>
        ///     The definition is registered in <see cref="SessionGlobal.KDictionary"/> under the sanitized name.
        /// </remarks>
        public SessionUInt(
            string name,
            string title,
            string description,
            SessionDefinitionGroup group,
            uint defaultValue,
            bool deletable = false,
            bool manualEditable = false
        )
        {
            Kind = SessionDefinitionKind.UIntKind;
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
        ///     Retrieves the value of the session property as an unsigned integer.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>
        ///     The value of the session property as an unsigned integer. Returns the default value if the property is not
        ///     found in the session or if the value cannot be parsed as an unsigned integer.
        /// </returns>
        private uint GetValue(HttpContext? httpContext)
        {
            uint result = uint.Parse(DefaultValue);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false)
            {
                uint.TryParse(value, out result);
            }

            return result;
        }

        /// <summary>
        ///     Increment the value of the session variable by a specified amount.
        /// </summary>
        /// <param name="httpContext">The HTTP context of the session.</param>
        /// <param name="increment">The amount by which the value should be incremented. Default is 1.</param>
        public void IncrementValue(HttpContext? httpContext, uint increment = 1)
        {
            _SetValue(httpContext, (GetValue(httpContext) + increment).ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Set the value of the session variable.
        /// </summary>
        /// <param name="httpContext">The HTTP context of the session.</param>
        /// <param name="value">The value to be set.</param>
        public void SetValue(HttpContext? httpContext, uint value)
        {
            _SetValue(httpContext, value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
