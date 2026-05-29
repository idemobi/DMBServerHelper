#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Defines the configuration lifecycle contract used by server helper configuration types.
    /// </summary>
    public interface IServerConfig
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets a value indicating whether a missing-configuration example has already been printed.
        /// </summary>

        [System.Text.Json.Serialization.JsonIgnore]
        public bool ExamplePrinted { set; get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the configuration lifecycle has already completed.
        /// </summary>

        [System.Text.Json.Serialization.JsonIgnore]
        public bool Loaded { set; get; }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Runs after optional configuration sources have been loaded and bound.
        /// </summary>
        /// <param name="appBuilder">
        ///     The host application builder supplied by the configuration lifecycle.
        /// </param>
        /// <param name="configBuilder">
        ///     The configuration builder supplied by the configuration lifecycle.
        /// </param>
        /// <param name="configRoot">
        ///     The resolved configuration root supplied by the configuration lifecycle.
        /// </param>
        public void AfterConfiguration(IHostApplicationBuilder appBuilder, IConfigurationBuilder configBuilder, IConfigurationRoot configRoot);

        /// <summary>
        ///     Indicates whether this configuration should register its assembly for API documentation.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> when the assembly should be registered; otherwise, <see langword="false" />.
        /// </returns>
        public bool ApiDescription();

        /// <summary>
        ///     Runs before optional configuration sources are loaded and bound.
        /// </summary>
        /// <param name="appBuilder">
        ///     The host application builder supplied by the configuration lifecycle.
        /// </param>
        /// <param name="configBuilder">
        ///     The configuration builder supplied by the configuration lifecycle.
        /// </param>
        /// <param name="configRoot">
        ///     The resolved configuration root supplied by the configuration lifecycle.
        /// </param>
        public void BeforeConfiguration(IHostApplicationBuilder appBuilder, IConfigurationBuilder configBuilder, IConfigurationRoot configRoot);

        /// <summary>
        ///     Indicates whether configuration files should be watched for reload-on-change.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> when JSON files should be reloaded after changes; otherwise, <see langword="false" />.
        /// </returns>
        public bool ConfigHotReload();

        /// <summary>
        ///     Indicates whether configuration files are optional.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> when missing JSON files are allowed; otherwise, <see langword="false" />.
        /// </returns>
        public bool ConfigIsOptional();

        /// <summary>
        ///     Indicates whether this configuration should be loaded from files or app settings.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> when the lifecycle should bind configuration; otherwise, <see langword="false" />.
        /// </returns>
        public bool NeedsConfigFileOrAppSettings();

        /// <summary>
        ///     Populates representative example values used by missing-configuration output.
        /// </summary>
        public void RandomFake();

        #endregion
    }
}