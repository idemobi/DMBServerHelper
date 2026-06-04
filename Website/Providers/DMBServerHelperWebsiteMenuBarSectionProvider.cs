#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.IO;
using DMBBootstrapBuilder;
using DMBServerHelperLabs.Navigation;
using Microsoft.AspNetCore.Mvc.Rendering;

#endregion

namespace DMBServerHelperWebsite;

internal sealed class DMBServerHelperWebsiteMenuBarSectionProvider : IMenuBarSectionProvider
{
    #region Instance fields and properties

    #region From interface IMenuBarSectionProvider

    public int Order => 100;

    #endregion

    #endregion

    #region Instance methods

    #region From interface IMenuBarSectionProvider

    public MenuBarModuleResult Build(TextWriter writer, IHtmlHelper html)
    {
        MenuBarModuleResult result = new();

        result.ActionList.Add(DMBServerHelperLabsNavigationAgent.CreateMenuGroup());

        return result;
    }

    public bool IsEnabled(IHtmlHelper html)
    {
        return true;
    }

    #endregion

    #endregion
}
