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
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public int ExecuteNonQuery(QueryCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The parameters to pass with the query.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public int ExecuteNonQuery(string query, object parameterProps = null)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(query));

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
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The parameters to pass with the query.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public Task<int> ExecuteNonQueryAsync(string query, object parameterProps = null)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(query));

            throw new NotImplementedException();
        }
    }
}
