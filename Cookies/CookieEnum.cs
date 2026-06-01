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
    ///     Represents a cookie definition for an enumeration type value.
    /// </summary>
    /// <typeparam name="T">The enumeration type</typeparam>
    public class CookieEnum<T> : CookieDefinition where T : Enum
    {
        #region Static methods

        /// <summary>
        ///     Retrieves the cookie definition for a given name.
        /// </summary>
        /// <param name="name">The name of the cookie definition.</param>
        /// <returns>The cookie definition if found, null otherwise.</returns>
        public static CookieEnum<T>? GetCookieDefinition(string name)
        {
            CookieEnum<T>? result = null;
            if (CookieGlobal.KDictionary.ContainsKey(name))
            {
                result = (CookieEnum<T>)CookieGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance fields and properties

        /// <summary>
        ///     Represents a cookie definition for an enumeration type.
        /// </summary>
        private Type _EnumType;

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Represents a cookie with an enumerated value.
        /// </summary>
        /// <param name="name">The cookie name. Whitespace is removed before registration.</param>
        /// <param name="title">The human-readable cookie title.</param>
        /// <param name="description">The human-readable cookie explanation.</param>
        /// <param name="group">The cookie purpose group.</param>
        /// <param name="defaultValue">The default enum value used when the cookie is absent.</param>
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
        public CookieEnum(
            string name,
            string title,
            string description,
            CookieDefinitionGroup group,
            T defaultValue,
            bool deletable = true,
            bool manualEditable = true,
            int duration = 365,
            bool autoRenew = false,
            bool secure = true,
            SameSiteMode limitSite = SameSiteMode.None
        )
        {
            _EnumType = typeof(T);
            Kind = CookieDefinitionKind.EnumKind;
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
        public string GenerateCookieDataString(T value)
        {
            return _GenerateCookieDataString(value.ToString());
        }

        /// <summary>
        ///     Generates an "onclick" JavaScript code that sets the value of the specified cookie.
        /// </summary>
        /// <param name="value">The value to set for the cookie.</param>
        /// <returns>The generated JavaScript code.</returns>
        public string GenerateOnClick(T value)
        {
            return _GenerateOnClick(value.ToString());
        }

        /// <summary>
        ///     Retrieves the value of the specified cookie.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>The value of the cookie as an enumeration.</returns>
        public T GetValue(HttpContext? httpContext)
        {
            Enum.TryParse(DefaultValue, out T result);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false && Enum.TryParse(value, out T parsedValue))
            {
                result = parsedValue;
            }

            return result;
        }

        /// <summary>
        ///     Generates the raw HTML form for manually editing the enum cookie.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>The raw HTML form for the enum cookie definition.</returns>
        public override string RawForm(HttpContext? httpContext)
        {
            string expires = DateTime.UtcNow.AddDays(Duration).ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture) + " GMT";
            string resultJavascript = GenerateDocumentCookieScriptForValueExpression("this.value", expires);
            string result = "<!-- " + nameof(CookieEnum<T>) + " RawForm-->";
            if (ManualEditable)
            {
                T value = GetValue(httpContext);
                result = result + "<select class=\"btn-primary form-select form-select-lg\" onchange=\"" + resultJavascript + ";window.location.reload();\"> ";
                foreach (T enumValue in Enum.GetValues(typeof(T)))
                {
                    if (value.ToString() == enumValue.ToString())
                    {
                        result = result + "<option value=\"" + enumValue.ToString() + "\" selected>" + enumValue.ToString() + "</option>";
                    }
                    else
                    {
                        result = result + "<option value=\"" + enumValue.ToString() + "\">" + enumValue.ToString() + "</option>";
                    }
                }

                result = result + "</select>";
            }

            return result;
        }

        /// <summary>
        ///     Sets the value of the cookie.
        /// </summary>
        /// <param name="httpContext">The HttpContext used to set the cookie value.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(HttpContext? httpContext, T value)
        {
            _SetValue(httpContext, value.ToString());
        }

        #endregion
    }
}
