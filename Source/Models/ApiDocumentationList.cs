#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Reflection;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Stores assemblies that should be included in generated API documentation.
    /// </summary>
    /// <remarks>
    ///     Configuration loaders can register assemblies when their API surface should be visible to
    ///     documentation generation tools.
    /// </remarks>
    public static class ApiDocumentationList
    {
        #region Static fields and properties

        private static readonly List<Assembly> ApiAssemblies = new List<Assembly>();

        private static readonly object Lock = new object();

        #endregion

        #region Static methods

        /// <summary>
        ///     Registers an assembly for API documentation generation.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly to register. Duplicate references are ignored.
        /// </param>
        public static void AddApiAssembly(Assembly assembly)
        {
            lock (Lock)
            {
                if (!ApiAssemblies.Contains(assembly))
                {
                    ApiAssemblies.Add(assembly);
                }
            }
        }

        /// <summary>
        ///     Registers the assembly that defines the specified type.
        /// </summary>
        /// <param name="type">
        ///     The type whose declaring assembly should be registered.
        /// </param>
        public static void AddApiType(Type type)
        {
            AddApiAssembly(type.Assembly);
        }

        /// <summary>
        ///     Gets the assemblies registered for API documentation generation.
        /// </summary>
        /// <returns>
        ///     A snapshot array of registered assemblies.
        /// </returns>
        public static Assembly[] GetAssemblies()
        {
            lock (Lock)
            {
                return ApiAssemblies.ToArray();
            }
        }

        #endregion
    }
}