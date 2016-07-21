namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Enumeration of supported query types.
    /// </summary>
    public enum QueryType
    {
        /// <summary>
        /// No query type is specifed.
        /// </summary>
        None,

        /// <summary>
        /// Indicates a select query.
        /// </summary>
        Select,

        /// <summary>
        /// Indicates an insert query.
        /// </summary>
        Insert,

        /// <summary>
        /// Indicates an update query.
        /// </summary>
        Update,

        /// <summary>
        /// Indicates a delete query.
        /// </summary>
        Delete
    }
}
