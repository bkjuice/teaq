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
    /// <summary>
    /// Extenstions to support SQL Parameter processing.
    /// </summary>
    internal static class SqlParameterExtensions
    {
        /// <summary>
        /// Nexts the index of the batch if the provided batch specification is not null.
        /// </summary>
        /// <param name="batch">The batch specification.</param>
        /// <returns>
        /// The next batch increment value used to qualify query parameters.
        /// </returns>
        public static int? TryNextBatchIndex(this QueryBatch batch)
        {
            if (batch == null)
            {
                return null;
            }

            return batch.NextBatchIndex();
        }

        /// <summary>
        /// Gets the provided value or db null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The provided value or DbNull if the value is null.</returns>
        public static object ValueOrDBNull(this object value)
        {
            if (value != null)
            {
                return value;
            }

            return DBNull.Value;
        }

        /// <summary>
        /// Makes the batch parameters from the provided list of SQL Parameter items.
        /// </summary>
        /// <param name="parameters">The sql parameters to qualify for the batch.</param>
        /// <param name="batch">The batch.</param>
        /// <returns>A copy of the sql parameters properly qualified for the batch.</returns>
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

        /// <summary>
        /// Makes the parameter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="sourceColumnName">Name of the source column.</param>
        /// <param name="parameterBaseName">Name of the parameter base.</param>
        /// <param name="batchQualifier">The batch qualifier.</param>
        /// <param name="parameterQualifier">The parameter qualifier.</param>
        /// <param name="indexer">The indexer.</param>
        /// <returns>
        /// The SQL parameter with a qualified name.
        /// </returns>
        public static SqlParameter MakeParameter(
            this object value, 
            string sourceColumnName, 
            string parameterBaseName, 
            int? batchQualifier, 
            int? parameterQualifier, 
            int? indexer)
        {
            return value.MakeParameter(sourceColumnName, null, parameterBaseName, batchQualifier, parameterQualifier, indexer);
        }

        /// <summary>
        /// Makes the parameter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="sourceColumnName">Name of the source column.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="parameterBaseName">Name of the parameter base.</param>
        /// <param name="batchQualifier">The batch qualifier.</param>
        /// <param name="parameterQualifier">The parameter qualifier.</param>
        /// <param name="indexer">The indexer, for lists of values.</param>
        /// <returns>
        /// The SQL parameter with a qualified name.
        /// </returns>
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
                parameter.SqlDbType = SqlDbType.VarChar;
            }

            parameter.SourceColumn = sourceColumnName;
            return parameter;
        }

        /// <summary>
        /// Copies the parameters.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// A list of deep copies of the global SQL Parameters.
        /// </returns>
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

        /// <summary>
        /// Copies the parameters.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// A list of deep copies of the global SQL Parameters.
        /// </returns>
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

        /// <summary>
        /// Copies the instance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The copy.</returns>
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

        /// <summary>
        /// Removes the leading @ symbol, if found.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The parameter name without a leading @ symbol.</returns>
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

        /// <summary>
        /// Gets the name of the qualified parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter base.</param>
        /// <param name="batchQualifier">The batch qualifier.</param>
        /// <param name="parameterQualifier">The parameter qualifier.</param>
        /// <param name="indexer">The indexer.</param>
        /// <returns>The qualified parameter name.</returns>
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
