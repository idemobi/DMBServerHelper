#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Selects custom ASP.NET Core MVC validation adapters for DMBServerHelper validation attributes.
    /// </summary>
    /// <remarks>
    ///     Unknown validation attributes are delegated to the default <see cref="ValidationAttributeAdapterProvider" />.
    /// </remarks>
    public class CustomValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        #region Instance fields and properties

        private readonly IValidationAttributeAdapterProvider baseProvider = new ValidationAttributeAdapterProvider();

        #endregion

        #region Instance methods

        #region From interface IValidationAttributeAdapterProvider

        /// <summary>
        ///     Gets a validation adapter for a validation attribute.
        /// </summary>
        /// <param name="attribute">
        ///     The validation attribute that needs an MVC adapter.
        /// </param>
        /// <param name="stringLocalizer">
        ///     The optional string localizer supplied by MVC.
        /// </param>
        /// <returns>
        ///     A custom adapter for <see cref="BoolMustBeFalseAttribute" /> or <see cref="BoolMustBeTrueAttribute" />,
        ///     or the default adapter for other attributes.
        /// </returns>
        public IAttributeAdapter? GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer? stringLocalizer)
        {
            if (attribute is BoolMustBeFalseAttribute boolMustBeFalseAttribute)
            {
                return new BoolMustBeFalseAttributeAdapter(boolMustBeFalseAttribute, stringLocalizer);
            }

            if (attribute is BoolMustBeTrueAttribute tBoolMustBeTrueAttribute)
            {
                return new BoolMustBeTrueAttributeAdapter(tBoolMustBeTrueAttribute, stringLocalizer);
            }

            return baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
        }

        #endregion

        #endregion
    }
}