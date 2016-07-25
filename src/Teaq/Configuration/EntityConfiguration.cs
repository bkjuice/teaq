using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Teaq.Configuration
{
    /// <summary>
    /// Class that contains configuration info for a specified entity class.
    /// </summary>
    internal class EntityConfiguration : IEntityConfiguration
    {
        /// <summary>
        /// The property column mappings dictionary, to match properties to column names when generating a query.
        /// </summary>
        private Dictionary<string, string> propertyColumnMappings;

        /// <summary>
        /// The property column reverse mappings dictionary, to match column names to properties when materializing an object.
        /// </summary>
        private Dictionary<string, string> propertyColumnReverseMappings;

        /// <summary>
        /// The excluded properties that will not be part of query generation.
        /// </summary>
        private HashSet<string> excludedProperties;

        /// <summary>
        /// The names of database key for the mapped table.
        /// </summary>
        private HashSet<string> keyColumns;

        /// <summary>
        /// The names of  computed columns for the mapped table.
        /// </summary>
        private HashSet<string> computedColumns;

        /// <summary>
        /// The column data types mapped to column names and properties.
        /// </summary>
        private Dictionary<string, ColumnDataType> columnDataTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityConfiguration" /> class.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        public EntityConfiguration(string schemaName, string tableName)
        {
            this.TableName = tableName.EnsureBracketedIdentifier();
            this.SchemaName = schemaName.EnsureBracketedIdentifier();
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string TableName { get; private set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string SchemaName { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has an identity column.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has identity; otherwise, <c>false</c>.
        /// </value>
        public bool HasIdentity { get; set; }

        /// <summary>
        /// Gets or sets the name of the concurrency property.
        /// </summary>
        public string ConcurrencyProperty { get; set; }

        /// <summary>
        /// Excludes the specified property from query generation.
        /// </summary>
        /// <param name="propertyName">Name of the property to exclude.</param>
        public void ExcludeProperty(string propertyName)
        {
            if (this.excludedProperties == null)
            {
                this.excludedProperties = new HashSet<string>();
            }

            this.excludedProperties.Add(propertyName);
        }

        /// <summary>
        /// Adds the computed column.
        /// </summary>
        /// <param name="columnName">Name of the column to add.</param>
        public void AddComputedColumn(string columnName)
        {
            if (this.computedColumns == null)
            {
                this.computedColumns = new HashSet<string>();
            }

            this.computedColumns.Add(columnName);
        }

        /// <summary>
        /// Adds the key column.
        /// </summary>
        /// <param name="columnName">Name of the column to add.</param>
        public void AddKeyColumn(string columnName)
        {
            if (this.keyColumns == null)
            {
                this.keyColumns = new HashSet<string>();
            }

            this.keyColumns.Add(columnName);
        }

        /// <summary>
        /// Adds the property column mapping.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="columnName">Name of the column.</param>
        public void AddPropertyColumnMapping(string propertyName, string columnName)
        {
            Contract.Requires(propertyName != null);
            Contract.Requires(columnName != null);

            if (this.propertyColumnMappings == null)
            {
                this.propertyColumnMappings = new Dictionary<string, string>();
                this.propertyColumnReverseMappings = new Dictionary<string, string>();
            }

            this.propertyColumnMappings.Add(propertyName, columnName);
            this.propertyColumnReverseMappings.Add(columnName, propertyName);
        }

        /// <summary>
        /// Adds a mapping of a property name to the type of the column data.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="dataType">The type of data.</param>
        public void AddColumnDataType(string propertyName, ColumnDataType dataType)
        {
            Contract.Requires(propertyName != null);
            Contract.Requires(dataType != null);

            if (this.columnDataTypes == null)
            {
                this.columnDataTypes = new Dictionary<string, ColumnDataType>();
            }

            this.columnDataTypes.Add(propertyName, dataType);
        }

        /// <summary>
        /// Indicates if the property is a key column.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// True of the property is a key column, false otherwise.
        /// </returns>
        public bool IsKeyColumn(string propertyName)
        {
            if (this.keyColumns == null)
            {
                return false;
            }

            return this.keyColumns.Contains(propertyName);
        }

        /// <summary>
        /// Indicates if the property is excluded.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// True of the property is excluded, false otherwise.
        /// </returns>
        public bool IsExcluded(string propertyName)
        {
            if (this.excludedProperties == null)
            {
                return false;
            }

            return this.excludedProperties.Contains(propertyName);
        }

        /// <summary>
        /// Determines whether the specified property name is computed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        ///   <c>true</c> if the specified property name is computed; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComputed(string propertyName)
        {
            if (this.computedColumns == null)
            {
                return false;
            }

            return this.computedColumns.Contains(propertyName);
        }

        /// <summary>
        /// Gets a column name mapping for the property if one exists.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The column name if mapped otherwise the property name.
        /// </returns>
        public string ColumnMapping(string propertyName)
        {
            if (this.propertyColumnMappings != null)
            {
                string mappedValue;
                if (this.propertyColumnMappings.TryGetValue(propertyName, out mappedValue))
                {
                    return mappedValue;
                }
            }

            return propertyName;
        }

        /// <summary>
        /// Gets a property mapping for the column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>
        /// The property name if mapped otherwise the column name.
        /// </returns>
        public string PropertyMapping(string columnName)
        {
            ////Contract.Requires(string.IsNullOrEmpty(columnName) == false);
            ////Contract.Ensures(string.IsNullOrEmpty(Contract.Result<string>()) == false);
             
            if (this.propertyColumnReverseMappings != null)
            {
                string mappedValue;
                if (this.propertyColumnReverseMappings.TryGetValue(columnName, out mappedValue))
                {
                    return mappedValue;
                }
            }

            return columnName;
        }

        /// <summary>
        /// Gets the column data type if defined.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The column data type or null if not explicitly defined.
        /// </returns>
        public ColumnDataType ColumnDataType(string propertyName)
        {
            if (columnDataTypes != null)
            {
                ColumnDataType target;
                if (this.columnDataTypes.TryGetValue(propertyName, out target))
                {
                    return target;
                }
            }

            return null;
        }
    }
}
