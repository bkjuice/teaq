using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Teaq
{
    /// <summary>
    /// Concrete implementation of a ADO.NET persistence context.
    /// </summary>
    internal sealed class BatchWriter : BatchContext, IBatchWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchContext" /> class.
        /// </summary>
        /// <param name="connection">The connection string.</param>
        /// <param name="batch">The batch.</param>
        /// <param name="connectionBuilder">The connection builder. Enables injection for testability.</param>
        internal BatchWriter(
            string connection, 
            QueryBatch batch,
            IConnectionBuilder connectionBuilder)
            : base(connection, batch, connectionBuilder)
        {
            Contract.Requires(string.IsNullOrEmpty(connection) == false);
        }

        /// <summary>
        /// Saves all pending changes to the backing store.
        /// </summary>
        public void SaveChanges()
        {
            var context = new DataContext(this.ConnectionString, this.ConnectionBuilder);
            while (this.QueryBatch.HasBatch)
            {
                var queryCommand = this.QueryBatch.NextBatch();
                context.ExecuteNonQuery(queryCommand);
            }
        }

        /// <summary>
        /// Saves all changes asynchronously in parallel, and returns a single awaitable handle.
        /// </summary>
        /// <returns>The awaitable task.</returns>
        public async Task SaveChangesAsync()
        {
            var waitHandles = new List<Task<int>>();
            var context = new DataContext(this.ConnectionString, this.ConnectionBuilder);
            while (this.QueryBatch.HasBatch)
            {
                var queryCommand = this.QueryBatch.NextBatch();
                waitHandles.Add(context.ExecuteNonQueryAsync(queryCommand));
            }

            await Task.WhenAll(waitHandles);
        }
    }
}
