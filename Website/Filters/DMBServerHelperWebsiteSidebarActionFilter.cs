#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBPageBuilder;
using DMBServerHelperLabs.Navigation;
using Microsoft.AspNetCore.Mvc.Filters;

#endregion

namespace DMBServerHelperWebsite;

internal sealed class DMBServerHelperWebsiteSidebarActionFilter : IActionFilter
{
    #region Instance methods

    #region From interface IActionFilter

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is not RawBootstrapController controller)
        {
            return;
        }

        string? currentController = context.RouteData.Values["controller"]?.ToString();
        string? currentAction = context.RouteData.Values["action"]?.ToString();

        if (!string.Equals(currentController, "ServerHelper", System.StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        controller.SetSidebar(DMBServerHelperLabsNavigationAgent.CreateSidebar(currentController, currentAction));
        controller.AddBreadcrumb(
            ActionItemFactory.Url("Home", "/", IconStruct.Bootstrap("bi-house")),
            ActionItemFactory.AspRoute("ServerHelper", "Introduction")
                .SetTitle("DMBServerHelper")
                .SetIcon(IconStruct.Bootstrap("bi-server")),
            ActionItemFactory.AspRoute("ServerHelper", string.IsNullOrWhiteSpace(currentAction) ? "Introduction" : currentAction)
                .SetTitle(DMBServerHelperLabsNavigationAgent.ResolveActionTitle(currentAction))
                .SetIcon(DMBServerHelperLabsNavigationAgent.ResolveActionIcon(currentAction))
        );
    }

    #endregion

    #endregion
}
