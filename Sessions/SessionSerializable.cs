#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj SessionSerializable.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Text.Json;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents a session definition for a serializable type.
    /// </summary>
    /// <typeparam name="T">The reference type serialized to and from JSON.</typeparam>
    public class SessionSerializable<T> : SessionDefinition where T : class
    {
        #region Static methods

        /// <summary>
        ///     Retrieves the session definition for the specified name.
        /// </summary>
        /// <param name="name">The name of the session definition.</param>
        /// <returns>The session definition for the specified name or <c>null</c> if not found.</returns>
        public static SessionSerializable<T>? GetSessionDefinition(string name)
        {
            SessionSerializable<T>? result = null;
            if (SessionGlobal.KDictionary.ContainsKey(name))
            {
                result = (SessionSerializable<T>)SessionGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Represents a session definition that serializes a generic type.
        /// </summary>
        /// <param name="name">The session key. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable session title.</param>
        /// <param name="description">The human-readable session explanation.</param>
        /// <param name="group">The session purpose group.</param>
        /// <param name="defaultValue">The default object serialized to JSON when the session value is absent.</param>
        /// <param name="deletable">A value indicating whether the session entry may be deleted by bulk deletion helpers.</param>
        /// <param name="manualEditable">A value indicating whether diagnostics may expose a manual editor.</param>
        /// <remarks>
        ///     The default value is serialized with <see cref="JsonSerializer"/> and the definition is registered in
        ///     <see cref="SessionGlobal.KDictionary"/> under the sanitized name.
        /// </remarks>
        public SessionSerializable(
            string name,
            string title,
            string description,
            SessionDefinitionGroup group,
            T defaultValue,
            bool deletable = false,
            bool manualEditable = false
        )
        {
            Kind = SessionDefinitionKind.StringKind;
            Name = SpaceCleaner(name);
            Title = title;
            Explication = description;
            Group = group;
            if (defaultValue != null)
            {
                string tValue = JsonSerializer.Serialize(defaultValue);
                DefaultValue = tValue.Replace("'", "\'");
            }
            else
            {
                string tValue = JsonSerializer.Serialize(default(T));
                DefaultValue = tValue.Replace("'", "\'");
            }

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
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>The value of the session variable.</returns>
        public T? GetValue(HttpContext? httpContext)
        {
            T? result = default(T);
            string? value = _GetValue(httpContext);
            if (value != null)
            {
                result = JsonSerializer.Deserialize<T>(value.Replace("\'", "'"));
            }

            return result;
        }

        /// <summary>
        ///     Sets the value of the session variable with the specified value for the provided HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext used to set the session variable value.</param>
        /// <param name="value">The value to be set for the session variable.</param>
        /// <remarks>
        ///     The method serializes the value to JSON and sets it as the session variable value.
        ///     If the provided value is not null, it is serialized using the JsonSerializer.Serialize method.
        ///     Any single quotes in the serialized JSON string are replaced with escaped single quotes.
        /// </remarks>
        public void SetValue(HttpContext? httpContext, T? value)
        {
            if (value != null)
            {
                string jsonValue = JsonSerializer.Serialize(value);
                _SetValue(httpContext, jsonValue.Replace("'", "\'"));
            }
        }

        #endregion
    }
}
