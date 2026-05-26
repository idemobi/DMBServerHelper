#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj CookieUShort.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Globalization;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Defines a strongly typed unsigned short integer cookie.
    /// </summary>
    public class CookieUShort : CookieDefinition
    {
        #region Static methods

        /// <summary>
        ///     Gets a registered unsigned short cookie definition by name.
        /// </summary>
        /// <param name="name">The name of the cookie definition to retrieve.</param>
        /// <returns>The registered definition, or <see langword="null"/> when no matching definition exists.</returns>
        public static CookieUShort? GetCookieDefinition(string name)
        {
            CookieUShort? result = null;
            if (CookieGlobal.KDictionary.ContainsKey(name))
            {
                result = (CookieUShort)CookieGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Represents a cookie definition for an unsigned short integer value.
        /// </summary>
        public CookieUShort(
            string name,
            string title,
            string description,
            CookieDefinitionGroup group,
            ushort defaultValue,
            bool deletable = true,
            bool manualEditable = true,
            int duration = 365,
            bool autoRenew = false,
            bool secure = true,
            SameSiteMode limitSite = SameSiteMode.None
        )
        {
            Kind = CookieDefinitionKind.UIntKind;
            Name = SpaceCleaner(name);
            Title = title;
            Explication = description;
            Group = group;
            Duration = duration;
            DefaultValue = defaultValue.ToString();
            LimitSite = limitSite;
            Secure = secure;
            AutoRenew = autoRenew;
            Deletable = deletable;
            ManualEditable = manualEditable;
            if (group == CookieDefinitionGroup.Functional || group == CookieDefinitionGroup.Consent)
            {
                Deletable = false;
                ManualEditable = false;
            }

            if (CookieGlobal.KDictionary.ContainsKey(Name))
            {
                CookieGlobal.KDictionary[Name] = this;
            }
            else
            {
                CookieGlobal.KDictionary.TryAdd(Name, this);
            }
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Generates a string representation of the cookie data based on the given value.
        /// </summary>
        /// <param name="value">The value to set for the cookie.</param>
        /// <returns>A formatted string representing the cookie data.</returns>
        public string GenerateCookieDataString(uint value)
        {
            return _GenerateCookieDataString(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Generates an "onclick" JavaScript code that sets the value of the specified cookie.
        /// </summary>
        /// <param name="value">The value to set for the cookie.</param>
        /// <returns>The generated JavaScript code.</returns>
        public string GenerateOnClick(uint value)
        {
            return _GenerateOnClick(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Retrieves the value of the cookie from the given HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext from which to retrieve the cookie value.</param>
        /// <returns>
        ///     The value of the cookie as a ushort, or the default value if the cookie does not exist or cannot be parsed as
        ///     a ushort.
        /// </returns>
        public ushort GetValue(HttpContext? httpContext)
        {
            ushort result = ushort.Parse(DefaultValue);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false)
            {
                ushort.TryParse(value, out result);
            }

            return result;
        }

        /// <summary>
        ///     Generates the raw form of the cookie definition without HTML encoding for the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The raw form of the cookie definition.</returns>
        public override string RawForm(HttpContext? httpContext)
        {
            string result = "<!-- " + nameof(CookieUShort) + " RawForm-->";
            if (ManualEditable)
            {
            }

            return result;
        }

        /// <summary>
        ///     Sets the value of the specified cookie.
        /// </summary>
        /// <param name="httpContext">The current HttpContext.</param>
        /// <param name="value">The value to set for the cookie.</param>
        public void SetValue(HttpContext? httpContext, uint value)
        {
            _SetValue(httpContext, value.ToString());
        }

        #endregion
    }
}
