using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Implementations of this interface provide the ability to synchronously execute a SQL statement 
    /// against the target repository and get the number of rows affected.
    /// </summary>
    [ContractClass(typeof(Contracts.NonQueryProviderContractClass))]
    public interface INonQueryProvider
    {
        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The parameters to pass with the query.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        int ExecuteNonQuery(string query, object parameterProps = null);

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
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The parameters to pass with the query.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        Task<int> ExecuteNonQueryAsync(string query, object parameterProps = null);

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The command built from a configured data model.</param>
        /// <returns>
        /// An awaitable result with the number of rows affected.
        /// </returns>
        Task<int> ExecuteNonQueryAsync(QueryCommand command);
    }
}
