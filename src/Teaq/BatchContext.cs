using System.Diagnostics.Contracts;

namespace Teaq
{
    /// <summary>
    /// Concrete implementation of a ADO.NET persistence context.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "The interface is all that is visible to the library user.")]
    public class BatchContext : DataContextBase, IBatchContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchContext" /> class.
        /// </summary>
        /// <param name="connection">The connection string.</param>
        /// <param name="batch">The batch.</param>
        /// <param name="connectionBuilder">The connection builder. Enables injection for testability.</param>
        internal BatchContext(
            string connection, 
            QueryBatch batch,
            IConnectionBuilder connectionBuilder)
            : base(connection, connectionBuilder)
        {
            Contract.Requires(string.IsNullOrEmpty(connection) == false);
            Contract.Requires(batch != null);

            this.QueryBatch = batch;
        }

        /// <summary>
        /// Gets the target batch.
        /// </summary>
        public QueryBatch QueryBatch { get; private set; }
    }
}
