using System.Data;

namespace Teaq
{
    /// <summary>
    /// Interface to inject database infrastructure for testability.
    /// </summary>
    public interface IConnectionBuilder
    {
        /// <summary>
        /// Creates the specified connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The database specific connection.</returns>
        IDbConnection Create(string connectionString);
    }
}
