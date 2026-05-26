#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj SessionGlobal.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     An abstract class representing a session definition.
    /// </summary>
    public abstract class SessionGlobal
    {
        #region Static fields and properties

        /// <summary>
        ///     Stores registered session definitions by sanitized session name.
        /// </summary>
        public static ConcurrentDictionary<string, SessionDefinition> KDictionary = new ConcurrentDictionary<string, SessionDefinition>();

        #endregion

        #region Static methods

        /// <summary>
        ///     Deletes all session variables from the provided HttpContext.
        /// </summary>
        /// <param name="httpContext">The HttpContext from which the session variables should be deleted.</param>
        public static void DeleteAllDeletableSession(HttpContext? httpContext)
        {
            // delete all session's information
            foreach (KeyValuePair<string, SessionDefinition> sessionKeyValue in SessionGlobal.KDictionary)
            {
                if (sessionKeyValue.Value.Deletable == true)
                {
                    sessionKeyValue.Value.DeleteFrom(httpContext);
                }
            }
        }

        #endregion
    }
}
