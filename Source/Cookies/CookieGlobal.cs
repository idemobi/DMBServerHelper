#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Stores the global registry of cookie definitions.
    /// </summary>
    public abstract class CookieGlobal
    {
        #region Static fields and properties

        /// <summary>
        ///     Stores registered cookie definitions by sanitized cookie name.
        /// </summary>
        public static readonly ConcurrentDictionary<string, CookieDefinition> KDictionary = new ConcurrentDictionary<string, CookieDefinition>();

        #endregion

        #region Static methods

        /// <summary>
        ///     Deletes all cookies that are not in the Functional group and marked as deletable.
        /// </summary>
        /// <param name="httpContext">The HttpContext object representing the current request.</param>
        public static void DeleteAllCookie(HttpContext? httpContext)
        {
            foreach (KeyValuePair<string, CookieDefinition> cookieKeyValue in KDictionary)
            {
                if (cookieKeyValue.Value.Group != CookieDefinitionGroup.Functional)
                {
                    if (cookieKeyValue.Value.Deletable == true)
                    {
                        cookieKeyValue.Value.DeleteCookie(httpContext);
                    }
                }
            }
        }

        #endregion
    }
}