#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Net;
using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;
using Nager.PublicSuffix.RuleProviders.CacheProviders;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents a composite domain object that encapsulates information about a domain, including its name, port,
    ///     subdomain,
    ///     localhost status, and HTTPS website. Uses input parsing to configure properties from a given URL.
    /// </summary>
    public class DomainComposite
    {
        #region Constants

        /// <summary>
        ///     Represents the default hostname used to identify the local server instance.
        ///     This constant is primarily utilized in comparison operations and domain parsing
        ///     within the <see cref="DomainComposite" /> class to determine whether the input
        ///     corresponds to a local server address.
        /// </summary>
        private const string _LOCALHOST = "localhost";

        private const string _WWW = "www";

        #endregion

        #region Static fields and properties

        private static DomainParser? PublicSuffixDomainParser;
        private static bool PublicSuffixDomainParserBuildAttempted;

        private static readonly object PublicSuffixDomainParserLock = new object();

        private static readonly HashSet<string> CommonCompoundPublicSuffixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ac.jp",
            "ac.nz",
            "ac.uk",
            "co.jp",
            "co.nz",
            "co.uk",
            "com.ar",
            "com.au",
            "com.br",
            "com.cn",
            "com.hk",
            "com.mx",
            "com.sg",
            "com.tr",
            "edu.au",
            "go.jp",
            "gov.au",
            "gov.cn",
            "gov.uk",
            "govt.nz",
            "ne.jp",
            "net.au",
            "net.br",
            "net.cn",
            "net.nz",
            "nhs.uk",
            "or.jp",
            "org.au",
            "org.br",
            "org.cn",
            "org.nz",
            "org.uk",
            "plc.uk",
            "sch.uk"
        };

        private static readonly HashSet<string> CommonCountrySecondLevelLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ac",
            "co",
            "com",
            "edu",
            "go",
            "gov",
            "net",
            "or",
            "org"
        };

        #endregion

        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the domain name of a composite domain object.
        ///     This represents the primary domain without subdomains or additional segments.
        ///     Commonly used in conjunction with other properties such as <see cref="DomainComposite.Port" />
        ///     or <see cref="DomainComposite.SubDomain" /> to construct a fully qualified domain.
        /// </summary>
        public string Domain { set; get; } = string.Empty;

        /// <summary>
        ///     Gets or sets the HTTPS website URL constructed based on the domain, port,
        ///     subdomain, and localhost settings of the <see cref="DomainComposite" /> object.
        ///     This property automatically adjusts its value to reflect the appropriate URL format:
        ///     - For localhost environments indicated by <see cref="DomainComposite.Localhost" />,
        ///     it uses "https://localhost" and includes the <see cref="DomainComposite.Port" />
        ///     if specified.
        ///     - For non-localhost environments, it includes "https://www." if there is no subdomain
        ///     as indicated by <see cref="DomainComposite.SubDomain" />; otherwise, the subdomain
        ///     is omitted.
        /// </summary>
        public string HttpsWebsite { set; get; } = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the domain represents a localhost environment.
        ///     If set to <c>true</c>, the domain is interpreted as a local development server
        ///     and will primarily use default patterns for localhost handling. This property
        ///     influences the construction of <see cref="DomainComposite.HttpsWebsite" />
        ///     in combination with <see cref="DomainComposite.Port" /> and other properties.
        /// </summary>
        public bool Localhost { set; get; } = false;

        /// <summary>
        ///     Gets or sets the port number of the composite domain object.
        ///     This represents the specific network port associated with the domain.
        ///     The port is often used together with <see cref="DomainComposite.Domain" />
        ///     and <see cref="DomainComposite.SubDomain" /> to define a fully qualified URL or network address.
        /// </summary>
        public string Port { set; get; } = string.Empty;

        /// <summary>
        ///     Gets or sets the subdomain of the composite domain object.
        ///     This represents the segment of the address that precedes the primary domain
        ///     and is used to identify subdivisions or specific sections within a domain.
        ///     Commonly used in conjunction with other properties such as
        ///     <see cref="DomainComposite.Domain" /> and <see cref="DomainComposite.Port" />
        ///     to construct a fully qualified domain or URL.
        /// </summary>
        public string SubDomain { set; get; } = string.Empty;

        #endregion

        #region Static methods

        private static string ComposeHttpsWebsite(string host, string port)
        {
            if (string.IsNullOrEmpty(host))
            {
                return string.Empty;
            }

            string hostSegment = host.Contains(':', StringComparison.Ordinal) && host.StartsWith("[", StringComparison.Ordinal) == false
                ? "[" + host + "]"
                : host;

            return string.IsNullOrEmpty(port)
                ? "https://" + hostSegment
                : "https://" + hostSegment + ":" + port;
        }

        private static bool IsCompoundPublicSuffix(string[] parts)
        {
            if (parts.Length < 3)
            {
                return false;
            }

            string twoLabelSuffix = parts[^2] + "." + parts[^1];
            if (CommonCompoundPublicSuffixes.Contains(twoLabelSuffix))
            {
                return true;
            }

            return parts[^1].Length == 2 && CommonCountrySecondLevelLabels.Contains(parts[^2]);
        }

        private static DomainParser? GetOrCreatePublicSuffixDomainParser(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                timeout = TimeSpan.FromSeconds(1);
            }

            lock (PublicSuffixDomainParserLock)
            {
                if (PublicSuffixDomainParserBuildAttempted)
                {
                    return PublicSuffixDomainParser;
                }

                PublicSuffixDomainParserBuildAttempted = true;
                try
                {
                    using HttpClient httpClient = new HttpClient
                    {
                        Timeout = timeout
                    };
                    LocalFileSystemCacheProvider cacheProvider = new LocalFileSystemCacheProvider();
                    CachedHttpRuleProvider ruleProvider = new CachedHttpRuleProvider(cacheProvider, httpClient);
                    ruleProvider.BuildAsync().GetAwaiter().GetResult();
                    PublicSuffixDomainParser = new DomainParser(ruleProvider);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    PublicSuffixDomainParser = null;
                }

                return PublicSuffixDomainParser;
            }
        }

        private static string StripPresentationSubdomain(string subDomain)
        {
            if (string.Equals(subDomain, _WWW, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if (subDomain.StartsWith(_WWW + ".", StringComparison.OrdinalIgnoreCase))
            {
                return subDomain[(_WWW.Length + 1)..];
            }

            return subDomain;
        }

        private static bool TryParseWithPublicSuffixList(string host, bool onlineRefreshEnabled, TimeSpan refreshTimeout, out string domain, out string subDomain)
        {
            domain = string.Empty;
            subDomain = string.Empty;
            if (onlineRefreshEnabled == false)
            {
                return false;
            }

            DomainParser? parser = GetOrCreatePublicSuffixDomainParser(refreshTimeout);
            if (parser == null)
            {
                return false;
            }

            try
            {
                var domainInfo = parser.Parse(host);
                if (string.IsNullOrWhiteSpace(domainInfo.RegistrableDomain))
                {
                    return false;
                }

                domain = domainInfo.RegistrableDomain.ToLowerInvariant();
                subDomain = domainInfo.Subdomain ?? string.Empty;
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }

        private static bool TryReadExplicitPort(Uri uri, out string port)
        {
            port = string.Empty;
            string original = uri.OriginalString;
            int schemeIndex = original.IndexOf("://", StringComparison.Ordinal);
            if (schemeIndex < 0)
            {
                return false;
            }

            string authority = original[(schemeIndex + 3)..];
            int endIndex = authority.IndexOfAny(new[] { '/', '?', '#' });
            if (endIndex >= 0)
            {
                authority = authority[..endIndex];
            }

            int userInfoIndex = authority.LastIndexOf('@');
            if (userInfoIndex >= 0)
            {
                authority = authority[(userInfoIndex + 1)..];
            }

            if (authority.StartsWith("[", StringComparison.Ordinal))
            {
                int endBracketIndex = authority.IndexOf(']');
                if (endBracketIndex >= 0 && authority.Length > endBracketIndex + 1 && authority[endBracketIndex + 1] == ':')
                {
                    port = authority[(endBracketIndex + 2)..];
                    return int.TryParse(port, out _);
                }

                return false;
            }

            int lastColonIndex = authority.LastIndexOf(':');
            if (lastColonIndex < 0 || authority.IndexOf(':') != lastColonIndex)
            {
                return false;
            }

            port = authority[(lastColonIndex + 1)..];
            return int.TryParse(port, out _);
        }

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Represents a composite domain object used for handling and parsing domain-related information.
        /// </summary>
        /// <param name="input">
        ///     The domain, host name, or URL to analyze. Values without an HTTP scheme are parsed as HTTP URLs.
        /// </param>
        /// <remarks>
        ///     The constructor recognizes <c>localhost</c>, preserves a non-default local port, separates root
        ///     domains from subdomains, and composes <see cref="HttpsWebsite" /> as an HTTPS URL.
        /// </remarks>
        public DomainComposite(string input)
            : this(input, true, TimeSpan.FromSeconds(3))
        {
        }

        /// <summary>
        ///     Represents a composite domain object used for handling and parsing domain-related information.
        /// </summary>
        /// <param name="input">
        ///     The domain, host name, or URL to analyze. Values without an HTTP scheme are parsed as HTTP URLs.
        /// </param>
        /// <param name="publicSuffixListOnlineRefreshEnabled">
        ///     A value indicating whether the parser may refresh the Public Suffix List online.
        /// </param>
        /// <param name="publicSuffixListRefreshTimeout">
        ///     The maximum time allowed for the online Public Suffix List refresh.
        /// </param>
        /// <remarks>
        ///     The constructor first handles localhost and IP literals locally. Public domain names use Nager.PublicSuffix
        ///     when the online refresh is enabled and available, then fall back to the built-in parser if needed.
        /// </remarks>
        public DomainComposite(string input, bool publicSuffixListOnlineRefreshEnabled, TimeSpan publicSuffixListRefreshTimeout)
        {
            if (string.IsNullOrWhiteSpace(input) == false)
            {
                input = input.Trim();

                if (!input.StartsWith("http://") && !input.StartsWith("https://"))
                {
                    input = "http://" + input;
                }

                try
                {
                    Uri uri = new Uri(input);
                    string host = uri.Host.ToLowerInvariant();
                    Port = TryReadExplicitPort(uri, out string explicitPort) ? explicitPort : string.Empty;

                    if (host == _LOCALHOST)
                    {
                        Localhost = true;
                        Domain = _LOCALHOST;
                        HttpsWebsite = ComposeHttpsWebsite(Domain, Port);
                        return;
                    }

                    if (IPAddress.TryParse(host, out IPAddress? address))
                    {
                        Localhost = IPAddress.IsLoopback(address);
                        Domain = host;
                        SubDomain = string.Empty;
                        HttpsWebsite = ComposeHttpsWebsite(Domain, Port);
                        return;
                    }

                    if (TryParseWithPublicSuffixList(host, publicSuffixListOnlineRefreshEnabled, publicSuffixListRefreshTimeout, out string parsedDomain, out string parsedSubDomain))
                    {
                        Domain = parsedDomain;
                        SubDomain = StripPresentationSubdomain(parsedSubDomain.ToLowerInvariant());
                    }
                    else
                    {
                        string[] parts = host.Split('.', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2)
                        {
                            Domain = host;
                            SubDomain = string.Empty;
                        }
                        else
                        {
                            int domainLabelCount = IsCompoundPublicSuffix(parts) ? 3 : 2;
                            if (parts.Length < domainLabelCount)
                            {
                                domainLabelCount = parts.Length;
                            }

                            Domain = string.Join(".", parts[^domainLabelCount..]);
                            SubDomain = parts.Length > domainLabelCount ? string.Join(".", parts[..^domainLabelCount]) : string.Empty;
                            SubDomain = StripPresentationSubdomain(SubDomain);
                        }
                    }

                    if (string.IsNullOrEmpty(SubDomain))
                    {
                        HttpsWebsite = ComposeHttpsWebsite(_WWW + "." + Domain, Port);
                    }
                    else
                    {
                        HttpsWebsite = ComposeHttpsWebsite(SubDomain + "." + Domain, Port);
                    }

                    if (string.IsNullOrEmpty(Domain))
                    {
                        HttpsWebsite = string.Empty;
                    }
                    else if (Domain.Contains('.', StringComparison.Ordinal) == false)
                    {
                        HttpsWebsite = ComposeHttpsWebsite(Domain, Port);
                    }
                    else if (string.IsNullOrEmpty(SubDomain))
                    {
                        if (Domain.StartsWith(_WWW + ".", StringComparison.OrdinalIgnoreCase))
                        {
                            HttpsWebsite = ComposeHttpsWebsite(Domain, Port);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        #endregion
    }
}
