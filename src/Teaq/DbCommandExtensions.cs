using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Teaq.Configuration;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    ///  Sponsor class to enable fluent build of commands to be executed via a data context instance.
    /// </summary>
    public static partial class DbCommandExtensions
    {
        /// <summary>
        /// Creates a command parameter using the specified 
        /// name and optional data type information.
        /// </summary>
        /// <param name="value">The value for the parameter.</param>
        /// <param name="name">The name of the parameter. This is the exact name to be used.</param>
        /// <param name="type">Optional parameter type information.</param>
        /// <returns></returns>
        public static IDbDataParameter AsDbParameter(this object value, string name, ColumnDataType type = null)
        {
            return value.MakeParameter(name, type);
        }

        /// <summary>
        /// Opens the specified command's connection if not already open.
        /// </summary>
        /// <param name="command">The command with the connection to open.</param>
        /// <returns>The current command instance.</returns>
        public static IDbCommand Open(this IDbCommand command)
        {
            if (command.Connection?.State != ConnectionState.Open)
            {
                command.Connection?.Open();
            }

            return command;
        }

        /// <summary>
        /// Creates a command clean-up callback closure.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The cleanup action to use when enumerating results.</returns>
        public static Action CleanupCallback(this IDbCommand command)
        {
            return () =>
            {
                command?.Connection?.Close();
                command?.Dispose();
            };
        }

        /// <summary>
        /// Executes the provide command asynchronously and returns an awaitable data reader instance.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="commandBehavior">The command behavior to use.</param>
        /// <returns>
        /// An awaitable data reader to read.
        /// </returns>
        public static async Task<IDataReader> ExecuteReaderAsync(this IDbCommand command, CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            var sqlCommand = command as SqlCommand;
            if (sqlCommand != null)
            {
                return await sqlCommand.ExecuteReaderAsync(commandBehavior);
            }

            Func<IDataReader> awaitable = () => command.ExecuteReader(commandBehavior);
            return await awaitable.InvokeAsAwaitable();
        }

        /// <summary>
        /// Executes the command against the backing store asynchronously.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The awaitable number of rows affected.
        /// </returns>
        public static async Task<int> ExecuteNonQueryAsync(this IDbCommand command)
        {
            var sqlCommand = command as SqlCommand;
            if (sqlCommand != null)
            {
                return await sqlCommand.ExecuteNonQueryAsync();
            }

            Func<int> awaitable = () => command.ExecuteNonQuery();
            return await awaitable.InvokeAsAwaitable();
        }

        /// <summary>
        /// Executes the command against the backing store asynchronously.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The awaitable first value returned in the first row.
        /// </returns>
        public static async Task<object> ExecuteScalarAsync(this IDbCommand command)
        {
            var sqlCommand = command as SqlCommand;
            if (sqlCommand != null)
            {
                return await sqlCommand.ExecuteScalarAsync();
            }

            Func<object> awaitable = () => command.ExecuteScalar();
            return await awaitable.InvokeAsAwaitable();
        }
    }
}