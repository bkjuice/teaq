using System;
using System.Collections.Generic;
using System.Reflection;
using Teaq.FastReflection;
using Teaq.Configuration;
using System.Diagnostics.Contracts;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Extensions for use when generating queries from command specifications.
    /// </summary>
    internal static class QueryPrepExtensions
    {
        /// <summary>
        /// The scope functions method table.
        /// </summary>
        private static readonly Func<RuntimeTypeHandle, string, IEntityConfiguration, bool>[] ScopeFunctions =
           new Func<RuntimeTypeHandle, string, IEntityConfiguration, bool>[]
           {
                (t, s, c) => true,
                (t, s, c) => t.IsPrimitiveOrStringOrNullable() && !(c?.IsExcluded(s) == true),
                (t, s, c) => t.IsPrimitiveOrStringOrNullable() && !(c?.IsExcluded(s) == true || c?.IsComputed(s) == true),
                (t, s, c) => t.IsPrimitiveOrStringOrNullable() && !(c?.IsExcluded(s) == true || c?.IsComputed(s) == true || c?.IsKeyColumn(s) == true),
                (t, s, c) => true,
           };

        /// <summary>
        /// Extracts the column list.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <param name="selectColumnList">The select column list.</param>
        /// <param name="config">The config.</param>
        /// <returns>
        /// The column list.
        /// </returns>
        public static string ExtractSelectColumnList(
            this Type target,
            string tableAlias,
            string[] selectColumnList,
            IEntityConfiguration config = null)
        {
            Contract.Requires(selectColumnList != null || target != null);
            Contract.Requires(selectColumnList.Length > 0 || target != null);

            if (selectColumnList?.Length > 0)
            {
                Contract.Assert(selectColumnList != null);
                return target.FormatColumns(tableAlias, config, selectColumnList);
            }

            var properties = target.GetTypeDescription(MemberTypes.Constructor | MemberTypes.Property).GetProperties();
            var count = properties.GetLength(0);
            var names = new List<string>(count);
            var scopeFunc = QueryType.Select.ScopeFunc();
            for (int i = 0; i < count; i++)
            {
                var property = properties[i];
                var name = property.MemberName;

                if (!scopeFunc(property.PropertyTypeHandle, name, config))
                {
                    continue;
                }

                if (config != null)
                {
                    name = config.ColumnMapping(name);
                }

                names.Add(name);
            }

            return target.FormatColumns(tableAlias, config, names.ToArray());
        }

        /// <summary>
        /// Gets a function to determine if a columns is in scope for the specified query type.
        /// </summary>
        /// <param name="queryType">Type of the query.</param>
        /// <returns>
        /// True if the column is in scope, false otherwise.
        /// </returns>
        public static Func<RuntimeTypeHandle, string, IEntityConfiguration, bool> ScopeFunc(this QueryType queryType)
        {
            return ScopeFunctions[(int)queryType];
        }

        /// <summary>
        /// Checks the columns.
        /// </summary>
        /// <param name="targetType">Type of the target entity.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <param name="config">The config.</param>
        /// <param name="selectColumnList">The select column list.</param>
        /// <returns>
        /// The list of columns or empty.
        /// </returns>
        public static string FormatColumns(
            this Type targetType,
            string tableAlias,
            IEntityConfiguration config = null,
            params string[] selectColumnList)
        {
            Contract.Requires(selectColumnList != null);

            var len = selectColumnList.GetLength(0);
            if (len == 0)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(tableAlias))
            {
                tableAlias = targetType.AsUnqualifiedTable(config);
            }

            var prefix = tableAlias + ".";
            if (len == 1)
            {
                return prefix + selectColumnList[0].EnsureBracketedIdentifier();
            }
            else
            {
                // This sort of sucks...any other way to handle this?
                for(int i = 0; i < selectColumnList.Length; i++)
                {
                    selectColumnList[i] = selectColumnList[i].EnsureBracketedIdentifier();
                }

                var union = ", " + tableAlias + ".";
                return prefix + string.Join(union, selectColumnList);
            }
        }

        /// <summary>
        /// Qualifies the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <param name="config">The config.</param>
        /// <param name="entityType">The configured type of the entity being queried.</param>
        /// <returns>
        /// The filter expression.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
             "Microsoft.Design",
             "CA1004:GenericMethodsShouldProvideTypeParameter",
             Justification = "Fluent type usage, by design. Usage will be in the context of building a typed query.")]
        public static string AsQualifiedColumn(this string columnName, string tableAlias, IEntityConfiguration config, Type entityType)
        {
            if (!string.IsNullOrEmpty(tableAlias))
            {
                return tableAlias.EnsureBracketedIdentifier() + "." + columnName.EnsureBracketedIdentifier();
            }
            else
            {
                var tableName = entityType.AsUnqualifiedTable(config);
                return tableName + "." + columnName.EnsureBracketedIdentifier();
            }
        }

        /// <summary>
        /// Creates the qualified table name from the specified type using the optional entity configuration.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="config">The config.</param>
        /// <returns>The schema qualified table name.</returns>
        public static string AsQualifiedTable(this Type target, IEntityConfiguration config)
        {
            Contract.Requires(target != null);

            return config == null ? "[dbo]." + target.Name.EnsureBracketedIdentifier() : config.SchemaName + "." + config.TableName;
        }

        /// <summary>
        /// Creates the unqualified table name from the specified type using the optional entity configuration.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="config">The config.</param>
        /// <returns>The unqualified table name.</returns>
        public static string AsUnqualifiedTable(this Type target, IEntityConfiguration config)
        {
            Contract.Requires(target != null);

            return config?.TableName ?? target.Name.EnsureBracketedIdentifier();
        }
    }
}
