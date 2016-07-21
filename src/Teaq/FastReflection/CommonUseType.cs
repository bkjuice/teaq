namespace Teaq.FastReflection
{
    /// <summary>
    /// Enumerates names of commonly used type categories when performing reflection based tasks.
    /// </summary>
    public enum CommonUseType
    {
        /// <summary>
        /// An unspecified assumed complex type.
        /// </summary>
        UnspecifiedComplexType,

        /// <summary>
        /// An enumerated type.
        /// </summary>
        EnumType,

        /// <summary>
        /// A nullable enumerated type.
        /// </summary>
        NullableEnumType,

        /// <summary>
        /// An array type.
        /// </summary>
        ArrayType,

        /// <summary>
        /// A dictionary type.
        /// </summary>
        DictionaryType,

        /// <summary>
        /// A list type that implements IList.
        /// </summary>
        ListType,

        /// <summary>
        /// An enumerable type that implements IEnumerable.
        /// </summary>
        EnumerableType,

        /// <summary>
        /// A delegate type.
        /// </summary>
        DelegateType,

        /// <summary>
        /// A primitive type.
        /// </summary>
        PrimitiveType,

        /// <summary>
        /// A nullable primitive type.
        /// </summary>
        NullablePrimitiveType,

        /// <summary>
        /// A string type.
        /// </summary>
        StringType,
    }
}
