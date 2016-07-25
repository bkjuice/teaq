using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using Teaq.Configuration;

namespace Teaq.Expressions
{
    internal static class SqlParameterExtensions
    {
        public static SqlParameter[] QualifyForBatch(this SqlParameterCollection parameters, QueryBatch batch)
        {
            Contract.Requires(parameters != null);
            Contract.Requires(batch != null);

            var batchQualifier = batch.NextBatchIndex();
            var sqlParams = parameters.Cast<SqlParameter>().ToArray().Copy();
            for (int i = 0; i < sqlParams?.Length; ++i)
            {
                var param = sqlParams[i];
                param.SourceColumn = param.SourceColumn ?? param.ParameterName.RemoveLeadingSymbol();
                param.ParameterName = param.ParameterName.GetQualifiedParameterName(batchQualifier, -1, -1);
            }

            return sqlParams;
        }

        public static SqlParameter MakeParameter(
            this object value,
            string sourceColumnName,
            ColumnDataType dataType,
            string parameterBaseName,
            int? batchQualifier,
            int? parameterQualifier,
            int? indexer)
        {
            var name = parameterBaseName.GetQualifiedParameterName(batchQualifier, parameterQualifier, indexer);
            if (value == null)
            {
                value = DBNull.Value;
            }

            var parameter = new SqlParameter(name, value);
            if (dataType != null)
            {
                parameter.SqlDbType = dataType.SqlDataType;
                if (dataType.Size.HasValue)
                {
                    parameter.Size = dataType.Size.Value;
                }

                if (dataType.Precision.HasValue)
                {
                    parameter.Precision = dataType.Precision.Value;
                }

                if (dataType.Scale.HasValue)
                {
                    parameter.Scale = dataType.Scale.Value;
                }
            }
            else if (value.GetType().TypeHandle.Equals(typeof(string).TypeHandle))
            {
                parameter.SqlDbType =
                    Repository.DefaultStringType == Repository.StringDataType.Varchar ? SqlDbType.VarChar : SqlDbType.NVarChar;
            }

            parameter.SourceColumn = sourceColumnName;
            return parameter;
        }

        public static SqlParameter[] Copy(this SqlParameter[] source)
        {
            if (source == null)
            {
                return null;
            }

            var copy = new SqlParameter[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                copy[i] = source[i].CopyInstance();
            }

            return copy;
        }

        public static SqlParameter[] Copy(this List<SqlParameter> source)
        {
            if (source == null)
            {
                return null;
            }

            var copy = new SqlParameter[source.Count];
            for (int i = 0; i < source.Count; i++)
            {
                copy[i] = source[i].CopyInstance();
            }

            return copy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SqlParameter CopyInstance(this SqlParameter source)
        {
            Contract.Requires(source != null);

            var copy = new SqlParameter(source.ParameterName, source.SqlDbType, source.Size);
            copy.Value = source.Value;
            copy.SourceColumn = source.SourceColumn;
            copy.TypeName = source.TypeName;
            copy.Precision = source.Precision;
            return copy;
        }

        private static string RemoveLeadingSymbol(this string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                return string.Empty;
            }

            if (parameterName[0] == '@')
            {
                return parameterName.Substring(1);
            }

            return parameterName;
        }

        private static string GetQualifiedParameterName(this string parameterName, int? batchQualifier, int? parameterQualifier, int? indexer)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                parameterName = "@p";
            }

            if (batchQualifier > 0)
            {
                parameterName += batchQualifier.ToString();
            }
            
            if (parameterQualifier > 0)
            {
                parameterName += "x" + parameterQualifier.ToString();
            }

            if (indexer > 0)
            {
                parameterName += "n" + indexer.ToString();
            }

            return parameterName;
        }
    }
}
