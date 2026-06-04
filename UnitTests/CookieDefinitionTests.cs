#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Globalization;
using DMBServerHelper;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using SetCookieHeaderValue = Microsoft.Net.Http.Headers.SetCookieHeaderValue;

#endregion

namespace DMBserverHelperUnitTest;

[TestFixture]
internal sealed class CookieDefinitionTests
{
    private enum CookieDisplayMode
    {
        Compact,
        Expanded
    }

    #region Setup/Teardown

    [SetUp]
    public void SetUp()
    {
        CookieGlobal.KDictionary.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        CookieGlobal.KDictionary.Clear();
    }

    #endregion

    [TestCase(CookieDefinitionGroup.Functional)]
    [TestCase(CookieDefinitionGroup.Consent)]
    public void ConstructorForFunctionalOrConsentCookieForcesNonEditableAndNonDeletable(CookieDefinitionGroup group)
    {
        CookieBool cookie = new CookieBool(
            "Required",
            "Required",
            "Required cookie.",
            group,
            true,
            deletable: true,
            manualEditable: true);

        Assert.Multiple(() =>
        {
            Assert.That(cookie.Deletable, Is.False);
            Assert.That(cookie.ManualEditable, Is.False);
        });
    }

    [Test]
    public void ConstructorRegistersSanitizedCookieName()
    {
        CookieString cookie = new CookieString(
            " User Preference ",
            "User preference",
            "Stores the user preference.",
            CookieDefinitionGroup.Optional,
            "default");

        Assert.Multiple(() =>
        {
            Assert.That(cookie.Name, Is.EqualTo("UserPreference"));
            Assert.That(CookieGlobal.KDictionary.ContainsKey("UserPreference"), Is.True);
            Assert.That(CookieString.GetCookieDefinition("UserPreference"), Is.SameAs(cookie));
        });
    }

    [Test]
    public void GetValueWhenCookieIsMissingReturnsDefaultValue()
    {
        CookieInt cookie = new CookieInt(
            "Counter",
            "Counter",
            "Counter cookie.",
            CookieDefinitionGroup.Optional,
            42);

        Assert.That(cookie.GetValue(new DefaultHttpContext()), Is.EqualTo(42));
    }

    [Test]
    public void CookieEnumGetValueWhenCookieValueIsInvalidReturnsDefaultValue()
    {
        DefaultHttpContext context = new DefaultHttpContext();
        context.Request.Headers.Cookie = "DisplayMode=InvalidValue";
        CookieEnum<CookieDisplayMode> cookie = new CookieEnum<CookieDisplayMode>(
            "DisplayMode",
            "Display mode",
            "Stores the display mode.",
            CookieDefinitionGroup.Optional,
            CookieDisplayMode.Expanded);

        Assert.That(cookie.GetValue(context), Is.EqualTo(CookieDisplayMode.Expanded));
    }

    [Test]
    public void CookieFloatGetValueUsesInvariantCulture()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
        try
        {
            DefaultHttpContext context = new DefaultHttpContext();
            context.Request.Headers.Cookie = "Ratio=1.25000";
            CookieFloat cookie = new CookieFloat(
                "Ratio",
                "Ratio",
                "Stores a ratio.",
                CookieDefinitionGroup.Optional,
                2.5F);

            Assert.That(cookie.GetValue(context), Is.EqualTo(1.25F));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Test]
    public void TypedCookieGetValueWhenStoredValueIsInvalidReturnsDefaultValue()
    {
        DefaultHttpContext context = new DefaultHttpContext();
        context.Request.Headers.Cookie = "Feature=not-a-bool; Counter=invalid; Unsigned=-1; ShortValue=invalid; UShortValue=-1; LongValue=invalid; Ratio=invalid";

        CookieBool boolCookie = new CookieBool("Feature", "Feature", "Feature flag.", CookieDefinitionGroup.Optional, true);
        CookieInt intCookie = new CookieInt("Counter", "Counter", "Counter cookie.", CookieDefinitionGroup.Optional, 42);
        CookieUInt uintCookie = new CookieUInt("Unsigned", "Unsigned", "Unsigned cookie.", CookieDefinitionGroup.Optional, 12);
        CookieShort shortCookie = new CookieShort("ShortValue", "Short value", "Short cookie.", CookieDefinitionGroup.Optional, 7);
        CookieUShort ushortCookie = new CookieUShort("UShortValue", "UShort value", "Unsigned short cookie.", CookieDefinitionGroup.Optional, 9);
        CookieLong longCookie = new CookieLong("LongValue", "Long value", "Long cookie.", CookieDefinitionGroup.Optional, 123456789L);
        CookieFloat floatCookie = new CookieFloat("Ratio", "Ratio", "Ratio cookie.", CookieDefinitionGroup.Optional, 2.5F);

        Assert.Multiple(() =>
        {
            Assert.That(boolCookie.GetValue(context), Is.True);
            Assert.That(intCookie.GetValue(context), Is.EqualTo(42));
            Assert.That(uintCookie.GetValue(context), Is.EqualTo(12U));
            Assert.That(shortCookie.GetValue(context), Is.EqualTo((short)7));
            Assert.That(ushortCookie.GetValue(context), Is.EqualTo((ushort)9));
            Assert.That(longCookie.GetValue(context), Is.EqualTo(123456789L));
            Assert.That(floatCookie.GetValue(context), Is.EqualTo(2.5F));
        });
    }

    [Test]
    public void TypedCookieGetValueWhenDefaultValueIsCorruptedDoesNotThrow()
    {
        CookieBool boolCookie = new CookieBool("Feature", "Feature", "Feature flag.", CookieDefinitionGroup.Optional, true) { DefaultValue = "invalid" };
        CookieInt intCookie = new CookieInt("Counter", "Counter", "Counter cookie.", CookieDefinitionGroup.Optional, 42) { DefaultValue = "invalid" };
        CookieUInt uintCookie = new CookieUInt("Unsigned", "Unsigned", "Unsigned cookie.", CookieDefinitionGroup.Optional, 12) { DefaultValue = "invalid" };
        CookieShort shortCookie = new CookieShort("ShortValue", "Short value", "Short cookie.", CookieDefinitionGroup.Optional, 7) { DefaultValue = "invalid" };
        CookieUShort ushortCookie = new CookieUShort("UShortValue", "UShort value", "Unsigned short cookie.", CookieDefinitionGroup.Optional, 9) { DefaultValue = "invalid" };
        CookieLong longCookie = new CookieLong("LongValue", "Long value", "Long cookie.", CookieDefinitionGroup.Optional, 123456789L) { DefaultValue = "invalid" };
        CookieFloat floatCookie = new CookieFloat("Ratio", "Ratio", "Ratio cookie.", CookieDefinitionGroup.Optional, 2.5F) { DefaultValue = "invalid" };

        Assert.Multiple(() =>
        {
            Assert.That(boolCookie.GetValue(new DefaultHttpContext()), Is.False);
            Assert.That(intCookie.GetValue(new DefaultHttpContext()), Is.EqualTo(0));
            Assert.That(uintCookie.GetValue(new DefaultHttpContext()), Is.EqualTo(0U));
            Assert.That(shortCookie.GetValue(new DefaultHttpContext()), Is.EqualTo((short)0));
            Assert.That(ushortCookie.GetValue(new DefaultHttpContext()), Is.EqualTo((ushort)0));
            Assert.That(longCookie.GetValue(new DefaultHttpContext()), Is.EqualTo(0L));
            Assert.That(floatCookie.GetValue(new DefaultHttpContext()), Is.EqualTo(0F));
        });
    }

    [Test]
    public void GenerateCookieDataStringEncodesNameAndValueAndRespectsSecureFlag()
    {
        CookieString cookie = new CookieString(
            "Unsafe Cookie",
            "Unsafe cookie",
            "Stores unsafe text.",
            CookieDefinitionGroup.Optional,
            "default",
            secure: false,
            limitSite: SameSiteMode.Lax);

        string cookieData = cookie.GenerateCookieDataString("value with spaces;and:semicolon");

        Assert.Multiple(() =>
        {
            Assert.That(cookieData, Does.StartWith("UnsafeCookie=value%20with%20spaces%3Band%3Asemicolon;"));
            Assert.That(cookieData, Does.Contain("samesite=Lax"));
            Assert.That(cookieData, Does.Not.Contain("Secure"));
            Assert.That(cookieData, Does.Not.Contain("value with spaces;and:semicolon"));
        });
    }

    [Test]
    public void JavaScriptCookieWritersEncodeUnsafeValues()
    {
        CookieString cookie = new CookieString(
            "Unsafe Cookie",
            "Unsafe cookie",
            "Stores unsafe text.",
            CookieDefinitionGroup.Optional,
            "default");
        const string UnsafeValue = "value\";alert(1);//<script>";

        string generatedScript = cookie.GenerateOnClick(UnsafeValue);
        string installScript = cookie.InstallOnClick(UnsafeValue);
        string deleteScript = cookie.DeleteOnClick(UnsafeValue);

        Assert.Multiple(() =>
        {
            Assert.That(generatedScript, Does.Contain("encodeURIComponent"));
            Assert.That(installScript, Does.Contain("encodeURIComponent"));
            Assert.That(deleteScript, Does.Contain("encodeURIComponent"));
            Assert.That(generatedScript, Does.Not.Contain(UnsafeValue));
            Assert.That(installScript, Does.Not.Contain(UnsafeValue));
            Assert.That(deleteScript, Does.Not.Contain(UnsafeValue));
            Assert.That(generatedScript, Does.Not.Contain("\""));
            Assert.That(installScript, Does.Not.Contain("\""));
            Assert.That(deleteScript, Does.Not.Contain("\""));
            Assert.That(generatedScript, Does.Not.Contain("<script>"));
            Assert.That(installScript, Does.Not.Contain("<script>"));
            Assert.That(deleteScript, Does.Not.Contain("<script>"));
        });
    }

    [Test]
    public void GetValueAsStringForHtmlEncodesCookieValue()
    {
        DefaultHttpContext context = new DefaultHttpContext();
        context.Request.Headers.Cookie = "UnsafeCookie=%3Cscript%3Ealert(1)%3C%2Fscript%3E";
        CookieString cookie = new CookieString(
            "UnsafeCookie",
            "Unsafe cookie",
            "Stores unsafe text.",
            CookieDefinitionGroup.Optional,
            "default");

        string htmlValue = cookie.GetValueAsString(context, forHtml: true);

        Assert.Multiple(() =>
        {
            Assert.That(htmlValue, Does.Not.Contain("<script>"));
            Assert.That(htmlValue, Does.Not.Contain("</script>"));
            Assert.That(htmlValue, Does.Contain("&lt;script&gt;"));
            Assert.That(htmlValue, Does.Not.Contain("&l</span>"));
            Assert.That(htmlValue, Does.Not.Contain("&g</span>"));
            Assert.That(htmlValue, Does.StartWith("<span>"));
            Assert.That(htmlValue, Does.EndWith("</span>"));
        });
    }

    [Test]
    public void SetValueWritesResponseCookieWithConfiguredOptions()
    {
        DefaultHttpContext context = new DefaultHttpContext();
        CookieBool cookie = new CookieBool(
            "Feature",
            "Feature",
            "Feature flag.",
            CookieDefinitionGroup.Optional,
            false,
            secure: true,
            limitSite: SameSiteMode.Strict);

        cookie.SetValue(context, true, seconds: 60);

        string setCookieHeader = context.Response.Headers.SetCookie.ToString();
        Assert.Multiple(() =>
        {
            Assert.That(setCookieHeader, Does.Contain("Feature=True"));
            Assert.That(setCookieHeader, Does.Contain("samesite=strict").IgnoreCase);
            Assert.That(setCookieHeader, Does.Contain("secure").IgnoreCase);
        });
    }

    [Test]
    public void SetValueWhenSecureIsFalseDoesNotWriteSecureAttribute()
    {
        DefaultHttpContext context = new DefaultHttpContext();
        CookieBool cookie = new CookieBool(
            "Feature",
            "Feature",
            "Feature flag.",
            CookieDefinitionGroup.Optional,
            false,
            secure: false,
            limitSite: SameSiteMode.Lax);

        cookie.SetValue(context, true, seconds: 60);

        string setCookieHeader = context.Response.Headers.SetCookie.ToString();
        Assert.Multiple(() =>
        {
            Assert.That(setCookieHeader, Does.Contain("Feature=True"));
            Assert.That(setCookieHeader, Does.Contain("samesite=lax").IgnoreCase);
            Assert.That(setCookieHeader.ToLowerInvariant(), Does.Not.Contain("secure"));
        });
    }

    [Test]
    public void SetValueWithoutCustomLifetimeUsesDurationAsDays()
    {
        DefaultHttpContext context = new DefaultHttpContext();
        CookieBool cookie = new CookieBool(
            "TwoDayFeature",
            "Two day feature",
            "Feature flag.",
            CookieDefinitionGroup.Optional,
            false,
            duration: 2);
        DateTimeOffset minimumExpiration = DateTimeOffset.UtcNow.AddDays(2).AddMinutes(-1);
        DateTimeOffset maximumExpiration = DateTimeOffset.UtcNow.AddDays(2).AddMinutes(1);

        cookie.SetValue(context, true);

        SetCookieHeaderValue setCookie = SetCookieHeaderValue.Parse(context.Response.Headers.SetCookie.ToString());
        DateTimeOffset expiration = setCookie.Expires.GetValueOrDefault();
        Assert.Multiple(() =>
        {
            Assert.That(setCookie.Expires, Is.Not.Null);
            Assert.That(expiration, Is.GreaterThanOrEqualTo(minimumExpiration));
            Assert.That(expiration, Is.LessThanOrEqualTo(maximumExpiration));
        });
    }
}
