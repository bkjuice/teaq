namespace Teaq
{
    /// <summary>
    /// Enumeration of the kind of string data type to use as a default unless otherwise specified explicitly in a model.
    /// </summary>
    public enum SqlStringType
    {
        /// <summary>
        /// Specifies varchar as the default string data type unless explicitly specified using a model definition.
        /// </summary>
        Varchar,

        /// <summary>
        /// Specifies nvarchar as the default string data type unless explicitly specified using a model definition.
        /// </summary>
        NVarchar
    }
}
