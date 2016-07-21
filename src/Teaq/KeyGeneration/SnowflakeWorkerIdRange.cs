namespace Teaq.KeyGeneration
{
    /// <summary>
    /// Enumerates the allowed worker ID number ranges for a Snowflake key domain.
    /// </summary>
    public enum SnowflakeWorkerIdRange
    {
        /// <summary>
        /// The default initialization value. This is not valid for use when 
        /// configuring a key domain.
        /// </summary>
        NotSet,

        /// <summary>
        /// Sets the expected key domain to be a maximum of 255 machines.
        /// </summary>
        Max255 = 8,

        /// <summary>
        /// Sets the expected key domain to be a maximum of 1023 machines.
        /// </summary>
        Max1023 = 10,

        /// <summary>
        /// Sets the expected key domain to be a maximum of 4095 machines.
        /// </summary>
        Max4095 = 12
    }
}
