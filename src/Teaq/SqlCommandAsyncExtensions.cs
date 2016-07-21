using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Teaq
{
    /// <summary>
    /// Sponsor class containing methods to invoke commands asynchronously.
    /// </summary>
    internal static class SqlCommandAsyncExtensions
    {
        /// <summary>
        /// Executes the provide command asynchronously and returns a data reader instance.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandBehavior">The command behavior.</param>
        /// <returns>
        /// The data reader to read.
        /// </returns>
        internal static async Task<IDataReader> ExecuteReaderAsync(this IDbCommand command, CommandBehavior commandBehavior = CommandBehavior.Default)
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
        /// The number of rows affected.
        /// </returns>
        internal static async Task<int> ExecuteNonQueryAsync(this IDbCommand command)
        {
            var sqlCommand = command as SqlCommand;
            if (sqlCommand != null)
            {
                return await sqlCommand.ExecuteNonQueryAsync();
            }

            Func<int> awaitable = () => command.ExecuteNonQuery();
            return await awaitable.InvokeAsAwaitable();
        }
    }
}
