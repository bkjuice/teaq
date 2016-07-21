using System.Data.SqlClient;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Typed instance of the query command.
    /// </summary>
    /// <typeparam name="T">The entity type to be materialized.</typeparam>
    /// <seealso cref="Teaq.QueryGeneration.QueryCommand" />
    public class QueryCommand<T> : QueryCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommand" /> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="canSplitBatch">if set to <c>true</c> [can split batch].</param>
        public QueryCommand(string commandText, bool canSplitBatch = true) : base(commandText, null, canSplitBatch)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommand" /> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="canSplitBatch">if set to <c>true</c> [can split batch].</param>
        public QueryCommand(string commandText, SqlParameter[] parameters, bool canSplitBatch = true)
            : base(commandText, parameters, canSplitBatch)
        {
        }
    }
}
