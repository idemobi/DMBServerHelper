#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

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
    ///     text, and can create per-resource combined localizers through <see cref="GetLocalizer{T}" />.
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
        ///     Gets or sets the shared data annotation localizer.
        /// </summary>
        /// <remarks>
        ///     This member is kept for source compatibility. New code should use
        ///     <see cref="DataAnnotationLocalizer" /> for configuration and <see cref="GetDataAnnotation(string)" />
        ///     for lookups.
        /// </remarks>
        [Obsolete($"Direct access is obsolete, use {nameof(DataAnnotationLocalizer)} or {nameof(GetDataAnnotation)}")]
        public static ICombinedStringLocalizer DataAnnotation
        {
            get => DataAnnotationLocalizer;
            set => DataAnnotationLocalizer = value;
        }

        /// <summary>
        ///     Gets or sets the shared localizer used for data annotation messages.
        /// </summary>
        /// <remarks>
        ///     This property is the non-obsolete configuration point used by <see cref="GetDataAnnotation(string)" />
        ///     and <see cref="GetDataAnnotation(string, object[])" />.
        /// </remarks>
        public static ICombinedStringLocalizer DataAnnotationLocalizer { get; set; } = new CombinedStringLocalizer();

        /// <summary>
        ///     Gets or sets the shared internal package localizer.
        /// </summary>
        /// <remarks>
        ///     This member is kept for source compatibility. New code should use
        ///     <see cref="InternalLocalizer" /> for configuration and <see cref="GetInternal(string)" /> for lookups.
        /// </remarks>
        [Obsolete($"Direct access is obsolete, use {nameof(InternalLocalizer)} or {nameof(GetInternal)}")]
        public static ICombinedStringLocalizer Internal
        {
            get => InternalLocalizer;
            set => InternalLocalizer = value;
        }

        /// <summary>
        ///     Gets or sets the shared localizer used for internal package messages.
        /// </summary>
        /// <remarks>
        ///     This property is the non-obsolete configuration point used by <see cref="GetInternal(string)" />
        ///     and <see cref="GetInternal(string, object[])" />.
        /// </remarks>
        public static ICombinedStringLocalizer InternalLocalizer { get; set; } = new CombinedStringLocalizer();

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
        public static LocalizedString GetDataAnnotation(string key)
        {
            return DataAnnotationLocalizer[key];
        }

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
        public static LocalizedString GetDataAnnotation(string key, params object[] args)
        {
            return DataAnnotationLocalizer[key, args];
        }

        /// <summary>
        ///     Resolves an internal package localized string by key.
        /// </summary>
        /// <param name="key">
        ///     The localization key to resolve.
        /// </param>
        /// <returns>
        ///     The localized string returned by the internal localizer.
        /// </returns>
        public static LocalizedString GetInternal(string key)
        {
            return InternalLocalizer[key];
        }

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
        public static LocalizedString GetInternal(string key, params object[] args)
        {
            return InternalLocalizer[key, args];
        }

        /// <summary>
        ///     Gets the combined localizer associated with a resource marker type.
        /// </summary>
        /// <typeparam name="T">
        ///     The resource marker type.
        /// </typeparam>
        /// <returns>
        ///     A cached <see cref="ICombinedStringLocalizer" /> for <typeparamref name="T" />.
        /// </returns>
        /// <remarks>
        ///     The first call creates an instance of <see cref="ClassToUse" />. Later calls reuse the same
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