#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Provides the generic configuration loading lifecycle for a typed server configuration.
    /// </summary>
    /// <typeparam name="T">
    ///     The concrete configuration type managed by the lifecycle.
    /// </typeparam>
    /// <remarks>
    ///     The lifecycle loads optional JSON configuration files, binds the configuration section named
    ///     after <typeparamref name="T" />, invokes pre- and post-configuration hooks, and prevents duplicate
    ///     loading through <see cref="IServerConfig.Loaded" />.
    /// </remarks>
    [Serializable]
    public abstract class GenericConfiguration<T> : IServerConfig where T : IServerConfig, new()
    {
        #region Static fields and properties

        /// <summary>
        ///     Stores the loaded configuration instance for the concrete configuration type.
        /// </summary>
        public static T Config = new T();

        private static readonly object LoadLock = new object();

        #endregion

        #region Static methods

        /// <summary>
        ///     Adds the default and environment-specific JSON files for the concrete configuration type.
        /// </summary>
        /// <param name="config">
        ///     The configuration builder that receives the JSON file sources.
        /// </param>
        /// <remarks>
        ///     The method looks for files named <c>{ConfigurationType}.json</c> and
        ///     <c>{ConfigurationType}.{ASPNETCORE_ENVIRONMENT}.json</c>. Optional and hot-reload behavior
        ///     comes from <see cref="ConfigIsOptional" /> and <see cref="ConfigHotReload" />.
        /// </remarks>
        public static void AddConfigInConfigurationBuilder(IConfigurationBuilder config)
        {
            try
            {
                config.AddJsonFile($"{typeof(T).Name}.json", GenericConfiguration<T>.Config.ConfigIsOptional(), GenericConfiguration<T>.Config.ConfigHotReload());
            }
            catch (Exception tException)
            {
                ServerHelperConfiguration.Logger.Error($"Unable to add configuration file '{typeof(T).Name}.json'.", tException);
            }

            string env = GetEnvironmentVariable();
            if (string.IsNullOrEmpty(env))
            {
                return;
            }

            try
            {
                config.AddJsonFile($"{typeof(T).Name}.{env}.json", GenericConfiguration<T>.Config.ConfigIsOptional(), GenericConfiguration<T>.Config.ConfigHotReload());
            }
            catch (Exception tException)
            {
                ServerHelperConfiguration.Logger.Error($"Unable to add configuration file '{typeof(T).Name}.{env}.json'.", tException);
            }
        }

        /// <summary>
        ///     Gets the current ASP.NET Core environment name used to select environment-specific configuration files.
        /// </summary>
        /// <returns>
        ///     The value of the <c>ASPNETCORE_ENVIRONMENT</c> environment variable, or an empty string when it is not set.
        /// </returns>
        /// <remarks>
        ///     The returned value is appended to configuration file names such as
        ///     <c>{ConfigurationType}.{Environment}.json</c> by <see cref="AddConfigInConfigurationBuilder" />.
        /// </remarks>
        protected static string GetEnvironmentVariable()
        {
            string? env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(env))
            {
                env = "";
            }

            return env;
        }

        /// <summary>
        ///     Loads the common configuration lifecycle from a web application builder.
        /// </summary>
        /// <param name="builder">
        ///     The web application builder whose configuration and services are used.
        /// </param>
        public static void LoadCommonConfig(WebApplicationBuilder builder)
        {
            LoadCommonConfig(builder, builder.Configuration, builder.Configuration);
        }

        /// <summary>
        ///     Loads the common configuration lifecycle from explicit host and configuration objects.
        /// </summary>
        /// <param name="appBuilder">
        ///     The host application builder used by lifecycle hooks.
        /// </param>
        /// <param name="configBuilder">
        ///     The configuration builder used to add optional JSON files.
        /// </param>
        /// <param name="configRoot">
        ///     The configuration root used to bind the concrete configuration section.
        /// </param>
        /// <remarks>
        ///     The method returns immediately when <see cref="IServerConfig.Loaded" /> is already set. When
        ///     <see cref="ApiDescription" /> is enabled, the concrete configuration assembly is registered in
        ///     <see cref="ApiDocumentationList" />.
        /// </remarks>
        public static void LoadCommonConfig(IHostApplicationBuilder appBuilder, IConfigurationBuilder configBuilder, IConfigurationRoot configRoot)
        {
            lock (LoadLock)
            {
                if (Config.Loaded)
                {
                    return;
                }

                if (Config.ApiDescription())
                {
                    ApiDocumentationList.AddApiAssembly(typeof(T).Assembly);
                }

                Config.BeforeConfiguration(appBuilder, configBuilder, configRoot);

                if (Config.NeedsConfigFileOrAppSettings())
                {
                    AddConfigInConfigurationBuilder(configBuilder);
                    LoadConfigWithConfigurationRoot(configRoot);
                }

                Config.AfterConfiguration(appBuilder, configBuilder, configRoot);
                Config.Loaded = true;
            }
        }

        /// <summary>
        ///     Binds the concrete configuration section from a configuration root.
        /// </summary>
        /// <param name="config">
        ///     The configuration root containing a section named after the concrete configuration type.
        /// </param>
        /// <remarks>
        ///     If the section is missing, the current configuration instance is left unchanged and an example
        ///     configuration notice is written to the console.
        /// </remarks>
        public static void LoadConfigWithConfigurationRoot(IConfigurationRoot config)
        {
            T? configFromSection = config.GetSection(typeof(T).Name).Get<T>();
            if (configFromSection != null)
            {
                GenericConfiguration<T>.Config = configFromSection;
            }
            else
            {
                ServerHelperConfiguration.Logger.Information($"{typeof(T).Name} {nameof(LoadConfigWithConfigurationRoot)} ( {nameof(IConfigurationRoot)} …) => No configuration section");
                PrintExample();
            }
        }

        /// <summary>
        ///     Prints a one-time missing-configuration notice for the concrete configuration type.
        /// </summary>
        /// <param name="notFound">
        ///     A value indicating whether the notice should explicitly state that no configuration file was found.
        /// </param>
        /// <remarks>
        ///     The method sets <see cref="IServerConfig.ExamplePrinted" /> to avoid repeated console output and
        ///     calls <see cref="RandomFake" /> on a new example instance so derived configurations can prepare
        ///     representative values.
        /// </remarks>
        public static void PrintExample(bool notFound = false)
        {
            if (Config.ExamplePrinted)
            {
                return;
            }

            Config.ExamplePrinted = true;
            T example = new T();
            example.RandomFake();

            string env = GetEnvironmentVariable();
            if (string.IsNullOrEmpty(env) == false)
            {
                env = env + ".";
            }

            if (notFound)
            {
                ServerHelperConfiguration.Logger.Warning($"{typeof(T).Name} config not found in appsettings.{env}json or in {typeof(T).Name}.{env}json!");
            }
        }

        #endregion

        #region Instance fields and properties

        #region From interface IServerConfig

        /// <summary>
        ///     Gets or sets a value indicating whether the example configuration notice has already been printed.
        /// </summary>
        [JsonIgnore]
        public bool ExamplePrinted { set; get; }

        /// <summary>
        ///     Gets or sets a value indicating whether this configuration has already completed the loading lifecycle.
        /// </summary>
        [JsonIgnore]
        public bool Loaded { set; get; } = false;

        #endregion

        #endregion

        #region Instance methods

        #region From interface IServerConfig

        /// <summary>
        ///     Runs after optional file loading and configuration binding have completed.
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
        public abstract void AfterConfiguration(IHostApplicationBuilder appBuilder, IConfigurationBuilder configBuilder, IConfigurationRoot configRoot);

        /// <summary>
        ///     Indicates whether the concrete configuration assembly should be registered for API documentation.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> when <see cref="ApiDocumentationList" /> should include the concrete assembly;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public abstract bool ApiDescription();

        /// <summary>
        ///     Runs before optional file loading and configuration binding.
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
        public abstract void BeforeConfiguration(IHostApplicationBuilder appBuilder, IConfigurationBuilder configBuilder, IConfigurationRoot configRoot);

        /// <summary>
        ///     Indicates whether JSON configuration files should be loaded with reload-on-change enabled.
        /// </summary>
        /// <returns>
        ///     <see langword="false" /> by default.
        /// </returns>
        public virtual bool ConfigHotReload()
        {
            return false;
        }

        /// <summary>
        ///     Indicates whether JSON configuration files are optional.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> by default.
        /// </returns>
        public virtual bool ConfigIsOptional()
        {
            return true;
        }

        /// <summary>
        ///     Indicates whether the configuration should be loaded from JSON files or app settings.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> when the lifecycle should add and bind configuration sources.
        /// </returns>
        public abstract bool NeedsConfigFileOrAppSettings();

        /// <summary>
        ///     Populates the configuration with representative example values.
        /// </summary>
        /// <remarks>
        ///     This method is used by <see cref="PrintExample" /> when configuration is missing.
        /// </remarks>
        public abstract void RandomFake();

        #endregion

        #endregion
    }
}
