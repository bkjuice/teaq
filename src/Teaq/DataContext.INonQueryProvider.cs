using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Concrete implementation of a ADO.NET persistence context.
    /// </summary>
    public sealed partial class DataContext
    {
        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The command built from a configured data model.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int ExecuteNonQuery(QueryCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                var dbCommand = connection.BuildTextCommand(command.CommandText, command.GetParameters());
                dbCommand.Open();
                return dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The command built from a configured data model.</param>
        /// <returns>
        /// An awaitable result with the number of rows affected.
        /// </returns>
        public async Task<int> ExecuteNonQueryAsync(QueryCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                var dbCommand = connection.BuildTextCommand(command.CommandText, command.GetParameters());
                dbCommand.Open();
                return await dbCommand.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Executes the command and returns the first value in the first row, if one exists.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to return.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first value in the first row returned by the query.</returns>
        public TValue? ExecuteScalar<TValue>(QueryCommand command) where TValue : struct
        {
            Contract.Requires<ArgumentNullException>(command != null);

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
            Contract.Requires<ArgumentNullException>(command != null);

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
            Contract.Requires<ArgumentNullException>(command != null);

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
            Contract.Requires<ArgumentNullException>(command != null);

            return Convert(await this.ExecuteScalarAsync(command.CommandText, command.GetParameters()));
        }
    }
}
