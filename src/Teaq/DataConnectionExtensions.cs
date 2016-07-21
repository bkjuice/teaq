using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Teaq.Configuration;

namespace Teaq
{
    /// <summary>
    /// Extensions to enable command execution and object materialization from a database connection instance.
    /// </summary>
    public static class DataConnectionExtensions
    {
        /// <summary>
        /// Reads the first entity from the open connection using the provided command text.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// The first entity retrieved or the default value.
        /// </returns>
        public static T ReadFirstEntity<T>(
            this IDbConnection connection, 
            string commandText,
            object[] parameters, 
            IDataModel dataModel = null, 
            CommandType commandKind = CommandType.Text,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            Contract.Requires(connection != null);
            Contract.Requires(!string.IsNullOrEmpty(commandText));

            var results = connection.ReadEntities<T>(commandText, parameters, commandKind, null, dataModel, 8, nullPolicy);
            if (results.Count == 0)
            {
                return default(T);
            }

            return results[0];
        }

        /// <summary>
        /// Reads the first entity from the open connection using the provided command text.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="handler">The custom handler. If provided and the handler can read the result set, execution is delegated to
        /// the handler to produce the results.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// The first entity retrieved or the default value.
        /// </returns>
        public static T ReadFirstEntity<T>(
            this IDbConnection connection, 
            string commandText, 
            object[] parameters, 
            IDataHandler<T> handler, 
            CommandType commandKind = CommandType.Text,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            Contract.Requires(connection != null);
            Contract.Requires(!string.IsNullOrEmpty(commandText));
            Contract.Requires(handler != null);

            var results = connection.ReadEntities<T>(commandText, parameters, commandKind, handler, null, 8, nullPolicy);
            if (results.Count == 0)
            {
                return default(T);
            }

            return results[0];
        }

        /// <summary>
        /// Enumerates entities from the open connection using the provided command text.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// A list of values.
        /// </returns>
        public static IEnumerable<T> EnumerateEntities<T>(
            this IDbConnection connection, 
            string commandText, 
            object[] parameters, 
            IDataModel dataModel = null, 
            CommandType commandKind = CommandType.Text,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            Contract.Requires(connection != null);
            Contract.Requires(!string.IsNullOrEmpty(commandText));

            return connection.EnumerateEntities<T>(commandText, parameters, commandKind, null, dataModel, nullPolicy);
        }

        /// <summary>
        /// Enumerates entities from the open connection using the provided command text.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="handler">The custom handler. If provided and the handler can read the result set, execution is delegated to the handler.
        /// to produce the results.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// A list of values.
        /// </returns>
        public static IEnumerable<T> EnumerateEntities<T>(
            this IDbConnection connection, 
            string commandText, 
            object[] parameters,
            IDataHandler<T> handler, 
            CommandType commandKind = CommandType.Text,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            Contract.Requires(connection != null);
            Contract.Requires(!string.IsNullOrEmpty(commandText));
            Contract.Requires(handler != null);

            return connection.EnumerateEntities<T>(commandText, parameters, commandKind, handler, null, nullPolicy);
        }

        /// <summary>
        /// Enumerates entities from the open connection using the provided command text.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// An awaitable collection of entities of the specified type.
        /// </returns>
        public static async Task<IEnumerable<T>> EnumerateEntitiesAsync<T>(
            this IDbConnection connection,
            string commandText,
            object[] parameters,
            IDataModel dataModel = null,
            CommandType commandKind = CommandType.Text,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            return await connection.EnumerateEntitiesAsync<T>(commandText, parameters, commandKind, null, dataModel, nullPolicy);
        }

        /// <summary>
        /// Enumerates entities from the open connection using the provided command text.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="commandKind">Kind of the command.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// An awaitable collection of entities of the specified type.
        /// </returns>
        public static async Task<IEnumerable<T>> EnumerateEntitiesAsync<T>(
            this IDbConnection connection,
            string commandText,
            object[] parameters,
            IDataHandler<T> handler,
            CommandType commandKind = CommandType.Text,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            return await connection.EnumerateEntitiesAsync<T>(commandText, parameters, commandKind, handler, null, nullPolicy);
        }

        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <param name="estimatedRowCount">The optional estimated row count. Use this value to optimize the buffer size of the list.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// A list of entities or values of the specified type.
        /// </returns>
        public static List<T> ReadEntities<T>(
            this IDbConnection connection, 
            string commandText, 
            object[] parameters, 
            IDataModel dataModel = null, 
            CommandType commandKind = CommandType.Text, 
            int estimatedRowCount = 64,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            Contract.Requires<ArgumentNullException>(connection != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(commandText));
            Contract.Requires<ArgumentException>(estimatedRowCount >= 0);

            return connection.ReadEntities<T>(commandText, parameters, commandKind, null, dataModel, estimatedRowCount, nullPolicy);
        }

        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="handler">The custom handler. If provided and the handler can read the result set, execution is delegated to the handler
        /// to produce the results.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <param name="estimatedRowCount">The optional estimated row count. Use this value to optimize the buffer size of the list.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// A list of values.
        /// </returns>
        public static List<T> ReadEntities<T>(
            this IDbConnection connection, 
            string commandText,
            object[] parameters, 
            IDataHandler<T> handler,
            CommandType commandKind = CommandType.Text, 
            int estimatedRowCount = 64,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            Contract.Requires<ArgumentNullException>(connection != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(commandText));
            Contract.Requires<ArgumentNullException>(handler != null);
            Contract.Requires<ArgumentException>(estimatedRowCount >= 0);

            return connection.ReadEntities<T>(commandText, parameters, commandKind, handler, null, estimatedRowCount, nullPolicy);

        }

        /// <summary>
        /// Executes the query and returns the number of rows affected.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <returns>The number of rows affected</returns>
        public static int ExecuteNonQuery(this IDbConnection connection, string commandText, object[] parameters, CommandType commandKind = CommandType.Text)
        {
            Contract.Requires(connection != null);
            Contract.Requires(!string.IsNullOrEmpty(commandText));

            using (var command = connection.PrepareCommand(commandText, parameters, commandKind))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes the query and returns the number of rows affected.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <returns>The number of rows affected</returns>
        public static async Task<int> ExecuteNonQueryAsync(this IDbConnection connection, string commandText, object[] parameters, CommandType commandKind = CommandType.Text)
        {
            Contract.Requires(connection != null);
            Contract.Requires(!string.IsNullOrEmpty(commandText));

            using (var command = connection.PrepareCommand(commandText, parameters, commandKind))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                return await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Prepares the command using the provided connection instance.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="type">The command kind, default is 'Text'.</param>
        /// <returns>The prepared command instance.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "Microsoft.Security",
                    "CA2100:Review SQL queries for security vulnerabilities",
                    Justification = "Queries must be validated upstream and are expected to be parameterized. Text commands will be escaped.")]
        internal static IDbCommand PrepareCommand(this IDbConnection connection, string commandText, object[] parameters, CommandType type)
        {
            Contract.Requires(connection != null);

            var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = type;
            if (parameters != null)
            {
                for (int i = 0; i < parameters.GetLength(0); i++)
                {
                    command.Parameters.Add(parameters[i]);
                }
            }

            return command;
        }

        /// <summary>
        /// Reads the entities from the provided connection.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <param name="handler">The custom handler. If provided and the handler can read the result set, execution is delegated to the handler
        /// to produce the results.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="estimatedRowCount">The optional estimated row count. Use this value to optimize the buffer size of the list.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// A list of values.
        /// </returns>
        internal static List<T> ReadEntities<T>(
            this IDbConnection connection, 
            string commandText, 
            object[] parameters, 
            CommandType commandKind, 
            IDataHandler<T> handler, 
            IDataModel dataModel, 
            int estimatedRowCount,
            NullPolicyKind nullPolicy)
        {
            Contract.Requires(connection != null);
            Contract.Requires(!string.IsNullOrEmpty(commandText));
            Contract.Requires(estimatedRowCount >= 0);

            using (var command = connection.PrepareCommand(commandText, parameters, commandKind))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (var reader = command.ExecuteReader())
                {
                    return reader.ReadEntities<T>(handler, dataModel, estimatedRowCount, nullPolicy);
                }
            }
        }

        /// <summary>
        /// Reads the entities from the provided connection.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <param name="handler">The custom handler. If provided and the handler can read the result set, execution is delegated to the handler
        /// to produce the results.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// A list of values.
        /// </returns>
        internal static async Task<IEnumerable<T>> EnumerateEntitiesAsync<T>(
            this IDbConnection connection,
            string commandText,
            object[] parameters,
            CommandType commandKind,
            IDataHandler<T> handler,
            IDataModel dataModel,
            NullPolicyKind nullPolicy)
        {
            var command = connection.PrepareCommand(commandText, parameters, commandKind);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var reader = await command.ExecuteReaderAsync();
            return reader.EnumerateEntities<T>(handler, dataModel, nullPolicy, () =>
            {
                reader.Close();
                connection.Close();
                command.Dispose();
            });
        }

        /// <summary>
        /// Reads the entities from the provided connection.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandKind">The command kind, default is 'Text'.</param>
        /// <param name="handler">The custom handler. If provided and the handler can read the result set, execution is delegated to the handler
        /// to produce the results.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// A list of values.
        /// </returns>
        private static IEnumerable<T> EnumerateEntities<T>(
            this IDbConnection connection, 
            string commandText, 
            object[] parameters, 
            CommandType commandKind, 
            IDataHandler<T> handler,
            IDataModel dataModel,
            NullPolicyKind nullPolicy)
        {
            Contract.Requires(connection != null);
            Contract.Requires(!string.IsNullOrEmpty(commandText));

            using (var command = connection.PrepareCommand(commandText, parameters, commandKind))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                var reader = command.ExecuteReader();
                return reader.EnumerateEntities<T>(handler, dataModel, nullPolicy, () => reader.Close());
            }
        }
    }
}