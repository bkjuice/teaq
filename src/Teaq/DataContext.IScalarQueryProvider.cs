using System;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    public sealed partial class DataContext : IScalarQueryProvider
    {
        /// <summary>
        /// Executes the command and returns the first value in the first row, if one exists.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first value in the first row returned by the query.</returns>
        public TValue? ExecuteScalar<TValue>(QueryCommand command) where TValue : struct
        {
            return Convert<TValue>(this.ExecuteScalar(command.CommandText, command.GetParameters()));
        }

        /// <summary>
        /// Executes the command and returns the first value in the first row as a string, if one exists.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        public string ExecuteScalar(QueryCommand command)
        {
            return Convert(this.ExecuteScalar(command.CommandText, command.GetParameters()));
        }

        /// <summary>
        /// Executes the command asynchronously and returns the first value in the first row, if one exists.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first value in the first row returned by the query.</returns>
        public async Task<TValue?> ExecuteScalarAsync<TValue>(QueryCommand command) where TValue : struct
        {
            return Convert<TValue>(await this.ExecuteScalarAsync(command.CommandText, command.GetParameters()));
        }

        /// <summary>
        /// Executes the command asynchronously and returns the first value in the first row as a string, if one exists.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        public async Task<string> ExecuteScalarAsync(QueryCommand command)
        {
            return Convert(await this.ExecuteScalarAsync(command.CommandText, command.GetParameters()));
        }

        /// <summary>
        /// Executes the command and returns the first value in the first row, if one exists.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The parameter properties to use as query parameters.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        public TValue? ExecuteScalar<TValue>(string query, object parameterProps = null) where TValue : struct
        {
            return Convert<TValue>(this.ExecuteScalar<TValue>(query, parameterProps.GetAnonymousParameters()));
        }

        /// <summary>
        /// Executes the command asynchronously and returns the first value in the first row as a string, if one exists.
        /// </summary>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The parameter properties to use as query parameters.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        public async Task<TValue?> ExecuteScalarAsync<TValue>(string query, object parameterProps = null) where TValue : struct
        {
            return Convert<TValue>(await this.ExecuteScalarAsync(query, parameterProps.GetAnonymousParameters()));
        }

        /// <summary>
        /// Executes the command and returns the first value in the first row as a string, if one exists.
        /// </summary>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The parameter properties to use as query parameters.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        public string ExecuteScalar(string query, object parameterProps = null)
        {
            return Convert(this.ExecuteScalar(query, parameterProps.GetAnonymousParameters()));
        }

        /// <summary>
        /// Executes the command asynchronously and returns the first value in the first row as a string, if one exists.
        /// </summary>
        /// <param name="query">The inline query to execute.</param>
        /// <param name="parameterProps">The parameter properties to use as query parameters.</param>
        /// <returns>
        /// The first value in the first row returned by the query.
        /// </returns>
        public async Task<string> ExecuteScalarAsync(string query, object parameterProps = null)
        {
            return Convert(await this.ExecuteScalarAsync(query, parameterProps.GetAnonymousParameters()));
        }
    }
}