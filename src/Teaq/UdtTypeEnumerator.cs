using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.SqlServer.Server;
using Teaq.Configuration;
using Teaq.FastReflection;

namespace Teaq
{
    /// <summary>
    /// Enables a collection of user defined data transfer objects to be passed as a user defined table type to SQL Server.
    /// </summary>
    /// <typeparam name="T">The target type to be enumerated.</typeparam>
    public class UdtTypeEnumerator<T> : IEnumerable<SqlDataRecord> where T : class
    {
        /// <summary>
        /// The supported types used for data table column types.
        /// </summary>
        private static readonly Dictionary<Type, ColumnDataType> typeMappings = new Dictionary<Type, ColumnDataType>
        {
            { typeof(byte?), new ColumnDataType { SqlDataType = SqlDbType.TinyInt } },
            { typeof(bool?), new ColumnDataType { SqlDataType = SqlDbType.Bit } },
            { typeof(char?), new ColumnDataType { SqlDataType = SqlDbType.Char } },
            { typeof(short?), new ColumnDataType { SqlDataType = SqlDbType.SmallInt } },
            { typeof(int?), new ColumnDataType { SqlDataType = SqlDbType.Int } },
            { typeof(long?), new ColumnDataType { SqlDataType = SqlDbType.BigInt } },
            { typeof(float?), new ColumnDataType { SqlDataType = SqlDbType.Float } },
            { typeof(decimal?), new ColumnDataType { SqlDataType = SqlDbType.Decimal } },
            { typeof(Guid?), new ColumnDataType { SqlDataType = SqlDbType.UniqueIdentifier } },
            { typeof(DateTime?), new ColumnDataType { SqlDataType = SqlDbType.DateTime } },
            { typeof(DateTimeOffset?), new ColumnDataType { SqlDataType = SqlDbType.DateTimeOffset} },
            { typeof(byte), new ColumnDataType { SqlDataType = SqlDbType.TinyInt } },
            { typeof(bool), new ColumnDataType { SqlDataType = SqlDbType.Bit } },
            { typeof(char), new ColumnDataType { SqlDataType = SqlDbType.Char } },
            { typeof(short), new ColumnDataType { SqlDataType = SqlDbType.SmallInt } },
            { typeof(int), new ColumnDataType { SqlDataType = SqlDbType.Int } },
            { typeof(long), new ColumnDataType { SqlDataType = SqlDbType.BigInt } },
            { typeof(float), new ColumnDataType { SqlDataType = SqlDbType.Float } },
            { typeof(decimal), new ColumnDataType { SqlDataType = SqlDbType.Decimal } },
            { typeof(Guid), new ColumnDataType { SqlDataType = SqlDbType.UniqueIdentifier } },
            { typeof(DateTime), new ColumnDataType { SqlDataType = SqlDbType.DateTime } },
            { typeof(DateTimeOffset), new ColumnDataType { SqlDataType = SqlDbType.DateTimeOffset} },
            { typeof(string), new ColumnDataType { SqlDataType = SqlDbType.VarChar } },
            { typeof(byte[]), new ColumnDataType { SqlDataType = SqlDbType.VarBinary } },
        };

        /// <summary>
        /// The properties in scope for the type T.
        /// </summary>
        private static readonly PropertyDescription[] PropertiesInScope = GetPropertiesInScopeForType();

        /// <summary>
        /// The methods for the generic type parameter.
        /// </summary>
        private static readonly SetColumnMethod[] methodsForType = ReflectUdtStrategy(PropertiesInScope);

        /// <summary>
        /// Method to set the <see cref="SqlDataRecord" /> that represents a column in the user defined data table.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="value">The value.</param>
        /// <param name="record">The record.</param>
        private delegate void SetColumnMethod(int ordinal, T value, SqlDataRecord record);

        /// <summary>
        /// The column metadata.
        /// </summary>
        private SqlMetaData[] columnMetadata;

        /// <summary>
        /// The items to be enumerated.
        /// </summary>
        private IEnumerable<T> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdtTypeEnumerator{T}" /> class.
        /// </summary>
        /// <param name="items">The items to enumarate as a SQL Server user defined table.</param>
        /// <param name="model">The model for any column mappings that must be supported.</param>
        public UdtTypeEnumerator(IEnumerable<T> items, IDataModel model = null)
        {
            Contract.Requires(items != null);

            this.items = items;
            this.columnMetadata = GetColumnMetadata(PropertiesInScope, model);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<SqlDataRecord> GetEnumerator()
        {
            foreach (var item in items)
            {
                var record = new SqlDataRecord(this.columnMetadata);
                for (int i = 0; i < methodsForType.Length; ++i)
                {
                    methodsForType[i](i, item, record);
                }

                yield return record;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets a value indicating if the property is an included column.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <returns>
        /// True if the property is an included column, false otherwise.
        /// </returns>
        private static bool IsIncludedColumn(PropertyDescription prop)
        {
            var data = prop.GetAttributeData();
            if (data == null || data.Length == 0) return true;
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i].AttributeType == typeof(UdtColumnExcludeAttribute)) return false;
            }

            return true;
        }

        /// <summary>
        /// Gets an explicitly defined columns order, if it exists.
        /// </summary>
        /// <param name="prop">The property to check.</param>
        /// <returns>The order. If not specified, <see cref="int.MaxValue"/> is returned.</returns>
        private static int ColumnOrder(PropertyDescription prop)
        {
            var data = prop.GetAttributeData();
            if (data == null || data.Length == 0) return int.MaxValue;
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i].AttributeType == typeof(UdtColumnOrderAttribute))
                {
                    if (data[i].ConstructorArguments.Count > 0)
                    {
                        return (int)data[i].ConstructorArguments[0].Value;
                    }
                }
            }

            return int.MaxValue;
        }

        /// <summary>
        /// Gets the properties in scope for the compile time type parameter.
        /// </summary>
        /// <returns>The array of properties in scope.</returns>
        private static PropertyDescription[] GetPropertiesInScopeForType()
        {
            return typeof(T).GetTypeDescription().GetProperties()
                .Where(p => IsIncludedColumn(p))
                .OrderBy(p => ColumnOrder(p)).ToArray();
        }

        /// <summary>
        /// Reflects the udt column order and value setter strategy.
        /// </summary>
        /// <param name="propsInScope">The properties in scope.</param>
        /// <returns>
        /// The array of columns for the user defined table.
        /// </returns>
        private static SetColumnMethod[] ReflectUdtStrategy(PropertyDescription[] propsInScope)
        {
            var methods = new List<SetColumnMethod>(propsInScope.Length);
            foreach (var property in propsInScope)
            {
                var targetType = property.PropertyType;
                SetColumnMethod method = (i, v, r) =>
                {
                    r.SetValue(i, property.GetValue(v));
                };

                methods.Add(method);
            }

            return methods.ToArray();
        }

        /// <summary>
        /// Gets the column metadata.
        /// </summary>
        /// <param name="propsInScope">The props in scope.</param>
        /// <param name="model">The model.</param>
        /// <returns>The array of metadata definitions for the user defined table.</returns>
        private static SqlMetaData[] GetColumnMetadata(PropertyDescription[] propsInScope, IDataModel model)
        {
            var items = new List<SqlMetaData>(propsInScope.Length);
            for (int i = 0; i < propsInScope.Length; ++i)
            {
                var prop = propsInScope[i];
                var columnName = prop.MemberName;
                var columnType = default(ColumnDataType);
                var config = model.TryGetEntityConfig<T>();
                if (config != null)
                {
                    columnType = config.ColumnDataType(columnName);
                    columnName = config.ColumnMapping(columnName);
                }

                if (columnType == null)
                {
                    if (!typeMappings.TryGetValue(prop.PropertyType, out columnType))
                    {
                        throw new NotSupportedException(
                            "The property type {0} is not supported by default. Mark this property excluded on type {1}, or define a data model column for this property."
                            .ToFormat(prop.MemberName, typeof(T).FullName));
                    }
                }

                items.Add(CreateMetadata(columnName, columnType));
            }

            return items.ToArray();
        }

        /// <summary>
        /// Creates the metadata.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="columnType">Type of the column.</param>
        /// <returns>The <see cref="SqlMetaData"/> instance for the column type.</returns>
        private static SqlMetaData CreateMetadata(string columnName, ColumnDataType columnType)
        {
            if (columnType.Precision.HasValue && columnType.Scale.HasValue)
            {
                return new SqlMetaData(columnName, columnType.SqlDataType, columnType.Precision.Value, columnType.Scale.Value);
            }
            else if (columnType.Size.HasValue)
            {
                return new SqlMetaData(columnName, columnType.SqlDataType, (long)columnType.Size.Value);
            }
            else
            {
                return new SqlMetaData(columnName, columnType.SqlDataType);
            }
        }
    }
}
