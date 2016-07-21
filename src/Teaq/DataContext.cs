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
    /// Concrete implementation of a ADO.NET persistence context.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "The interface is all that is visible to the library user.")]
    public sealed class DataContext : DataContextBase, IDataContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        internal DataContext(string connectionString)
            : this(connectionString, new SqlConnectionBuilder())
        {
            Contract.Requires(string.IsNullOrEmpty(connectionString) == false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="connectionBuilder">The connection builder. Enables injection for testability.</param>
        internal DataContext(string connectionString, IConnectionBuilder connectionBuilder)
            : base(connectionString, connectionBuilder)
        {
            Contract.Requires(string.IsNullOrEmpty(connectionString) == false);
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
        public List<TEntity> Query<TEntity>(QueryCommand<TEntity> command, IDataModel model = null)
        {
            return this.Query(command.CommandText, CommandType.Text, default(IDataHandler<TEntity>), model ?? command.Model, command.GetParameters());
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
            return this.Query(command.CommandText, CommandType.Text, readerHandler, null, command.GetParameters());
        }

        /// <summary>
        /// Executes a query built using a fluent expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="command">The command built from the data model.</param>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        public List<TEntity> Query<TEntity>(QueryCommand command, IDataModel model = null)
        {
            return this.Query(command.CommandText, CommandType.Text, default(IDataHandler<TEntity>), model ?? command.Model, command.GetParameters());
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
        public List<TEntity> Query<TEntity>(QueryCommand command, IDataHandler<TEntity> readerHandler)
        {
            return this.Query(command.CommandText, CommandType.Text, readerHandler, null, command.GetParameters());
        }

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        public List<TEntity> Query<TEntity>(string commandText, params object[] parameters)
        {
            return this.Query(commandText, CommandType.Text, default(IDataHandler<TEntity>), null, parameters);
        }

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="model">The optional data model.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        public List<TEntity> Query<TEntity>(string commandText, IDataModel model = null, params object[] parameters)
        {
            return this.Query(commandText, CommandType.Text, default(IDataHandler<TEntity>), model, parameters);
        }

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
        public List<TEntity> Query<TEntity>(string commandText, CommandType commandKind, params object[] parameters)
        {
            return this.Query(commandText, commandKind, default(IDataHandler<TEntity>), null, parameters);
        }

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="model">The optional data model.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        public List<TEntity> Query<TEntity>(string commandText, CommandType commandKind, IDataModel model = null, params object[] parameters)
        {
            return this.Query(commandText, commandKind, default(IDataHandler<TEntity>), model, parameters);
        }

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
        public List<TEntity> Query<TEntity>(string commandText, IDataHandler<TEntity> readerHandler, params object[] parameters)
        {
            return this.Query(commandText, CommandType.Text, readerHandler, null, parameters);
        }

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
        public List<TEntity> Query<TEntity>(string commandText, CommandType commandKind, IDataHandler<TEntity> readerHandler, params object[] parameters)
        {
            return this.Query(commandText, commandKind, readerHandler, null, parameters);
        }

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The command built from a configured data model.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int ExecuteNonQuery(QueryCommand command)
        {
            return this.ExecuteNonQuery(command.CommandText, CommandType.Text, command.GetParameters());
        }

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command string.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int ExecuteNonQuery(string commandText, params object[] parameters)
        {
            return this.ExecuteNonQuery(commandText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command string.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "Not subject to user input and expected parameterization.")]
        public int ExecuteNonQuery(string commandText, CommandType commandKind, params object[] parameters)
        {
            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                return connection.ExecuteNonQuery(commandText, parameters, commandKind);
            }
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
            return await this.QueryAsync(command.CommandText, CommandType.Text, default(IDataHandler<TEntity>), model, command.GetParameters());

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
            return await this.QueryAsync(command.CommandText, CommandType.Text, readerHandler, null, command.GetParameters());

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
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand command, IDataModel model = null)
        {
            return await this.QueryAsync(command.CommandText, CommandType.Text, default(IDataHandler<TEntity>), model, command.GetParameters());
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
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(QueryCommand command, IDataHandler<TEntity> readerHandler)
        {
            return await this.QueryAsync(command.CommandText, CommandType.Text, readerHandler, null, command.GetParameters());
        }

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, params object[] parameters)
        {
            return await this.QueryAsync(commandText, CommandType.Text, default(IDataHandler<TEntity>), null, parameters);
        }

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="model">The optional data model.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, IDataModel model = null, params object[] parameters)
        {
            return await this.QueryAsync(commandText, CommandType.Text, default(IDataHandler<TEntity>), model, parameters);
        }

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
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, CommandType commandKind, params object[] parameters)
        {
            return await this.QueryAsync(commandText, commandKind, default(IDataHandler<TEntity>), null, parameters);
        }

        /// <summary>
        /// Executes a query directly against the backing store.
        /// </summary>
        /// <typeparam name="TEntity">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="commandText">The sql statement to execute.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="model">The optional data model.</param>
        /// <param name="parameters">Parameters to pass to the Sql statement. This cannot be null but may be empty.</param>
        /// <returns>
        /// An awaitable, enumerable collection of results of the specified type.
        /// </returns>
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, CommandType commandKind, IDataModel model = null, params object[] parameters)
        {
            return await this.QueryAsync(commandText, commandKind, default(IDataHandler<TEntity>), model, parameters);
        }

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
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, IDataHandler<TEntity> readerHandler, params object[] parameters)
        {
            return await this.QueryAsync(commandText, CommandType.Text, readerHandler, null, parameters);
        }

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
        public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, CommandType commandKind, IDataHandler<TEntity> readerHandler, params object[] parameters)
        {
            return await this.QueryAsync(commandText, commandKind, readerHandler, null, parameters);
        }

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The command built from a configured data model.</param>
        /// <returns>
        /// An awaitable result with the number of rows affected.
        /// </returns>
        public async Task<int> ExecuteNonQueryAsync(QueryCommand command)
        {
            return await this.ExecuteNonQueryAsync(command.CommandText, CommandType.Text, command.GetParameters());
        }

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command string.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>
        /// An awaitable result with the number of rows affected.
        /// </returns>
        public async Task<int> ExecuteNonQueryAsync(string commandText, params object[] parameters)
        {
            return await this.ExecuteNonQueryAsync(commandText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command string.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>
        /// An awaitable result with the number of rows affected.
        /// </returns>
        public async Task<int> ExecuteNonQueryAsync(string commandText, CommandType commandKind, params object[] parameters)
        {
            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                return await connection.ExecuteNonQueryAsync(commandText, parameters, commandKind);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA2100:Review SQL queries for security vulnerabilities",
            Justification = "Not subject to user input and expected parameterization.")]
        private List<TEntity> Query<TEntity>(string commandText, CommandType commandKind, IDataHandler<TEntity> readerHandler, IDataModel model, params object[] parameters)
        {
            Contract.Requires(string.IsNullOrEmpty(commandText) == false);

            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                return connection.ReadEntities(commandText, parameters, commandKind, readerHandler, model, 64, NullPolicyKind.IncludeAsDefaultValue);
            }
        }

        private async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, CommandType commandKind, IDataHandler<TEntity> readerHandler, IDataModel model, params object[] parameters)
        {
            Contract.Requires(string.IsNullOrEmpty(commandText) == false);

            return await this.ConnectionBuilder.Create(this.ConnectionString)
                .EnumerateEntitiesAsync(commandText, parameters, commandKind, readerHandler, model, NullPolicyKind.IncludeAsDefaultValue);
        }
    }
}
