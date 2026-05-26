#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBServerHelper.csproj SessionDefinitionKind.cs create at 2026/04/07 21:04:27
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Defines the serialized value kind used by a session definition.
    /// </summary>
    public enum SessionDefinitionKind
    {
        /// <summary>
        ///     Indicates that the session value is stored as a string.
        /// </summary>
        StringKind,

        /// <summary>
        ///     Indicates that the session value is stored as an enum name.
        /// </summary>
        EnumKind,

        /// <summary>
        ///     Indicates that the session value is stored as a signed integer.
        /// </summary>
        IntKind,

        /// <summary>
        ///     Indicates that the session value is stored as an unsigned integer.
        /// </summary>
        UIntKind,

        /// <summary>
        ///     Indicates that the session value is stored as a signed long integer.
        /// </summary>
        /// <remarks>
        ///     This member is used by typed long session definitions.
        /// </remarks>
        LongKind,

        /// <summary>
        ///     Indicates that the session value is stored as an unsigned long integer.
        /// </summary>
        ULongKind,

        /// <summary>
        ///     Indicates that the session value is stored as a boolean.
        /// </summary>
        BoolKind,

        /// <summary>
        ///     Indicates that the session value is stored as a floating-point number.
        /// </summary>
        FloatKind,
    }
}
