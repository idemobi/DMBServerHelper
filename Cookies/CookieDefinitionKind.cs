#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBServerHelper
{
    /// <summary>
    ///     Defines the serialized value kind used by a cookie definition.
    /// </summary>
    public enum CookieDefinitionKind
    {
        /// <summary>
        ///     Indicates that the cookie stores a string value.
        /// </summary>
        StringKind,

        /// <summary>
        ///     Indicates that the cookie stores an enum value by name.
        /// </summary>
        EnumKind,

        /// <summary>
        ///     Indicates that the cookie stores a signed integer value.
        /// </summary>
        IntKind,

        /// <summary>
        ///     Indicates that the cookie stores an unsigned integer value.
        /// </summary>
        UIntKind,

        /// <summary>
        ///     Indicates that the cookie stores a signed long integer value.
        /// </summary>
        LongKind,

        /// <summary>
        ///     Indicates that the cookie stores an unsigned long integer value.
        /// </summary>
        ULongKind,

        /// <summary>
        ///     Indicates that the cookie stores a boolean value.
        /// </summary>
        BoolKind,

        /// <summary>
        ///     Indicates that the cookie stores a floating-point value.
        /// </summary>
        FloatKind,
    }
}