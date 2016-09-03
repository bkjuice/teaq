using System;

namespace Teaq
{
    /// <summary>
    /// Interface that composes all repository query capabilities offered by Teaq.
    /// </summary>
    public interface IDataContext : 
        IScalarQueryProvider, 
        INonQueryProvider,
        IEntityQueryProvider, 
        IValueQueryProvider,
        IDisposable
    {
        /// <summary>
        /// Gets the connection string to the underlying data store.
        /// </summary>
        string ConnectionString { get; }
    }
}
