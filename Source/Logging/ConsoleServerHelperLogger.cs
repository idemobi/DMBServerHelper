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
        #region Static methods

        private static ConsoleColor GetWarningLineColor(string line)
        {
            string trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("Missing secret:", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("Invalid secret:", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.Red;
            }

            if (trimmedLine.StartsWith("Set this environment variable", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("Store the value", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("Create this Azure Key Vault secret", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("Add the value", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("Configure ServerHelperConfiguration", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.Yellow;
            }

            if (trimmedLine.StartsWith("Expected type:", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("Format:", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("Example value:", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("Accepted values:", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.Magenta;
            }

            if (trimmedLine.EndsWith(":", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.Cyan;
            }

            if (trimmedLine.StartsWith("export ", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("$env:", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("launchctl ", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("echo ", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("[Environment]::", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("az ", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.Contains("=<secret-value>", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.Green;
            }

            if (trimmedLine.StartsWith("Save,", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("Grant the application identity", StringComparison.OrdinalIgnoreCase) ||
                trimmedLine.StartsWith("Do not commit", StringComparison.OrdinalIgnoreCase))
            {
                return ConsoleColor.DarkYellow;
            }

            return ConsoleColor.Gray;
        }

        private static void WriteColoredLine(TextWriter writer, string line, ConsoleColor color)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                writer.WriteLine(line);
            }
            finally
            {
                Console.ForegroundColor = previousColor;
            }
        }

        private static void WriteColoredWarning(string message)
        {
            if (Console.IsErrorRedirected)
            {
                Console.Error.WriteLine(message);
                return;
            }

            foreach (string line in message.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
            {
                WriteColoredLine(Console.Error, line, GetWarningLineColor(line));
            }
        }

        private static void WriteLine(TextWriter writer, string message, ConsoleColor color, bool isRedirected)
        {
            if (isRedirected)
            {
                writer.WriteLine(message);
                return;
            }

            WriteColoredLine(writer, message, color);
        }

        #endregion

        #region Instance methods

        #region From interface IServerHelperLogger

        /// <inheritdoc />
        public void Error(string message)
        {
            WriteLine(Console.Error, message, ConsoleColor.Red, Console.IsErrorRedirected);
        }

        /// <inheritdoc />
        public void Error(string message, Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            WriteLine(Console.Error, $"{message} Exception={exception.GetType().Name}.", ConsoleColor.Red, Console.IsErrorRedirected);
        }

        /// <inheritdoc />
        public void Information(string message)
        {
            WriteLine(Console.Out, message, ConsoleColor.Gray, Console.IsOutputRedirected);
        }

        /// <inheritdoc />
        public void Warning(string message)
        {
            WriteColoredWarning(message);
        }

        #endregion

        #endregion
    }
}
