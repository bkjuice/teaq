using System.Data;
using System.Data.SqlClient;

namespace Teaq
{
    /// <summary>
    /// Default implementation of the connection builder that implements the testable builder interface.
    /// </summary>
    public class SqlConnectionBuilder : IConnectionBuilder
    {
        /// <summary>
        /// Creates the specified connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// The database specific connection.
        /// </returns>
        public IDbConnection Create(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
