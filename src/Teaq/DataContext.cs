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
            return await this.QueryAsync(command.CommandText, new DelegatingReaderHandler<TEntity>(handler), null, command.GetParameters());
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
            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                var dbCommand = connection.BuildTextCommand(command.CommandText, command.GetParameters());
                dbCommand.Open();
                return dbCommand.ExecuteNonQuery();
            }
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
            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                var dbCommand = connection.BuildTextCommand(command.CommandText, command.GetParameters());
                dbCommand.Open();
                return await dbCommand.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Executes the command and returns the first value in the first row, if one exists.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first value in the first row returned by the query.</returns>
        public TValue? ExecuteScalar<TValue>(QueryCommand command) where TValue : struct
        {
            return Convert<TValue>(this.ExecuteScalar(command.CommandText, command.GetParameters()));
        }

        /// <summary>
        /// Executes the command and returns the first value in the first row as a string, if one exists.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        public string ExecuteScalar(QueryCommand command)
        {
            return Convert(this.ExecuteScalar(command.CommandText, command.GetParameters()));
        }

        /// <summary>
        /// Executes the command asynchronously and returns the first value in the first row, if one exists.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first value in the first row returned by the query.</returns>
        public async Task<TValue?> ExecuteScalarAsync<TValue>(QueryCommand command) where TValue : struct
        {
            return Convert<TValue>(await this.ExecuteScalarAsync(command.CommandText, command.GetParameters()));
        }

        /// <summary>
        /// Executes the command asynchronously and returns the first value in the first row as a string, if one exists.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        public async Task<string> ExecuteScalarAsync(QueryCommand command)
        {
            return Convert(await this.ExecuteScalarAsync(command.CommandText, command.GetParameters()));
        }

        private static bool ResultIsNull(object result)
        {
            // TODO: Test a null result with SQL Server (leaky abs):
            if (result == null || ReferenceEquals(DBNull.Value, result))
            {
                return true;
            }

            return false;
        }

        private static string Convert(object result)
        {
            if (ResultIsNull(result))
            {
                return string.Empty;
            }

            return result.ToString();
        }

        private static TValue? Convert<TValue>(object result) where TValue: struct
        {
            if (ResultIsNull(result))
            {
                return new TValue?();
            }

            var expected = typeof(TValue);
            var converter = DataReaderExtensions.GetConverter(result.GetType().TypeHandle, expected.TypeHandle, expected);
            return (TValue)converter(result);
        }

        private object ExecuteScalar(string commandText, IDbDataParameter[] parameters)
        {
            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                return connection
                    .BuildTextCommand(commandText, parameters)
                    .Open()
                    .ExecuteScalar();
            }
        }

        private async Task<object> ExecuteScalarAsync(string commandText, IDbDataParameter[] parameters)
        {
            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                return await connection
                    .BuildTextCommand(commandText, parameters)
                    .Open()
                    .ExecuteScalarAsync();
            }
        }

        private List<TEntity> Query<TEntity>(string commandText, IDataHandler<TEntity> readerHandler, IDataModel model, params IDbDataParameter[] parameters)
        {
            Contract.Requires(string.IsNullOrEmpty(commandText) == false);

            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                return connection
                        .BuildTextCommand(commandText, parameters)
                        .ReadEntities(readerHandler, model, NullPolicyKind.IncludeAsDefaultValue, 64);
            }
        }

        private async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, IDataHandler<TEntity> readerHandler, IDataModel model, params IDbDataParameter[] parameters)
        {
            Contract.Requires(string.IsNullOrEmpty(commandText) == false);

            return await this.ConnectionBuilder.Create(this.ConnectionString)
                .BuildTextCommand(commandText, parameters)
                .EnumerateEntitiesAsync(readerHandler, model, NullPolicyKind.IncludeAsDefaultValue);
        }
    }
}
