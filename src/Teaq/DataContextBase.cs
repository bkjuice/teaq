using System;
using System.Diagnostics.Contracts;

namespace Teaq
{
    /// <summary>
    /// Base implementation of an ADO.NET persistence context.
    /// </summary>
    public abstract class DataContextBase : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextBase" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        internal DataContextBase(string connectionString)
            : this(connectionString, new SqlConnectionBuilder())
        {
            Contract.Requires(string.IsNullOrEmpty(connectionString) == false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextBase" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="connectionBuilder">The connection builder. Enables injection for testability.</param>
        internal DataContextBase(string connectionString, IConnectionBuilder connectionBuilder)
        {
            Contract.Requires(string.IsNullOrEmpty(connectionString) == false);
            Contract.Requires(connectionBuilder != null);
            Contract.Ensures(string.IsNullOrEmpty(this.ConnectionString) == false);
            Contract.Ensures(this.ConnectionBuilder != null);

            this.ConnectionString = connectionString;
            this.ConnectionBuilder = connectionBuilder;
        }

        /// <summary>
        /// Gets a connection string to the underlying data store.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the connection builder.
        /// </summary>
        protected IConnectionBuilder ConnectionBuilder { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DataContextBase"/> is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        protected bool Disposed { get; private set; }

        /// <summary>
        /// Frees all resources registered or acquired during query execution.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
            this.Disposed = true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
