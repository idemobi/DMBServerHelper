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
    ///     Represents a session definition for an unsigned short integer value.
    /// </summary>
    public class SessionUShort : SessionDefinition
    {
        #region Static methods

        /// <summary>
        ///     Get the session definition with the given name.
        /// </summary>
        /// <param name="name">The name of the session definition.</param>
        /// <returns>The session definition object, or null if not found.</returns>
        public static SessionUShort? GetSessionDefinition(string name)
        {
            SessionUShort? result = null;
            if (SessionGlobal.KDictionary.ContainsKey(name))
            {
                result = (SessionUShort)SessionGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Represents a session definition of ushort type.
        /// </summary>
        /// <param name="name">The session key. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable session title.</param>
        /// <param name="description">The human-readable session explanation.</param>
        /// <param name="group">The session purpose group.</param>
        /// <param name="defaultValue">The default unsigned short value used when the session value is absent or invalid.</param>
        /// <param name="deletable">A value indicating whether the session entry may be deleted by bulk deletion helpers.</param>
        /// <param name="manualEditable">A value indicating whether diagnostics may expose a manual editor.</param>
        /// <remarks>
        ///     The definition is registered in <see cref="SessionGlobal.KDictionary" /> under the sanitized name.
        /// </remarks>
        public SessionUShort(
            string name,
            string title,
            string description,
            SessionDefinitionGroup group,
            ushort defaultValue,
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
        ///     Retrieves the value of the session property as an unsigned short (ushort) stored in the HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>The value of the session property as an unsigned short (ushort).</returns>
        public ushort GetValue(HttpContext? httpContext)
        {
            ushort.TryParse(DefaultValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort result);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false && ushort.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort parsedValue))
            {
                result = parsedValue;
            }

            return result;
        }

        /// <summary>
        ///     Increments the value of the session variable.
        /// </summary>
        /// <param name="httpContext">The HTTP context of the session.</param>
        /// <param name="increment">The value by which the session variable should be incremented. Default is 1.</param>
        public void IncrementValue(HttpContext? httpContext, ushort increment = 1)
        {
            _SetValue(httpContext, (GetValue(httpContext) + increment).ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Sets the value of the session variable.
        /// </summary>
        /// <param name="httpContext">The HTTP context of the session</param>
        /// <param name="value">The value to be set</param>
        public void SetValue(HttpContext? httpContext, ushort value)
        {
            _SetValue(httpContext, value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}