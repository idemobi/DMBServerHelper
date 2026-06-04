#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Collections.Generic;
using Microsoft.Extensions.Localization;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Represents a combined string localizer that aggregates multiple <see cref="IStringLocalizer" /> instances.
    ///     This class implements <see cref="ICombinedStringLocalizer" />, providing functionality for localizing strings
    ///     across multiple resources, injecting additional resources, and performing string manipulation for localization
    ///     icons.
    /// </summary>
    public class CombinedStringLocalizer : ICombinedStringLocalizer
    {
        #region Static fields and properties

        /// <summary>
        ///     A static synchronization object used to ensure thread-safe access
        ///     to critical sections of the code within the <see cref="CombinedStringLocalizer" /> class.
        /// </summary>
        private readonly object _lock = new object();

        #endregion

        #region Instance fields and properties

        /// <summary>
        ///     Represents a dictionary that maps resource names to their corresponding
        ///     <see cref="IStringLocalizer" /> instances. This is used to manage and access
        ///     localization resources in the combined string localizer implementation.
        /// </summary>
        private readonly Dictionary<string, IStringLocalizer> _localizerDico = new Dictionary<string, IStringLocalizer>();

        /// <summary>
        ///     Represents a collection of <see cref="IStringLocalizer" /> instances utilized for resolving localized strings.
        ///     The <see cref="_localizers" /> field stores the list of <see cref="IStringLocalizer" /> objects that are used
        ///     in priority order when resolving localization resources.
        /// </summary>
        private readonly List<IStringLocalizer> _localizers = new List<IStringLocalizer>();

        #region From interface ICombinedStringLocalizer

        /// <summary>
        ///     A reference to the current instance of the class where this code is executed.
        ///     This keyword provides access to the members of the containing instance
        ///     of the class, enabling interaction with non-static members.
        /// </summary>
        public LocalizedString this[string? name]
        {
            get
            {
                if (name == null)
                {
                    return new LocalizedString(string.Empty, string.Empty, resourceNotFound: true);
                }

                IStringLocalizer[] localizers;
                lock (_lock)
                {
                    localizers = _localizers.ToArray();
                }

                foreach (IStringLocalizer localizer in localizers)
                {
                    LocalizedString result = localizer[name];
                    if (!result.ResourceNotFound)
                    {
                        if (result.Value.Trim() == "__EMPTY__")
                        {
                            return new LocalizedString(
                                result.Name,
                                string.Empty,
                                resourceNotFound: false,
                                searchedLocation: result.SearchedLocation
                            );
                        }

                        if (result.Value.Trim() == "__SPACE__")
                        {
                            return new LocalizedString(
                                result.Name,
                                " ",
                                resourceNotFound: false,
                                searchedLocation: result.SearchedLocation
                            );
                        }

                        return result;
                    }
                }
                #if DEBUG
                return new LocalizedString(name, $"{name}", resourceNotFound: true);
                //return new LocalizedString(name, $"{name} 😨", resourceNotFound: true);
                #else
                return new LocalizedString(name, name, resourceNotFound: true);
                #endif
            }
        }

        /// <summary>
        ///     A reference to the current instance of the class or struct where the <see langword="this" /> keyword is used.
        ///     Provides access to the members of the current instance within its containing scope.
        /// </summary>
        public LocalizedString this[string? name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    return new LocalizedString("", "");
                }

                IStringLocalizer[] localizers;
                lock (_lock)
                {
                    localizers = _localizers.ToArray();
                }

                foreach (IStringLocalizer localizer in localizers)
                {
                    var result = localizer[name, arguments];
                    if (!result.ResourceNotFound)
                    {
                        if (result.Value.Trim() == "__EMPTY__")
                        {
                            return new LocalizedString(
                                result.Name,
                                string.Empty,
                                resourceNotFound: false,
                                searchedLocation: result.SearchedLocation
                            );
                        }

                        if (result.Value.Trim() == "__SPACE__")
                        {
                            return new LocalizedString(
                                result.Name,
                                " ",
                                resourceNotFound: false,
                                searchedLocation: result.SearchedLocation
                            );
                        }

                        return result;
                    }
                }
                #if DEBUG
                return new LocalizedString(name, name);
                // return new LocalizedString(name, $"{name} 😨");
                #else
                return new LocalizedString(name,name);
                #endif
            }
        }

        #endregion

        #endregion

        #region Instance methods

        #region From interface ICombinedStringLocalizer

        /// <summary>
        ///     Gets all localized strings available from the registered <see cref="IStringLocalizer" /> instances.
        /// </summary>
        /// <param name="includeParentCultures">
        ///     Specifies whether to include strings from parent cultures in the result.
        /// </param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}" /> of <see cref="LocalizedString" />, containing all the localized strings.
        /// </returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            Dictionary<string, LocalizedString> allStrings = new Dictionary<string, LocalizedString>();
            IStringLocalizer[] localizers;
            lock (_lock)
            {
                localizers = _localizers.ToArray();
            }

            foreach (IStringLocalizer localizer in localizers)
            {
                foreach (var localizedString in localizer.GetAllStrings(includeParentCultures))
                {
                    allStrings[localizedString.Name] = localizedString;
                }
            }

            return allStrings.Values;
        }

        /// <summary>
        ///     Injects a localization resource into the internal collection to be used for string localization.
        /// </summary>
        /// <param name="resourceName">
        ///     The name of the resource to be injected. This represents the key for the resource in the internal dictionary.
        /// </param>
        /// <param name="localizer">
        ///     An instance of <see cref="IStringLocalizer" /> corresponding to the resource being injected.
        /// </param>
        /// <remarks>
        ///     This method ensures thread safety by locking the operation while adding the resource to the internal collection.
        ///     If the resource already exists in the dictionary or the localizer is already present, the resource is not added
        ///     again.
        /// </remarks>
        public void InjectResource(string resourceName, IStringLocalizer localizer)
        {
            lock (_lock)
            {
                if (localizer != null)
                {
                    if (_localizerDico.ContainsKey(resourceName) == false && _localizerDico.ContainsValue(localizer) == false)
                    {
                        _localizerDico.Add(resourceName, localizer);
                        _localizers.Insert(0, localizer);
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
