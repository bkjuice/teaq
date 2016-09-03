using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq.Contracts
{
    /// <summary>
    /// Contract class.
    /// </summary>
    /// <seealso cref="Teaq.IValueQueryProvider" />
    [ExcludeFromCodeCoverage]
    [ContractClassFor(typeof(IValueQueryProvider))]
    public abstract class ValueQueryProviderContractClass : IValueQueryProvider
    {
        /// <summary>
        /// Queries the repository for a collection of primitive, nullable values.
        /// </summary>
        /// <typeparam name="T">The value type to be selected.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">An optional handler to read the collection of nullable values.</param>
        /// <returns>
        /// The collection of values with null values included.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        /// <exception cref="NotImplementedException"></exception>
        public List<T?> QueryNullableValues<T>(QueryCommand command, Func<IDataReader, T?> handler = null) where T : struct
        {
            Contract.Requires<ArgumentNullException>(command != null);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the repository for a collection of primitive, nullable values.
        /// </summary>
        /// <typeparam name="T">The value type to be selected.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">An optional handler to read the collection of nullable values.</param>
        /// <returns>
        /// The collection of values with null values included.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public Task<IEnumerable<T?>> QueryNullableValuesAsync<T>(QueryCommand command, Func<IDataReader, T?> handler = null) where T : struct
        {
            Contract.Requires<ArgumentNullException>(command != null);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the repository for a collection of string values.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">An optional handler to read the collection of values.</param>
        /// <returns>
        /// The collection of string values.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public List<string> QueryStringValues(QueryCommand command, Func<IDataReader, string> handler = null)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the repository for a collection of string values.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">An optional handler to read the collection of values.</param>
        /// <returns>
        /// The collection of string values.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public Task<IEnumerable<string>> QueryStringValuesAsync(QueryCommand command, Func<IDataReader, string> handler = null)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the repository for a collection of primitive values.
        /// </summary>
        /// <typeparam name="T">The value type to be returned.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">An optional handler to read the collection of values.</param>
        /// <returns>
        /// The collection of values with null values ommitted.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public List<T> QueryValues<T>(QueryCommand command, Func<IDataReader, T?> handler = null) where T : struct
        {
            Contract.Requires<ArgumentNullException>(command != null);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes an inline query that is optionally parameterized using an anonymous type.
        /// </summary>
        /// <typeparam name="T">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The optional parameter property container.</param>
        /// <param name="handler">The optional handler to extract the value.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        public List<T> QueryValues<T>(string query, object parameterProps = null, Func<IDataReader, T?> handler = null) where T : struct
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(query));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes an inline query that is optionally parameterized using an anonymous type.
        /// </summary>
        /// <typeparam name="T">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The optional parameter property container.</param>
        /// <param name="handler">The optional handler to extract the value.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        public Task<IEnumerable<T>> QueryValuesAsync<T>(string query, object parameterProps = null, Func<IDataReader, T?> handler = null) where T : struct
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(query));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the repository for a collection of primitive values.
        /// </summary>
        /// <typeparam name="T">The value type to be returned.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">An optional handler to read the collection of values.</param>
        /// <returns>
        /// The collection of values with null values ommitted.
        /// </returns>
        /// <exception cref="NotImplementedException">Contract implementation only.</exception>
        public Task<IEnumerable<T>> QueryValuesAsync<T>(QueryCommand command, Func<IDataReader, T?> handler = null) where T : struct
        {
            Contract.Requires<ArgumentNullException>(command != null);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes an inline query that is optionally parameterized using an anonymous type.
        /// </summary>
        /// <typeparam name="T">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The optional parameter property container.</param>
        /// <param name="handler">The optional handler to extract the value.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<T?> QueryNullableValues<T>(string query, object parameterProps = null, Func<IDataReader, T?> handler = null) where T : struct
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(query));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes an inline query that is optionally parameterized using an anonymous type.
        /// </summary>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The optional parameter property container.</param>
        /// <param name="handler">The optional handler to extract the value.</param>
        /// <returns>
        /// The collection of string values.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<string> QueryStringValues(string query, object parameterProps = null, Func<IDataReader, string> handler = null)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(query));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes an inline query that is optionally parameterized using an anonymous type.
        /// </summary>
        /// <typeparam name="T">The type of item that will be returned in the expected result set.</typeparam>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The optional parameter property container.</param>
        /// <param name="handler">The optional handler to extract the value.</param>
        /// <returns>
        /// An enumerable collection of results of the specified type.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IEnumerable<T?>> QueryNullableValuesAsync<T>(string query, object parameterProps = null, Func<IDataReader, T?> handler = null) where T : struct
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(query));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes an inline query that is optionally parameterized using an anonymous type.
        /// </summary>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The optional parameter property container.</param>
        /// <param name="handler">The optional handler to extract the value.</param>
        /// <returns>
        /// The collection of string values.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IEnumerable<string>> QueryStringValuesAsync(string query, object parameterProps = null, Func<IDataReader, string> handler = null)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(query));

            throw new NotImplementedException();
        }
    }
}
