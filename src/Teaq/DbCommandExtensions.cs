using System;
using System.Data;
using Teaq.Configuration;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    ///  Sponsor class to enable fluent build of commands to be executed via a data context instance.
    /// </summary>
    public static partial class DbCommandExtensions
    {
        /// <summary>
        /// Creates a command parameter using the specified 
        /// name and optional data type information.
        /// </summary>
        /// <param name="value">The value for the parameter.</param>
        /// <param name="name">The name of the parameter. This is the exact name to be used.</param>
        /// <param name="type">Optional parameter type information.</param>
        /// <returns></returns>
        public static IDbDataParameter AsDbParameter(this object value, string name, ColumnDataType type = null)
        {
            return value.MakeParameter(name, type);
        }

        /// <summary>
        /// Opens the specified command's connection if not already open.
        /// </summary>
        /// <param name="command">The command with the connection to open.</param>
        /// <returns>The current command instance.</returns>
        public static IDbCommand Open(this IDbCommand command)
        {
            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
            }

            return command;
        }

        /// <summary>
        /// Creates a command clean-up callback.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The cleanup action to use when enumerating results.</returns>
        public static Action CleanupCallback(this IDbCommand command)
        {
            return () =>
            {
                command?.Connection?.Close();
                command?.Dispose();
            };
        }
    }
}