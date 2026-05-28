#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj CookieLong.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Globalization;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Defines a strongly typed long integer cookie.
    /// </summary>
    public class CookieLong : CookieDefinition
    {
        #region Static methods

        /// <summary>
        ///     Retrieves the cookie definition for a given name.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <returns>The cookie definition for the given name, or null if no definition is found.</returns>
        public static CookieLong? GetCookieDefinition(string name)
        {
            CookieLong? result = null;
            if (CookieGlobal.KDictionary.ContainsKey(name))
            {
                result = (CookieLong)CookieGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a strongly typed long integer cookie definition.
        /// </summary>
        /// <param name="name">The cookie name. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable cookie title.</param>
        /// <param name="description">The human-readable cookie explanation.</param>
        /// <param name="group">The cookie purpose group.</param>
        /// <param name="defaultValue">The default long integer value used when the cookie is absent or invalid.</param>
        /// <param name="deletable">A value indicating whether the cookie may be deleted by bulk deletion helpers.</param>
        /// <param name="manualEditable">A value indicating whether raw form rendering may expose a manual editor.</param>
        /// <param name="duration">The duration value assigned to <see cref="CookieDefinition.Duration"/>.</param>
        /// <param name="autoRenew">A value indicating whether reads should renew the cookie.</param>
        /// <param name="secure">A value indicating whether the cookie should be marked secure.</param>
        /// <param name="limitSite">The SameSite policy used when writing the cookie.</param>
        /// <remarks>
        ///     Functional and consent cookies are forced to be non-deletable and non-manually editable.
        ///     The definition is registered in <see cref="CookieGlobal.KDictionary"/> under the sanitized name.
        /// </remarks>
        public CookieLong(
            string name,
            string title,
            string description,
            CookieDefinitionGroup group,
            long defaultValue,
            bool deletable = true,
            bool manualEditable = true,
            int duration = 365,
            bool autoRenew = false,
            bool secure = true,
            SameSiteMode limitSite = SameSiteMode.None
        )
        {
            Kind = CookieDefinitionKind.LongKind;
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
        public string GenerateCookieDataString(long value)
        {
            return _GenerateCookieDataString(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Generates an "onclick" JavaScript code that sets the value of the specified cookie.
        /// </summary>
        /// <param name="value">The value to set for the cookie.</param>
        /// <returns>The generated JavaScript code.</returns>
        public string GenerateOnClick(long value)
        {
            return _GenerateOnClick(value.ToString());
        }

        /// <summary>
        ///     Gets the value of the cookie as a long integer.
        /// </summary>
        /// <param name="httpContext">The HttpContext object representing the current request.</param>
        /// <returns>
        ///     The value of the cookie as a long integer. If the cookie does not exist or cannot be parsed as a long, the
        ///     default value is returned.
        /// </returns>
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
        ///     Generates the raw form of the cookie definition without HTML encoding for the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        ///     The raw form of the cookie definition.
        /// </returns>
        public override string RawForm(HttpContext? httpContext)
        {
            string result = "<!-- " + nameof(CookieLong) + " RawForm-->";
            if (ManualEditable)
            {
            }

            return result;
        }

        /// <summary>
        ///     Sets the value of a specific cookie to the provided long value.
        /// </summary>
        /// <param name="httpContext">The HttpContext object representing the current HTTP request.</param>
        /// <param name="value">The long value to be set for the cookie.</param>
        public void SetValue(HttpContext? httpContext, long value)
        {
            _SetValue(httpContext, value.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
