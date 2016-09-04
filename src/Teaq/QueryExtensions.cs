using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Text;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Sponsor class for syntax convieniences when building queries.
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Builds the unbounded query.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="selectColumnList">The select column list.</param>
        /// <returns>
        /// The query command to execute.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Type is passed using gemneric expression to match rest of API.")]
        public static QueryCommand BuildUnboundedSelectCommand<T>(this IDataModel model, params string[] selectColumnList)
        {
            Contract.Requires(model != null);
            Contract.Ensures(Contract.Result<QueryCommand>() != null);

            return model
                .ForEntity<T>()
                .BuildSelect(columnList: selectColumnList)
                .ToCommand();
        }

        /// <summary>
        /// Builds the query.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="selectColumnList">The select column list.</param>
        /// <returns>
        /// The query command to execute.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This is driven by .NET framework design.")]
        public static QueryCommand<T> BuildSelectCommand<T>(
            this IDataModel model,
            Expression<Func<T, bool>> selector,
            params string[] selectColumnList)
        {
            Contract.Requires(model != null);
            Contract.Ensures(Contract.Result<QueryCommand>() != null);

            return model
                .ForEntity<T>()
                .BuildSelect(columnList: selectColumnList)
                .Where(selector)
                .ToCommand();
        }

        /// <summary>
        /// Builds the delete.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// The query command to execute.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This is driven by .NET framework design.")]
        public static QueryCommand BuildDeleteCommand<T>(this IDataModel model, Expression<Func<T, bool>> filter)
        {
            Contract.Requires(model != null);
            Contract.Ensures(Contract.Result<QueryCommand>() != null);

            return model.ForEntity<T>().BuildDelete(filter).ToCommand();
        }

        /// <summary>
        /// Adds the specified batch.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="query">The literal query statement that will not return results.</param>
        /// <param name="canSplitBatch">If set to <c>true</c> the batch can be split after this command, if needed.</param>
        /// <param name="parameters">The optional parameters anonymous object.</param>
        public static void Add(this QueryBatch batch, string query, bool canSplitBatch = true, object parameters = null)
        {
            Contract.Requires(batch != null);
            Contract.Requires(!string.IsNullOrEmpty(query));

            batch.Add(new QueryCommand(query, parameters.GetAnonymousParameters(), canSplitBatch));
        }

        /// <summary>
        /// Adds the specified batch.
        /// </summary>
        /// <typeparam name="T">The type of entity that will be read.</typeparam>
        /// <param name="batch">The batch.</param>
        /// <param name="query">The literal query.</param>
        /// <param name="canSplitBatch">
        /// If set to <c>true</c> the batch can be split after this command, if needed.
        /// </param>
        /// <param name="parameters">The optional parameters anonymous object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Fluent by design.")]
        public static void Add<T>(this QueryBatch batch, string query, bool canSplitBatch = true, object parameters = null)
        {
            Contract.Requires(batch != null);
            Contract.Requires(!string.IsNullOrEmpty(query));

            batch.Add<T>(new QueryCommand(query, parameters.GetAnonymousParameters(), canSplitBatch));
        }

        /// <summary>
        /// Adds a query to a batch for a stored procedure already configured via IDbCommand.
        /// </summary>
        /// <typeparam name="T">The type of resulting entity to be materialized.</typeparam>
        /// <param name="batch">The batch to which the stored procedure invocation will be added.</param>
        /// <param name="command">The command to convert to a batch query command.</param>
        /// <param name="canSplitBatch">
        /// If set to <c>true</c> the batch can be split after this command, if needed.
        /// </param>
        public static void AddStoredProcedure<T>(this QueryBatch batch, SqlCommand command, bool canSplitBatch = true)
        {
            Contract.Requires(batch != null);

            batch.Add<T>(batch.BuildSprocQueryCommand(command, canSplitBatch));
        }

        /// <summary>
        /// Adds a query to a batch for a stored procedure already configured via IDbCommand.
        /// </summary>
        /// <param name="batch">The batch to which the stored procedure invocation will be added.</param>
        /// <param name="command">The command to convert to a batch query command.</param>
        /// <param name="canSplitBatch">
        /// If set to <c>true</c> the batch can be split after this command, if needed.
        /// </param>
        public static void AddStoredProcedure(this QueryBatch batch, SqlCommand command, bool canSplitBatch = true)
        {
            Contract.Requires(batch != null);

            batch.Add(batch.BuildSprocQueryCommand(command, canSplitBatch));
        }

        /// <summary>
        /// Adds a query to a batch using a text command already configured via IDbCommand.
        /// </summary>
        /// <typeparam name="T">The type of resulting entity to be materialized.</typeparam>
        /// <param name="batch">The batch to which the stored procedure invocation will be added.</param>
        /// <param name="command">The command to convert to a batch query command.</param>
        /// <param name="canSplitBatch">
        /// If set to <c>true</c> the batch can be split after this command, if needed.
        /// </param>
        public static void AddTextCommand<T>(this QueryBatch batch, SqlCommand command, bool canSplitBatch = true)
        {
            Contract.Requires(batch != null);

            batch.Add<T>(batch.BuildTextQueryCommand(command, canSplitBatch));
        }

        /// <summary>
        /// Adds a query to a batch using a text command already configured via IDbCommand.
        /// </summary>
        /// <param name="batch">The batch to which the stored procedure invocation will be added.</param>
        /// <param name="command">The command to convert to a batch query command.</param>
        /// <param name="canSplitBatch">
        /// If set to <c>true</c> the batch can be split after this command, if needed.
        /// </param>
        public static void AddTextCommand(this QueryBatch batch, SqlCommand command, bool canSplitBatch = true)
        {
            Contract.Requires(batch != null);

            batch.Add(batch.BuildTextQueryCommand(command, canSplitBatch));
        }

        internal static QueryCommand BuildSprocQueryCommand(this QueryBatch batch, SqlCommand command, bool canSplitBatch)
        {
            Contract.Requires(command != null);

            if (command.CommandType != CommandType.StoredProcedure)
            {
                throw new InvalidOperationException("This method only supports stored procedure command conversions.");
            }

            // batch qualify all parameters:
            var sqlParams = command.Parameters.QualifyForBatch(batch);
            var builder = new StringBuilder(32 + (8 * command.Parameters.Count));
            builder.Append("\r\nexec ").Append(command.CommandText).Append(" ");
            for (int i = 0; i < sqlParams.Length; ++i)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(command.Parameters[i].ParameterName).Append("=");
                builder.Append(sqlParams[i].ParameterName);
            }

            return new QueryCommand(builder.ToString(), sqlParams, canSplitBatch);
        }

        internal static QueryCommand BuildTextQueryCommand(this QueryBatch batch, SqlCommand command, bool canSplitBatch)
        {
            Contract.Requires(command != null);

            if (command.CommandType != CommandType.Text)
            {
                throw new InvalidOperationException("This method only supports text command conversions.");
            }

            var sqlParams = command.Parameters.QualifyForBatch(batch);
            var builder = new StringBuilder(command.CommandText.Length + (2 * command.Parameters.Count));
            builder.Append("\r\n" + command.CommandText);

            int i = 0;
            foreach(SqlParameter p in command.Parameters)
            {
                builder.Replace(p.ParameterName, sqlParams[i].ParameterName);
                i++;
            }

            return new QueryCommand(builder.ToString(), sqlParams, canSplitBatch);
        }
    }
}