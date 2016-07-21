using System.Reflection;

namespace Teaq.FastReflection
{
    /// <summary>
    /// Container class for common reflection binding scenarios.
    /// </summary>
    public static class CommonBindings
    {
        /// <summary>
        /// Defines a value for <see cref="MemberTypes"/> that includes fields and constructors.
        /// </summary>
        public const MemberTypes FieldsAndCtors = MemberTypes.Constructor | MemberTypes.Field;

        /// <summary>
        /// Defines a value for <see cref="MemberTypes"/> that includes properties and constructors.
        /// </summary>
        public const MemberTypes PropertiesAndCtors = MemberTypes.Constructor | MemberTypes.Property;

        /// <summary>
        /// Defines a value for <see cref="MemberTypes"/> that includes properties and fields.
        /// </summary>
        public const MemberTypes PropertiesAndFields = MemberTypes.Field | MemberTypes.Property;

        /// <summary>
        /// Defines a value for <see cref="MemberTypes"/> that includes properties, fields and constructors.
        /// </summary>
        public const MemberTypes PropertiesFieldsAndCtors = MemberTypes.Field | MemberTypes.Property | MemberTypes.Constructor;

        /// <summary>
        /// Defines a value for <see cref="BindingFlags"/> that includes all public instance data.
        /// </summary>
        public const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        /// <summary>
        /// Defines a value for <see cref="BindingFlags"/> that includes all public instance data for the specified type, ignoring inheritence.
        /// </summary>
        public const BindingFlags PublicInstanceDeclaredOnly = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        /// <summary>
        /// Defines a value for <see cref="BindingFlags"/> that includes all instance data, public and private.
        /// </summary>
        public const BindingFlags PublicAndPrivateInstance = 
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic;
    }
}
