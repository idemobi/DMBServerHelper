#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Provides ASP.NET Core MVC client validation metadata for <see cref="BoolMustBeFalseAttribute" />.
    /// </summary>
    public class BoolMustBeFalseAttributeAdapter : AttributeAdapterBase<BoolMustBeFalseAttribute>
    {
        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoolMustBeFalseAttributeAdapter" /> class.
        /// </summary>
        /// <param name="attribute">The validation attribute being adapted.</param>
        /// <param name="stringLocalizer">The optional string localizer supplied by MVC.</param>
        public BoolMustBeFalseAttributeAdapter(BoolMustBeFalseAttribute attribute, IStringLocalizer? stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Adds client validation metadata for the false-required rule.
        /// </summary>
        /// <param name="context">
        ///     The MVC client validation context whose attribute collection is updated.
        /// </param>
        public override void AddValidation(ClientModelValidationContext context)
        {
            // MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-BoolMustBeFalse", GetErrorMessage(context));
        }

        /// <summary>
        ///     Gets the localized error message for client validation metadata.
        /// </summary>
        /// <param name="validationContext">
        ///     The MVC validation context containing model metadata.
        /// </param>
        /// <returns>
        ///     The formatted validation message.
        /// </returns>
        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return Attribute.FormatErrorMessage(validationContext.ModelMetadata.DisplayName ?? string.Empty);
        }

        #endregion
    }
}