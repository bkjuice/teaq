namespace Teaq.Configuration
{
    /// <summary>
    /// Entity specific configuration interface.
    /// </summary>
    public interface IEntityConfiguration
    {
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        string TableName { get; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        string SchemaName { get; }

        /// <summary>
        /// Gets the concurrency column.
        /// </summary>
        /// <value>
        /// The concurrency column.
        /// </value>
        string ConcurrencyProperty { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has an identity column.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has identity; otherwise, <c>false</c>.
        /// </value>
        bool HasIdentity { get; }

        /// <summary>
        /// Indicates if the property is a key column.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>True of the property is a key column, false otherwise.</returns>
        bool IsKeyColumn(string propertyName);

        /// <summary>
        /// Indicates if the property is excluded.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>True of the property is excluded, false otherwise.</returns>
        bool IsExcluded(string propertyName);

        /// <summary>
        /// Determines whether the specified property name is computed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        ///   <c>true</c> if the specified property name is computed; otherwise, <c>false</c>.
        /// </returns>
        bool IsComputed(string propertyName);

        /// <summary>
        /// Gets a column name mapping for the property if one exists.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The column name if mapped otherwise the property name.</returns>
        string ColumnMapping(string propertyName);

        /// <summary>
        /// Gets a property mapping for the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The property name if mapped otherwise the column name.</returns>
        string PropertyMapping(string columnName);

        /// <summary>
        /// Gets the column data type if defined.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The column data type or null if not explicitly defined.</returns>
        ColumnDataType ColumnDataType(string propertyName);
    }
}