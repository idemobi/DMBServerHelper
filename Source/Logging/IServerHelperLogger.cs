#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

using System;

namespace DMBServerHelper
{
    /// <summary>
    ///     Receives general diagnostics produced by DMBServerHelper infrastructure.
    /// </summary>
    /// <remarks>
    ///     Host applications can implement this interface to route framework diagnostics to their
    ///     own logging infrastructure. Secret diagnostics may use this logger through the default
    ///     <see cref="ISecretLogger" /> adapter, but secret values must never be written to messages.
    /// </remarks>
    public interface IServerHelperLogger
    {
        /// <summary>
        ///     Writes an informational diagnostic.
        /// </summary>
        /// <param name="message">The diagnostic message to write.</param>
        void Information(string message);

        /// <summary>
        ///     Writes a warning diagnostic.
        /// </summary>
        /// <param name="message">The diagnostic message to write.</param>
        void Warning(string message);

        /// <summary>
        ///     Writes an error diagnostic.
        /// </summary>
        /// <param name="message">The diagnostic message to write.</param>
        void Error(string message);

        /// <summary>
        ///     Writes an error diagnostic associated with an exception.
        /// </summary>
        /// <param name="message">The diagnostic message to write.</param>
        /// <param name="exception">The exception associated with the diagnostic.</param>
        void Error(string message, Exception exception);
    }
}
