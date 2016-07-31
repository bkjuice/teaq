using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Teaq
{
    /// <summary>
    /// Sponsor class containing extension methods to invoke <see cref="IDbCommand"/> execute methods asynchronously.
    /// </summary>
    public static class DbCommandAsyncExtensions
    {
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
