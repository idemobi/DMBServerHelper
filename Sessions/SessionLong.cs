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
    ///     Represents a long session definition.
    /// </summary>
    public class SessionLong : SessionDefinition
    {
        #region Static methods

        /// <summary>
        ///     Gets a registered long integer session definition by name.
        /// </summary>
        /// <param name="name">The name of the session definition.</param>
        /// <returns>The registered definition, or <see langword="null" /> when no matching definition exists.</returns>
        public static SessionLong? GetSessionDefinition(string name)
        {
            SessionLong? result = null;
            if (SessionGlobal.KDictionary.ContainsKey(name))
            {
                result = (SessionLong)SessionGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a strongly typed long integer session definition.
        /// </summary>
        /// <param name="name">The name of the session.</param>
        /// <param name="title">The title of the session.</param>
        /// <param name="description">The description of the session.</param>
        /// <param name="group">The group of the session.</param>
        /// <param name="defaultValue">The default value of the session.</param>
        /// <param name="deletable">Optional parameter, indicating if the session can be deleted.</param>
        /// <param name="manualEditable">Optional parameter, indicating if the session can be manually edited.</param>
        public SessionLong(
            string name,
            string title,
            string description,
            SessionDefinitionGroup group,
            long defaultValue,
            bool deletable = false,
            bool manualEditable = false
        )
        {
            Kind = SessionDefinitionKind.LongKind;
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
        ///     Gets the value of the session.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>The value of the session as a long.</returns>
        public long GetValue(HttpContext? httpContext)
        {
            long result = long.Parse(DefaultValue);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false)
            {
                long.TryParse(value, out result);
            }

            return result;
        }

        /// <summary>
        ///     Increments the value of the session by a specified amount.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <param name="increment">The amount by which the value should be incremented. Default value is 1.</param>
        public void IncrementValue(HttpContext? httpContext, long increment = 1)
        {
            _SetValue(httpContext, (GetValue(httpContext) + increment).ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Sets the value of the session to the specified long value.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext" /> object representing the current HTTP request.</param>
        /// <param name="value">The long value to set as the session value.</param>
        public void SetValue(HttpContext? httpContext, long value)
        {
            _SetValue(httpContext, value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}