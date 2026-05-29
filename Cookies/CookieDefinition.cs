#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents the definition of a cookie.
    /// </summary>
    public abstract class CookieDefinition
    {
        #region Static fields and properties

        private static readonly Regex SpaceCleanerRgx = new Regex(@"\s+", RegexOptions.Compiled);

        #endregion

        #region Static methods

        /// <summary>
        ///     Removes all whitespace characters from a cookie definition name.
        /// </summary>
        /// <param name="sString">
        ///     The string to normalize.
        /// </param>
        /// <returns>
        ///     The input string without whitespace.
        /// </returns>
        public static string SpaceCleaner(string sString)
        {
            return SpaceCleanerRgx.Replace(sString, string.Empty);
        }

        #endregion

        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets a value indicating whether the cookie should be automatically renewed.
        /// </summary>
        public bool AutoRenew = false;

        /// <summary>
        ///     Represents the default value of a cookie definition.
        /// </summary>
        public string DefaultValue = string.Empty;

        /// <summary>
        ///     Represents whether a cookie definition can be deleted.
        /// </summary>
        public bool Deletable = true;

        /// <summary>
        ///     Represents the duration (in seconds) for which the cookie is valid.
        /// </summary>
        public int Duration = 3600;

        /// <summary>
        ///     Represents the explanation of a cookie definition.
        /// </summary>
        public string Explication = string.Empty;

        /// <summary>
        ///     Represents the consent or purpose group associated with the cookie definition.
        /// </summary>
        public CookieDefinitionGroup Group = CookieDefinitionGroup.Optional;

        /// <summary>
        ///     Represents the serialized value kind of the cookie definition.
        /// </summary>
        public CookieDefinitionKind Kind = CookieDefinitionKind.StringKind;

        /// <summary>
        ///     Represents the same site mode for a cookie.
        /// </summary>
        public SameSiteMode LimitSite = SameSiteMode.Strict;

        /// <summary>
        ///     Represents whether a cookie definition can be manually edited.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the cookie definition can be manually edited; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         If the <see cref="CookieDefinition.Group" /> is set to <see cref="CookieDefinitionGroup.Functional" />
        ///         or <see cref="CookieDefinitionGroup.Consent" />, the <see cref="ManualEditable" /> is automatically set to
        ///         <c>false</c>.
        ///     </para>
        ///     <seealso cref="CookieGlobal.KDictionary" />
        /// </remarks>
        public bool ManualEditable = false;

        /// <summary>
        ///     Represents the name of a cookie.
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        ///     Represents a flag indicating whether the cookie should be sent only over secure HTTPS connections.
        /// </summary>
        public bool Secure = true;

        /// <summary>
        ///     Represents the title of a cookie definition.
        /// </summary>
        public string Title = string.Empty;

        #endregion

        #region Instance methods

        /// <summary>
        ///     Generates a string representation of the cookie data based on the given value.
        /// </summary>
        /// <param name="value">The value to set for the cookie.</param>
        /// <returns>A formatted string representing the cookie data.</returns>
        protected string _GenerateCookieDataString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            string rReturn = Name + "=" + value + ";" +
                             " expires=" + DateTime.UtcNow.AddDays(Duration).ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture) + " GMT;" +
                             " path=/;" +
                             " samesite=" + LimitSite.ToString()
                             + "; Secure";
            return rReturn;
        }

        /// <summary>
        ///     Generates an "onclick" JavaScript code that sets the value of the specified cookie.
        /// </summary>
        /// <param name="value">The value to set for the cookie.</param>
        /// <returns>The generated JavaScript code.</returns>
        protected string _GenerateOnClick(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            string rReturn = "var now = new Date(); var year = now.getFullYear(); var month = now.getMonth(); var day = now.getDate(); var next = new Date(year, month, day+ " + Duration +
                             "); document.cookie = '" + Name + "=" + value + "; SameSite=" + LimitSite.ToString() + "; Path=/; expires=' + next.toUTCString() + '; Secure';";
            return rReturn;
        }

        /// <summary>
        ///     Retrieves the value of the specified cookie from the given HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext from which to retrieve the cookie value.</param>
        /// <returns>The value of the specified cookie, or null if the cookie does not exist.</returns>
        protected string? _GetValue(HttpContext? httpContext)
        {
            if (httpContext == null)
            {
                return null;
            }

            foreach (var cookie in httpContext.Request.Cookies)
            {
                if (cookie.Key != Name)
                {
                    continue;
                }

                if (AutoRenew)
                {
                    _SetValue(httpContext, cookie.Value);
                }

                return cookie.Value;
            }

            return null;
        }

        /// <summary>
        ///     Sets the value of the specified cookie.
        /// </summary>
        /// <param name="httpContext">The current HttpContext.</param>
        /// <param name="value">The value to set for the cookie.</param>
        protected void _SetValue(HttpContext? httpContext, string value)
        {
            _SetValue(httpContext, value, Duration * 25 * 3600);
        }

        /// <summary>
        ///     Sets the cookie value with a custom lifetime.
        /// </summary>
        /// <param name="httpContext">
        ///     The current HTTP context, or <see langword="null" /> to skip writing.
        /// </param>
        /// <param name="value">
        ///     The serialized cookie value to write.
        /// </param>
        /// <param name="seconds">
        ///     The number of seconds from now until the cookie expires.
        /// </param>
        protected void _SetValue(HttpContext? httpContext, string value, double seconds)
        {
            CookieOptions cookieOptions = new CookieOptions();
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            cookieOptions.Expires = DateTimeOffset.Now.AddSeconds(seconds);
            cookieOptions.SameSite = LimitSite;
            cookieOptions.Secure = Secure;
            if (httpContext != null)
            {
                httpContext.Response.Cookies.Append(Name, value, cookieOptions);
            }
        }

        /// <summary>
        ///     Deletes the specified cookie from the HTTP response.
        /// </summary>
        /// <param name="httpContext">The HttpContext containing the response that the cookie will be deleted from.</param>
        public void DeleteCookie(HttpContext? httpContext)
        {
            if (httpContext != null)
            {
                _SetValue(httpContext, DefaultValue);
                httpContext.Response.Cookies.Delete(Name);
            }
        }

        /// <summary>
        ///     Generates an "onclick" JavaScript code that deletes the specified cookie.
        /// </summary>
        /// <param name="value">The value to delete from the cookie. Default value is "delete".</param>
        /// <returns>The generated JavaScript code that deletes the cookie.</returns>
        public string DeleteOnClick(string value = "delete")
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            string rReturn = "document.cookie = '" + Name + "=" + value + "; SameSite=" + LimitSite.ToString() + "; Path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT; Secure';";
            return rReturn;
        }

        /// <summary>
        ///     Checks if a cookie with the specified name exists in the current HTTP context's request cookies.
        /// </summary>
        /// <param name="httpContext">The current HTTP context.</param>
        /// <returns>true if a cookie with the specified name exists, otherwise false.</returns>
        public bool Exists(HttpContext httpContext)
        {
            bool result = false;
            foreach (var tCookie in httpContext.Request.Cookies)
            {
                if (tCookie.Key == Name)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        ///     Generates a string representation of the cookie data based on the default value.
        /// </summary>
        /// <returns>A string that represents the cookie data using the default value.</returns>
        public string GenerateCookieJavascriptDefaultValue()
        {
            return _GenerateOnClick(DefaultValue);
        }

        /// <summary>
        ///     Retrieves the value of the cookie as a string.
        /// </summary>
        /// <param name="httpContext">The HttpContext object for the current request.</param>
        /// <param name="forHtml">A boolean value indicating whether the value should be formatted for HTML display.</param>
        /// <returns>The value of the cookie as a string.</returns>
        public string GetValueAsString(HttpContext? httpContext, bool forHtml = false)
        {
            if (forHtml)
            {
                string? value = _GetValue(httpContext);
                if (value != null)
                {
                    return "<span>" + Regex.Replace(value, ".{12}", "$0</span>&hairsp;<span>") + "</span>";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                string? value = _GetValue(httpContext);
                if (value != null)
                {
                    return value;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        ///     Generates JavaScript code to install a cookie with the specified configuration.
        /// </summary>
        /// <returns>The JavaScript code to install the cookie.</returns>
        public string InstallOnClick()
        {
            string result = "document.cookie = '" + Name + "=" + DefaultValue + "; SameSite=" + LimitSite.ToString() + "; Path=/; expires=" + DateTime.UtcNow.AddDays(Duration).ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture) + " GMT; Secure';";
            return result;
        }

        /// <summary>
        ///     Generates JavaScript code to install the cookie with the specified value.
        /// </summary>
        /// <param name="value">The value to set for the cookie.</param>
        /// <returns>A JavaScript code that sets the cookie with the specified value.</returns>
        public string InstallOnClick(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            string result = "document.cookie = '" + Name + "=" + value + "; SameSite=" + LimitSite.ToString() + "; Path=/; expires=" + DateTime.UtcNow.AddDays(Duration).ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture) + " GMT; Secure';";
            return result;
        }

        /// <summary>
        ///     Generates the raw form of the cookie definition without HTML encoding for the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The raw form of the cookie definition.</returns>
        public virtual string RawForm(HttpContext? httpContext)
        {
            string rReturn = "<!-- " + nameof(CookieDefinition) + " RawForm -->";
            return rReturn;
        }

        #endregion
    }
}