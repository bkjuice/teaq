using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using Teaq.Configuration;
using Teaq.Expressions;
using Teaq.FastReflection;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Base class for runtime query construction.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    internal abstract class QueryStatementBuilder<T>
    {
        /// <summary>
        /// The empty array of parameters.
        /// </summary>
        protected static readonly SqlParameter[] EmptyParameters = new SqlParameter[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStatementBuilder{T}"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public QueryStatementBuilder(IDataModel model)
        {
            var t = typeof(T);
            if (model != null)
            {
                this.Config = model.GetEntityConfig(t);
                this.ConcreteType = model.GetConcreteType<T>();
                this.DataModel = model;
            }
            else
            {
                this.ConcreteType = t;
            }
        }

        /// <summary>
        /// Gets or sets the option clause.
        /// </summary>
        public string OptionClause { get; set; }

        /// <summary>
        /// The configuration.
        /// </summary>
        protected IEntityConfiguration Config { get; private set; }

        /// <summary>
        /// The entity configuration for the type to process.
        /// </summary>
        protected IDataModel DataModel { get; private set; }

        /// <summary>
        /// Gets the type of the concrete entity.
        /// </summary>
        protected Type ConcreteType { get; private set; }

        /// <summary>
        /// Extracts the command text as an SQL statement.
        /// </summary>
        /// <param name="batch">The optional query batch.</param>
        /// <param name="canSplitBatch">if set to <c>true</c> the batch can split before this specific command is executed.</param>
        /// <returns>
        /// The SQL statement.
        /// </returns>
        public QueryCommand<T> BuildCommand(QueryBatch batch, bool canSplitBatch)
        {
            SqlParameter[] parameters;
            var statement =
                this.BuildStatement(out parameters, batch) +
                this.GetOptionClause();

            var command = new QueryCommand<T>(statement, parameters, canSplitBatch);
            command.Model = this.DataModel;
            return command;
        }

        /// <summary>
        /// Builds the query statement and associated parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="batch">The batch.</param>
        /// <returns>
        /// The query statement.
        /// </returns>
        protected abstract string BuildStatement(out SqlParameter[] parameters, QueryBatch batch = null);

        /// <summary>
        /// Gets the column parameters.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="queryType">Type of the query.</param>
        /// <param name="batchIndex">Index of the batch, if one exists.</param>
        /// <returns>
        /// The array of SQL Parameters for the specified column values.
        /// </returns>
        protected SqlParameter[] GetColumnParameters(T target, QueryType queryType, int? batchIndex)
        {
            Contract.Ensures(Contract.Result<SqlParameter[]>() != null);

            if (target == null)
            {
                return EmptyParameters;
            }

            var properties = 
                this.ConcreteType
                    .GetTypeDescription(MemberTypes.Constructor | MemberTypes.Property)
                    .GetProperties();

            var items = new List<SqlParameter>();
            var scopeFunc = queryType.ScopeFunc();
            var config = this.Config;
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                var targetColumn = property.MemberName;
                if (!scopeFunc(property.PropertyTypeHandle, targetColumn, config))
                {
                    continue;
                }

                ColumnDataType columnDataType = null;
                if (config != null)
                {
                    columnDataType = config.ColumnDataType(targetColumn);
                    targetColumn = config.ColumnMapping(targetColumn);
                }

                var parameter = property
                        .GetValue(target)
                        .MakeParameter(targetColumn, columnDataType, "@" + targetColumn, batchIndex, null, null);

                items.Add(parameter);
            }

            return items.ToArray();
        }

        /// <summary>
        /// Gets the filter parameters and the filter clause.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="batch">The batch.</param>
        /// <param name="filterClause">The filter clause.</param>
        /// <param name="alias">The alias.</param>
        /// <returns>
        /// The array of SQL parameters and the resulting filter clause, or empty for both if a filter expression doesn't exist.
        /// </returns>
        protected SqlParameter[] GetFilterParametersAndClause(Expression<Func<T, bool>> filter, QueryBatch batch, out string filterClause, string alias)
        {
            filterClause = string.Empty;
            return filter?.Parameterize("@p", batch, out filterClause, this.DataModel, columnQualifier: alias) ?? EmptyParameters;
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <returns>The table name for this entity.</returns>
        protected string GetTableName()
        {
            return this.ConcreteType.AsQualifiedTable(this.Config);
        }

        /// <summary>
        /// Gets the options clause if one exists.
        /// </summary>
        /// <returns>
        /// The options clause or empty if one doesn't exist.
        /// </returns>
        private string GetOptionClause()
        {
            if (string.IsNullOrEmpty(this.OptionClause)) return string.Empty;
            var clause = this.OptionClause.Trim();
            if (!clause.StartsWith("OPTION", StringComparison.OrdinalIgnoreCase))
            {
               return "OPTION " + clause;
            }

            return clause;
        }
    }
}
