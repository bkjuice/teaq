using System;
using Teaq.Configuration;
using Teaq.QueryGeneration;

namespace Teaq.Tests.Stubs
{
    internal class QueryBuilderStub<T> : QueryBuilder<T>
    {
        public QueryBuilderStub() : base() { }

        public QueryBuilderStub(IDataModel dataModel) : base(dataModel) { }

        public IDataModel VerifiableDataModel
        {
            get
            {
                return this.DataModel;
            }
        }

        public static string VerifiableCheckColumns(string tableAlias, Type targetType, IEntityConfiguration config, params string[] selectColumnList)
        {
            return targetType.FormatColumns(tableAlias, config, selectColumnList);
        }

        public static string VerifiableFormatColumns(string tableAlias, Type targetType, IEntityConfiguration config, params string[] selectColumnList)
        {
            return targetType.FormatColumns(tableAlias, config, selectColumnList);
        }

        public static bool VerifiableColumnInScope(string propertyName, Type propertyType, QueryType queryType, IEntityConfiguration config = null)
        {
            var scopeFunc = queryType.ScopeFunc();
            return scopeFunc(propertyType.TypeHandle, propertyName, config);
        }
    }
}