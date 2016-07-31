using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Teaq.Configuration;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Concrete implementation of a Teaq data context that supports iterative reads over multiple result sets.
    /// </summary>
    internal sealed class BatchReader : BatchContext, IBatchReader
    {
        /// <summary>
        /// The current reader when iterating a result batch.
        /// </summary>
        private IDataReader currentReader;

        /// <summary>
        /// The current command when iterating a result batch.
        /// </summary>
        private IDbCommand currentCommand;

        /// <summary>
        /// The current connection when iterating a result batch.
        /// </summary>
        private IDbConnection currentConnection;

        /// <summary>
        /// Indicates this instance has a pending enumerable.
        /// </summary>
        private bool hasPendingEnumerable;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchReader"/> class.
        /// </summary>
        /// <param name="connection">The connection string.</param>
        /// <param name="batch">The batch.</param>
        internal BatchReader(
            string connection, 
            QueryBatch batch)
            : this(connection, batch, new SqlConnectionBuilder())
        {
            Contract.Requires(string.IsNullOrEmpty(connection) == false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchReader" /> class.
        /// </summary>
        /// <param name="connection">The connection string.</param>
        /// <param name="batch">The batch.</param>
        /// <param name="connectionBuilder">The connection builder. Enables injection for testability.</param>
        internal BatchReader(
            string connection,
            QueryBatch batch,
            IConnectionBuilder connectionBuilder)
            : base(connection, batch, connectionBuilder)
        {
            Contract.Requires(string.IsNullOrEmpty(connection) == false);
        }

        /// <summary>
        /// Gets the current reader. Use this for direct access to result sets in cases where ORM is not helpful.
        /// </summary>
        public IDataReader CurrentReader
        {
            get
            {
                return this.currentReader;
            }
        }

        /// <summary>
        /// Gets the next data reader result, if available.
        /// </summary>
        /// <returns>
        /// True if there is a next result set or next result set from a next batch of queries to run, false otherwise if all results are iterated.
        /// </returns>
        public bool NextResult()
        {
            if (this.MoveNext())
            {
                return true;
            }

            return this.TryReadIteration();
        }

        /// <summary>
        /// Gets the next data reader result, if available.
        /// </summary>
        /// <returns>
        /// True if there is a next result set or next result set from a next batch of queries to run, false otherwise if all results are iterated.
        /// </returns>
        public async Task<bool> NextResultAsync()
        {
            if (this.MoveNext())
            {
                return true;
            }

            return await this.TryReadIterationAsync();
        }

        /// <summary>
        /// Provides access to the underlying data store for query composition.
        /// </summary>
        /// <typeparam name="TEntity">The concrete entity type.</typeparam>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}" /> collection of entities.
        /// </returns>
        public List<TEntity> ReadEntitySet<TEntity>(IDataModel model = null)
        {
            return this.ReadEntitySet<TEntity>(model, null);
        }

        /// <summary>
        /// Provides access to the underlying data store for query composition.
        /// </summary>
        /// <typeparam name="TEntity">The concrete entity type.</typeparam>
        /// <param name="handler">The explicit data handler.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}" /> collection of entities.
        /// </returns>
        public List<TEntity> ReadEntitySet<TEntity>(IDataHandler<TEntity> handler)
        {
            return this.ReadEntitySet<TEntity>(null, handler);
        }

        /// <summary>
        /// Enumerates the entities in the batch for the current row set.
        /// </summary>
        /// <typeparam name="TEntity">The concrete entity type.</typeparam>
        /// An <see cref="IEnumerable{T}" /> collection of entities.
        public IEnumerable<TEntity> EnumerateEntitySet<TEntity>(IDataHandler<TEntity> handler)
        {
            return this.EnumerateEntitySet(null, handler);
        }

        /// <summary>
        /// Enumerates the entities in the batch for the current row set.
        /// </summary>
        /// <typeparam name="TEntity">The concrete entity type.</typeparam>
        /// <param name="model">The optional data model.</param>
        /// <returns></returns>
        /// An <see cref="IEnumerable{T}" /> collection of entities.
        public IEnumerable<TEntity> EnumerateEntitySet<TEntity>(IDataModel model = null)
        {
            return this.EnumerateEntitySet<TEntity>(model, null);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            if (this.currentCommand != null)
            {
                this.currentCommand.Dispose();
                this.currentCommand = null;
            }

            if (this.currentReader != null)
            {
                if (!this.currentReader.IsClosed)
                {
                    this.currentReader.Close();
                }

                this.currentReader = null;
            }

            if (this.currentConnection != null)
            {
                if (this.currentConnection.State != ConnectionState.Closed)
                {
                    this.currentConnection.Close();
                }

                this.currentConnection = null;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
            }
        }

        private bool MoveNext()
        {
            this.CheckPendingEnumerable("read the next result");
            this.QueryBatch.MoveToNextResultType();
            if (this.currentReader != null)
            {
                if (this.currentReader.NextResult())
                {
                    return true;
                }

                this.Close();
            }

            return false;
        }

        private List<TEntity> ReadEntitySet<TEntity>(IDataModel model, IDataHandler<TEntity> handler)
        {
            this.CheckPendingEnumerable("read the current result as a list");
            return this.currentReader.ReadEntities(handler, model, 64, NullPolicyKind.IncludeAsDefaultValue);
        }

        private IEnumerable<TEntity> EnumerateEntitySet<TEntity>(IDataModel model, IDataHandler<TEntity> handler)
        {
            this.SetForEnumeration();
            return this.currentReader?.EnumerateEntities(handler, model, NullPolicyKind.IncludeAsDefaultValue, this.HandleCompletedEnumeration) ?? new List<TEntity>();
        }

        private void SetForEnumeration()
        {
            this.CheckPendingEnumerable("read the next result (again)");
            this.hasPendingEnumerable = true;
        }

        private void HandleCompletedEnumeration()
        {
            this.hasPendingEnumerable = false;
        }

        private bool TryReadIteration()
        {
            if (this.NextCommand())
            {
                this.currentReader = this.currentCommand.ExecuteReader();
            }

            return this.currentReader != null;
        }

        private async Task<bool> TryReadIterationAsync()
        {
            if (this.NextCommand())
            {
                this.currentReader = await this.currentCommand.ExecuteReaderAsync();
            }

            return this.currentReader != null;
        }

        private bool NextCommand()
        {
            Contract.Ensures(this.currentConnection != null);
            Contract.Ensures(this.currentCommand != null);

            if (!this.QueryBatch.HasBatch)
            {
                return false;
            }

            var query = this.QueryBatch.NextBatch();
            if (this.currentConnection == null)
            {
                this.currentConnection = this.ConnectionBuilder.Create(this.ConnectionString);
                try
                {
                    this.currentConnection.Open();
                }
                catch(NullReferenceException e)
                {
                    throw new InvalidOperationException($"The provided connection builder of type { this.ConnectionBuilder.GetType().FullName } returned a null connection object. Connection builder instances must return a valid IDBConnectionInstance.", e);
                }
            }

            this.currentCommand = this.currentConnection.BuildTextCommand(query.CommandText, query.GetParameters());
            return true;
        }

        private void CheckPendingEnumerable(string context)
        {
            if (this.hasPendingEnumerable)
            {
                throw new InvalidOperationException(
                    "You cannot {0} when there is a pending enumeration of entities. Execute the enumeration first before attempting any other operation. Note, this usage is not threadsafe."
                    .ToFormat(context));
            }
        }
    }
}
