#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj SessionString.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents a session string definition used for managing session values.
    /// </summary>
    public class SessionString : SessionDefinition
    {
        #region Static methods

        /// <summary>
        ///     Gets the session definition with the specified name.
        /// </summary>
        /// <param name="name">The name of the session definition.</param>
        /// <returns>The session definition with the specified name, or null if not found.</returns>
        public static SessionString? GetSessionDefinition(string name)
        {
            SessionString? rReturn = null;
            if (SessionGlobal.KDictionary.ContainsKey(name))
            {
                rReturn = (SessionString)SessionGlobal.KDictionary[name];
            }

            return rReturn;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Represents a session string definition.
        /// </summary>
        public SessionString(
            string name,
            string title,
            string description,
            SessionDefinitionGroup group,
            string defaultValue,
            bool deletable = false,
            bool manualEditable = false
        )
        {
            Kind = SessionDefinitionKind.StringKind;
            Name = SpaceCleaner(name);
            Title = title;
            Explication = description;
            Group = group;
            DefaultValue = defaultValue.Replace("'", "\'");
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
        ///     Retrieves the value of the session string.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>The value of the session string.</returns>
        public string GetValue(HttpContext? httpContext)
        {
            string result = DefaultValue;
            string? value = _GetValue(httpContext);
            if (value != null)
            {
                result = value;
            }

            return result.Replace("\'", "'");
        }

        /// <summary>
        ///     Sets the value of the session string.
        /// </summary>
        /// <param name="httpContext">The HttpContext object representing the current request.</param>
        /// <param name="value">The value to set for the session string.</param>
        public void SetValue(HttpContext? httpContext, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            _SetValue(httpContext, value.Replace("'", "\'"));
        }

        #endregion
    }
}