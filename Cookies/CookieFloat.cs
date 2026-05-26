#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj CookieFloat.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Globalization;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Defines a strongly typed floating-point cookie.
    /// </summary>
    public class CookieFloat : CookieDefinition
    {
        #region Constants

        /// <summary>
        ///     Represents the format used for storing float values in a cookie.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <see cref="K_RecordFormat" /> is a constant field used in the <see cref="CookieFloat" /> class that
        ///         specifies the format for float values.
        ///     </para>
        ///     <para>
        ///         This format is used when setting or getting the value of a float cookie.
        ///     </para>
        ///     <para>
        ///         Usage:
        ///         <code>
        /// float myValue = GetValueFromCookie(); // Get the float value from the cookie
        /// string formattedValue = myValue.ToString(K_RecordFormat); // Format the float value using the <see
        ///                 cref="K_RecordFormat" /> constant
        /// SetValueToCookie(formattedValue); // Set the formatted value to the cookie
        /// </code>
        ///     </para>
        ///     <para>
        ///         Note: The <see cref="K_RecordFormat" /> field is defined internally in the <see cref="CookieFloat" /> class and
        ///         is not intended to be modified or accessed outside of the class.
        ///     </para>
        /// </remarks>
        private const string K_RecordFormat = "0.00000";

        #endregion

        #region Static methods

        /// <summary>
        ///     Retrieves the cookie definition with the specified name.
        /// </summary>
        /// <param name="name">The name of the cookie definition to retrieve.</param>
        /// <returns>The cookie definition with the specified name, or null if not found.</returns>
        public static CookieFloat? GetCookieDefinition(string name)
        {
            CookieFloat? result = null;
            if (CookieGlobal.KDictionary.ContainsKey(name))
            {
                result = (CookieFloat)CookieGlobal.KDictionary[name];
            }

            return result;
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a strongly typed floating-point cookie definition.
        /// </summary>
        public CookieFloat(
            string name,
            string title,
            string description,
            CookieDefinitionGroup group,
            float defaultValue,
            bool deletable = true,
            bool manualEditable = true,
            int duration = 365,
            bool autoRenew = false,
            bool secure = true,
            SameSiteMode limitSite = SameSiteMode.None
        )
        {
            Kind = CookieDefinitionKind.FloatKind;
            Name = SpaceCleaner(name);
            Title = title;
            Explication = description;
            Group = group;
            Duration = duration;
            DefaultValue = defaultValue.ToString(K_RecordFormat, CultureInfo.InvariantCulture);
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
        public string GenerateCookieDataString(float value)
        {
            return _GenerateCookieDataString(value.ToString(K_RecordFormat, CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Generates an "onclick" JavaScript code that sets the value of the specified cookie.
        /// </summary>
        /// <param name="value">The value to set for the cookie.</param>
        /// <returns>The generated JavaScript code.</returns>
        public string GenerateOnClick(float value)
        {
            return _GenerateOnClick(value.ToString(K_RecordFormat, CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Retrieves the value of the cookie from the specified HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext from which to retrieve the cookie value.</param>
        /// <returns>
        ///     The value of the cookie as a float, or the default value if the cookie does not exist or cannot be parsed to a
        ///     float.
        /// </returns>
        public float GetValue(HttpContext? httpContext)
        {
            float result = float.Parse(DefaultValue);
            string? value = _GetValue(httpContext);
            if (string.IsNullOrEmpty(value) == false)
            {
                float.TryParse(value, out result);
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
            string result = "<!-- " + nameof(CookieFloat) + " RawForm-->";
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
        public void SetValue(HttpContext? httpContext, float value)
        {
            _SetValue(httpContext, value.ToString(K_RecordFormat, CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
