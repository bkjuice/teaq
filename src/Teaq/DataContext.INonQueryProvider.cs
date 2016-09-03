using System.Threading.Tasks;
using Teaq.QueryGeneration;

namespace Teaq
{
    public sealed partial class DataContext : INonQueryProvider
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
            using (var connection = this.ConnectionBuilder.Create(this.ConnectionString))
            {
                var dbCommand = connection.BuildTextCommand(command.CommandText, command.GetParameters());
                dbCommand.Open();
                return await dbCommand.ExecuteNonQueryAsync();
            }
        }

    }
}