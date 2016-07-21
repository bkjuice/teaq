using System.Data;

namespace Teaq.Configuration
{
    /// <summary>
    /// Configuration to define an explicit data type for an entity property.
    /// </summary>
    public class ColumnDataType
    {
        /// <summary>
        /// Gets or sets the SQL data type.
        /// </summary>
        public SqlDbType SqlDataType { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        public byte? Precision { get; set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        public byte? Scale { get; set; }
    }
}
