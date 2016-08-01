using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq.Contracts
{
    /// <summary>
    /// Contract class.
    /// </summary>
    /// <seealso cref="Teaq.INonQueryProvider" />
    [ExcludeFromCodeCoverage]
    [ContractClassFor(typeof(INonQueryProvider))]
    public abstract class NonQueryProviderContractClass : INonQueryProvider
    {
        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The command built from a configured data model.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public int ExecuteNonQuery(QueryCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The command built from a configured data model.</param>
        /// <returns>
        /// An awaitable result with the number of rows affected.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public Task<int> ExecuteNonQueryAsync(QueryCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the command and returns the first value in the first row as a string, if one exists.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public string ExecuteScalar(QueryCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the command and returns the first value in the first row, if one exists.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public TValue? ExecuteScalar<TValue>(QueryCommand command) where TValue : struct
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the command asynchronously and returns the first value in the first row as a string, if one exists.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public Task<string> ExecuteScalarAsync(QueryCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the command asynchronously and returns the first value in the first row, if one exists.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public Task<TValue?> ExecuteScalarAsync<TValue>(QueryCommand command) where TValue : struct
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }
    }
}
