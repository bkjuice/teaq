using System.Collections.Generic;
using System.Data;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Implementations of this interface provide the ability to synchronously query the target repository
    /// for a collection of results.
    /// </summary>
    public interface IQueryProvider
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
    }
}
