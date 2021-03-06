﻿using System.Data;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Fluent interface to complete the query build process.
    /// </summary>
    public interface IQueryCompletion<T>
    {
        /// <summary>
        /// Adds an options clause to the working query.
        /// </summary>
        /// <param name="optionClause">The literal OPTION clause to append to the select statement.</param>
        /// <returns>
        /// The fluent interface to complete query specification.
        /// </returns>
        /// <remarks>
        /// The provided literal clause will be normalized to start with "OPTION ".
        /// </remarks>
        IQueryCompletion<T> WithOption(string optionClause);

        /// <summary>
        /// Builds the result of the fluent query method chain.
        /// </summary>
        /// <returns>
        /// The query command.
        /// </returns>
        QueryCommand<T> ToCommand();

        /// <summary>
        /// Adds the query to the batch.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="canSplitBatch">if set to <c>true</c> the query [can be split into a separate batch if needed].</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the underlying batch is null.</exception>
        QueryCommand<T> AddToBatch(QueryBatch batch, bool canSplitBatch = true);
    }
}
