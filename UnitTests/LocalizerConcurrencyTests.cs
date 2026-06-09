#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBServerHelper;
using Microsoft.Extensions.Localization;
using NUnit.Framework;

#endregion

namespace DMBserverHelperUnitTest;

[TestFixture]
internal sealed class LocalizerConcurrencyTests
{
    private sealed class DictionaryStringLocalizer : IStringLocalizer
    {
        #region Instance fields and properties

        private readonly Dictionary<string, string> _values;

        #endregion

        #region Instance constructors and destructors

        public DictionaryStringLocalizer(Dictionary<string, string> values)
        {
            _values = values;
        }

        #endregion

        #region Instance methods

        #region From interface IStringLocalizer

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _values.Select(item => new LocalizedString(item.Key, item.Value, resourceNotFound: false));
        }

        #endregion

        #endregion

        #region Instance indexers

        public LocalizedString this[string name]
        {
            get
            {
                if (_values.TryGetValue(name, out string? value))
                {
                    return new LocalizedString(name, value, resourceNotFound: false);
                }

                return new LocalizedString(name, name, resourceNotFound: true);
            }
        }

        public LocalizedString this[string name, params object[] arguments] => this[name];

        #endregion
    }

    [Test]
    public void CombinedStringLocalizerHandlesConcurrentInjectionAndReads()
    {
        CombinedStringLocalizer localizer = new CombinedStringLocalizer();

        Parallel.For(0, 64, index =>
        {
            string resourceName = "Resource" + index.ToString("D2");
            localizer.InjectResource(resourceName, new DictionaryStringLocalizer(new Dictionary<string, string>
            {
                ["Shared"] = "Value" + index.ToString("D2"),
                [resourceName] = resourceName
            }));

            _ = localizer["Shared"].Value;
            _ = localizer.GetAllStrings(includeParentCultures: false).ToArray();
        });

        LocalizedString sharedValue = localizer["Shared"];
        LocalizedString[] allStrings = localizer.GetAllStrings(includeParentCultures: false).ToArray();
        string[] allStringNames = allStrings.Select(item => item.Name).ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(sharedValue.ResourceNotFound, Is.False);
            Assert.That(allStringNames, Does.Contain("Shared"));
            Assert.That(allStringNames.Count(name => name.StartsWith("Resource", StringComparison.Ordinal)), Is.EqualTo(64));
        });
    }

    [Test]
    public void WebLocalizerReturnsSameInstanceUnderConcurrentAccess()
    {
        ICombinedStringLocalizer[] localizers = Enumerable.Range(0, 64)
            .AsParallel()
            .Select(_ => WebLocalizer.GetLocalizer<LocalizerConcurrencyTests>())
            .ToArray();

        Assert.That(localizers.Distinct().Count(), Is.EqualTo(1));
    }
}