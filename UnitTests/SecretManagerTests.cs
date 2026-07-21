#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Reflection;
using DMBServerHelper;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

#endregion

namespace DMBserverHelperUnitTest;

[TestFixture]
internal sealed class SecretManagerTests
{
    #region Setup/Teardown

    [SetUp]
    public void SetUp()
    {
        ServerHelperConfiguration.UseLogger(new ConsoleServerHelperLogger());
    }

    [TearDown]
    public void TearDown()
    {
        ServerHelperConfiguration.UseLogger(new ConsoleServerHelperLogger());
    }

    #endregion

    private sealed class CapturingSecretLogger : ISecretLogger
    {
        #region Instance fields and properties

        public List<string> Warnings { get; } = new List<string>();

        #endregion

        #region Instance methods

        #region From interface ISecretLogger

        public void Warning(string message)
        {
            Warnings.Add(message);
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

    private enum SampleSecretEnvironment
    {
        Development = 0,
        PlayTest = 1,
        Preproduction = 2,
        Production = 3
    }

    [Test]
    public void AutoStoreUsesAzureKeyVaultWhenProductionHasVaultUri()
    {
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.Auto,
            Environment = SecretUsageEnvironment.Production,
            AzureKeyVaultUri = "https://example-vault.vault.azure.net/"
        };

        string message = manager.BuildMissingSecretMessage(new SecretDefinition
        {
            Key = "DMB:Stripe:WebhookSecret",
            Owner = "DMBStripe",
            DisplayName = "Stripe webhook signing secret"
        });

        Assert.That(message, Does.Contain("DMB--Stripe--WebhookSecret"));
    }

    [Test]
    public void AutoStoreUsesUserSecretsForLocalUnitTests()
    {
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.Auto
        };
        manager.Configure(new ConfigurationBuilder().Build(), "Testing");

        string message = manager.BuildMissingSecretMessage(new SecretDefinition
        {
            Key = "DMB:ServerEmailHelper:NoReply:Smtp:Password",
            Owner = "DMBServerEmailHelper",
            DisplayName = "NoReply SMTP password"
        });

        Assert.Multiple(() =>
        {
            Assert.That(message, Does.Contain("Environment: LocalUnitTests"));
            Assert.That(message, Does.Contain("Secret store: UserSecrets"));
            Assert.That(message, Does.Contain("dotnet user-secrets set \"DMB:ServerEmailHelper:NoReply:Smtp:Password\" \"<secret-value>\""));
        });
    }

    [Test]
    public void DefaultSecretLoggerDelegatesWarningsToServerHelperLogger()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        CapturingServerHelperLogger logger = new CapturingServerHelperLogger();
        ServerHelperConfiguration.UseLogger(logger);
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            LogMissingSecrets = true
        };
        manager.Configure(configuration, "Development");
        manager.Require("DMB:Stripe:WebhookSecret", "DMBStripe", "Stripe webhook signing secret");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.GetRequired("DMB:Stripe:WebhookSecret"))!;

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("Missing secret: DMB:Stripe:WebhookSecret"));
            Assert.That(exception.Message, Does.Not.Contain("DMB__Stripe__WebhookSecret=<secret-value>"));
            Assert.That(logger.Warnings, Has.Count.EqualTo(1));
            Assert.That(logger.Warnings[0], Does.Contain("DMB__Stripe__WebhookSecret=<secret-value>"));
        });
    }

    [Test]
    public void ExplicitUsageEnvironmentOverridesEmptyHostEnvironmentName()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        SecretManager manager = new SecretManager
        {
            Environment = SecretUsageEnvironment.LocalWebsite,
            Store = SecretStoreKind.UserSecrets,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, string.Empty);
        manager.Require("DMB:Stripe:SecretKey", "DMBStripe", "Stripe API secret key");

        SecretValidationResult result = manager.ValidateRequiredSecrets();

        Assert.Multiple(() =>
        {
            Assert.That(manager.EffectiveFailFast, Is.False);
            Assert.That(result.Success, Is.False);
            Assert.That(result.Issues, Has.Count.EqualTo(1));
            Assert.That(result.Issues[0].Message, Does.Contain("Environment: LocalWebsite"));
            Assert.That(result.Issues[0].Message, Does.Not.Contain("Host environment: <empty>"));
        });
    }

    [Test]
    public void ExplicitUsageEnvironmentOverridesUnknownHostEnvironmentName()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        SecretManager manager = new SecretManager
        {
            Environment = SecretUsageEnvironment.Production,
            Store = SecretStoreKind.EnvironmentVariables,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, "BlueGreenSlotA");
        manager.Require("DMB:Stripe:SecretKey", "DMBStripe", "Stripe API secret key");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.ValidateRequiredSecrets())!;

        Assert.Multiple(() =>
        {
            Assert.That(manager.EffectiveFailFast, Is.True);
            Assert.That(exception.Message, Does.Contain("Environment: Production"));
            Assert.That(exception.Message, Does.Contain("DMB__Stripe__SecretKey=<secret-value>"));
        });
    }

    [Test]
    public void GetReadsSecretFromConfigurationWithoutStoringValue()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DMB:Stripe:SecretKey"] = "resolved-api-value"
            })
            .Build();

        SecretManager manager = new SecretManager();
        manager.Configure(configuration, "Development");

        Assert.That(manager.Get("DMB:Stripe:SecretKey"), Is.EqualTo("resolved-api-value"));
    }

    [Test]
    public void GetRequiredLogsMissingSecretThroughConfiguredLogger()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        CapturingSecretLogger logger = new CapturingSecretLogger();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            LogMissingSecrets = true
        };
        manager.UseLogger(logger);
        manager.Configure(configuration, "Development");
        manager.Require("DMB:Stripe:WebhookSecret", "DMBStripe", "Stripe webhook signing secret");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.GetRequired("DMB:Stripe:WebhookSecret"))!;

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("Missing secret: DMB:Stripe:WebhookSecret"));
            Assert.That(exception.Message, Does.Not.Contain("DMB__Stripe__WebhookSecret=<secret-value>"));
            Assert.That(logger.Warnings, Has.Count.EqualTo(1));
            Assert.That(logger.Warnings[0], Does.Contain("DMB__Stripe__WebhookSecret=<secret-value>"));
        });
    }

    [Test]
    public void GetRequiredReturnsExistingSecretValue()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DMB:Stripe:SecretKey"] = "resolved-api-value"
            })
            .Build();
        SecretManager manager = new SecretManager();
        manager.Configure(configuration, "Development");

        string value = manager.GetRequired("DMB:Stripe:SecretKey");

        Assert.That(value, Is.EqualTo("resolved-api-value"));
    }

    [Test]
    public void GetRequiredEnumReportsAcceptedValuesWhenValueIsInvalid()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DMB:Sample:Environment"] = "Mars"
            })
            .Build();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            Environment = SecretUsageEnvironment.LocalWebsite,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, "Development");
        manager.Require(new SecretDefinition
        {
            Key = "DMB:Sample:Environment",
            Owner = "DMBServerHelperUnitTests",
            DisplayName = "Sample environment"
        });

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.GetRequiredEnum<SampleSecretEnvironment>("DMB:Sample:Environment"))!;

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("Invalid secret: DMB:Sample:Environment"));
            Assert.That(exception.Message, Does.Contain("Expected type: SampleSecretEnvironment enum"));
            Assert.That(exception.Message, Does.Contain("Format: Use the textual enum name."));
            Assert.That(exception.Message, Does.Contain("- Development (0)"));
            Assert.That(exception.Message, Does.Contain("- Preproduction (2)"));
            Assert.That(exception.Message, Does.Contain("Configured value: hidden"));
            Assert.That(exception.Message, Does.Not.Contain("Mars"));
        });
    }

    [Test]
    public void GetRequiredInt64ReportsExpectedTypeWhenValueIsInvalid()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DMB:Sample:Project"] = "not-a-number"
            })
            .Build();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            Environment = SecretUsageEnvironment.LocalWebsite,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, "Development");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.GetRequiredInt64("DMB:Sample:Project"))!;

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("Invalid secret: DMB:Sample:Project"));
            Assert.That(exception.Message, Does.Contain("Expected type: Int64 whole number"));
            Assert.That(exception.Message, Does.Contain("Format: Use digits only"));
            Assert.That(exception.Message, Does.Contain("Configured value: hidden"));
            Assert.That(exception.Message, Does.Not.Contain("not-a-number"));
        });
    }

    [Test]
    public void GetRequiredThrowsSetupDiagnosticWhenSecretIsMissing()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, "Production");
        manager.Require("DMB:Stripe:WebhookSecret", "DMBStripe", "Stripe webhook signing secret");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.GetRequired("DMB:Stripe:WebhookSecret"))!;

        Assert.That(exception.Message, Does.Contain("DMB__Stripe__WebhookSecret=<secret-value>"));
    }

    [Test]
    public void MissingAzureKeyVaultDiagnosticUsesDoubleDashName()
    {
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.AzureKeyVault,
            Environment = SecretUsageEnvironment.PreProduction,
            AzureKeyVaultUri = "https://example-vault.vault.azure.net/"
        };

        string message = manager.BuildMissingSecretMessage(new SecretDefinition
        {
            Key = "DMB:Pennylane:ApiToken",
            Owner = "DMBPennylane",
            DisplayName = "Pennylane API token"
        });

        Assert.Multiple(() =>
        {
            Assert.That(message, Does.Contain("https://example-vault.vault.azure.net/"));
            Assert.That(message, Does.Contain("DMB--Pennylane--ApiToken"));
        });
    }

    [Test]
    public void MissingEnvironmentVariableDiagnosticUsesDoubleUnderscoreName()
    {
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            Environment = SecretUsageEnvironment.LocalWebsite
        };

        string message = manager.BuildMissingSecretMessage(new SecretDefinition
        {
            Key = "DMB:Stripe:WebhookSecret",
            Owner = "DMBStripe",
            DisplayName = "Stripe webhook signing secret"
        });

        Assert.That(message, Does.Contain("DMB__Stripe__WebhookSecret=<secret-value>"));
    }

    [Test]
    public void MissingEnvironmentVariableDiagnosticExplainsLocalAndAzureSetup()
    {
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            Environment = SecretUsageEnvironment.LocalWebsite
        };

        string message = manager.BuildMissingSecretMessage(new SecretDefinition
        {
            Key = "GDF:WebRuntime:Project:MissingDiagnosticTestOnly",
            Owner = "GDFWebRuntime",
            DisplayName = "GDF runtime project reference"
        });

        Assert.Multiple(() =>
        {
            Assert.That(message, Does.Contain("GDF__WebRuntime__Project__MissingDiagnosticTestOnly=<secret-value>"));
            Assert.That(message, Does.Contain("Current process:"));
            Assert.That(message, Does.Contain("Persistent local setup:"));
            Assert.That(message, Does.Contain("macOS zsh:"));
            Assert.That(message, Does.Contain("macOS GUI applications launched from Finder, Dock, or an IDE:"));
            Assert.That(message, Does.Contain("launchctl setenv GDF__WebRuntime__Project__MissingDiagnosticTestOnly \"<secret-value>\""));
            Assert.That(message, Does.Contain("Fully quit and restart the application and/or the IDE that launches it"));
            Assert.That(message, Does.Contain("Linux bash:"));
            Assert.That(message, Does.Contain("Windows PowerShell user profile:"));
            Assert.That(message, Does.Contain("Azure App Service application setting:"));
            Assert.That(message, Does.Contain("Azure Portal:"));
            Assert.That(message, Does.Not.Contain("Runtime visibility:"));
            Assert.That(message, Does.Not.Contain("launchctl getenv GDF__WebRuntime__Project__MissingDiagnosticTestOnly"));
            Assert.That(message, Does.Not.Contain("printenv GDF__WebRuntime__Project__MissingDiagnosticTestOnly"));

            if (OperatingSystem.IsWindows())
            {
                Assert.That(message, Does.Contain("$env:GDF__WebRuntime__Project__MissingDiagnosticTestOnly=\"<secret-value>\""));
            }
            else if (OperatingSystem.IsMacOS())
            {
                Assert.That(message, Does.Contain("macOS terminal:"));
                Assert.That(message, Does.Contain("export GDF__WebRuntime__Project__MissingDiagnosticTestOnly=\"<secret-value>\""));
            }
            else if (OperatingSystem.IsLinux())
            {
                Assert.That(message, Does.Contain("Linux terminal:"));
                Assert.That(message, Does.Contain("export GDF__WebRuntime__Project__MissingDiagnosticTestOnly=\"<secret-value>\""));
            }
            else
            {
                Assert.That(message, Does.Contain("POSIX terminal:"));
                Assert.That(message, Does.Contain("export GDF__WebRuntime__Project__MissingDiagnosticTestOnly=\"<secret-value>\""));
            }
        });
    }

    [Test]
    public void MissingSecretDiagnosticAsksForExplicitEnvironmentWhenHostEnvironmentNameIsEmpty()
    {
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.Auto
        };
        manager.Configure(new ConfigurationBuilder().Build(), string.Empty);

        string message = manager.BuildMissingSecretMessage(new SecretDefinition
        {
            Key = "DMB:Stripe:SecretKey",
            Owner = "DMBStripe",
            DisplayName = "Stripe API secret key"
        });

        Assert.Multiple(() =>
        {
            Assert.That(message, Does.Contain("Environment: Unspecified"));
            Assert.That(message, Does.Contain("Secret store: Unspecified"));
            Assert.That(message, Does.Contain("ServerHelperConfiguration:Secrets:Environment"));
            Assert.That(message, Does.Contain("Host environment: <empty>"));
        });
    }

    [Test]
    public void MissingSecretDiagnosticIncludesExpectedTypeAndAcceptedValues()
    {
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            Environment = SecretUsageEnvironment.LocalWebsite
        };

        string message = manager.BuildMissingSecretMessage(new SecretDefinition
        {
            Key = "DMB:Sample:Environment",
            Owner = "DMBServerHelperUnitTests",
            DisplayName = "Sample environment",
            ValueType = "SampleSecretEnvironment enum",
            FormatHint = "Use the textual enum name.",
            ExampleValue = "Preproduction",
            AcceptedValues = new List<string>
            {
                "Development (0)",
                "PlayTest (1)",
                "Preproduction (2)",
                "Production (3)"
            }
        });

        Assert.Multiple(() =>
        {
            Assert.That(message, Does.Contain("Expected type: SampleSecretEnvironment enum"));
            Assert.That(message, Does.Contain("Format: Use the textual enum name."));
            Assert.That(message, Does.Contain("Example value: Preproduction"));
            Assert.That(message, Does.Contain("Accepted values:"));
            Assert.That(message, Does.Contain("- Development (0)"));
            Assert.That(message, Does.Contain("- PlayTest (1)"));
            Assert.That(message, Does.Contain("- Preproduction (2)"));
            Assert.That(message, Does.Contain("- Production (3)"));
        });
    }

    [Test]
    public void MissingSecretDiagnosticAsksForExplicitEnvironmentWhenHostEnvironmentNameIsUnknown()
    {
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.Auto
        };
        manager.Configure(new ConfigurationBuilder().Build(), "BlueGreenSlotA");

        string message = manager.BuildMissingSecretMessage(new SecretDefinition
        {
            Key = "DMB:Stripe:SecretKey",
            Owner = "DMBStripe",
            DisplayName = "Stripe API secret key"
        });

        Assert.Multiple(() =>
        {
            Assert.That(message, Does.Contain("Environment: Unspecified"));
            Assert.That(message, Does.Contain("Secret store: Unspecified"));
            Assert.That(message, Does.Contain("ServerHelperConfiguration:Secrets:Environment"));
            Assert.That(message, Does.Contain("Host environment: BlueGreenSlotA"));
        });
    }

    [Test]
    public void RedactNeverExposesSecretValue()
    {
        SecretManager manager = new SecretManager();

        Assert.That(manager.Redact("sensitive-placeholder-value"), Is.EqualTo("********"));
    }

    [Test]
    public void RedactReturnsEmptyStringForMissingValue()
    {
        SecretManager manager = new SecretManager();

        Assert.That(manager.Redact(null), Is.EqualTo(string.Empty));
    }

    [Test]
    public void RequiredSecretsIsReadOnlyPublicSurface()
    {
        SecretManager manager = new SecretManager();
        manager.Require("DMB:Stripe:SecretKey", "DMBStripe", "Stripe API secret key");

        PropertyInfo property = typeof(SecretManager).GetProperty(nameof(SecretManager.RequiredSecrets))!;
        SecretDefinition returnedDefinition = manager.RequiredSecrets[0];
        returnedDefinition.DisplayName = "Mutated outside";

        Assert.Multiple(() =>
        {
            Assert.That(property.SetMethod, Is.Null);
            Assert.That(manager.RequiredSecrets, Is.Not.InstanceOf<List<SecretDefinition>>());
            Assert.That(manager.RequiredSecrets, Has.Count.EqualTo(1));
            Assert.That(manager.RequiredSecrets[0].DisplayName, Is.EqualTo("Stripe API secret key"));
        });
    }

    [Test]
    public void RequireRejectsEmptyKey()
    {
        SecretManager manager = new SecretManager();

        Assert.Throws<ArgumentException>(() => manager.Require(new SecretDefinition()));
    }

    [Test]
    public void RequireReplacesExistingDefinitionByKey()
    {
        SecretManager manager = new SecretManager();

        manager.Require("DMB:Stripe:SecretKey", "DMBStripe", "First name");
        manager.Require("DMB:Stripe:SecretKey", "DMBStripe", "Second name");

        Assert.Multiple(() =>
        {
            Assert.That(manager.RequiredSecrets, Has.Count.EqualTo(1));
            Assert.That(manager.RequiredSecrets[0].DisplayName, Is.EqualTo("Second name"));
        });
    }

    [Test]
    public void RequireStoresDetachedSecretDefinition()
    {
        SecretManager manager = new SecretManager();
        SecretDefinition definition = new SecretDefinition
        {
            Key = "DMB:Stripe:SecretKey",
            Owner = "DMBStripe",
            DisplayName = "Stripe API secret key"
        };

        manager.Require(definition);
        definition.DisplayName = "Mutated outside";

        Assert.That(manager.RequiredSecrets[0].DisplayName, Is.EqualTo("Stripe API secret key"));
    }

    [Test]
    public void UseLoggerRejectsNullLogger()
    {
        SecretManager manager = new SecretManager();

        Assert.Throws<ArgumentNullException>(() => manager.UseLogger(null!));
    }

    [Test]
    public void ValidateRequiredSecretsDoesNotLogWhenMissingSecretLoggingIsDisabled()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        CapturingSecretLogger logger = new CapturingSecretLogger();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            LogMissingSecrets = false
        };
        manager.UseLogger(logger);
        manager.Configure(configuration, "Development");
        manager.Require("DMB:Stripe:WebhookSecret", "DMBStripe", "Stripe webhook signing secret");

        SecretValidationResult result = manager.ValidateRequiredSecrets();

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(logger.Warnings, Is.Empty);
        });
    }

    [Test]
    public void ValidateRequiredSecretsDoesNotThrowByDefaultInLocalWebsite()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.UserSecrets,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, "Development");
        manager.Require("DMB:Email:SmtpPassword", "DMBServerEmailHelper", "SMTP password");

        SecretValidationResult result = manager.ValidateRequiredSecrets();

        Assert.Multiple(() =>
        {
            Assert.That(manager.EffectiveFailFast, Is.False);
            Assert.That(result.Success, Is.False);
            Assert.That(result.Issues, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void ValidateRequiredSecretsThrowsConciseExceptionWhenMissingSecretsAreLogged()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        CapturingSecretLogger logger = new CapturingSecretLogger();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            FailFast = true,
            LogMissingSecrets = true
        };
        manager.UseLogger(logger);
        manager.Configure(configuration, "Development");
        manager.Require("DMB:Stripe:WebhookSecret", "DMBStripe", "Stripe webhook signing secret");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.ValidateRequiredSecrets())!;

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("Missing required secrets: DMB:Stripe:WebhookSecret"));
            Assert.That(exception.Message, Does.Not.Contain("DMB__Stripe__WebhookSecret=<secret-value>"));
            Assert.That(logger.Warnings, Has.Count.EqualTo(1));
            Assert.That(logger.Warnings[0], Does.Contain("DMB__Stripe__WebhookSecret=<secret-value>"));
        });
    }

    [Test]
    public void ValidateRequiredSecretsReportsMissingRegisteredSecret()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.UserSecrets,
            Environment = SecretUsageEnvironment.LocalUnitTests,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, "Testing");
        manager.Require("DMB:Email:SmtpPassword", "DMBServerEmailHelper", "SMTP password");

        SecretValidationResult result = manager.ValidateRequiredSecrets();

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.Issues, Has.Count.EqualTo(1));
            Assert.That(result.Issues[0].Message, Does.Contain("dotnet user-secrets set"));
        });
    }

    [Test]
    public void ValidateRequiredSecretsSucceedsWhenRegisteredSecretExists()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DMB:Email:SmtpPassword"] = "resolved-email-value"
            })
            .Build();
        SecretManager manager = new SecretManager
        {
            LogMissingSecrets = false
        };
        manager.Configure(configuration, "Testing");
        manager.Require("DMB:Email:SmtpPassword", "DMBServerEmailHelper", "SMTP password");

        SecretValidationResult result = manager.ValidateRequiredSecrets();

        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.Issues, Is.Empty);
        });
    }

    [Test]
    public void ValidateRequiredSecretsThrowsByDefaultInPreProduction()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, "PreProduction");
        manager.Require("DMB:Pennylane:ApiToken", "DMBPennylane", "Pennylane API token");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.ValidateRequiredSecrets())!;

        Assert.Multiple(() =>
        {
            Assert.That(manager.FailFast, Is.False);
            Assert.That(manager.EffectiveFailFast, Is.True);
            Assert.That(exception.Message, Does.Contain("DMB__Pennylane__ApiToken=<secret-value>"));
        });
    }

    [Test]
    public void ValidateRequiredSecretsThrowsByDefaultInProduction()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, "Production");
        manager.Require("DMB:Stripe:SecretKey", "DMBStripe", "Stripe API secret key");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.ValidateRequiredSecrets())!;

        Assert.Multiple(() =>
        {
            Assert.That(manager.FailFast, Is.False);
            Assert.That(manager.EffectiveFailFast, Is.True);
            Assert.That(exception.Message, Does.Contain("DMB__Stripe__SecretKey=<secret-value>"));
        });
    }

    [Test]
    public void ValidateRequiredSecretsThrowsWhenFailFastIsEnabled()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            FailFast = true,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, "Development");
        manager.Require("DMB:Stripe:WebhookSecret", "DMBStripe", "Stripe webhook signing secret");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.ValidateRequiredSecrets())!;

        Assert.That(exception.Message, Does.Contain("DMB__Stripe__WebhookSecret=<secret-value>"));
    }

    [Test]
    public void ValidateRequiredSecretsThrowsWhenHostEnvironmentNameIsEmpty()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, string.Empty);
        manager.Require("DMB:Stripe:SecretKey", "DMBStripe", "Stripe API secret key");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.ValidateRequiredSecrets())!;

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("Host environment: <empty>"));
            Assert.That(exception.Message, Does.Contain("ServerHelperConfiguration:Secrets:Environment"));
            Assert.That(exception.Message, Does.Contain("Production"));
        });
    }

    [Test]
    public void ValidateRequiredSecretsThrowsWhenHostEnvironmentNameIsUnknown()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        SecretManager manager = new SecretManager
        {
            Store = SecretStoreKind.EnvironmentVariables,
            LogMissingSecrets = false
        };
        manager.Configure(configuration, "BlueGreenSlotA");
        manager.Require("DMB:Stripe:SecretKey", "DMBStripe", "Stripe API secret key");

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            manager.ValidateRequiredSecrets())!;

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("Host environment: BlueGreenSlotA"));
            Assert.That(exception.Message, Does.Contain("ServerHelperConfiguration:Secrets:Environment"));
            Assert.That(exception.Message, Does.Contain("Production"));
        });
    }
}
