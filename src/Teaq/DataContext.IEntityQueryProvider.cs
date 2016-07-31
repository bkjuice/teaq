using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Concrete implementation of a ADO.NET persistence context.
    /// </summary>
    public sealed partial class DataContext
    {
        /// <summary>
        /// Executes a query built using a fluent expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The query built using a fluent expression.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        public List<TEntity> Query<TEntity>(QueryCommand<TEntity> command, IDataModel model = null)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            return this.Query(command.CommandText, default(IDataHandler<TEntity>), model ?? command.Model, command.GetParameters());
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
        public List<TEntity> Query<TEntity>(QueryCommand<TEntity> command, IDataHandler<TEntity> readerHandler)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            return this.Query(command.CommandText, readerHandler, null, command.GetParameters());
        }

        /// <summary>
        /// Queries the repository for a collection of entities.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to be returned.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">The handler to read individual entities from the data reader.</param>
        /// <returns>
        /// The collection of entities.
        /// </returns>
        public List<TEntity> Query<TEntity>(QueryCommand command, Func<IDataReader, TEntity> handler)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            return this.Query(command.CommandText, new DelegatingReaderHandler<TEntity>(handler), null, command.GetParameters());
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
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand<TEntity> command, IDataModel model = null)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            return await this.QueryAsync(command.CommandText, default(IDataHandler<TEntity>), model ?? command.Model, command.GetParameters());
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
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand<TEntity> command, IDataHandler<TEntity> readerHandler)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            return await this.QueryAsync(command.CommandText, readerHandler, null, command.GetParameters());
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
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand command, Func<IDataReader, TEntity> handler)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            return await this.QueryAsync(command.CommandText, new DelegatingReaderHandler<TEntity>(handler), null, command.GetParameters());
        }
    }
}
