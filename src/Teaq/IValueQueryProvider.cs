using System;
using System.Collections.Generic;
using System.Data;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Implementations of this interface provide the ability to synchronously query the target repository
    /// for a collection of results.
    /// </summary>
    public interface IValueQueryProvider
    {
        /// <summary>
        /// Queries the repository for a collection of primitive values.
        /// </summary>
        /// <typeparam name="T">The value type to be returned.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">An optional handler to read the collection of values.</param>
        /// <returns>
        /// The collection of values with null values ommitted.
        /// </returns>
        List<T> QueryValues<T>(QueryCommand command, Func<IDataReader, T?> handler = null) where T : struct;

        /// <summary>
        /// Queries the repository for a collection of primitive, nullable values.
        /// </summary>
        /// <typeparam name="T">The value type to be selected.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">An optional handler to read the collection of nullable values.</param>
        /// <returns>The collection of values with null values included.</returns>
        List<T?> QueryNullableValues<T>(QueryCommand command, Func<IDataReader, T?> handler = null) where T : struct;

        /// <summary>
        /// Queries the repository for a collection of string values.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">An optional handler to read the collection of values.</param>
        /// <returns>
        /// The collection of string values.
        /// </returns>
        List<string> QueryStringValues(QueryCommand command, Func<IDataReader, string> handler = null);

        //// TODO: Overloads that include text, parameters and include <string> overloads. 
    }
}