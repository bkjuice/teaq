using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Helper class to allow for registration of command builders and data binding methods.
    /// </summary>
    public class BatchProcessor
    {
        /// <summary>
        /// The default connection builder instance.
        /// </summary>
        private static SqlConnectionBuilder defaultBuilder = new SqlConnectionBuilder();

        /// <summary>
        /// The handlers for the batch.
        /// </summary>
        private readonly List<Func<IDataReader, bool>> handlers = new List<Func<IDataReader, bool>>();

        /// <summary>
        /// The batch.
        /// </summary>
        private readonly QueryBatch batch = new QueryBatch();

        /// <summary>
        /// Gets the underlying query batch being built.
        /// </summary>
        public QueryBatch QueryBatch
        {
            get
            {
                Contract.Ensures(Contract.Result<QueryBatch>() != null);

                return this.batch;
            }
        }

        /// <summary>
        /// Adds the command and the corresponding result handler callback.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler that will read the result.</param>
        /// <remarks>
        /// The handler implementation should return true unless the consumption scenario 
        /// requires the batch processing to be short circuited.
        /// </remarks>
        public void AddCommand(QueryCommand command, Func<IDataReader, bool> handler)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            this.batch.Add(command);
            this.handlers.Add(handler);
        }

        /// <summary>
        /// Adds the command and the corresponding result handler callback.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="handler">The handler that will read the result.</param>
        /// <remarks>
        /// The handler implementation should return true unless the consumption scenario 
        /// requires the batch processing to be short circuited.
        /// </remarks>
        public void AddCommand(SqlCommand command, Func<IDataReader, bool> handler)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            Contract.Requires<ArgumentNullException>(handler != null);

            this.AddToBatch(command);
            this.handlers.Add(handler);
        }

        /// <summary>
        /// Executes the batch asynchronously.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="connectionBuilder">The connection builder.</param>
        /// <returns>
        /// An awaitable task with the resulting set of items that were not included in the query batch based on page state.
        /// </returns>
        public async Task ExecuteBatchAsync(string connectionString, IConnectionBuilder connectionBuilder = null)
        {
            // TODO: Make this proper async via ExecuteReaderAsync...
            Action awaitable = () => ExecuteBatch(connectionString, connectionBuilder);
            await awaitable.InvokeAsAwaitable();
        }

        /// <summary>
        /// Executes the batch.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="connectionBuilder">The connection builder.</param>
        public void ExecuteBatch(string connectionString, IConnectionBuilder connectionBuilder = null)
        {
            Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(connectionString) == false);

            connectionBuilder = connectionBuilder ?? defaultBuilder;
            using (var context = new BatchReader(connectionString, this.batch, connectionBuilder))
            {
                for (int i = 0; i < this.handlers.Count; ++i)
                {
                    if (context.NextResult())
                    {
                        if (!this.handlers[i](context.CurrentReader))
                        {
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds to batch.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="System.NotSupportedException">Table direct commands are not supported.</exception>
        private void AddToBatch(SqlCommand command)
        {
            Contract.Requires(command != null);

            if (command.CommandType == CommandType.StoredProcedure)
            {
                this.batch.AddStoredProcedure(command);
            }
            else
            {
                this.batch.AddTextCommand(command);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.batch != null);
            Contract.Invariant(this.handlers != null);
        }
    }
}