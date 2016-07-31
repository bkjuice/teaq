using System.Data;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Implementations of this interface provide the ability to asynchronously execute a SQL statement 
    /// against the target repository and get the number of rows affected.
    /// </summary>
    public interface IAsyncNonQueryProvider
    {
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
