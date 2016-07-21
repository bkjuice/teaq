using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Teaq.Configuration;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Builder for the delete statement.
    /// </summary>
    /// <typeparam name="T">The type of target entity.</typeparam>
    /// <seealso cref="Teaq.QueryGeneration.QueryStatementBuilder{T}" />
    internal class DeleteBuilder<T> : QueryStatementBuilder<T> 
    {
        /// <summary>
        /// The where clause.
        /// </summary>
        private Expression<Func<T, bool>> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteBuilder{T}" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="filter">The filter.</param>
        public DeleteBuilder(IDataModel model, Expression<Func<T, bool>> filter)
            : base(model)
        {
            this.filter = filter;
        }

        /// <summary>
        /// Extracts the delete command as an SQL statement with parameters if any.
        /// </summary>
        /// <param name="parameters">The delete parameters.</param>
        /// <param name="batch">The batch.</param>
        /// <returns>
        /// The SQL statement.
        /// </returns>
        protected override string BuildStatement(out SqlParameter[] parameters, QueryBatch batch = null)
        {
            string filterClause;
            parameters = this.GetFilterParametersAndClause(this.filter, batch, out filterClause, null);

            return "\r\ndelete from " + this.GetTableName() + "\r\n"
                + (string.IsNullOrEmpty(filterClause) ? string.Empty : "where " + filterClause + "\r\n");
        }
    }
}
