#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj DomainComposite.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

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

        #region Instance constructors and destructors

        /// <summary>
        ///     Represents a composite domain object used for handling and parsing domain-related information.
        /// </summary>
        /// <param name="input">
        ///     The domain, host name, or URL to analyze. Values without an HTTP scheme are parsed as HTTP URLs.
        /// </param>
        /// <remarks>
        ///     The constructor recognizes <c>localhost</c>, preserves a non-default local port, separates root
        ///     domains from subdomains, and composes <see cref="HttpsWebsite"/> as an HTTPS URL.
        /// </remarks>
        public DomainComposite(string input)
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
                    var uri = new Uri(input);
                    var host = uri.Host.ToLowerInvariant();

                    if (host == _LOCALHOST)
                    {
                        Localhost = true;
                        Domain = _LOCALHOST;
                        Port = uri.IsDefaultPort ? "" : $"{uri.Port}";
                    }

                    var parts = host.Split('.');
                    if (parts.Length >= 2)
                    {
                        var rootDomain = $"{parts[^2]}.{parts[^1]}";
                        var subDomain = parts.Length > 2 ? string.Join(".", parts[..^2]) : string.Empty;
                        Localhost = false;
                        Domain = rootDomain;
                        SubDomain = subDomain;
                    }

                    if (Localhost == true)
                    {
                        if (string.IsNullOrEmpty(Port))
                        {
                            HttpsWebsite = "https://" + Domain;
                        }
                        else
                        {
                            HttpsWebsite = "https://" + Domain + ":" + Port;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(SubDomain))
                        {
                            HttpsWebsite = "https://" + _WWW + "." + Domain;
                        }
                        else
                        {
                            HttpsWebsite = "https://" + SubDomain + "." + Domain;
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
