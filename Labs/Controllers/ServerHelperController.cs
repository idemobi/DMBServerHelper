#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DMBServerHelperLabs.Controllers
{
    /// <summary>
    ///     Provides documentation pages for DMBServerHelper.
    /// </summary>
    public class ServerHelperController : RawBootstrapController
    {
        #region Instance methods

        /// <summary>
        ///     Renders the DMBServerHelper architecture page.
        /// </summary>
        /// <returns>The architecture view.</returns>
        public IActionResult Architecture()
        {
            SetTitle("DMBServerHelper - Architecture");
            SetDescription("DMBServerHelper architecture");
            SetKeywords("DMBServerHelper", "ServerHelper", "Architecture", "ASP.NET Core");
            return View();
        }

        /// <summary>
        ///     Renders the DMBServerHelper examples page.
        /// </summary>
        /// <returns>The examples view.</returns>
        public IActionResult Examples()
        {
            SetTitle("DMBServerHelper - Examples");
            SetDescription("DMBServerHelper examples for cookies, sessions, localization, and domain composition");
            SetKeywords("DMBServerHelper", "ServerHelper", "Examples", "Cookies", "Sessions", "Localization", "DomainComposite");
            return View();
        }

        /// <summary>
        ///     Renders the DMBServerHelper getting started page.
        /// </summary>
        /// <returns>The getting started view.</returns>
        public IActionResult GettingStarted()
        {
            SetTitle("DMBServerHelper - Getting Started");
            SetDescription("DMBServerHelper getting started guide");
            SetKeywords("DMBServerHelper", "ServerHelper", "Getting Started", "ASP.NET Core");
            return View();
        }

        /// <summary>
        ///     Renders the DMBServerHelper introduction page.
        /// </summary>
        /// <returns>The introduction view.</returns>
        public IActionResult Introduction()
        {
            SetTitle("DMBServerHelper - Introduction");
            SetDescription("DMBServerHelper overview");
            SetKeywords("DMBServerHelper", "ServerHelper", "Foundation", "ASP.NET Core");
            return View();
        }

        #endregion
    }
}
