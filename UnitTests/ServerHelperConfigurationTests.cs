#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
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
    }

    [TearDown]
    public void TearDown()
    {
        ServerHelperConfiguration.Config = new ServerHelperConfiguration();
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
    public void ComposeUrlWithControllerOnlyEncodesControllerSegment()
    {
        ServerHelperConfiguration configuration = new ServerHelperConfiguration
        {
            DomainName = "example.com"
        };

        RunAfterConfiguration(configuration);

        Assert.That(configuration.ComposeUrl("Admin Area"), Is.EqualTo("https://www.example.com/Admin%20Area"));
    }
}
