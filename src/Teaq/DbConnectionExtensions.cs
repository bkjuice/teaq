using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;

namespace Teaq
{
    /// <summary>
    /// Extensions to enable command execution and object materialization from a database connection instance.
    /// </summary>
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// Builds the command using the provided connection instance.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters to use.</param>
        /// <returns>
        /// The prepared command instance.
        /// </returns>
        public static IDbCommand BuildTextCommand(this IDbConnection connection, string commandText, IEnumerable<IDbDataParameter> parameters = null)
        {
            Contract.Requires<ArgumentNullException>(connection != null);
            Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(commandText) == false);

            return connection.BuildCommand(commandText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Builds the stored procedure command using the provided connection instance.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="procedureName">The stored procedure name.</param>
        /// <param name="parameters">The parameters to use.</param>
        /// <returns>
        /// The prepared command instance.
        /// </returns>
        public static IDbCommand BuildStoredProcedureCommand(this IDbConnection connection, string procedureName, IEnumerable<IDbDataParameter> parameters = null)
        {
            Contract.Requires<ArgumentNullException>(connection != null);
            Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(procedureName) == false);

            return connection.BuildCommand(procedureName, CommandType.StoredProcedure, parameters);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Security",
                    "CA2100:Review SQL queries for security vulnerabilities",
                    Justification = "Queries must be validated upstream and are expected to be parameterized. Text commands will be escaped.")]
        private static IDbCommand BuildCommand(this IDbConnection connection, string commandText, CommandType type, IEnumerable<IDbDataParameter> parameters)
        {
            Contract.Requires(connection != null);
            Contract.Requires(string.IsNullOrEmpty(commandText) == false);

            var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = type;

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    command.Parameters.Add(p);
                }
            }

            return command;
        }
    }
}