#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Identifies the runtime context in which application secrets are consumed.
    /// </summary>
    public enum SecretUsageEnvironment
    {
        /// <summary>
        ///     Lets <see cref="SecretManager" /> infer the context from the host environment name.
        /// </summary>
        Auto,

        /// <summary>
        ///     Indicates that the host environment name is not recognized and must be configured explicitly.
        /// </summary>
        Unspecified,

        /// <summary>
        ///     Secrets are consumed by local automated tests.
        /// </summary>
        LocalUnitTests,

        /// <summary>
        ///     Secrets are consumed by a local website or manual preview host.
        /// </summary>
        LocalWebsite,

        /// <summary>
        ///     Secrets are consumed by a preproduction, staging, or acceptance environment.
        /// </summary>
        PreProduction,

        /// <summary>
        ///     Secrets are consumed by the production environment.
        /// </summary>
        Production
    }
}