using System.Data.SqlClient;
using System.Text;
using Teaq.Configuration;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Builder for the insert statement.
    /// </summary>
    /// <typeparam name="T">The type of target entity.</typeparam>
    /// <seealso cref="Teaq.QueryGeneration.QueryStatementBuilder{T}" />
    internal class InsertBuilder<T> : QueryStatementBuilder<T> 
    {
        /// <summary>
        /// The target
        /// </summary>
        private T target;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertBuilder{T}" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="target">The target.</param>
        public InsertBuilder(IDataModel model, T target)
            : base(model)
        {
            this.target = target;
        }

        /// <summary>
        /// Extracts the command text as an SQL statement.
        /// </summary>
        /// <param name="insertParameters">The insert parameters.</param>
        /// <param name="batch">The batch.</param>
        /// <returns>
        /// The SQL statement.
        /// </returns>
        protected override string BuildStatement(out SqlParameter[] insertParameters, QueryBatch batch = null)
        {
            int? batchIndex = null;
            if (batch != null)
            {
                batchIndex = batch.NextBatchIndex();
            }

            insertParameters = this.GetColumnParameters(this.target, QueryType.Insert, batchIndex);
            var columnList = new StringBuilder(insertParameters.Length * 32);
            var tableValues = new StringBuilder(insertParameters.Length * 32);
            for (int i = 0; i < insertParameters.GetLength(0); i++)
            {
                columnList.Append(insertParameters[i].SourceColumn).Append(", ");
                tableValues.Append(insertParameters[i].ParameterName).Append(", ");
            }

            return "\r\ninsert " + this.GetTableName() + "\r\n(\r\n"
                 + columnList.ToString(0, columnList.Length - 2) + ")\r\n"
                 + "values(\r\n" + tableValues.ToString(0, tableValues.Length - 2) + ")\r\n"
                 + this.GetScopeIdentity();
        }

        /// <summary>
        /// Gets the scope identity, if applicable.
        /// </summary>
        /// <returns>The scope identity function as a statement or empty if not applicable.</returns>
        private string GetScopeIdentity()
        {
            if (this.Config?.HasIdentity == true)
            {
                return  "\r\nselect SCOPE_IDENTITY()\r\n";
            }

            return string.Empty;
        }
    }
}
