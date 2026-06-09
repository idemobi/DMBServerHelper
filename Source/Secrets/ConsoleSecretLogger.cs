#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBServerHelper
{
    internal sealed class ConsoleSecretLogger : ISecretLogger
    {
        #region Instance methods

        #region From interface ISecretLogger

        void ISecretLogger.Warning(string message)
        {
            ServerHelperConfiguration.Logger.Warning(message);
        }

        #endregion

        #endregion
    }
}