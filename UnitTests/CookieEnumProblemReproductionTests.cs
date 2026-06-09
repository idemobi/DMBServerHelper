#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBServerHelper;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

#endregion

namespace DMBserverHelperUnitTest;

[TestFixture]
public class CookieEnumProblemReproductionTests
{
    #region Setup/Teardown

    [SetUp]
    public void SetUp()
    {
        CookieGlobal.KDictionary.Clear();
    }

    #endregion

    private enum Color
    {
        Red,
        Green,
        Blue
    }

    [Test]
    public void GetValue_IsCaseSensitive_ShouldFailForDifferentCase()
    {
        DefaultHttpContext context = new DefaultHttpContext();
        // CookieEnum uses Enum.TryParse(value, out T parsedValue) which is case-sensitive by default.
        context.Request.Headers.Cookie = "Color=green";

        CookieEnum<Color> cookie = new CookieEnum<Color>(
            "Color", "Color", "Color", CookieDefinitionGroup.Optional, Color.Red);

        Color value = cookie.GetValue(context);

        // If it was case-insensitive, it would be Color.Green.
        // But since it's case-sensitive, it should fail to parse "green" and return the default (Red).
        Assert.That(value, Is.EqualTo(Color.Red), "Expected it to be Red because 'green' (lowercase) shouldn't match 'Green'");
    }

    [Test]
    public void GetValue_ReparsesDefaultValueEveryTime()
    {
        CookieEnum<Color> cookie = new CookieEnum<Color>(
            "Color", "Color", "Color", CookieDefinitionGroup.Optional, Color.Green);

        // Manually corrupt the public DefaultValue field
        cookie.DefaultValue = "Invalid";

        DefaultHttpContext context = new DefaultHttpContext();
        // No cookie present

        Color value = cookie.GetValue(context);

        // Since DefaultValue is now "Invalid", Enum.TryParse("Invalid", out T result) will fail.
        // result will be default(Color) which is Color.Red (0).
        // But the original default was Color.Green.
        Assert.That(value, Is.EqualTo(Color.Red));
        Assert.That(value, Is.Not.EqualTo(Color.Green));
    }
}