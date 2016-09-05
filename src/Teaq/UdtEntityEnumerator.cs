using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Server;
using Teaq.Configuration;
using Teaq.FastReflection;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Enables a collection of user defined data transfer objects to be passed as a user defined table type to SQL Server.
    /// </summary>
    /// <typeparam name="T">The target type to be enumerated.</typeparam>
    public class UdtEntityEnumerator<T> : UdtEnumerator<T> where T : class
    {
        private static readonly PropertyDescription[] PropertiesInScope = GetPropertiesInScopeForType();

        private static readonly SetColumnMethod[] methodsForType = ReflectUdtStrategy(PropertiesInScope);

        private delegate void SetColumnMethod(int ordinal, T value, SqlDataRecord record);

        private SqlMetaData[] columnMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdtEntityEnumerator{T}" /> class.
        /// </summary>
        /// <param name="items">The items to enumarate as a SQL Server user defined table.</param>
        /// <param name="model">The model for any column mappings that must be supported.</param>
        public UdtEntityEnumerator(IEnumerable<T> items, IDataModel model = null)
            : base(items)
        {
            this.columnMetadata = GetColumnMetadata(PropertiesInScope, model);
        }

        /// <summary>
        /// Transforms the specified value to a <see cref="SqlDataRecord" />.
        /// </summary>
        /// <param name="entity">The entity to transform.</param>
        /// <returns>
        /// The resulting <see cref="SqlDataRecord" /> instance.
        /// </returns>
        protected override SqlDataRecord Transform(T entity)
        {
            var record = new SqlDataRecord(this.columnMetadata);
            for (int i = 0; i < methodsForType.Length; ++i)
            {
                methodsForType[i](i, entity, record);
            }

            return record;
        }

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

        private static PropertyDescription[] GetPropertiesInScopeForType()
        {
            return typeof(T).GetTypeDescription().GetProperties()
                .Where(p => IsIncludedColumn(p))
                .OrderBy(p => ColumnOrder(p)).ToArray();
        }

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

                if (columnType != null)
                {
                    items.Add(CreateMetadata(columnName, columnType));
                }
                else
                {
                    items.Add(prop.PropertyType.GetUdtMetaData(columnName));
                }
            }

            return items.ToArray();
        }

        private static SqlMetaData CreateMetadata(string columnName, ColumnDataType columnSpec)
        {
            if (columnSpec.Precision.HasValue && columnSpec.Scale.HasValue)
            {
                return new SqlMetaData(columnName, columnSpec.SqlDataType, columnSpec.Precision.Value, columnSpec.Scale.Value);
            }
            else if (columnSpec.Size.HasValue)
            {
                return new SqlMetaData(columnName, columnSpec.SqlDataType, columnSpec.Size.Value);
            }

            return new SqlMetaData(columnName, columnSpec.SqlDataType);
        }
    }
}
