using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Teaq.Configuration;

namespace Teaq
{
    /// <summary>
    /// Concrete implementation of a ADO.NET persistence context.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "The interface is all that is visible to the library user.")]
    public sealed partial class DataContext : DataContextBase, IDataContext
    {
        private const int DefaultBufferSize = 64;

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

        private List<string> QueryStringValues(string commandText, IDbDataParameter[] parameters, Func<IDataReader, string> handler)
        {
            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                return connection
                        .BuildTextCommand(commandText, parameters)
                        .Open()
                        .ExecuteReader()
                        .EnumerateStringValuesInternal(handler, null)
                        .ToList(DefaultBufferSize);
            }
        }

        private async Task<IEnumerable<string>> QueryStringValuesAsync(string commandText, IDbDataParameter[] parameters, Func<IDataReader, string> handler)
        {
            var command = this.ConnectionBuilder.Create(this.ConnectionString).BuildTextCommand(commandText, parameters);
            return 
                (await command.Open().ExecuteReaderAsync())
                .EnumerateStringValuesInternal(handler, command.CleanupCallback());
        }

        private List<T?> QueryNullableValues<T>(string commandText, IDbDataParameter[] parameters, Func<IDataReader, T?> handler) where T: struct
        {
            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                return connection
                        .BuildTextCommand(commandText, parameters)
                        .Open()
                        .ExecuteReader()
                        .EnumerateNullableValuesInternal(handler, null)
                        .ToList(DefaultBufferSize);
            }
        }

        private async Task<IEnumerable<T?>> QueryNullableValuesAsync<T>(string commandText, IDbDataParameter[] parameters, Func<IDataReader, T?> handler) where T : struct
        {
            var command = this.ConnectionBuilder.Create(this.ConnectionString).BuildTextCommand(commandText, parameters);
            return 
                (await command.Open().ExecuteReaderAsync())
                .EnumerateNullableValuesInternal(handler, command.CleanupCallback());
        }

        private List<T> QueryValues<T>(string commandText, IDbDataParameter[] parameters, Func<IDataReader, T?> handler) where T : struct
        {
            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                return connection
                        .BuildTextCommand(commandText, parameters)
                        .Open()
                        .ExecuteReader()
                        .EnumerateValuesInternal(handler, null)
                        .ToList(DefaultBufferSize);
            }
        }

        private async Task<IEnumerable<T>> QueryValuesAsync<T>(string commandText, IDbDataParameter[] parameters, Func<IDataReader, T?> handler) where T : struct
        {
            var command = this.ConnectionBuilder.Create(this.ConnectionString).BuildTextCommand(commandText, parameters);
            return
                (await command.Open().ExecuteReaderAsync())
                .EnumerateValuesInternal(handler, command.CleanupCallback());
        }

        private List<TEntity> Query<TEntity>(string commandText, IDbDataParameter[] parameters, IDataHandler<TEntity> readerHandler, IDataModel model)
        {
            Contract.Requires(string.IsNullOrEmpty(commandText) == false);

            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                return connection
                        .BuildTextCommand(commandText, parameters)
                        .Open()
                        .ExecuteReader()
                        .EnumerateEntitiesInternal(readerHandler, model, NullPolicyKind.IncludeAsDefaultValue, null)
                        .ToList(DefaultBufferSize);
            }
        }

        private async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string commandText, IDbDataParameter[] parameters, IDataHandler<TEntity> readerHandler, IDataModel model)
        {
            Contract.Requires(string.IsNullOrEmpty(commandText) == false);

            var command = this.ConnectionBuilder.Create(this.ConnectionString).BuildTextCommand(commandText, parameters);
            return 
                (await command.Open().ExecuteReaderAsync())
                    .EnumerateEntitiesInternal(readerHandler, model, NullPolicyKind.IncludeAsDefaultValue, command.CleanupCallback());
        }
    }
}
