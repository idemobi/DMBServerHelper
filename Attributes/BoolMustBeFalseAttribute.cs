#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.ComponentModel.DataAnnotations;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Specifies that a boolean value must be false. This validation attribute ensures that the associated
    ///     property, field, or parameter is explicitly set to false. If the value is not false, the validation fails.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BoolMustBeFalseAttribute : ValidationAttribute
    {
        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoolMustBeFalseAttribute" /> class.
        /// </summary>
        public BoolMustBeFalseAttribute()
            : base("The {0} field must be false.")
        {
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Formats the error message for the validation attribute by replacing placeholders
        ///     with the given name and localizing the message if applicable.
        /// </summary>
        /// <param name="name">The name to include in the formatted error message.</param>
        /// <returns>The formatted error message string.</returns>
        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "";
            }

            string message = ErrorMessageString ?? "The {0} field must be false.";

            return WebLocalizer.GetDataAnnotation(message, name);
        }

        /// <summary>
        ///     Validates whether the provided value is explicitly set to false.
        /// </summary>
        /// <param name="value">The value to validate, expected to be a boolean.</param>
        /// <returns>
        ///     <c>true</c>, if the value is a boolean and equals false; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid(object? value)
        {
            if (value is bool booleanValue)
            {
                return booleanValue == false;
            }

            return false;
        }

        #endregion
    }
}