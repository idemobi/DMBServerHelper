#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj CookieInt.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

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
            int result = int.Parse(DefaultValue);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false)
            {
                int.TryParse(value, out result);
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
