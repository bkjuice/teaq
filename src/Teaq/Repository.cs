﻿using System;
using System.Diagnostics.Contracts;
using Teaq.Configuration;

namespace Teaq
{
    /// <summary>
    /// Container class for Entity configurations specific to a data store.
    /// </summary>
    public static class Repository
    {
        /// <summary>
        /// The empty model.
        /// </summary>
        public readonly static IDataModel Default = BuildModel(a => { });

        /// <summary>
        /// The default string type.
        /// </summary>
        internal static StringDataType DefaultStringType = StringDataType.Varchar;

        /// <summary>
        /// Sets the default data type to use when handling strings not explicitly defined using a model.
        /// </summary>
        /// <param name="defaultType">The default type to use.</param>
        public static void SetDefaultStringType(StringDataType defaultType)
        {
            DefaultStringType = defaultType;
        }

        /// <summary>
        /// Builds the default context.
        /// </summary>
        /// <param name="connection">The connection string.</param>
        /// <returns>A persistence context to use for pass through query operations that do not require specialized mapping.</returns>
        public static IDataContext BuildContext(string connection)
        {
            Contract.Requires<ArgumentException>(string.IsNullOrEmpty(connection) == false);
            Contract.Ensures(Contract.Result<IDataContext>() != null);

            return new DataContext(connection);
        }

        /// <summary>
        /// Builds the default context.
        /// </summary>
        /// <param name="connection">The connection string.</param>
        /// <param name="connectionBuilder">The connection builder. Useful for test scenarios.</param>
        /// <returns>
        /// A persistence context to use for pass through query operations that do not require specialized mapping.
        /// </returns>
        public static IDataContext BuildContext(string connection, IConnectionBuilder connectionBuilder)
        {
            Contract.Requires<ArgumentException>(string.IsNullOrEmpty(connection) == false);
            Contract.Requires<ArgumentException>(connectionBuilder != null);
            Contract.Ensures(Contract.Result<IDataContext>() != null);

            return new DataContext(connection, connectionBuilder);
        }

        /// <summary>
        /// Builds the default context for a query batch.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="batch">The query batch to be executed.</param>
        /// <returns>The <see cref="IBatchReader"/> that will execute the query batch.</returns>
        public static IBatchReader BuildBatchReader(string connection, QueryBatch batch)
        {
            Contract.Requires<ArgumentNullException>(batch != null);
            Contract.Requires<ArgumentException>(string.IsNullOrEmpty(connection) == false);
            Contract.Ensures(Contract.Result<IBatchReader>() != null);

            return new BatchReader(connection, batch);
        }

        /// <summary>
        /// Builds the default context for a query batch.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="connectionBuilder">The connection builder. Useful for test scenarios.</param>
        /// <param name="batch">The query batch to be executed, for reading data.</param>
        /// <returns>The <see cref="IBatchReader"/> that will execute the query batch.</returns>
        public static IBatchReader BuildBatchReader(string connection, IConnectionBuilder connectionBuilder, QueryBatch batch)
        {
            Contract.Requires<ArgumentException>(string.IsNullOrEmpty(connection) == false);
            Contract.Requires<ArgumentException>(connectionBuilder != null);
            Contract.Ensures(Contract.Result<IBatchReader>() != null);

            return new BatchReader(connection, batch, connectionBuilder);
        }

        /// <summary>
        /// Builds the default context for a query batch.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <returns>
        /// The <see cref="IBatchReader" /> that will execute the query batch.
        /// </returns>
        public static IBatchReader BuildBatchReader(string connection)
        {
            Contract.Requires<ArgumentException>(string.IsNullOrEmpty(connection) == false);
            Contract.Ensures(Contract.Result<IBatchReader>() != null);

            return new BatchReader(connection, new QueryBatch(), new SqlConnectionBuilder());
        }

        /// <summary>
        /// Builds the default context for a query batch.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="connectionBuilder">The connection builder. Useful for test scenarios.</param>
        /// <param name="batch">The query batch to be executed, for writing data.</param>
        /// <returns>The <see cref="IBatchWriter"/> that will execute the query batch.</returns>
        public static IBatchWriter BuildBatchWriter(string connection, IConnectionBuilder connectionBuilder, QueryBatch batch)
        {
            Contract.Requires<ArgumentException>(string.IsNullOrEmpty(connection) == false);
            Contract.Requires<ArgumentException>(connectionBuilder != null);
            Contract.Ensures(Contract.Result<IBatchWriter>() != null);

            return new BatchWriter(connection, batch, connectionBuilder);
        }

        /// <summary>
        /// Builds the default context for a query batch.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <returns>
        /// The <see cref="IBatchWriter" /> that will execute the query batch.
        /// </returns>
        public static IBatchWriter BuildBatchWriter(string connection)
        {
            Contract.Requires<ArgumentException>(string.IsNullOrEmpty(connection) == false);
            Contract.Ensures(Contract.Result<IBatchWriter>() != null);

            return new BatchWriter(connection, new QueryBatch(), new SqlConnectionBuilder());
        }

        /// <summary>
        /// Builds the specified data configuration mapped to the provided name.
        /// </summary>
        /// <param name="configurationCallback">The configuration callback.</param>
        /// <returns>The configured data model.</returns>
        public static IDataModel BuildModel(Action<IDataModelBuilder> configurationCallback)
        {
            Contract.Requires(configurationCallback != null);
            Contract.Ensures(Contract.Result<IDataModel>() != null);

            var builder = new DataModel();
            configurationCallback(builder);
            return builder;
        }

        /// <summary>
        /// The kind of data type to use as a default unless otherwise specified.
        /// </summary>
        public enum StringDataType
        {
            /// <summary>
            /// Specifies varchar as the default string data type unless explicitly specified using a model definition.
            /// </summary>
            Varchar,

            /// <summary>
            /// Specifies nvarchar as the default string data type unless explicitly specified using a model definition.
            /// </summary>
            NVarchar
        }
    }
}