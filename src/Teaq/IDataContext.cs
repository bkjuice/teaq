using System;
using System.Diagnostics.Contracts;

namespace Teaq
{
    /// <summary>
    /// Interface that composes all repository query capabilities offered by Teaq.
    /// </summary>
    ////[ContractClass(typeof(Contracts.DataContextContractClass))]
    public interface IDataContext : 
        INonQueryProvider, 
        IEntityQueryProvider, 
        IDisposable
    {
        /// <summary>
        /// Gets the connection string to the underlying data store.
        /// </summary>
        string ConnectionString { get; }
    }
}
