using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Text;
using Teaq.Configuration;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Builder for the update statement.
    /// </summary>
    /// <typeparam name="T">The type of target entity.</typeparam>
    /// <seealso cref="Teaq.QueryGeneration.QueryStatementBuilder{T}" />
    internal class UpdateBuilder<T> : QueryStatementBuilder<T> 
    {
        /// <summary>
        /// The target instance, in the case of update or delete.
        /// </summary>
        private T target;

        /// <summary>
        /// The where clause.
        /// </summary>
        private Expression<Func<T, bool>> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateBuilder{T}"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="target">The target.</param>
        /// <param name="filter">The filter.</param>
        public UpdateBuilder(IDataModel model, T target, Expression<Func<T, bool>> filter)
            : base(model)
        {
            this.target = target;
            this.filter = filter;
        }

        /// <summary>
        /// Builds an update query.
        /// </summary>
        /// <param name="parameters">The update parameters.</param>
        /// <param name="batch">The batch.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        protected override string BuildStatement(out SqlParameter[] parameters, QueryBatch batch = null)
        {
            int? batchIndex = null;
            if (batch != null)
            {
                batchIndex = batch.NextBatchIndex();
            }

            parameters = this.GetColumnParameters(this.target, QueryType.Update, batchIndex);
            Contract.Assert(parameters != null);

            var setStatement = new StringBuilder(parameters.Length * 32);
            for (int i = 0; i < parameters.GetLength(0); i++)
            {
                setStatement
                    .AppendIdentifier(parameters[i].SourceColumn)
                    .Append(" = ")
                    .Append(parameters[i].ParameterName)
                    .Append(", ");
            }

            string filterClause;
            var filterParameters = this.GetFilterParametersAndClause(this.filter, batch, out filterClause, null);
            parameters = parameters.Combine(filterParameters);

            return "\r\nupdate " + this.GetTableName() + "\r\nset\r\n"
                + setStatement.ToString(0, setStatement.Length - 2) + "\r\n"
                + (string.IsNullOrEmpty(filterClause) ? string.Empty : "where " + filterClause + "\r\n");
        }
    }
}
