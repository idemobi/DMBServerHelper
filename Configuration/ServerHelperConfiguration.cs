#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text.Json.Serialization;
using Azure.Identity;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Provides the default server configuration shared by PageBuilder ecosystem packages.
    /// </summary>
    /// <remarks>
    ///     The configuration normalizes supported languages, analyzes the configured domain, optionally
    ///     configures Azure Blob Storage data protection persistence, and exposes helper methods for
    ///     composing canonical HTTPS URLs.
    /// </remarks>
    [Serializable]
    public class ServerHelperConfiguration : GenericConfiguration<ServerHelperConfiguration>
    {
        #region Static methods

        #if NUGET
        /// <summary>
        ///     Indicates that the current build was produced as a NuGet package.
        /// </summary>
        public const bool IsNuGetBuild = true;
        /// <summary>
        ///     Describes the current build mode.
        /// </summary>
        public const string BuildMode = "Package NuGet";
        #elif RELEASE
        /// <summary>
        ///     Indicates whether the current build was produced as a NuGet package.
        /// </summary>
        public const bool IsNuGetBuild = false;
        /// <summary>
        ///     Describes the current build mode.
        /// </summary>
        public const string BuildMode = "Direct ProjectReference mode Release";
        #elif DEBUG
        /// <summary>
        ///     Indicates whether the current build was produced as a NuGet package.
        /// </summary>
        public const bool IsNuGetBuild = false;

        /// <summary>
        ///     Describes the current build mode.
        /// </summary>
        public const string BuildMode = "Direct ProjectReference mode Debug";
        #else
        /// <summary>
        ///     Indicates whether the current build was produced as a NuGet package.
        /// </summary>
        public const bool IsNuGetBuild = false;
        /// <summary>
        ///     Describes the current build mode.
        /// </summary>
        public const string BuildMode = "Direct ProjectReference mode unknown";
        #endif
        /// <summary>
        ///     Indicates whether the current build was compiled with the <c>DEBUG</c> symbol.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> in debug builds; otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsDebug()
        {
            #if DEBUG
            return true;
            #else
            return false;
            #endif
        }

        private static string ComposePath(params string?[] segments)
        {
            return string.Join("/", segments.Select(EncodeUriComponent));
        }

        private static string EncodeQueryString(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                return string.Empty;
            }

            List<string> queryParameters = new List<string>();
            foreach (string queryParameter in queryString.Split('&'))
            {
                int separatorIndex = queryParameter.IndexOf('=');
                if (separatorIndex < 0)
                {
                    queryParameters.Add(EncodeUriComponent(queryParameter));
                    continue;
                }

                string key = queryParameter[..separatorIndex];
                string value = queryParameter[(separatorIndex + 1)..];
                queryParameters.Add($"{EncodeUriComponent(key)}={EncodeUriComponent(value)}");
            }

            return string.Join("&", queryParameters);
        }

        private static string EncodeRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            string relativePath = path.TrimStart('/');
            string fragment = string.Empty;
            int fragmentIndex = relativePath.IndexOf('#');
            if (fragmentIndex >= 0)
            {
                fragment = relativePath[(fragmentIndex + 1)..];
                relativePath = relativePath[..fragmentIndex];
            }

            string queryString = string.Empty;
            int queryIndex = relativePath.IndexOf('?');
            if (queryIndex >= 0)
            {
                queryString = relativePath[(queryIndex + 1)..];
                relativePath = relativePath[..queryIndex];
            }

            string encodedPath = string.Join("/", relativePath.Split('/').Select(EncodeUriComponent));
            string encodedQueryString = EncodeQueryString(queryString);
            string encodedFragment = EncodeUriComponent(fragment);

            return encodedPath
                   + (queryIndex >= 0 ? "?" + encodedQueryString : string.Empty)
                   + (fragmentIndex >= 0 ? "#" + encodedFragment : string.Empty);
        }

        private static string EncodeUriComponent(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            try
            {
                return Uri.EscapeDataString(Uri.UnescapeDataString(value));
            }
            catch (UriFormatException)
            {
                return Uri.EscapeDataString(value);
            }
        }

        #endregion

        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets a value indicating whether generated URLs may include the configured launch token.
        /// </summary>
        public bool AddLaunchToken { set; get; } = true;

        /// <summary>
        ///     Gets or sets the default culture name used by server-side helpers.
        /// </summary>
        /// <value>
        ///     The default value is <c>en-US</c>.
        /// </value>
        public string BaseLanguage { set; get; } = "en-US";

        /// <summary>
        ///     Gets or sets the prefix added by consumers to shared cookie names.
        /// </summary>
        public string CookiePrefix { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Azure Blob Storage container URL used to persist data protection keys.
        /// </summary>
        /// <remarks>
        ///     When this value starts with <c>https://</c>, <see cref="AfterConfiguration" /> configures
        ///     ASP.NET Core data protection to persist keys to a blob named from the analyzed domain.
        /// </remarks>
        public string DataProtectionBlobUrl { set; get; } = string.Empty;

        [JsonIgnore] private DomainComposite DomainAnalyzed { set; get; } = new DomainComposite(string.Empty);

        /// <summary>
        ///     Gets or sets the configured domain name or URL used for domain analysis and URL composition.
        /// </summary>
        public string DomainName { set; get; } = string.Empty;

        /// <summary>
        ///     Gets or sets the launch token value generated at configuration creation time.
        /// </summary>
        public string LaunchToken { set; get; } = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        /// <summary>
        ///     Gets or sets a value indicating whether domain analysis may refresh the Public Suffix List online.
        /// </summary>
        /// <remarks>
        ///     When enabled, <see cref="DomainComposite" /> uses Nager.PublicSuffix with its cached HTTP rule provider.
        ///     If the refresh fails, domain analysis falls back to the built-in parser.
        /// </remarks>
        public bool PublicSuffixListOnlineRefreshEnabled { set; get; } = true;

        /// <summary>
        ///     Gets or sets the maximum number of seconds allowed for the online Public Suffix List refresh.
        /// </summary>
        public int PublicSuffixListRefreshTimeoutSeconds { set; get; } = 3;

        /// <summary>
        ///     Stores the session cookie name used by web consumers.
        /// </summary>
        [NonSerialized] public string SessionCookieName = "NODNS";

        /// <summary>
        ///     Gets or sets the list of supported culture names.
        /// </summary>
        /// <value>
        ///     The default value contains <c>en-US</c> and <c>fr-FR</c>. Debug builds add <c>tlh</c>
        ///     during post-configuration when it is missing.
        /// </value>
        public List<string>? SupportLanguages { set; get; } = new List<string> { "en-US", "fr-FR" };

        #endregion

        #region Instance methods

        /// <summary>
        ///     Registers the shared static file middleware for an ASP.NET Core application.
        /// </summary>
        /// <param name="app">
        ///     The web application whose pipeline receives static file middleware.
        /// </param>
        public static void UseApp(WebApplication app)
        {
            //if (LibrariesWorkflow.SetModuleInstalledByExpression(() => app.UseStaticFiles(), nameof(ServerHelperConfiguration)))
            {
                Console.WriteLine($"The method 'UseStaticFiles' is installed by '{nameof(ServerHelperConfiguration)}'.");
                app.UseStaticFiles();
            }
        }

        /// <summary>
        ///     Normalizes server configuration values and registers optional data protection persistence.
        /// </summary>
        /// <param name="appBuilder">
        ///     The host application builder whose services may receive data protection registration.
        /// </param>
        /// <param name="configBuilder">
        ///     The configuration builder supplied by the loading lifecycle.
        /// </param>
        /// <param name="configRoot">
        ///     The configuration root supplied by the loading lifecycle.
        /// </param>
        /// <remarks>
        ///     The method removes empty supported language values, de-duplicates language names
        ///     case-insensitively, analyzes <see cref="DomainName" />, and configures data protection key
        ///     persistence when <see cref="DataProtectionBlobUrl" /> is an HTTPS URL.
        /// </remarks>
        public override void AfterConfiguration(IHostApplicationBuilder appBuilder, IConfigurationBuilder configBuilder, IConfigurationRoot configRoot)
        {
            SupportLanguages ??= new List<string>();
            #if DEBUG
            if (SupportLanguages.Contains("tlh", StringComparer.OrdinalIgnoreCase) == false)
            {
                SupportLanguages.Add("tlh");
            }
            #endif
            SupportLanguages = SupportLanguages
                .Where(language => string.IsNullOrWhiteSpace(language) == false)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            Config.DomainAnalyzed = new DomainComposite(
                Config.DomainName,
                Config.PublicSuffixListOnlineRefreshEnabled,
                TimeSpan.FromSeconds(Math.Max(1, Config.PublicSuffixListRefreshTimeoutSeconds)));
            if (string.IsNullOrEmpty(Config.DataProtectionBlobUrl) == false && Config.DataProtectionBlobUrl.StartsWith("https://"))
            {
                appBuilder.Services.AddDataProtection()
                    .SetApplicationName(Config.DomainAnalyzed.Domain)
                    .PersistKeysToAzureBlobStorage(
                        new Uri($"{Config.DataProtectionBlobUrl.TrimEnd('/')}/{Config.DomainAnalyzed.Domain.Replace(".", "-")}-keys.xml"),
                        new DefaultAzureCredential());
            }
        }

        /// <summary>
        ///     Indicates whether this configuration contributes API documentation assemblies.
        /// </summary>
        /// <returns>
        ///     Always <see langword="false" /> because this core configuration does not register its assembly automatically.
        /// </returns>
        public override bool ApiDescription()
        {
            return false;
        }

        /// <summary>
        ///     Runs before server helper configuration is loaded.
        /// </summary>
        /// <param name="appBuilder">
        ///     The host application builder supplied by the loading lifecycle.
        /// </param>
        /// <param name="configBuilder">
        ///     The configuration builder supplied by the loading lifecycle.
        /// </param>
        /// <param name="configRoot">
        ///     The configuration root supplied by the loading lifecycle.
        /// </param>
        /// <remarks>
        ///     The default server helper configuration does not need pre-configuration work.
        /// </remarks>
        public override void BeforeConfiguration(IHostApplicationBuilder appBuilder, IConfigurationBuilder configBuilder, IConfigurationRoot configRoot)
        {
        }

        /// <summary>
        ///     Composes an HTTPS URL for an MVC controller action.
        /// </summary>
        /// <param name="actionName">
        ///     The MVC action segment.
        /// </param>
        /// <param name="controllerName">
        ///     The MVC controller segment.
        /// </param>
        /// <returns>
        ///     A URL in the form <c>{HttpsWebsite}/{controllerName}/{actionName}</c> with encoded path segments.
        /// </returns>
        public string ComposeUrl([AspMvcAction] string actionName, [AspMvcController] string controllerName)
        {
            return DomainAnalyzed.HttpsWebsite + "/" + ComposePath(controllerName, actionName);
        }

        /// <summary>
        ///     Composes an HTTPS URL for an MVC controller action with path parameters.
        /// </summary>
        /// <param name="actionName">
        ///     The MVC action segment.
        /// </param>
        /// <param name="controllerName">
        ///     The MVC controller segment.
        /// </param>
        /// <param name="parameters">
        ///     Additional path segments appended after the action.
        /// </param>
        /// <returns>
        ///     A URL in the form <c>{HttpsWebsite}/{controllerName}/{actionName}/{parameters}</c> with encoded path segments.
        /// </returns>
        public string ComposeUrl([AspMvcAction] string actionName, [AspMvcController] string controllerName, params string[] parameters)
        {
            string url = DomainAnalyzed.HttpsWebsite + "/" + ComposePath(controllerName, actionName) + "/";
            if (parameters.Length > 0)
            {
                url += ComposePath(parameters);
            }

            return url;
        }

        /// <summary>
        ///     Composes an HTTPS URL for an MVC controller action with query string parameters.
        /// </summary>
        /// <param name="actionName">
        ///     The MVC action segment.
        /// </param>
        /// <param name="controllerName">
        ///     The MVC controller segment.
        /// </param>
        /// <param name="keysValues">
        ///     Query string keys and values. Keys and values are URL-encoded with <c>Uri.EscapeDataString</c>.
        /// </param>
        /// <returns>
        ///     A URL in the form <c>{HttpsWebsite}/{controllerName}/{actionName}?key=value</c> with encoded path and query segments.
        /// </returns>
        public string ComposeUrl([AspMvcAction] string actionName, [AspMvcController] string controllerName, Dictionary<string, string> keysValues)
        {
            List<string> parameters = new List<string>();
            foreach (KeyValuePair<string, string> key in keysValues)
            {
                parameters.Add($"{EncodeUriComponent(key.Key)}={EncodeUriComponent(key.Value)}");
            }

            return DomainAnalyzed.HttpsWebsite + "/" + ComposePath(controllerName, actionName) + "?" + string.Join("&", parameters);
        }

        /// <summary>
        ///     Composes an HTTPS URL for an MVC controller root.
        /// </summary>
        /// <param name="controllerName">
        ///     The MVC controller segment.
        /// </param>
        /// <returns>
        ///     A URL in the form <c>{HttpsWebsite}/{controllerName}</c> with an encoded controller segment.
        /// </returns>
        public string ComposeUrl([AspMvcController] string controllerName)
        {
            return DomainAnalyzed.HttpsWebsite + "/" + EncodeUriComponent(controllerName);
        }

        /// <summary>
        ///     Composes an HTTPS URL for an arbitrary application path.
        /// </summary>
        /// <param name="path">
        ///     The relative path appended after the analyzed HTTPS website.
        /// </param>
        /// <returns>
        ///     A URL in the form <c>{HttpsWebsite}/{path}</c> with encoded path segments and query values.
        /// </returns>
        public string ComposeUrlWithPath([PathReference] string path)
        {
            return DomainAnalyzed.HttpsWebsite + "/" + EncodeRelativePath(path);
        }

        /// <summary>
        ///     Gets the analyzed canonical HTTPS website URL.
        /// </summary>
        /// <returns>
        ///     The HTTPS URL produced by <see cref="DomainComposite" /> during <see cref="AfterConfiguration" />.
        /// </returns>
        public string GetHttpsUrl()
        {
            return DomainAnalyzed.HttpsWebsite;
        }

        /// <summary>
        ///     Indicates whether this configuration should be loaded from JSON files or app settings.
        /// </summary>
        /// <returns>
        ///     Always <see langword="true" /> because the server helper relies on external configuration.
        /// </returns>
        public override bool NeedsConfigFileOrAppSettings()
        {
            return true;
        }

        /// <summary>
        ///     Populates example configuration values.
        /// </summary>
        /// <remarks>
        ///     The default server helper implementation does not currently generate fake values.
        /// </remarks>
        public override void RandomFake()
        {
        }

        #endregion
    }
}
