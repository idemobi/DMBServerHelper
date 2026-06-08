#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Receives secret management diagnostics without exposing secret values.
    /// </summary>
    /// <remarks>
    ///     Implement this interface in host applications to route missing-secret instructions to the
    ///     application logging infrastructure.
    /// </remarks>
    public interface ISecretLogger
    {
        /// <summary>
        ///     Writes a warning diagnostic.
        /// </summary>
        /// <param name="message">The diagnostic message to write.</param>
        void Warning(string message);
    }
}
