using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Implementations of this interface provide the ability to synchronously execute a SQL statement 
    /// against the target repository and get the number of rows affected.
    /// </summary>
    [ContractClass(typeof(Contracts.ScalarQueryProviderContractClass))]
    public interface IScalarQueryProvider
    {
        /// <summary>
        /// Executes the command and returns the first value in the first row, if one exists.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first value in the first row returned by the query.</returns>
        TValue? ExecuteScalar<TValue>(QueryCommand command) where TValue : struct;

        /// <summary>
        /// Executes the command and returns the first value in the first row as a string, if one exists.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        string ExecuteScalar(QueryCommand command);

        /// <summary>
        /// Executes the command asynchronously and returns the first value in the first row, if one exists.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first value in the first row returned by the query.</returns>
        Task<TValue?> ExecuteScalarAsync<TValue>(QueryCommand command) where TValue : struct;

        /// <summary>
        /// Executes the command asynchronously and returns the first value in the first row as a string, if one exists.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        Task<string> ExecuteScalarAsync(QueryCommand command);
    }
}
