using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    public sealed partial class DataContext : IValueQueryProvider
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
        /// <exception cref="NotImplementedException"></exception>
        public List<T?> QueryNullableValues<T>(QueryCommand command, Func<IDataReader, T?> handler = null) where T : struct
        {
            return this.QueryNullableValues(command.CommandText, command.GetParameters(), handler).ToList(64);
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
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<T?>> QueryNullableValuesAsync<T>(QueryCommand command, Func<IDataReader, T?> handler = null) where T : struct
        {
            return await this.QueryNullableValuesAsync(command.CommandText, command.GetParameters(), handler);
        }

        /// <summary>
        /// Queries the repository for a collection of string values.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">An optional handler to read the collection of values.</param>
        /// <returns>
        /// The collection of string values.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<string> QueryStringValues(QueryCommand command, Func<IDataReader, string> handler = null)
        {
            return this.QueryStringValues(command.CommandText, command.GetParameters(), handler);
        }

        /// <summary>
        /// Queries the repository for a collection of string values.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="handler">An optional handler to read the collection of values.</param>
        /// <returns>
        /// The collection of string values.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<string>> QueryStringValuesAsync(QueryCommand command, Func<IDataReader, string> handler = null)
        {
            return await this.QueryStringValuesAsync(command.CommandText, command.GetParameters(), handler);
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
            return this.QueryValues(query, parameterProps.GetAnonymousParameters(), handler);
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
        /// <exception cref="NotImplementedException"></exception>
        public List<T> QueryValues<T>(QueryCommand command, Func<IDataReader, T?> handler = null) where T : struct
        {
            return this.QueryValues(command.CommandText, command.GetParameters(), handler).ToList(64);
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
        public async Task<IEnumerable<T>> QueryValuesAsync<T>(string query, object parameterProps = null, Func<IDataReader, T?> handler = null) where T : struct
        {
            return await this.QueryValuesAsync(query, parameterProps.GetAnonymousParameters(), handler);
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
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<T>> QueryValuesAsync<T>(QueryCommand command, Func<IDataReader, T?> handler = null) where T : struct
        {
            return await this.QueryValuesAsync(command.CommandText, command.GetParameters(), handler);
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
            return this.QueryNullableValues(query, parameterProps.GetAnonymousParameters(), handler);
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
            return this.QueryStringValues(query, parameterProps.GetAnonymousParameters(), handler);
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
        public async Task<IEnumerable<T?>> QueryNullableValuesAsync<T>(string query, object parameterProps = null, Func<IDataReader, T?> handler = null) where T : struct
        {
            return await this.QueryNullableValuesAsync(query, parameterProps.GetAnonymousParameters(), handler);
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
        public async Task<IEnumerable<string>> QueryStringValuesAsync(string query, object parameterProps = null, Func<IDataReader, string> handler = null)
        {
            return await this.QueryStringValuesAsync(query, parameterProps.GetAnonymousParameters(), handler);
        }
    }
}
