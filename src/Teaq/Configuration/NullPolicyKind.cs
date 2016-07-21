namespace Teaq.Configuration
{
    /// <summary>
    /// Enumerates the kind of null handling to perform for a specific property or primitive value.
    /// </summary>
    public enum NullPolicyKind
    {
        /// <summary>
        /// Indicates when reading a primitive as null, the default value will be used.
        /// </summary>
        IncludeAsDefaultValue,

        /// <summary>
        /// Indicates when reading a primitive as null, the value will be skipped.
        /// </summary>
        Skip,
    }
}
