#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Rebuilds runtime components that hold resolved secret values.
    /// </summary>
    /// <remarks>
    ///     Feature packages implement this interface when they keep long-lived managers, clients, or requestors
    ///     that must be recreated after a supported configuration reload or an explicit admin rotation command.
    /// </remarks>
    public interface ISecretRotationHandler
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the stable unique name of the handler.
        /// </summary>
        string Name { get; }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Refreshes resolved secret values and recreates runtime components when needed.
        /// </summary>
        /// <param name="secretManager">The central secret manager configured with the current host configuration.</param>
        void RotateResolvedSecrets(SecretManager secretManager);

        #endregion
    }
}