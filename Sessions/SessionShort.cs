#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj SessionShort.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Globalization;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Defines a strongly typed short integer session value.
    /// </summary>
    public class SessionShort : SessionDefinition
    {
        #region Static methods

        /// <summary>
        ///     Retrieves the session definition with the given name.
        /// </summary>
        /// <param name="name">The name of the session definition to retrieve.</param>
        /// <returns>The session definition with the given name if found; otherwise, returns null.</returns>
        public static SessionShort? GetSessionDefinition(string name)
        {
            SessionShort? result = null;
            if (SessionGlobal.KDictionary.ContainsKey(name))
            {
                result = (SessionShort)SessionGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Represents a short session definition.
        /// </summary>
        public SessionShort(
            string name,
            string title,
            string description,
            SessionDefinitionGroup group,
            short defaultValue,
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
        ///     Gets the value of this session definition.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The value of the session definition.</returns>
        public short GetValue(HttpContext? httpContext)
        {
            short result = short.Parse(DefaultValue);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false)
            {
                short.TryParse(value, out result);
            }

            return result;
        }

        /// <summary>
        ///     Increments the value of the session variable by a specified amount.
        /// </summary>
        /// <param name="httpContext">The HTTP context of the session.</param>
        /// <param name="increment">The amount to increment the value by (default is 1).</param>
        public void IncrementValue(HttpContext? httpContext, short increment = 1)
        {
            _SetValue(httpContext, (GetValue(httpContext) + increment).ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Sets the value of the session definition to the specified value.
        /// </summary>
        /// <param name="httpContext">The HttpContext object. If null, uses the default context.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(HttpContext? httpContext, short value)
        {
            _SetValue(httpContext, value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
