#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Globalization;
using DMBServerHelper;
using DMBserverHelperUnitTest.Helpers;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

#endregion

namespace DMBserverHelperUnitTest;

[TestFixture]
internal sealed class SessionDefinitionTests
{
    #region Setup/Teardown

    [SetUp]
    public void SetUp()
    {
        SessionGlobal.KDictionary.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        SessionGlobal.KDictionary.Clear();
    }

    #endregion

    private enum SessionDisplayMode
    {
        Compact,
        Expanded
    }

    private static DefaultHttpContext CreateSessionContext()
    {
        return new DefaultHttpContext
        {
            Session = new InMemorySession()
        };
    }

    [Test]
    public void ConstructorRegistersSanitizedSessionName()
    {
        SessionString session = new SessionString(
            " Current User ",
            "Current user",
            "Stores the current user.",
            SessionDefinitionGroup.Navigation,
            "anonymous");

        Assert.Multiple(() =>
        {
            Assert.That(session.Name, Is.EqualTo("CurrentUser"));
            Assert.That(SessionGlobal.KDictionary.ContainsKey("CurrentUser"), Is.True);
            Assert.That(SessionString.GetSessionDefinition("CurrentUser"), Is.SameAs(session));
        });
    }

    [Test]
    public void DeleteAllDeletableSessionRemovesOnlyDeletableDefinitions()
    {
        DefaultHttpContext context = CreateSessionContext();
        SessionString deletable = new SessionString(
            "Deletable",
            "Deletable",
            "Can be deleted.",
            SessionDefinitionGroup.Navigation,
            "default",
            deletable: true);
        SessionString retained = new SessionString(
            "Retained",
            "Retained",
            "Cannot be deleted.",
            SessionDefinitionGroup.Navigation,
            "default",
            deletable: false);

        deletable.SetValue(context, "remove");
        retained.SetValue(context, "keep");

        SessionGlobal.DeleteAllDeletableSession(context);

        Assert.Multiple(() =>
        {
            Assert.That(deletable.Exists(context), Is.False);
            Assert.That(retained.Exists(context), Is.True);
            Assert.That(retained.GetValue(context), Is.EqualTo("keep"));
        });
    }

    [Test]
    public void GetValueWhenSessionValueIsMissingReturnsDefaultValue()
    {
        DefaultHttpContext context = CreateSessionContext();
        SessionString session = new SessionString(
            "CurrentUser",
            "Current user",
            "Stores the current user.",
            SessionDefinitionGroup.Navigation,
            "anonymous");

        Assert.That(session.GetValue(context), Is.EqualTo("anonymous"));
    }

    [Test]
    public void SessionEnumGetValueWhenStoredValueIsInvalidReturnsDefaultValue()
    {
        DefaultHttpContext context = CreateSessionContext();
        SessionEnum<SessionDisplayMode> session = new SessionEnum<SessionDisplayMode>(
            "DisplayMode",
            "Display mode",
            "Stores the display mode.",
            SessionDefinitionGroup.Navigation,
            SessionDisplayMode.Expanded);
        context.Session.SetString("DisplayMode", "InvalidValue");

        Assert.That(session.GetValue(context), Is.EqualTo(SessionDisplayMode.Expanded));
    }

    [Test]
    public void SessionFloatGetValueUsesInvariantCulture()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
        try
        {
            DefaultHttpContext context = CreateSessionContext();
            SessionFloat session = new SessionFloat(
                "Ratio",
                "Ratio",
                "Stores a ratio.",
                SessionDefinitionGroup.Navigation,
                2.5F);
            context.Session.SetString("Ratio", "1.25000");

            Assert.That(session.GetValue(context), Is.EqualTo(1.25F));
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Test]
    public void SessionSerializableGetValueWhenSessionIsMissingReturnsDefaultValue()
    {
        DefaultHttpContext context = CreateSessionContext();
        SessionSerializable<Dictionary<string, string>> session = new SessionSerializable<Dictionary<string, string>>(
            "Profile",
            "Profile",
            "Stores profile data.",
            SessionDefinitionGroup.Navigation,
            new Dictionary<string, string> { ["Name"] = "default" });

        Dictionary<string, string>? value = session.GetValue(context);

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.Not.Null);
            Assert.That(value!["Name"], Is.EqualTo("default"));
        });
    }

    [Test]
    public void SessionSerializableGetValueWhenStoredJsonIsInvalidReturnsDefaultValue()
    {
        DefaultHttpContext context = CreateSessionContext();
        SessionSerializable<Dictionary<string, string>> session = new SessionSerializable<Dictionary<string, string>>(
            "Profile",
            "Profile",
            "Stores profile data.",
            SessionDefinitionGroup.Navigation,
            new Dictionary<string, string> { ["Name"] = "default" });
        context.Session.SetString("Profile", "{ invalid json");

        Dictionary<string, string>? value = session.GetValue(context);

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.Not.Null);
            Assert.That(value!["Name"], Is.EqualTo("default"));
        });
    }

    [Test]
    public void SessionSerializableGetValueWhenStoredJsonIsValidReturnsStoredValue()
    {
        DefaultHttpContext context = CreateSessionContext();
        SessionSerializable<Dictionary<string, string>> session = new SessionSerializable<Dictionary<string, string>>(
            "Profile",
            "Profile",
            "Stores profile data.",
            SessionDefinitionGroup.Navigation,
            new Dictionary<string, string> { ["Name"] = "default" });
        session.SetValue(context, new Dictionary<string, string> { ["Name"] = "stored" });

        Dictionary<string, string>? value = session.GetValue(context);

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.Not.Null);
            Assert.That(value!["Name"], Is.EqualTo("stored"));
        });
    }

    [Test]
    public void SessionUIntIncrementWhenStoredValueIsInvalidUsesDefaultValue()
    {
        DefaultHttpContext context = CreateSessionContext();
        SessionUInt session = new SessionUInt(
            "Unsigned",
            "Unsigned",
            "Unsigned value.",
            SessionDefinitionGroup.Navigation,
            12);
        context.Session.SetString("Unsigned", "invalid");

        session.IncrementValue(context);

        Assert.That(context.Session.GetString("Unsigned"), Is.EqualTo("13"));
    }

    [Test]
    public void SetValueAndIncrementValueUpdateStoredSessionValue()
    {
        DefaultHttpContext context = CreateSessionContext();
        SessionInt session = new SessionInt(
            "Counter",
            "Counter",
            "Stores a counter.",
            SessionDefinitionGroup.Navigation,
            10);

        session.SetValue(context, 2);
        session.IncrementValue(context, 3);

        Assert.That(session.GetValue(context), Is.EqualTo(5));
    }

    [Test]
    public void TypedSessionGetValueWhenDefaultValueIsCorruptedDoesNotThrow()
    {
        DefaultHttpContext context = CreateSessionContext();
        SessionBool boolSession = new SessionBool("Feature", "Feature", "Feature flag.", SessionDefinitionGroup.Navigation, true) { DefaultValue = "invalid" };
        SessionInt intSession = new SessionInt("Counter", "Counter", "Counter value.", SessionDefinitionGroup.Navigation, 42) { DefaultValue = "invalid" };
        SessionShort shortSession = new SessionShort("ShortValue", "Short value", "Short value.", SessionDefinitionGroup.Navigation, 7) { DefaultValue = "invalid" };
        SessionUShort ushortSession = new SessionUShort("UShortValue", "UShort value", "Unsigned short value.", SessionDefinitionGroup.Navigation, 9) { DefaultValue = "invalid" };
        SessionLong longSession = new SessionLong("LongValue", "Long value", "Long value.", SessionDefinitionGroup.Navigation, 123456789L) { DefaultValue = "invalid" };
        SessionFloat floatSession = new SessionFloat("Ratio", "Ratio", "Ratio value.", SessionDefinitionGroup.Navigation, 2.5F) { DefaultValue = "invalid" };

        Assert.Multiple(() =>
        {
            Assert.That(boolSession.GetValue(context), Is.False);
            Assert.That(intSession.GetValue(context), Is.EqualTo(0));
            Assert.That(shortSession.GetValue(context), Is.EqualTo((short)0));
            Assert.That(ushortSession.GetValue(context), Is.EqualTo((ushort)0));
            Assert.That(longSession.GetValue(context), Is.EqualTo(0L));
            Assert.That(floatSession.GetValue(context), Is.EqualTo(0F));
        });
    }

    [Test]
    public void TypedSessionGetValueWhenStoredValueIsInvalidReturnsDefaultValue()
    {
        DefaultHttpContext context = CreateSessionContext();
        SessionBool boolSession = new SessionBool("Feature", "Feature", "Feature flag.", SessionDefinitionGroup.Navigation, true);
        SessionInt intSession = new SessionInt("Counter", "Counter", "Counter value.", SessionDefinitionGroup.Navigation, 42);
        SessionShort shortSession = new SessionShort("ShortValue", "Short value", "Short value.", SessionDefinitionGroup.Navigation, 7);
        SessionUShort ushortSession = new SessionUShort("UShortValue", "UShort value", "Unsigned short value.", SessionDefinitionGroup.Navigation, 9);
        SessionLong longSession = new SessionLong("LongValue", "Long value", "Long value.", SessionDefinitionGroup.Navigation, 123456789L);
        SessionFloat floatSession = new SessionFloat("Ratio", "Ratio", "Ratio value.", SessionDefinitionGroup.Navigation, 2.5F);

        context.Session.SetString("Feature", "not-a-bool");
        context.Session.SetString("Counter", "invalid");
        context.Session.SetString("ShortValue", "invalid");
        context.Session.SetString("UShortValue", "-1");
        context.Session.SetString("LongValue", "invalid");
        context.Session.SetString("Ratio", "invalid");

        Assert.Multiple(() =>
        {
            Assert.That(boolSession.GetValue(context), Is.True);
            Assert.That(intSession.GetValue(context), Is.EqualTo(42));
            Assert.That(shortSession.GetValue(context), Is.EqualTo((short)7));
            Assert.That(ushortSession.GetValue(context), Is.EqualTo((ushort)9));
            Assert.That(longSession.GetValue(context), Is.EqualTo(123456789L));
            Assert.That(floatSession.GetValue(context), Is.EqualTo(2.5F));
        });
    }
}