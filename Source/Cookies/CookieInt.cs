#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents a cookie definition for an integer value.
    /// </summary>
    public class CookieInt : CookieDefinition
    {
        #region Static methods

        /// <summary>
        ///     Gets the cookie definition with the specified name.
        /// </summary>
        /// <param name="name">The name of the cookie definition to retrieve.</param>
        /// <returns>The cookie definition with the specified name, if found; otherwise, null.</returns>
        public static CookieInt? GetCookieDefinition(string name)
        {
            CookieInt? result = null;
            if (CookieGlobal.KDictionary.ContainsKey(name))
            {
                result = (CookieInt)CookieGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a strongly typed integer cookie definition.
        /// </summary>
        /// <param name="name">The cookie name. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable cookie title.</param>
        /// <param name="description">The human-readable cookie explanation.</param>
        /// <param name="group">The cookie purpose group.</param>
        /// <param name="defaultValue">The default integer value used when the cookie is absent or invalid.</param>
        /// <param name="deletable">A value indicating whether the cookie may be deleted by bulk deletion helpers.</param>
        /// <param name="manualEditable">A value indicating whether raw form rendering may expose a manual editor.</param>
        /// <param name="duration">The cookie lifetime, in days, assigned to <see cref="CookieDefinition.Duration" />.</param>
        /// <param name="autoRenew">A value indicating whether reads should renew the cookie.</param>
        /// <param name="secure">A value indicating whether the cookie should be marked secure.</param>
        /// <param name="limitSite">The SameSite policy used when writing the cookie.</param>
        /// <remarks>
        ///     Functional and consent cookies are forced to be non-deletable and non-manually editable.
        ///     The definition is registered in <see cref="CookieGlobal.KDictionary" /> under the sanitized name.
        /// </remarks>
        public CookieInt(
            string name,
            string title,
            string description,
            CookieDefinitionGroup group,
            int defaultValue,
            bool deletable = true,
            bool manualEditable = true,
            int duration = 365,
            bool autoRenew = false,
            bool secure = true,
            SameSiteMode limitSite = SameSiteMode.None
        )
        {
            Kind = CookieDefinitionKind.IntKind;
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
        ///     Generates the cookie data string based on the given value.
        /// </summary>
        /// <param name="value">The value to set for the cookie.</param>
        /// <returns>A formatted string representing the cookie data.</returns>
        public string GenerateCookieDataString(int value)
        {
            return _GenerateCookieDataString(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Generates an "onclick" JavaScript code that sets the value of the specified cookie.
        /// </summary>
        /// <param name="value">The value to set for the cookie.</param>
        /// <returns>The generated JavaScript code.</returns>
        public string GenerateOnClick(int value)
        {
            return _GenerateOnClick(value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Retrieves the integer value of the cookie from the given HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext from which to retrieve the cookie value.</param>
        /// <returns>The integer value of the cookie.</returns>
        public int GetValue(HttpContext? httpContext)
        {
            int.TryParse(DefaultValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedValue))
            {
                result = parsedValue;
            }

            return result;
        }

        /// <summary>
        ///     Generates the raw form of the cookie definition without HTML encoding for the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The raw form of the cookie definition.</returns>
        [Obsolete("RawForm is obsolete and kept only for backward compatibility. Use a dedicated FormBuilder or admin component instead.")]
        public override string RawForm(HttpContext? httpContext)
        {
            string result = "<!-- " + nameof(CookieInt) + " RawForm-->";
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
        public void SetValue(HttpContext? httpContext, int value)
        {
            _SetValue(httpContext, value.ToString());
        }

        #endregion
    }
}
