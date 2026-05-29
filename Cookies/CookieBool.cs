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
    ///     Defines a strongly typed boolean cookie.
    /// </summary>
    public class CookieBool : CookieDefinition
    {
        #region Static methods

        /// <summary>
        ///     Gets a registered boolean cookie definition by name.
        /// </summary>
        /// <param name="name">The sanitized cookie definition name.</param>
        /// <returns>The registered definition, or <see langword="null" /> when no matching boolean cookie exists.</returns>
        public static CookieBool? GetCookieDefinition(string name)
        {
            CookieBool? result = null;
            if (CookieGlobal.KDictionary.ContainsKey(name))
            {
                result = (CookieBool)CookieGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CookieBool" /> class.
        /// </summary>
        /// <param name="name">The cookie name. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable cookie title.</param>
        /// <param name="description">The human-readable cookie explanation.</param>
        /// <param name="group">The cookie purpose group.</param>
        /// <param name="defaultValue">The default boolean value used when the cookie is absent or invalid.</param>
        /// <param name="deletable">A value indicating whether the cookie may be deleted by bulk deletion helpers.</param>
        /// <param name="manualEditable">A value indicating whether raw form rendering may expose a manual editor.</param>
        /// <param name="duration">The cookie duration value used by generated JavaScript cookie writers.</param>
        /// <param name="autoRenew">A value indicating whether reads should renew the cookie.</param>
        /// <param name="secure">A value indicating whether the cookie should be marked secure.</param>
        /// <param name="limitSite">The SameSite policy used when writing the cookie.</param>
        /// <remarks>
        ///     Functional and consent cookies are forced to be non-deletable and non-manually editable.
        ///     The definition is registered in <see cref="CookieGlobal.KDictionary" /> under the sanitized name.
        /// </remarks>
        public CookieBool(
            string name,
            string title,
            string description,
            CookieDefinitionGroup group,
            bool defaultValue,
            bool deletable = true,
            bool manualEditable = true,
            int duration = 365,
            bool autoRenew = false,
            bool secure = true,
            SameSiteMode limitSite = SameSiteMode.None
        )
        {
            Kind = CookieDefinitionKind.BoolKind;
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
        ///     Generates the cookie data string for a given value.
        /// </summary>
        /// <param name="value">The value to be converted to a cookie data string.</param>
        /// <returns>The generated cookie data string.</returns>
        public string GenerateCookieDataString(bool value)
        {
            return _GenerateCookieDataString(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Generates JavaScript code for setting a cookie value when a specific event is triggered.
        /// </summary>
        /// <param name="value">The value to be set in the cookie.</param>
        /// <returns>The JavaScript code for setting the cookie value.</returns>
        public string GenerateOnClick(bool value)
        {
            return _GenerateOnClick(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Retrieves the value of the cookie.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>
        ///     The value of the cookie as a boolean. If the cookie does not exist or cannot be parsed to a boolean, the
        ///     default value is returned.
        /// </returns>
        public bool GetValue(HttpContext? httpContext)
        {
            bool result = bool.Parse(DefaultValue);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false)
            {
                bool.TryParse(value, out result);
            }

            return result;
        }

        /// <summary>
        ///     Generates the raw HTML form for manually editing the boolean cookie.
        /// </summary>
        /// <param name="httpContext">The HttpContext object representing the current HTTP request.</param>
        /// <returns>The raw HTML form for the cookie definition.</returns>
        public override string RawForm(HttpContext? httpContext)
        {
            string rReturn = "<!-- " + nameof(CookieBool) + " RawForm-->";
            if (ManualEditable)
            {
                rReturn = rReturn + " <div class=\"form-check form-switch\">";
                if (GetValue(httpContext))
                {
                    rReturn = rReturn + " <input class=\"form-check-input\" type=\"checkbox\" onclick=\"" + InstallOnClick("false") + ";window.location.reload();\" checked>";
                }
                else
                {
                    rReturn = rReturn + " <input class=\"form-check-input\" type=\"checkbox\" onclick=\"" + InstallOnClick("true") + ";window.location.reload();\">";
                }

                rReturn = rReturn + "</div>";
            }

            return rReturn;
        }

        /// <summary>
        ///     Sets the value of the cookie.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="value">The value to set for the cookie.</param>
        public void SetValue(HttpContext? httpContext, bool value)
        {
            _SetValue(httpContext, value.ToString());
        }

        /// <summary>
        ///     Sets the cookie value with a custom lifetime.
        /// </summary>
        /// <param name="httpContext">The HTTP context used to write the response cookie.</param>
        /// <param name="value">The boolean value to serialize.</param>
        /// <param name="seconds">The number of seconds from now until the cookie expires.</param>
        public void SetValue(HttpContext? httpContext, bool value, double seconds)
        {
            _SetValue(httpContext, value.ToString(), seconds);
        }

        #endregion
    }
}