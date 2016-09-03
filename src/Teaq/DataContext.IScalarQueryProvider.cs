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
    }
}