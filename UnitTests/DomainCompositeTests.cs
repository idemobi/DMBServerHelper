#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBServerHelper;
using NUnit.Framework;

#endregion

namespace DMBserverHelperUnitTest;

[TestFixture]
internal sealed class DomainCompositeTests
{
    [TestCase("example.com", "example.com", "", false, "", "https://www.example.com")]
    [TestCase("https://docs.example.com/articles", "example.com", "docs", false, "", "https://docs.example.com")]
    [TestCase("example.co.uk", "example.co.uk", "", false, "", "https://www.example.co.uk")]
    [TestCase("https://api.example.co.uk/articles", "example.co.uk", "api", false, "", "https://api.example.co.uk")]
    [TestCase("https://docs.example.com:8443/articles", "example.com", "docs", false, "8443", "https://docs.example.com:8443")]
    [TestCase("http://docs.example.com:80/articles", "example.com", "docs", false, "80", "https://docs.example.com:80")]
    [TestCase("https://www.docs.example.com/articles", "example.com", "docs", false, "", "https://docs.example.com")]
    [TestCase("intranet:8080", "intranet", "", false, "8080", "https://intranet:8080")]
    [TestCase("localhost", "localhost", "", true, "", "https://localhost")]
    [TestCase("localhost:7174", "localhost", "", true, "7174", "https://localhost:7174")]
    [TestCase("127.0.0.1", "127.0.0.1", "", true, "", "https://127.0.0.1")]
    [TestCase("127.0.0.1:5000", "127.0.0.1", "", true, "5000", "https://127.0.0.1:5000")]
    [TestCase("[::1]:5001", "::1", "", true, "5001", "https://[::1]:5001")]
    [TestCase("203.0.113.10:5000", "203.0.113.10", "", false, "5000", "https://203.0.113.10:5000")]
    public void ConstructorParsesDomainPartsAndComposesHttpsWebsite(
        string input,
        string expectedDomain,
        string expectedSubDomain,
        bool expectedLocalhost,
        string expectedPort,
        string expectedHttpsWebsite
    )
    {
        DomainComposite composite = new DomainComposite(input, false, TimeSpan.Zero);

        Assert.Multiple(() =>
        {
            Assert.That(composite.Domain, Is.EqualTo(expectedDomain));
            Assert.That(composite.SubDomain, Is.EqualTo(expectedSubDomain));
            Assert.That(composite.Localhost, Is.EqualTo(expectedLocalhost));
            Assert.That(composite.Port, Is.EqualTo(expectedPort));
            Assert.That(composite.HttpsWebsite, Is.EqualTo(expectedHttpsWebsite));
        });
    }

    [TestCase("https://user:password@docs.example.com:8443/articles", "8443", "https://docs.example.com:8443")]
    [TestCase("https://docs.example.com/articles:8443", "", "https://docs.example.com")]
    public void ConstructorReadsExplicitPortFromAuthorityOnly(string input, string expectedPort, string expectedHttpsWebsite)
    {
        DomainComposite composite = new DomainComposite(input, false, TimeSpan.Zero);

        Assert.Multiple(() =>
        {
            Assert.That(composite.Port, Is.EqualTo(expectedPort));
            Assert.That(composite.HttpsWebsite, Is.EqualTo(expectedHttpsWebsite));
        });
    }

    [Test]
    public void ConstructorWithBlankInputKeepsEmptyValues()
    {
        DomainComposite composite = new DomainComposite("   ");

        Assert.Multiple(() =>
        {
            Assert.That(composite.Domain, Is.Empty);
            Assert.That(composite.SubDomain, Is.Empty);
            Assert.That(composite.HttpsWebsite, Is.Empty);
            Assert.That(composite.Localhost, Is.False);
        });
    }
}