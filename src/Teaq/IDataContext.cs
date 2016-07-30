using System;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Testable surrogate interface used to abstract data access control. 
    /// This interface reduces the data context surface that must be mocked for unit testing.
    /// </summary>
    public interface IDataContext : IAsyncNonQueryProvider, IAsyncQueryProvider, INonQueryProvider, IQueryProvider, IDisposable
    {
        /// <summary>
        /// Gets the connection string to the underlying data store.
        /// </summary>
        string ConnectionString { get; }
    }
}
