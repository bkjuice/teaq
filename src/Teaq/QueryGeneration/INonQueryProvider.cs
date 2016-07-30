using System.Data;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Implementations of this interface provide the ability to synchronously execute a SQL statement 
    /// against the target repository and get the number of rows affected.
    /// </summary>
    public interface INonQueryProvider
    {
        /// <summary>
        /// Executes the given command and returns the number of rows affected.
        /// </summary>
        /// <param name="command">The command built from a configured data model.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        int ExecuteNonQuery(QueryCommand command);

        /// <summary>
        /// Executes the given command and returns the number of rows affected. 
        /// </summary>
        /// <param name="commandText">The command string.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The number of rows affected.</returns>
        int ExecuteNonQuery(string commandText, params object[] parameters);

        /// <summary>
        /// Executes the given command and returns the number of rows affected. 
        /// </summary>
        /// <param name="commandText">The command string.</param>
        /// <param name="commandKind">The kind of command to execute.</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The number of rows affected.</returns>
        int ExecuteNonQuery(string commandText, CommandType commandKind, params object[] parameters);
    }
}
