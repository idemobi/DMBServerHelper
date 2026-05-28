#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj CookieString.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Defines a strongly typed string cookie.
    /// </summary>
    public class CookieString : CookieDefinition
    {
        #region Static methods

        /// <summary>
        ///     Retrieves the cookie definition for the specified cookie name.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <returns>The cookie definition.</returns>
        public static CookieString? GetCookieDefinition(string name)
        {
            CookieString? result = null;
            if (CookieGlobal.KDictionary.ContainsKey(name))
            {
                result = (CookieString)CookieGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Represents a string cookie definition.
        /// </summary>
        /// <param name="name">The cookie name. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable cookie title.</param>
        /// <param name="description">The human-readable cookie explanation.</param>
        /// <param name="group">The cookie purpose group.</param>
        /// <param name="defaultValue">The default string value used when the cookie is absent.</param>
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
        public CookieString(
            string name,
            string title,
            string description,
            CookieDefinitionGroup group,
            string defaultValue,
            bool deletable = true,
            bool manualEditable = true,
            int duration = 365,
            bool autoRenew = false,
            bool secure = true,
            SameSiteMode limitSite = SameSiteMode.None
        )
        {
            Kind = CookieDefinitionKind.StringKind;
            Name = SpaceCleaner(name);
            Title = title;
            Explication = description;
            Group = group;
            Duration = duration;
            DefaultValue = defaultValue.Replace("'", "\'");
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
        public string GenerateCookieDataString(string value)
        {
            return _GenerateCookieDataString(value);
        }

        /// <summary>
        ///     Generates an "onclick" JavaScript code that sets the value of the specified cookie.
        /// </summary>
        /// <param name="value">The value to set for the cookie.</param>
        /// <returns>The generated JavaScript code.</returns>
        public string GenerateOnClick(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            return _GenerateOnClick(value.Replace("'", "\'"));
        }

        /// <summary>
        ///     Gets the current string cookie value.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>The cookie value, or the configured default value when the cookie is absent.</returns>
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
        ///     Generates the raw form of the cookie definition without HTML encoding for the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The raw form of the cookie definition.</returns>
        public override string RawForm(HttpContext? httpContext)
        {
            string result = "<!-- " + nameof(CookieString) + " RawForm -->";
            if (ManualEditable)
            {
            }

            return result;
        }

        /// <summary>
        ///     Sets the value of the cookie associated with the specified HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext to associate the cookie with.</param>
        /// <param name="value">The value to be set in the cookie. If empty or null, an empty string will be set.</param>
        public void SetValue(HttpContext? httpContext, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            _SetValue(httpContext, value.Replace("'", "\'"));
        }

        /// <summary>
        ///     Sets the value of the cookie associated with the specified HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext to associate the cookie with.</param>
        /// <param name="value">The value to be set in the cookie. If empty or null, an empty string will be set.</param>
        /// <param name="seconds">The number of seconds from now until the cookie expires.</param>
        public void SetValue(HttpContext? httpContext, string? value, double seconds)
        {
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            _SetValue(httpContext, value.Replace("'", "\'"), seconds);
        }

        #endregion
    }
}
