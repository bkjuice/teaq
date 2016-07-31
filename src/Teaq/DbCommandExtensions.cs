using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Teaq.Configuration;

namespace Teaq
{
    /// <summary>
    ///  Sponsor class to enable fluent build of commands to be executed via a data context instance.
    /// </summary>
    public static class DbCommandExtensions
    {
        /////// <summary>
        /////// Executes the command synchronously and returns the first entity in the data set.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="model">The data model to use when materializing the first entity.</param>
        /////// <param name="policy">The null policy to apply when finding the first result. Null values are included in the result set as default(T) by default.</param>
        /////// <returns>
        /////// The first entity if available or null.
        /////// </returns>
        ////public static T ReadFirstEntity<T>(this IDbCommand command, IDataModel model = null, NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);

        ////    return command.EnumerateEntities<T>(null, model, policy).FirstOrDefault();
        ////}

        /////// <summary>
        /////// Executes the command asynchronously and returns the first entity in the data set.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="model">The data model to use when materializing the first entity.</param>
        /////// <param name="policy">The null policy to apply when finding the first result. Null values are included in the result set as default(T) by default.</param>
        /////// <returns>
        /////// An awaitable first entity if available or null.
        /////// </returns>
        ////public static async Task<T> ReadFirstEntityAsync<T>(this IDbCommand command, IDataModel model = null, NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);

        ////    return (await command.EnumerateEntitiesAsync<T>(null, model, policy)).FirstOrDefault();
        ////}

        /////// <summary>
        /////// Executes the command synchronously and returns the first entity in the data set.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="handler">The custom handler to which reading the underlying data will be delegated.</param>
        /////// <param name="policy">The null policy to apply when finding the first result. Null values are included in the result set as default(T) by default.</param>
        /////// <returns>
        /////// The first entity if available or null.
        /////// </returns>
        ////public static T ReadFirstEntity<T>(this IDbCommand command, IDataHandler<T> handler, NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);
        ////    Contract.Requires<ArgumentNullException>(handler != null);

        ////    return command.EnumerateEntities<T>(handler, null, policy).FirstOrDefault();
        ////}

        /////// <summary>
        /////// Executes the command asynchronously and returns the first entity in the data set.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="handler">The custom handler to which reading the underlying data will be delegated.</param>
        /////// <param name="policy">The null policy to apply when finding the first result. Null values are included in the result set as default(T) by default.</param>
        /////// <returns>
        /////// An awaitable first entity if available or null.
        /////// </returns>
        ////public static async Task<T> ReadFirstEntityAsync<T>(this IDbCommand command, IDataHandler<T> handler, NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);
        ////    Contract.Requires<ArgumentNullException>(handler != null);

        ////    return (await command.EnumerateEntitiesAsync<T>(handler, null, policy)).FirstOrDefault();
        ////}

        /////// <summary>
        /////// Executes the command synchronously and returns a collection of entities.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="model">The data model to use when materializing entities.</param>
        /////// <param name="policy">The null policy. Null values are included in the result set as default(T) by default.</param>
        /////// <returns>
        /////// A collection of entities.
        /////// </returns>
        ////public static IEnumerable<T> EnumerateEntities<T>(this IDbCommand command, IDataModel model = null, NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);

        ////    return command.EnumerateEntities<T>(null, model, policy);
        ////}

        /////// <summary>
        /////// Executes the command synchronously and returns a collection of entities.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="handler">The custom handler to which reading the underlying data will be delegated.</param>
        /////// <param name="policy">The null policy. Null values are included in the result set as default(T) by default.</param>
        /////// <returns>
        /////// A collection of entities.
        /////// </returns>
        ////public static IEnumerable<T> EnumerateEntities<T>(IDbCommand command, IDataHandler<T> handler, NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);
        ////    Contract.Requires<ArgumentNullException>(handler != null);

        ////    return command.EnumerateEntities<T>(handler, null, policy);
        ////}

        /////// <summary>
        /////// Executes the command asynchronously and returns an awaitable collection of entities.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="model">The data model to use when materializing entities.</param>
        /////// <param name="policy">The null policy. Null values are included in the result set as default(T) by default.</param>
        /////// <returns>
        /////// A collection of entities.
        /////// </returns>
        ////public static async Task<IEnumerable<T>> EnumerateEntitiesAsync<T>(this IDbCommand command, IDataModel model = null, NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);

        ////    return await command.EnumerateEntitiesAsync<T>(null, model, policy);
        ////}

        /////// <summary>
        /////// Executes the command asynchronously and returns an awaitable collection of entities.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="handler">The custom handler to which reading the underlying data will be delegated.</param>
        /////// <param name="policy">The null policy. Null values are included in the result set as default(T) by default.</param>
        /////// <returns>
        /////// A collection of entities.
        /////// </returns>
        ////public static async Task<IEnumerable<T>> EnumerateEntitiesAsync<T>(this IDbCommand command, IDataHandler<T> handler, NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);
        ////    Contract.Requires<ArgumentNullException>(handler != null);

        ////    return await command.EnumerateEntitiesAsync<T>(handler, null, policy);
        ////}

        /////// <summary>
        /////// Executes the command synchronously and returns a collection of entities.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="model">The data model to use when materializing entities.</param>
        /////// <param name="policy">The null policy. Null values are included in the result set as default(T) by default.</param>
        /////// <param name="estimatedRowCount">The optional estimated row count. Use this value to optimize the buffer size of the list.</param>
        /////// <returns>
        /////// A collection of entities.
        /////// </returns>
        ////public static List<T> ReadEntities<T>(
        ////    this IDbCommand command,
        ////    IDataModel model = null,
        ////    NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue,
        ////    int estimatedRowCount = 64)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);
        ////    Contract.Requires<ArgumentException>(estimatedRowCount >= 0);

        ////    return command.ReadEntities<T>(null, model, policy, estimatedRowCount);
        ////}

        /////// <summary>
        /////// Executes the command synchronously and returns a collection of entities.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="handler">
        /////// The custom handler to which reading the underlying data will be delegated.
        /////// </param>
        /////// <param name="estimatedRowCount">The optional estimated row count. Use this value to optimize the buffer size of the list.</param>
        /////// <param name="policy">The null policy. Null values are included in the result set as default(T) by default.</param>
        /////// <returns>
        /////// A collection of entities.
        /////// </returns>
        ////public static List<T> ReadEntities<T>(
        ////    this IDbCommand command,
        ////    IDataHandler<T> handler,
        ////    NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue,
        ////    int estimatedRowCount = 64)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);
        ////    Contract.Requires<ArgumentNullException>(handler != null);
        ////    Contract.Requires<ArgumentException>(estimatedRowCount >= 0);

        ////    return command.ReadEntities(handler, null, policy, estimatedRowCount);
        ////}

        /////// <summary>
        /////// Executes the command asynchronously and returns an awaitable collection of entities.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="model">The data model to use when materializing entities.</param>
        /////// <param name="policy">The null policy. Null values are included in the result set as default(T) by default.</param>
        /////// <param name="estimatedRowCount">The optional estimated row count. Use this value to optimize the buffer size of the list.</param>
        /////// <returns>
        /////// A collection of entities.
        /////// </returns>
        ////public static async Task<List<T>> ReadEntitiesAsync<T>(
        ////    this IDbCommand command,
        ////    IDataModel model = null,
        ////    NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue,
        ////    int estimatedRowCount = 64)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);
        ////    Contract.Requires<ArgumentException>(estimatedRowCount >= 0);

        ////    return await command.ReadEntitiesAsync<T>(null, model, policy, estimatedRowCount);
        ////}

        /////// <summary>
        /////// Executes the command asynchronously and returns an awaitable collection of entities.
        /////// </summary>
        /////// <typeparam name="T">The entity type.</typeparam>
        /////// <param name="command">The command to execute.</param>
        /////// <param name="handler">
        /////// The custom handler to which reading the underlying data will be delegated.
        /////// </param>
        /////// <param name="estimatedRowCount">The optional estimated row count. Use this value to optimize the buffer size of the list.</param>
        /////// <param name="policy">The null policy. Null values are included in the result set as default(T) by default.</param>
        /////// <returns>
        /////// A collection of entities.
        /////// </returns>
        ////public static async Task<List<T>> ReadEntitiesAsync<T>(
        ////    this IDbCommand command,
        ////    IDataHandler<T> handler,
        ////    NullPolicyKind policy = NullPolicyKind.IncludeAsDefaultValue,
        ////    int estimatedRowCount = 64)
        ////{
        ////    Contract.Requires<ArgumentNullException>(command != null);
        ////    Contract.Requires<ArgumentNullException>(handler != null);
        ////    Contract.Requires<ArgumentException>(estimatedRowCount >= 0);

        ////    return await command.ReadEntitiesAsync(handler, null, policy, estimatedRowCount);
        ////}

        ////internal static List<T> ReadEntities<T>(this IDbCommand command, IDataHandler<T> handler, IDataModel model, NullPolicyKind policy, int count)
        ////{
        ////    Contract.Requires(command != null);
        ////    Contract.Requires(count >= 0);

        ////    command.Open();
        ////    using (var reader = command.ExecuteReader())
        ////    {
        ////        return reader.ReadEntities<T>(handler, model, count, policy);
        ////    }
        ////}

        ////internal static async Task<IEnumerable<T>> EnumerateEntitiesAsync<T>(this IDbCommand command, IDataHandler<T> handler, IDataModel model, NullPolicyKind policy)
        ////{
        ////    Contract.Requires(command != null);

        ////    command.Open();
        ////    var reader = await command.ExecuteReaderAsync();
        ////    return reader.EnumerateEntities<T>(handler, model, policy, () =>
        ////    {
        ////        reader.Close();
        ////        command.Connection.Close();
        ////        command.Dispose();
        ////    });
        ////}

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

        ////private static async Task<List<T>> ReadEntitiesAsync<T>(this IDbCommand command, IDataHandler<T> handler, IDataModel model, NullPolicyKind policy, int count)
        ////{
        ////    Contract.Requires(command != null);
        ////    Contract.Requires(count >= 0);

        ////    command.Open();
        ////    using (var reader = await command.ExecuteReaderAsync())
        ////    {
        ////        return reader.ReadEntities<T>(handler, model, count, policy);
        ////    }
        ////}
        
        ////private static IEnumerable<T> EnumerateEntities<T>(this IDbCommand command, IDataHandler<T> handler, IDataModel model, NullPolicyKind policy)
        ////{
        ////    Contract.Requires(command != null);

        ////    command.Open();
        ////    var reader = command.ExecuteReader();
        ////    return reader.EnumerateEntities<T>(handler, model, policy, () => reader.Close());
        ////}
    }
}