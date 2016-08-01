using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq.Contracts
{
    /// <summary>
    /// Contract class for the <see cref="IEntityQueryProvider"/> interface.
    /// </summary>
    /// <seealso cref="Teaq.IEntityQueryProvider" />
    [ContractClassFor(typeof(IEntityQueryProvider))]
    public abstract class EntityQueryProviderContractClass : IEntityQueryProvider
    {
        /// <summary>
        /// Queries the repository for a collection of entities.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to be returned.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">The handler to read individual entities from the data reader.</param>
        /// <returns>
        /// The collection of entities.
        /// </returns>
        /// <exception cref="NotImplementedException">This is a contract only implementation.</exception>
        public List<TEntity> Query<TEntity>(QueryCommand command, Func<IDataReader, TEntity> handler)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes a query built using a fluent expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The command built from the data model.</param>
        /// <param name="readerHandler">The data reader handler.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        /// <exception cref="NotImplementedException">This is a contract only implementation.</exception>
        public List<TEntity> Query<TEntity>(QueryCommand<TEntity> command, IDataHandler<TEntity> readerHandler)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes a query built using a fluent expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The query built using a fluent expression.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        /// <exception cref="NotImplementedException">This is a contract only implementation.</exception>
        public List<TEntity> Query<TEntity>(QueryCommand<TEntity> command, IDataModel model = null)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the repository asynchronously for a collection of entities.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to be returned.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">The handler to read individual entities from the data reader.</param>
        /// <returns>
        /// The awaitable collection of entities.
        /// </returns>
        /// <exception cref="NotImplementedException">This is a contract only implementation.</exception>
        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand command, Func<IDataReader, TEntity> handler)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes a query built using a fluent expression to be materialized by a custom handler.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The query built using a fluent expression.</param>
        /// <param name="readerHandler">The data reader handler to use.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        /// <exception cref="NotImplementedException">This is a contract only implementation.</exception>
        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand<TEntity> command, IDataHandler<TEntity> readerHandler)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes a query built using a fluent expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The query built using a fluent expression.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        /// <exception cref="NotImplementedException">This is a contract only implementation.</exception>
        public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand<TEntity> command, IDataModel model = null)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }
    }
}
