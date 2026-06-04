#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBComponentBuilder;
using DMBEffectBuilder;
using DMBPageBuilder;
using DMBServerHelper;
using DMBServerHelperLabs.Controllers;
using DMBServerHelperWebsite;
using DMBServerWebHelper;

#endregion

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ServerHelperConfiguration.LoadCommonConfig(builder);
ServerHelperConfiguration.Config.CookiePrefix = "DSH";
ServerWebHelperConfiguration.LoadCommonConfig(builder);
PageBuilderConfiguration.LoadCommonConfig(builder);
BootstrapBuilderConfiguration.LoadCommonConfig(builder);
ComponentBuilderConfiguration.LoadCommonConfig(builder);
EffectBuilderConfiguration.LoadCommonConfig(builder);

var mvcBuilder = builder.Services.AddControllersWithViews();
mvcBuilder.AddApplicationPart(typeof(ServerHelperController).Assembly);
mvcBuilder.AddMvcOptions(options => options.Filters.Add(new DMBServerHelperWebsiteSidebarActionFilter()));

builder.Services.AddTransient<IMenuBarSectionProvider, DMBServerHelperWebsiteMenuBarSectionProvider>();
builder.Services.AddTransient<IProfileBarSectionProvider, ThemeBarSectionProvider>();
builder.Services.AddTransient<IProfileBarSectionProvider, DebugBarSectionProvider>();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

ServerWebHelperConfiguration.UseApp(app);

app.MapGet("/", context =>
{
    context.Response.Redirect("/ServerHelper/Introduction");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ServerHelper}/{action=Introduction}/{id?}");

app.Run();
