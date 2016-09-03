using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Implementation to interact with the underlying repository.
    /// </summary>
    public sealed partial class DataContext : IEntityQueryProvider
    {
        /// <summary>
        /// Executes an inline query that is optionally parameterized using an anonymous type.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The optional parameter property container.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        public List<TEntity> Query<TEntity>(string query, object parameterProps = null, IDataModel model = null) where TEntity : class
        {
            return this.Query(query, parameterProps.GetAnonymousParameters(), default(IDataHandler<TEntity>), model);
        }

        /// <summary>
        /// Executes an inline query that is optionally parameterized using an anonymous type.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="readerHandler">The data reader handler.</param>
        /// <param name="parameterProps">The optional parameter property container.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        public List<TEntity> Query<TEntity>(string query, IDataHandler<TEntity> readerHandler, object parameterProps = null) where TEntity : class
        {
            return this.Query(query, parameterProps.GetAnonymousParameters(), readerHandler, null);
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
        public List<TEntity> Query<TEntity>(QueryCommand<TEntity> command, IDataModel model = null) where TEntity : class
        {
            return this.Query(command.CommandText, command.GetParameters(), default(IDataHandler<TEntity>), model ?? command.Model);
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
        public List<TEntity> Query<TEntity>(QueryCommand<TEntity> command, IDataHandler<TEntity> readerHandler) where TEntity : class
        {
            return this.Query(command.CommandText, command.GetParameters(), readerHandler, null);
        }

        /// <summary>
        /// Queries the repository for a collection of entities that may differ from the type used to build the query.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to be returned.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// The collection of entities.
        /// </returns>
        public List<TEntity> QueryAs<TEntity>(QueryCommand command, IDataModel model = null) where TEntity : class
        {
            return this.Query<TEntity>(command.CommandText, command.GetParameters(), null, model);
        }

        /// <summary>
        /// Queries the repository for a collection of entities that may differ from the type used to build the query.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to be returned.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">The handler to read individual entities from the data reader.</param>
        /// <returns>
        /// The collection of entities.
        /// </returns>
        public List<TEntity> QueryAs<TEntity>(QueryCommand command, Func<IDataReader, TEntity> handler) where TEntity : class
        {
            return this.Query(command.CommandText, command.GetParameters(), new DelegatingReaderHandler<TEntity>(handler), null);
        }

        /// <summary>
        /// Executes an inline query that is optionally parameterized using an anonymous type.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The optional parameter property container.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, object parameterProps = null, IDataModel model = null) where TEntity : class
        {
            return await this.QueryAsync(query, parameterProps.GetAnonymousParameters(), default(IDataHandler<TEntity>), model);
        }

        /// <summary>
        /// Executes an inline query that is optionally parameterized using an anonymous type.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="handler">The data reader handler.</param>
        /// <param name="parameterProps">The optional parameter property container.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, IDataHandler<TEntity> handler, object parameterProps = null) where TEntity : class
        {
            return await this.QueryAsync(query, parameterProps.GetAnonymousParameters(), handler, null);
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
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand<TEntity> command, IDataModel model = null) where TEntity : class
        {
            return await this.QueryAsync(command.CommandText, command.GetParameters(), default(IDataHandler<TEntity>), model ?? command.Model);
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
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand<TEntity> command, IDataHandler<TEntity> readerHandler) where TEntity : class
        {
            return await this.QueryAsync(command.CommandText, command.GetParameters(), readerHandler, null);
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
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand command, Func<IDataReader, TEntity> handler) where TEntity : class
        {
            return await this.QueryAsync(command.CommandText, command.GetParameters(), new DelegatingReaderHandler<TEntity>(handler), null);
        }

        /// <summary>
        /// Queries the repository for a collection of entities that may differ from the type used to build the query.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to be returned.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// The collection of entities.
        /// </returns>
        public async Task<IEnumerable<TEntity>> QueryAsAsync<TEntity>(QueryCommand command, IDataModel model = null) where TEntity : class
        {
            return await this.QueryAsync<TEntity>(command.CommandText, command.GetParameters(), null, model);
        }

        /// <summary>
        /// Queries the repository for a collection of entities that may differ from the type used to build the query.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to be returned.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">The handler to read individual entities from the data reader.</param>
        /// <returns>
        /// The collection of entities.
        /// </returns>
        public async Task<IEnumerable<TEntity>> QueryAsAsync<TEntity>(QueryCommand command, Func<IDataReader, TEntity> handler) where TEntity : class
        {
            return await this.QueryAsync(command.CommandText, command.GetParameters(), new DelegatingReaderHandler<TEntity>(handler), null);
        }
    }
}