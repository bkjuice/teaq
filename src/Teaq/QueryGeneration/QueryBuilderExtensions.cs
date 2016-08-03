using System;
using System.Collections.Generic;
using System.Reflection;
using Teaq.FastReflection;
using Teaq.Configuration;
using System.Diagnostics.Contracts;

namespace Teaq.QueryGeneration
{
    internal static class QueryBuilderExtensions
    {
        private static readonly string[] JoinStrings = new string[]
        {
            "INNER",
            "LEFT",
            "RIGHT",
        };

        private static readonly Func<RuntimeTypeHandle, string, IEntityConfiguration, bool>[] ScopeFunctions =
           new Func<RuntimeTypeHandle, string, IEntityConfiguration, bool>[]
           {
                (t, s, c) => true,
                (t, s, c) => t.IsPrimitiveOrStringOrNullable() && !(c?.IsExcluded(s) == true),
                (t, s, c) => t.IsPrimitiveOrStringOrNullable() && !(c?.IsExcluded(s) == true || c?.IsComputed(s) == true),
                (t, s, c) => t.IsPrimitiveOrStringOrNullable() && !(c?.IsExcluded(s) == true || c?.IsComputed(s) == true || c?.IsKeyColumn(s) == true),
                (t, s, c) => true,
           };

        public static string ToJoinString(this JoinType join)
        {
            return JoinStrings[(int)join];
        }

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

        public static Func<RuntimeTypeHandle, string, IEntityConfiguration, bool> ScopeFunc(this QueryType queryType)
        {
            return ScopeFunctions[(int)queryType];
        }

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

        public static string AsQualifiedTable(this Type target, IEntityConfiguration config)
        {
            Contract.Requires(target != null);

            return config == null ? "[dbo]." + target.Name.EnsureBracketedIdentifier() : config.SchemaName + "." + config.TableName;
        }

        public static string AsUnqualifiedTable(this Type target, IEntityConfiguration config)
        {
            Contract.Requires(target != null);

            return config?.TableName ?? target.Name.EnsureBracketedIdentifier();
        }

        internal static Type EnsureConcreteType<T>(this IDataModel dataModel)
        {
            return dataModel?.GetConcreteType<T>() ?? typeof(T);
        }

        internal static IEntityConfiguration TryGetEntityConfig<T>(this IDataModel dataModel)
        {
            return dataModel?.GetEntityConfig(typeof(T));
        }
    }
}
