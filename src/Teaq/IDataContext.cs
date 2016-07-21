using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Testable surrogate interface used to abstract data access control. 
    /// This interface reduces the data context surface that must be mocked for unit testing.
    /// </summary>
    public interface IDataContext : IDisposable
    {
        /// <summary>
        /// Gets the connection string to the underlying data store.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Executes a query built using a fluent expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The query built using a fluent expression.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        List<TEntity> Query<TEntity>(QueryCommand<TEntity> command, IDataModel model = null);

        /// <summary>
        /// Executes a query built using a fluent expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The command built from the data model.</param>
        /// <param name="readerHandler">The data reader handler.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        List<TEntity> Query<TEntity>(QueryCommand<TEntity> command, IDataHandler<TEntity> readerHandler);

        /// <summary>
        /// Executes a query built using a fluent expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The command built from the data model.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        List<TEntity> Query<TEntity>(QueryCommand command, IDataModel model = null);

        /// <summary>
        /// Executes a query built using a fluent expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The command built from the data model.</param>
        /// <param name="readerHandler">The data reader handler.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        List<TEntity> Query<TEntity>(QueryCommand command, IDataHandler<TEntity> readerHandler);

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        List<TEntity> Query<TEntity>(string commandText, params object[] parameters);

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="model">The optional data model.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>An enumerable collection of results of the specified type.</returns>
        List<TEntity> Query<TEntity>(string commandText, IDataModel model = null, params object[] parameters);

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        List<TEntity> Query<TEntity>(string commandText, CommandType commandKind, params object[] parameters);

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="model">The optional data model.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>An enumerable collection of results of the specified type.</returns>
        List<TEntity> Query<TEntity>(string commandText, CommandType commandKind, IDataModel model = null, params object[] parameters);

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="readerHandler">The data reader handler.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        List<TEntity> Query<TEntity>(string commandText, IDataHandler<TEntity> readerHandler, params object[] parameters);

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="readerHandler">The data reader handler.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        List<TEntity> Query<TEntity>(string commandText, CommandType commandKind, IDataHandler<TEntity> readerHandler, params object[] parameters);

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The command built from a configured data model.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        int ExecuteNonQuery(QueryCommand command);

        /// <summary>
        /// Executes the given command and returns the number of rows affected. 
        /// </summary>
        /// <param name="commandText">The command string.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The number of rows affected.</returns>
        int ExecuteNonQuery(string commandText, params object[] parameters);

        /// <summary>
        /// Executes the given command and returns the number of rows affected. 
        /// </summary>
        /// <param name="commandText">The command string.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The number of rows affected.</returns>
        int ExecuteNonQuery(string commandText, CommandType commandKind, params object[] parameters);

        /// <summary>
        /// Executes a query built using a fluent expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The query built using a fluent expression.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand<TEntity> command, IDataModel model = null);

        /// <summary>
        /// Executes a query built using a fluent expression to be materialized by a custom handler.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The query built using a fluent expression.</param>
        /// <param name="readerHandler">The data reader handler to use.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand<TEntity> command, IDataHandler<TEntity> readerHandler);

        /// <summary>
        /// Executes a query built using a fluent expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The query built using a fluent expression.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand command, IDataModel model = null);

        /// <summary>
        /// Executes a query built using a fluent expression to be materialized by a custom handler.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The query built using a fluent expression.</param>
        /// <param name="readerHandler">The data reader handler to use.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand command, IDataHandler<TEntity> readerHandler);

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, params object[] parameters);

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="model">The optional data model.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>An awaitable, enumerable collection of results of the specified type.</returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, IDataModel model = null, params object[] parameters);

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, CommandType commandKind, params object[] parameters);

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="model">The optional data model.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>An awaitable, enumerable collection of results of the specified type.</returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, CommandType commandKind, IDataModel model = null, params object[] parameters);

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="readerHandler">The data reader handler.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, IDataHandler<TEntity> readerHandler, params object[] parameters);
       
        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="readerHandler">The data reader handler.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, CommandType commandKind, IDataHandler<TEntity> readerHandler, params object[] parameters);

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The command built from a configured data model.</param>
        /// <returns>
        /// An awaitable result with the number of rows affected.
        /// </returns>
        Task<int> ExecuteNonQueryAsync(QueryCommand command);

        /// <summary>
        /// Executes the given command and returns the number of rows affected. 
        /// </summary>
        /// <param name="commandText">The command string.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>An awaitable result with the number of rows affected.</returns>
        Task<int> ExecuteNonQueryAsync(string commandText, params object[] parameters);

        /// <summary>
        /// Executes the given command and returns the number of rows affected. 
        /// </summary>
        /// <param name="commandText">The command string.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>An awaitable result with the number of rows affected.</returns>
        Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandKind, params object[] parameters);
    }
}
