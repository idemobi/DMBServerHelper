#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj SessionEnum.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents a session definition for an enum value.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    public class SessionEnum<T> : SessionDefinition where T : Enum
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
        ///     Represents a session definition for an Enum type value.
        /// </summary>
        /// <typeparam name="T">The Enum type.</typeparam>
        public Type EnumType;

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     A class representing a session definition for an enum value.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
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
            T rReturn = (T)Enum.Parse(typeof(T), DefaultValue);
            string? tValue = _GetValue(httpContext);
            if (string.IsNullOrEmpty(tValue) == false)
            {
                rReturn = (T)Enum.Parse(typeof(T), tValue);
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
