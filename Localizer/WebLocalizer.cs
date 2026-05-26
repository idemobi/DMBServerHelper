#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj WebLocalizer.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using Microsoft.Extensions.Localization;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Provides shared access to combined localizers used by server-side PageBuilder packages.
    /// </summary>
    /// <remarks>
    ///     The helper exposes dedicated localizers for data annotation messages and internal package
    ///     text, and can create per-resource combined localizers through <see cref="GetLocalizer{T}"/>.
    /// </remarks>
    public static class WebLocalizer
    {
        #region Static fields and properties

        /// <summary>
        ///     Represents a dictionary that maps a <see cref="Type" /> to its corresponding
        ///     implementation of <see cref="ICombinedStringLocalizer" />.
        ///     This dictionary is used to store and manage all localizer instances needed
        ///     by the application.
        /// </summary>
        private static Dictionary<Type, ICombinedStringLocalizer> All = new Dictionary<Type, ICombinedStringLocalizer>();

        /// <summary>
        ///     Represents the type of the string localizer class to be used by <see cref="WebLocalizer" />.
        ///     The default value is set to <see cref="CombinedStringLocalizer" />; however, it can be overridden
        ///     to point to a different implementation, such as <see cref="CombinedStringLocalizer" />.
        ///     This enables flexibility in selecting the desired implementation of <see cref="ICombinedStringLocalizer" />.
        /// </summary>
        public static Type ClassToUse = typeof(CombinedStringLocalizer);

        /// <summary>
        ///     Provides access to a shared instance of <see cref="ICombinedStringLocalizer" />
        ///     intended for localizing data annotation-related strings.
        /// </summary>
        /// <remarks>
        ///     This static property is used throughout the system for retrieving localized strings
        ///     that correspond to data annotation attributes or related messages.
        ///     It enables consistent localization of validation messages and UI labels.
        /// </remarks>
        [Obsolete($"Direct access is obsolete, use {nameof(GetDataAnnotation)}")]
        public static ICombinedStringLocalizer DataAnnotation = new CombinedStringLocalizer();

        /// <summary>
        ///     A static instance of <see cref="ICombinedStringLocalizer" /> used for managing localized string resources.
        ///     This variable is utilized across various components for accessing or manipulating string localizations.
        /// </summary>
        [Obsolete($"Direct access is obsolete, use {nameof(GetInternal)}")]
        public static ICombinedStringLocalizer Internal = new CombinedStringLocalizer();

        #endregion

        #region Static methods
        /// <summary>
        ///     Resolves a data annotation localized string by key.
        /// </summary>
        /// <param name="key">
        ///     The localization key to resolve.
        /// </param>
        /// <returns>
        ///     The localized string returned by the data annotation localizer.
        /// </returns>
        public static LocalizedString GetDataAnnotation(string key) => DataAnnotation[key];
        /// <summary>
        ///     Resolves a formatted data annotation localized string by key.
        /// </summary>
        /// <param name="key">
        ///     The localization key to resolve.
        /// </param>
        /// <param name="args">
        ///     Format arguments passed to the underlying localizer.
        /// </param>
        /// <returns>
        ///     The formatted localized string returned by the data annotation localizer.
        /// </returns>
        public static LocalizedString GetDataAnnotation(string key, params object[] args) => DataAnnotation[key, args];
        /// <summary>
        ///     Resolves an internal package localized string by key.
        /// </summary>
        /// <param name="key">
        ///     The localization key to resolve.
        /// </param>
        /// <returns>
        ///     The localized string returned by the internal localizer.
        /// </returns>
        public static LocalizedString GetInternal(string key) => Internal[key];
        /// <summary>
        ///     Resolves a formatted internal package localized string by key.
        /// </summary>
        /// <param name="key">
        ///     The localization key to resolve.
        /// </param>
        /// <param name="args">
        ///     Format arguments passed to the underlying localizer.
        /// </param>
        /// <returns>
        ///     The formatted localized string returned by the internal localizer.
        /// </returns>
        public static LocalizedString GetInternal(string key, params object[] args) => Internal[key, args];

        /// <summary>
        ///     Gets the combined localizer associated with a resource marker type.
        /// </summary>
        /// <typeparam name="T">
        ///     The resource marker type.
        /// </typeparam>
        /// <returns>
        ///     A cached <see cref="ICombinedStringLocalizer"/> for <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>
        ///     The first call creates an instance of <see cref="ClassToUse"/>. Later calls reuse the same
        ///     localizer for the same marker type.
        /// </remarks>
        public static ICombinedStringLocalizer GetLocalizer<T>() where T : class
        {
            var type = typeof(T);
            if (!All.TryGetValue(type, out var localizer))
            {
                localizer = Instance(ClassToUse);
                All[type] = localizer;
            }

            return localizer;
        }

        /// <summary>
        ///     Instantiates a new instance of a type that implements <see cref="ICombinedStringLocalizer" />.
        /// </summary>
        /// <param name="type">
        ///     The <see cref="System.Type" /> of the object to instantiate. This must implement
        ///     <see cref="ICombinedStringLocalizer" />.
        /// </param>
        /// <returns>An instantiated object of the specified type, cast to <see cref="ICombinedStringLocalizer" />.</returns>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the specified type does not implement <see cref="ICombinedStringLocalizer" /> or if an instance
        ///     of the specified type cannot be created.
        /// </exception>
        private static ICombinedStringLocalizer Instance(Type type)
        {
            if (!typeof(ICombinedStringLocalizer).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"The type {type.FullName} must implement {nameof(ICombinedStringLocalizer)}.");
            }

            return Activator.CreateInstance(type) as ICombinedStringLocalizer ?? throw new InvalidOperationException($"Impossible to create an instance of {type.FullName}.");
        }

        #endregion
    }
}
