using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    public sealed partial class DataContext : INonQueryProvider
    {
        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The command built from a configured data model.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int ExecuteNonQuery(QueryCommand command)
        {
            return this.ExecuteNonQuery(command.CommandText, command.GetParameters());
        }

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The parameters to pass with the query.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int ExecuteNonQuery(string query, object parameterProps = null)
        {
            return this.ExecuteNonQuery(query, parameterProps.GetAnonymousParameters());
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
            return await this.ExecuteNonQueryAsync(command.CommandText, command.GetParameters());
        }

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The parameters to pass with the query.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public async Task<int> ExecuteNonQueryAsync(string query, object parameterProps = null)
        {
            return await this.ExecuteNonQueryAsync(query, parameterProps.GetAnonymousParameters());
        }
    }
}