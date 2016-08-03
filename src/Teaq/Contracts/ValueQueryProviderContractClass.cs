﻿using System;
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
    }
}