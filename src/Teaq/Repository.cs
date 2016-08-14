using System;
using System.Data;
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

        internal static SqlStringType DefaultStringType = SqlStringType.Varchar;

        internal static int DefaultStringSize = -1;

        /// <summary>
        /// Sets the default data type to use when handling strings not explicitly defined using a model.
        /// </summary>
        /// <param name="defaultType">The default type to use.</param>
        public static void SetDefaultStringType(SqlStringType defaultType)
        {
            DefaultStringType = defaultType;
        }

        /// <summary>
        /// Sets the default size to use when handling strings not explicitly defined using a model. The initial default is -1 (max).
        /// </summary>
        /// <param name="size">The default size to use.</param>
        public static void SetDefaultStringSize(int size)
        {
            DefaultStringSize = size;
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
        /// <param name="batch">The query batch to be executed, for reading data.</param>
        /// <param name="connectionBuilder">The connection builder. Useful for test scenarios.</param>
        /// <returns>
        /// The <see cref="IBatchReader" /> that will execute the query batch.
        /// </returns>
        public static IBatchReader BuildBatchReader(string connection, QueryBatch batch, IConnectionBuilder connectionBuilder)
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
        /// The <see cref="IBatchWriter" /> that will execute the query batch.
        /// </returns>
        public static IBatchWriter BuildBatchWriter(string connection)
        {
            Contract.Requires<ArgumentException>(string.IsNullOrEmpty(connection) == false);
            Contract.Ensures(Contract.Result<IBatchWriter>() != null);

            return new BatchWriter(connection, new QueryBatch(), new SqlConnectionBuilder());
        }

        /// <summary>
        /// Builds the default context for a query batch.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="batch">The batch to be written.</param>
        /// <returns>
        /// The <see cref="IBatchWriter" /> that will execute the query batch.
        /// </returns>
        public static IBatchWriter BuildBatchWriter(string connection, QueryBatch batch)
        {
            Contract.Requires<ArgumentException>(string.IsNullOrEmpty(connection) == false);
            Contract.Ensures(Contract.Result<IBatchWriter>() != null);

            return new BatchWriter(connection, batch, new SqlConnectionBuilder());
        }

        /// <summary>
        /// Builds the default context for a query batch.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="batch">The query batch to be executed, for writing data.</param>
        /// <param name="connectionBuilder">The connection builder. Useful for test scenarios.</param>
        /// <returns>
        /// The <see cref="IBatchWriter" /> that will execute the query batch.
        /// </returns>
        public static IBatchWriter BuildBatchWriter(string connection, QueryBatch batch, IConnectionBuilder connectionBuilder)
        {
            Contract.Requires<ArgumentException>(string.IsNullOrEmpty(connection) == false);
            Contract.Requires<ArgumentException>(connectionBuilder != null);
            Contract.Ensures(Contract.Result<IBatchWriter>() != null);

            return new BatchWriter(connection, batch, connectionBuilder);
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

        internal static ColumnDataType GetDefaultStringType()
        {
            return new ColumnDataType
            {
                SqlDataType = DefaultStringType == SqlStringType.NVarchar ? SqlDbType.NVarChar : SqlDbType.VarBinary,
                Size = DefaultStringSize,
            };
        }
    }
}
