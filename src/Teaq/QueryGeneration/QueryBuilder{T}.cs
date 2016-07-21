using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Base class for entity centric query builders.
    /// </summary>
    /// <typeparam name="T">The target entity type.</typeparam>
    internal partial class QueryBuilder<T> :
        IQueryBuilder<T>,
        IQuerySelectSpecification<T>,
        IQuerySelectFilterSpecification<T>,
        IQuerySelectOrderSpecification<T>,
        IQueryGroupedSpecification<T>,
        IQueryCompletion<T>
    {
        /// <summary>
        /// Optional data model configuration.
        /// </summary>
        private readonly IDataModel dataModel;

        /// <summary>
        /// The statement builder
        /// </summary>
        private QueryStatementBuilder<T> statementBuilder;

        /// <summary>
        /// The select builder (special case due to fluent options).
        /// </summary>
        private SelectBuilder<T> selectBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBuilder{T}" /> class.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        public QueryBuilder(IDataModel dataModel = null)
        {
            this.dataModel = dataModel;
        }

        /// <summary>
        /// Gets the data model.
        /// </summary>
        protected IDataModel DataModel
        {
            get
            {
                return this.dataModel;
            }
        }

        /// <summary>
        /// Builds the select query.
        /// </summary>
        /// <param name="tableAlias">The table alias.</param>
        /// <param name="top">The top.</param>
        /// <param name="columns">The column list.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        public IQuerySelectSpecification<T> BuildSelectAs(string tableAlias, int top, params string[] columns)
        {
            return this.InitializeSelectStatement(tableAlias, top, columns);
        }

        /// <summary>
        /// Builds the select query.
        /// </summary>
        /// <param name="tableAlias">Optional table alias.</param>
        /// <param name="columns">The column list.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        public IQuerySelectSpecification<T> BuildSelectAs(string tableAlias, params string[] columns)
        {
            return this.InitializeSelectStatement(tableAlias, null, columns);
        }

        /// <summary>
        /// Builds the select query.
        /// </summary>
        /// <param name="columns">The column list.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        public IQuerySelectSpecification<T> BuildSelect(params string[] columns)
        {
            return this.InitializeSelectStatement(null, null, columns);
        }

        /// <summary>
        /// Builds the select query.
        /// </summary>
        /// <param name="top">The top.</param>
        /// <param name="columns">The column list.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        public IQuerySelectSpecification<T> BuildSelect(int top, params string[] columns)
        {
            return this.InitializeSelectStatement(null, top, columns);
        }

        /// <summary>
        /// Builds an update query.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        public IQueryCompletion<T> BuildUpdate(T target, Expression<Func<T, bool>> filter = null)
        {
            ////Contract.Requires<ArgumentNullException>(target != null);

            this.statementBuilder = new UpdateBuilder<T>(this.dataModel, target, filter);
            return this;
        }

        /// <summary>
        /// Builds the insert query.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        public IQueryCompletion<T> BuildInsert(T target)
        {
            ////Contract.Requires(target != null);

            this.statementBuilder = new InsertBuilder<T>(this.dataModel, target);
            return this;
        }

        /// <summary>
        /// Builds the delete query.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        public IQueryCompletion<T> BuildDelete(Expression<Func<T, bool>> filter = null)
        {
            this.statementBuilder = new DeleteBuilder<T>(this.dataModel, filter);
            return this;
        }

        /// <summary>
        /// Adds a (nolock) hint to the working query.
        /// </summary>
        /// <returns>
        /// The fluent interface to continue building the query.
        /// </returns>
        public IQuerySelectFilterSpecification<T> WithNoLock()
        {
            this.selectBuilder.HintClause = ("(nolock)");
            return this;
        }

        /// <summary>
        /// Joins the specified join type.
        /// </summary>
        /// <typeparam name="TJoined">The type of the joined.</typeparam>
        /// <param name="joinType">Type of the join.</param>
        /// <param name="onExpression">The on expression.</param>
        /// <param name="joinColumnList">The join column list.</param>
        /// <returns>
        /// The fluent interface to continue building the query.
        /// </returns>
        public IQuerySelectFilterSpecification<T> Join<TJoined>(
            JoinType joinType,
            Expression<Func<T, TJoined, bool>> onExpression, 
            params string[] joinColumnList)
        {
            this.selectBuilder.SetJoin<TJoined>(joinType, onExpression, joinColumnList);
            return this;
        }

        /// <summary>
        /// Specifies a grouping clause using the provided properties.
        /// </summary>
        /// <param name="propertyExpressions">The property expression array.</param>
        /// <returns>
        /// The fluent interface to build the remaining query specification.
        /// </returns>
        public IQueryGroupedSpecification<T> GroupBy(params Expression<Func<T, object>>[] propertyExpressions)
        {
            this.selectBuilder.GroupingProperties = propertyExpressions;
            return this;
        }

        /// <summary>
        /// Wheres the specified filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        /// <returns>
        /// The fluent interface to continue building the query.
        /// </returns>
        public IQuerySelectOrderSpecification<T> Where(Expression<Func<T, bool>> filterExpression)
        {
            this.selectBuilder.Filter = filterExpression;
            return this;
        }

        /// <summary>
        /// Orders the by.
        /// </summary>
        /// <param name="orderByExpression">The order by expression.</param>
        /// <returns>
        /// The fluent interface to continue building the query.
        /// </returns>
        public IQueryCompletion<T> OrderBy(Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> orderByExpression)
        {
            this.selectBuilder.OrderByExpression  = orderByExpression;
            return this;
        }

        /// <summary>
        /// Extracts the query command and clears all internal state.
        /// </summary>
        /// <returns>The query command.</returns>
        public QueryCommand<T> ToCommand()
        {
            return this.statementBuilder.BuildCommand(null, false);
        }

        /// <summary>
        /// Adds to the underlying query batch and clears all internal state.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="canSplitBatch">if set to <c>true</c> the query [can be split into a separate batch if needed].</param>
        public QueryCommand<T> AddToBatch(QueryBatch batch, bool canSplitBatch = true)
        {
            ////Contract.Requires(batch != null);

            var command = this.statementBuilder.BuildCommand(batch, canSplitBatch);
            batch.Add<T>(command, canSplitBatch);
            return command;
        }

        /// <summary>
        /// Adds an options clause to the working query.
        /// </summary>
        /// <param name="optionsClause">The literal options clause to append to the select statement.</param>
        /// <returns>
        /// The fluent interface to complete query specification.
        /// </returns>
        public IQueryCompletion<T> WithOption(string optionsClause)
        {
            this.statementBuilder.OptionClause  = optionsClause;
            return this;
        }

        /// <summary>
        /// Builds the select query.
        /// </summary>
        /// <param name="tableAlias">The table alias.</param>
        /// <param name="top">The top.</param>
        /// <param name="columns">The column list.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        private IQuerySelectSpecification<T> InitializeSelectStatement(string tableAlias, int? top, params string[] columns)
        {
            this.selectBuilder = new SelectBuilder<T>(this.DataModel, columns);
            this.selectBuilder.Top = top;
            this.selectBuilder.Alias = tableAlias;
            this.statementBuilder = this.selectBuilder;
            return this;
        }
    }
}