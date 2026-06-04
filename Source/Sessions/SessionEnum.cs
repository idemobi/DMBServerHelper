#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents a session definition for an enum value.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    public class SessionEnum<T> : SessionDefinition where T : struct, Enum
    {
        #region Static methods

        /// <summary>
        ///     Gets the session definition for the specified name.
        /// </summary>
        /// <param name="name">The name of the session definition.</param>
        /// <returns>The session definition for the specified name, or null if not found.</returns>
        public static SessionEnum<T>? GetSessionDefinition(string name)
        {
            SessionEnum<T>? result = null;
            if (SessionGlobal.KDictionary.ContainsKey(name))
            {
                result = (SessionEnum<T>)SessionGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance fields and properties

        /// <summary>
        ///     Stores the enum type handled by this session definition.
        /// </summary>
        public Type EnumType;

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     A class representing a session definition for an enum value.
        /// </summary>
        /// <param name="name">The session key. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable session title.</param>
        /// <param name="description">The human-readable session explanation.</param>
        /// <param name="group">The session purpose group.</param>
        /// <param name="defaultValue">The default enum value used when the session value is absent.</param>
        /// <param name="deletable">A value indicating whether the session entry may be deleted by bulk deletion helpers.</param>
        /// <param name="manualEditable">A value indicating whether diagnostics may expose a manual editor.</param>
        /// <remarks>
        ///     The definition is registered in <see cref="SessionGlobal.KDictionary" /> under the sanitized name.
        /// </remarks>
        public SessionEnum(
            string name,
            string title,
            string description,
            SessionDefinitionGroup group,
            T defaultValue,
            bool deletable = false,
            bool manualEditable = false
        )
        {
            EnumType = typeof(T);
            Kind = SessionDefinitionKind.EnumKind;
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
        ///     Gets the current enum session value.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>The parsed enum value, or the configured default value when the session value is absent.</returns>
        public T GetValue(HttpContext? httpContext)
        {
            Enum.TryParse(DefaultValue, out T rReturn);
            string? tValue = _GetValue(httpContext);
            if (string.IsNullOrEmpty(tValue) == false && Enum.TryParse(tValue, out T parsedValue))
            {
                rReturn = parsedValue;
            }

            return rReturn;
        }

        /// <summary>
        ///     Sets the value of the session variable with the specified <paramref name="httpContext" /> and
        ///     <paramref name="value" />.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(HttpContext? httpContext, T value)
        {
            _SetValue(httpContext, value.ToString());
        }

        #endregion
    }
}
