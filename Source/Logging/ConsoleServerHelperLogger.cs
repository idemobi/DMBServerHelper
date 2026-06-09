#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Writes DMBServerHelper diagnostics to the console.
    /// </summary>
    /// <remarks>
    ///     Informational messages are written to <see cref="Console.Out" />. Warnings and errors are
    ///     written to <see cref="Console.Error" />. Exception diagnostics include the exception type
    ///     by default and avoid stack traces so framework logs remain concise.
    /// </remarks>
    public sealed class ConsoleServerHelperLogger : IServerHelperLogger
    {
        #region Instance methods

        #region From interface IServerHelperLogger

        /// <inheritdoc />
        public void Error(string message)
        {
            Console.Error.WriteLine(message);
        }

        /// <inheritdoc />
        public void Error(string message, Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            Console.Error.WriteLine($"{message} Exception={exception.GetType().Name}.");
        }

        /// <inheritdoc />
        public void Information(string message)
        {
            Console.Out.WriteLine(message);
        }

        /// <inheritdoc />
        public void Warning(string message)
        {
            Console.Error.WriteLine(message);
        }

        #endregion

        #endregion
    }
}