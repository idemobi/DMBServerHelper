#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBPageBuilder;

#endregion

namespace DMBServerHelperLabs.Navigation;

/// <summary>
///     Provides reusable navigation fragments for DMBServerHelper labs hosts.
/// </summary>
/// <remarks>
///     The agent only builds DMBServerHelper-specific menu and sidebar fragments. Host websites remain
///     responsible for assembling these fragments into their own navbar providers, sidebar filters, and
///     global navigation structures.
/// </remarks>
public static class DMBServerHelperLabsNavigationAgent
{
    #region Static methods

    /// <summary>
    ///     Creates an action item for a DMBServerHelper labs page.
    /// </summary>
    /// <param name="action">The MVC action name on the <c>ServerHelper</c> controller.</param>
    /// <param name="title">The action title shown in navigation UI.</param>
    /// <param name="icon">The Bootstrap Icons CSS class used by the action.</param>
    /// <param name="currentController">The current MVC controller name used to mark the action active.</param>
    /// <param name="currentAction">The current MVC action name used to mark the action active.</param>
    /// <returns>The configured <see cref="AspRouteActionItem" />.</returns>
    public static AspRouteActionItem CreateAction(
        string action,
        string title,
        string icon,
        string? currentController = null,
        string? currentAction = null
    )
    {
        bool active =
            string.Equals(currentController, "ServerHelper", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase);

        return ActionItemFactory.AspRoute("ServerHelper", action)
            .SetTitle(title)
            .SetIcon(IconStruct.Bootstrap(icon))
            .SetActive(active);
    }

    /// <summary>
    ///     Creates the DMBServerHelper navbar menu group.
    /// </summary>
    /// <returns>The configured <see cref="GroupActionItem" /> containing DMBServerHelper labs page links.</returns>
    public static GroupActionItem CreateMenuGroup()
    {
        return ActionItemFactory.Group("DMBServerHelper", IconStruct.Bootstrap("bi-server"))
            .AddItems(
                CreateAction("Introduction", "Introduction", "bi-info-circle"),
                CreateAction("GettingStarted", "Getting Started", "bi-play-circle"),
                CreateAction("Architecture", "Architecture", "bi-diagram-3"),
                CreateAction("Examples", "Examples", "bi-code-square")
            );
    }

    /// <summary>
    ///     Creates the DMBServerHelper sidebar component.
    /// </summary>
    /// <param name="currentController">The current MVC controller name used to mark the active item.</param>
    /// <param name="currentAction">The current MVC action name used to mark the active item.</param>
    /// <param name="sidebarId">The HTML identifier applied to the sidebar component.</param>
    /// <param name="localStorageKey">The browser local-storage key used for sidebar state.</param>
    /// <returns>The configured <see cref="SideBarComponent" />.</returns>
    public static SideBarComponent CreateSidebar(
        string? currentController,
        string? currentAction,
        string sidebarId = "server_helper_sidebar",
        string localStorageKey = "dmbserverhelper.labs.sidebar"
    )
    {
        SideBarComponent sidebar = new SideBarComponent()
            .WithId(sidebarId)
            .WithLocalStorageKey(localStorageKey)
            .WithAutoExpandActivePath()
            .WithRememberExpandedState();

        sidebar.AddSection(CreateSidebarSection(currentController, currentAction));

        return sidebar;
    }

    /// <summary>
    ///     Creates the DMBServerHelper sidebar section.
    /// </summary>
    /// <param name="currentController">The current MVC controller name used to mark the active item.</param>
    /// <param name="currentAction">The current MVC action name used to mark the active item.</param>
    /// <returns>The configured <see cref="SideBarSectionComponent" />.</returns>
    public static SideBarSectionComponent CreateSidebarSection(string? currentController, string? currentAction)
    {
        return new SideBarSectionComponent("DMBServerHelper")
            .Add(ActionItemFactory.Group("General", IconStruct.Bootstrap("bi-info-circle"))
                .AddItems(
                    CreateAction("Introduction", "Introduction", "bi-info-circle", currentController, currentAction),
                    CreateAction("GettingStarted", "Getting Started", "bi-play-circle", currentController, currentAction),
                    CreateAction("Architecture", "Architecture", "bi-diagram-3", currentController, currentAction),
                    CreateAction("Examples", "Examples", "bi-code-square", currentController, currentAction)
                ));
    }

    /// <summary>
    ///     Resolves the Bootstrap icon for a DMBServerHelper labs action.
    /// </summary>
    /// <param name="actionName">The MVC action name to resolve.</param>
    /// <returns>The icon value represented as an <see cref="IconStruct" />.</returns>
    public static IconStruct ResolveActionIcon(string? actionName)
    {
        return actionName switch
        {
            "GettingStarted" => IconStruct.Bootstrap("bi-play-circle"),
            "Architecture" => IconStruct.Bootstrap("bi-diagram-3"),
            "Examples" => IconStruct.Bootstrap("bi-code-square"),
            _ => IconStruct.Bootstrap("bi-info-circle")
        };
    }

    /// <summary>
    ///     Resolves the display title for a DMBServerHelper labs action.
    /// </summary>
    /// <param name="actionName">The MVC action name to resolve.</param>
    /// <returns>The display title for the action.</returns>
    public static string ResolveActionTitle(string? actionName)
    {
        return actionName switch
        {
            "GettingStarted" => "Getting Started",
            "Architecture" => "Architecture",
            "Examples" => "Examples",
            _ => "Introduction"
        };
    }

    #endregion
}