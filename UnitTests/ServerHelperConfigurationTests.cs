#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBServerHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

#endregion

namespace DMBserverHelperUnitTest;

[TestFixture]
internal sealed class ServerHelperConfigurationTests
{
    #region Setup/Teardown

    [SetUp]
    public void SetUp()
    {
        ServerHelperConfiguration.Config = new ServerHelperConfiguration();
        ServerHelperConfiguration.UseLogger(new ConsoleServerHelperLogger());
    }

    [TearDown]
    public void TearDown()
    {
        ServerHelperConfiguration.Config = new ServerHelperConfiguration();
        ServerHelperConfiguration.UseLogger(new ConsoleServerHelperLogger());
    }

    #endregion

    private static void RunAfterConfiguration(ServerHelperConfiguration configuration)
    {
        configuration.PublicSuffixListOnlineRefreshEnabled = false;
        ServerHelperConfiguration.Config = configuration;
        HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder();
        ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        IConfigurationRoot configurationRoot = configurationBuilder.Build();

        configuration.AfterConfiguration(hostBuilder, configurationBuilder, configurationRoot);
    }

    private sealed class CountingSecretRotationHandler : ISecretRotationHandler
    {
        #region Instance fields and properties

        public int CallCount { get; private set; }

        public SecretManager? LastSecretManager { get; private set; }

        #region From interface ISecretRotationHandler

        public string Name { get; }

        #endregion

        #endregion

        #region Instance constructors and destructors

        public CountingSecretRotationHandler(string name)
        {
            Name = name;
        }

        #endregion

        #region Instance methods

        #region From interface ISecretRotationHandler

        public void RotateResolvedSecrets(SecretManager secretManager)
        {
            CallCount++;
            LastSecretManager = secretManager;
        }

        #endregion

        #endregion
    }

    private sealed class CapturingServerHelperLogger : IServerHelperLogger
    {
        #region Instance fields and properties

        public List<string> Errors { get; } = new List<string>();

        public List<string> InformationMessages { get; } = new List<string>();

        public List<string> Warnings { get; } = new List<string>();

        #endregion

        #region Instance methods

        #region From interface IServerHelperLogger

        public void Error(string message)
        {
            Errors.Add(message);
        }

        public void Error(string message, Exception exception)
        {
            Errors.Add($"{message} {exception.GetType().Name}");
        }

        public void Information(string message)
        {
            InformationMessages.Add(message);
        }

        public void Warning(string message)
        {
            Warnings.Add(message);
        }

        #endregion

        #endregion
    }

    [Test]
    public void AfterConfigurationNormalizesSupportedLanguagesAndAnalyzesDomain()
    {
        ServerHelperConfiguration configuration = new ServerHelperConfiguration
        {
            DomainName = "example.com",
            SupportLanguages = new List<string> { "en-US", "", "fr-FR", "EN-us", "   " }
        };

        RunAfterConfiguration(configuration);

        Assert.Multiple(() =>
        {
            Assert.That(configuration.SupportLanguages, Is.Not.Null);
            Assert.That(configuration.SupportLanguages, Has.No.Empty);
            Assert.That(configuration.SupportLanguages!.Count, Is.EqualTo(configuration.SupportLanguages.Distinct(StringComparer.OrdinalIgnoreCase).Count()));
            Assert.That(configuration.SupportLanguages, Does.Contain("en-US"));
            Assert.That(configuration.SupportLanguages, Does.Contain("fr-FR"));
            Assert.That(configuration.GetHttpsUrl(), Is.EqualTo("https://www.example.com"));
        });
    }

    [Test]
    public void ComposeUrlWithControllerAndActionUsesAnalyzedHttpsWebsite()
    {
        ServerHelperConfiguration configuration = new ServerHelperConfiguration
        {
            DomainName = "docs.example.com"
        };

        RunAfterConfiguration(configuration);

        Assert.That(configuration.ComposeUrl("Index", "Home"), Is.EqualTo("https://docs.example.com/Home/Index"));
    }

    [Test]
    public void ComposeUrlWithControllerOnlyEncodesControllerSegment()
    {
        ServerHelperConfiguration configuration = new ServerHelperConfiguration
        {
            DomainName = "example.com"
        };

        RunAfterConfiguration(configuration);

        Assert.That(configuration.ComposeUrl("Admin Area"), Is.EqualTo("https://www.example.com/Admin%20Area"));
    }

    [Test]
    public void ComposeUrlWithPathParametersUrlEncodesSegments()
    {
        ServerHelperConfiguration configuration = new ServerHelperConfiguration
        {
            DomainName = "example.com"
        };

        RunAfterConfiguration(configuration);

        string url = configuration.ComposeUrl("Details View", "Products Area", "sku/123", "édition spéciale");

        Assert.That(url, Is.EqualTo("https://www.example.com/Products%20Area/Details%20View/sku%2F123/%C3%A9dition%20sp%C3%A9ciale"));
    }

    [Test]
    public void ComposeUrlWithPathUrlEncodesSegmentsAndPreservesQuery()
    {
        ServerHelperConfiguration configuration = new ServerHelperConfiguration
        {
            DomainName = "example.com"
        };

        RunAfterConfiguration(configuration);

        string url = configuration.ComposeUrlWithPath("docs/a path/file name?return url=/a path/#top section");

        Assert.That(url, Is.EqualTo("https://www.example.com/docs/a%20path/file%20name?return%20url=%2Fa%20path%2F#top%20section"));
    }

    [Test]
    public void ComposeUrlWithPublicPortUsesAnalyzedHttpsWebsite()
    {
        ServerHelperConfiguration configuration = new ServerHelperConfiguration
        {
            DomainName = "docs.example.com:8443"
        };

        RunAfterConfiguration(configuration);

        Assert.That(configuration.ComposeUrl("Index", "Home"), Is.EqualTo("https://docs.example.com:8443/Home/Index"));
    }

    [Test]
    public void ComposeUrlWithQueryValuesDoesNotDoubleEncodeAlreadyEncodedValues()
    {
        ServerHelperConfiguration configuration = new ServerHelperConfiguration
        {
            DomainName = "example.com"
        };

        RunAfterConfiguration(configuration);

        string url = configuration.ComposeUrl(
            "Details",
            "Products",
            new Dictionary<string, string> { ["returnUrl"] = "%2Fa%20path%2F" });

        Assert.That(url, Is.EqualTo("https://www.example.com/Products/Details?returnUrl=%2Fa%20path%2F"));
    }

    [Test]
    public void ComposeUrlWithQueryValuesUrlEncodesKeysAndValues()
    {
        ServerHelperConfiguration configuration = new ServerHelperConfiguration
        {
            DomainName = "example.com"
        };

        RunAfterConfiguration(configuration);

        string url = configuration.ComposeUrl(
            "Details",
            "Products",
            new Dictionary<string, string> { ["return url"] = "/a path/" });

        Assert.That(url, Is.EqualTo("https://www.example.com/Products/Details?return%20url=%2Fa%20path%2F"));
    }

    [Test]
    public void RegisterSecretRotationHandlerReplacesHandlerWithSameName()
    {
        string handlerName = $"{nameof(RegisterSecretRotationHandlerReplacesHandlerWithSameName)}-{Guid.NewGuid()}";
        CountingSecretRotationHandler firstHandler = new CountingSecretRotationHandler(handlerName);
        CountingSecretRotationHandler secondHandler = new CountingSecretRotationHandler(handlerName);

        ServerHelperConfiguration.RegisterSecretRotationHandler(firstHandler);
        ServerHelperConfiguration.RegisterSecretRotationHandler(secondHandler);

        ServerHelperConfiguration.RotateResolvedSecrets();

        Assert.Multiple(() =>
        {
            Assert.That(firstHandler.CallCount, Is.EqualTo(0));
            Assert.That(secondHandler.CallCount, Is.EqualTo(1));
            Assert.That(secondHandler.LastSecretManager, Is.SameAs(ServerHelperConfiguration.Config.Secrets));
        });
    }

    [Test]
    public void RotateResolvedSecretsWithBuilderValidatesBeforeRotation()
    {
        HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Production"
        });
        ServerHelperConfiguration.Config.Secrets.Store = SecretStoreKind.EnvironmentVariables;
        ServerHelperConfiguration.Config.Secrets.LogMissingSecrets = false;
        string missingSecretKey = $"DMB:ServerHelper:UnitTests:Rotation:{Guid.NewGuid():N}";
        string missingSecretEnvironmentVariable = missingSecretKey.Replace(":", "__");
        ServerHelperConfiguration.Config.Secrets.Require(missingSecretKey, "DMBServerHelperUnitTests", "Rotation validation secret");
        CountingSecretRotationHandler handler = new CountingSecretRotationHandler(
            $"{nameof(RotateResolvedSecretsWithBuilderValidatesBeforeRotation)}-{Guid.NewGuid()}");

        ServerHelperConfiguration.RegisterSecretRotationHandler(handler);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            ServerHelperConfiguration.RotateResolvedSecrets(hostBuilder))!;

        Assert.Multiple(() =>
        {
            Assert.That(handler.CallCount, Is.EqualTo(0));
            Assert.That(exception.Message, Does.Contain($"{missingSecretEnvironmentVariable}=<secret-value>"));
        });
    }

    [Test]
    public void UseLoggerRejectsNullLogger()
    {
        Assert.Throws<ArgumentNullException>(() => ServerHelperConfiguration.UseLogger(null!));
    }

    [Test]
    public void UseLoggerReplacesServerHelperLogger()
    {
        CapturingServerHelperLogger logger = new CapturingServerHelperLogger();

        ServerHelperConfiguration.UseLogger(logger);
        ServerHelperConfiguration.Logger.Warning("custom warning");

        Assert.That(logger.Warnings, Is.EqualTo(new[] { "custom warning" }));
    }

    [Test]
    public void ValidateRequiredSecretsAggregatesAllRegisteredMissingSecrets()
    {
        ServerHelperConfiguration.Config.Secrets.Store = SecretStoreKind.EnvironmentVariables;
        ServerHelperConfiguration.Config.Secrets.Environment = SecretUsageEnvironment.LocalWebsite;
        ServerHelperConfiguration.Config.Secrets.FailFast = true;
        ServerHelperConfiguration.Config.Secrets.LogMissingSecrets = false;
        ServerHelperConfiguration.Config.Secrets.Configure(new ConfigurationBuilder().Build(), "Development");
        ServerHelperConfiguration.Config.Secrets.Require("DMB:Stripe:SecretKey", "DMBStripe", "Stripe API secret key");
        ServerHelperConfiguration.Config.Secrets.Require("DMB:Pennylane:ApiToken", "DMBPennylane", "Pennylane API token");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            ServerHelperConfiguration.ValidateRequiredSecrets())!;

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("DMB__Stripe__SecretKey=<secret-value>"));
            Assert.That(exception.Message, Does.Contain("DMB__Pennylane__ApiToken=<secret-value>"));
        });
    }

    [Test]
    public void ValidateRequiredSecretsWithBuilderUsesBuilderEnvironment()
    {
        HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Production"
        });
        ServerHelperConfiguration.Config.Secrets.Store = SecretStoreKind.EnvironmentVariables;
        ServerHelperConfiguration.Config.Secrets.LogMissingSecrets = false;
        ServerHelperConfiguration.Config.Secrets.Require("DMB:Stripe:WebhookSecret", "DMBStripe", "Stripe webhook signing secret");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            ServerHelperConfiguration.ValidateRequiredSecrets(hostBuilder))!;

        Assert.Multiple(() =>
        {
            Assert.That(ServerHelperConfiguration.Config.Secrets.FailFast, Is.False);
            Assert.That(ServerHelperConfiguration.Config.Secrets.EffectiveFailFast, Is.True);
            Assert.That(exception.Message, Does.Contain("Environment: Production"));
            Assert.That(exception.Message, Does.Contain("DMB__Stripe__WebhookSecret=<secret-value>"));
        });
    }
}