#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj BoolMustBeTrueAttribute.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.ComponentModel.DataAnnotations;

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Specifies that a boolean value must be <see langword="true"/>.
    /// </summary>
    /// <remarks>
    ///     The attribute can be applied to properties, fields, and method arguments. Error messages are resolved
    ///     through <see cref="WebLocalizer.GetDataAnnotation(string, object[])"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class BoolMustBeTrueAttribute : ValidationAttribute
    {
        #region Instance constructors and destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoolMustBeTrueAttribute"/> class.
        /// </summary>
        public BoolMustBeTrueAttribute()
            : base("The {0} field must be true.")
        {
        }

        #endregion

        #region Instance methods
        /// <summary>
        ///     Formats and localizes the validation error message.
        /// </summary>
        /// <param name="name">
        ///     The display name of the validated member.
        /// </param>
        /// <returns>
        ///     The localized validation message.
        /// </returns>
        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "";
            }

            string message = ErrorMessageString ?? "The {0} field must be true.";

            return WebLocalizer.GetDataAnnotation(message, name);
        }
        /// <summary>
        ///     Validates whether the provided value is explicitly <see langword="true"/>.
        /// </summary>
        /// <param name="value">
        ///     The value to validate.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> when <paramref name="value"/> is a boolean equal to <see langword="true"/>;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public override bool IsValid(object? value)
        {
            if (value is bool booleanValue)
            {
                return booleanValue == true;
            }

            return false;
        }

        #endregion
    }
}
