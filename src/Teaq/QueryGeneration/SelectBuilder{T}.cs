using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Teaq.Configuration;
using Teaq.Expressions;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Builder for the select statement.
    /// </summary>
    /// <typeparam name="T">The type of target entity.</typeparam>
    /// <seealso cref="Teaq.QueryGeneration.QueryStatementBuilder{T}" />
    internal class SelectBuilder<T> : QueryStatementBuilder<T>
    {
        /// <summary>
        /// Gets or sets the columns to use.
        /// </summary>
        private string[] columns;

        /// <summary>
        /// The join expression
        /// </summary>
        private string joinExpression;

        /// <summary>
        /// The join column list.
        /// </summary>
        private string[] joinColumnList;

        /// <summary>
        /// The joined type.
        /// </summary>
        private Type joinedType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectBuilder{T}"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="columnList">The column list.</param>
        public SelectBuilder(IDataModel model, string[] columnList = null)
            : base (model)
        {
            this.columns = columnList;
        }

        /// <summary>
        /// The top clause
        /// </summary>
        public int? Top { get; set; }

        /// <summary>
        /// The where clause.
        /// </summary>
        public Expression<Func<T, bool>> Filter { get; set; }

        /// <summary>
        /// The grouping clause.
        /// </summary>
        public Expression<Func<T, object>>[] GroupingProperties { get; set; }

        /// <summary>
        /// The order by clause.
        /// </summary>
        public Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> OrderByExpression { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the hint clause.
        /// </summary>
        public string HintClause { get; set; }

        /// <summary>
        /// Builds the select query.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="batch">The batch.</param>
        protected override string BuildStatement(out SqlParameter[] parameters, QueryBatch batch = null)
        {
            string filterClause;
            parameters = this.GetFilterParametersAndClause(this.Filter, batch, out filterClause, this.Alias);

            var groupingClause = this.GetGroupByClause();

            var concreteType = this.DataModel.EnsureConcreteType<T>();
            var columnClause = concreteType.ExtractSelectColumnList(this.Alias, this.columns, this.Config) + this.GetJoinColumns();

            var orderByClause = this.GetOrderByClause();
            var tableClause = this.GetTableName()
                + (string.IsNullOrEmpty(this.Alias) ? string.Empty : " "
                + this.Alias) + this.joinExpression;

            return "\r\nselect "
               + this.GetTopClause()
               + columnClause
               + "\r\n from " + tableClause + this.GetHintClause()
               + (string.IsNullOrEmpty(groupingClause) ? string.Empty : " " + groupingClause + "\r\n")
               + (string.IsNullOrEmpty(filterClause) ? string.Empty : " where " + filterClause + "\r\n")
               + (string.IsNullOrEmpty(orderByClause) ? string.Empty : " " + orderByClause + "\r\n");
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
        public void SetJoin<TJoined>(
            JoinType joinType,
            Expression<Func<T, TJoined, bool>> onExpression,
            params string[] joinColumnList)
        {
            Contract.Requires(onExpression != null);

            this.joinedType = this.DataModel.EnsureConcreteType<TJoined>();
            this.joinColumnList = joinColumnList;
            this.joinExpression = "\r\n" + joinType.ToString()
                + " Join " + this.joinedType.AsQualifiedTable(this.DataModel?.GetEntityConfig(typeof(TJoined)))
                + " on " + onExpression.ParseJoinExpression(this.DataModel);
        }

        /// <summary>
        /// Gets the join columns.
        /// </summary>
        /// <returns>The joined columns or null.</returns>
        private string GetJoinColumns()
        {
            if (this.joinedType == null) return string.Empty;
            return ", " + this.joinedType.ExtractSelectColumnList(null, this.joinColumnList, this.DataModel.GetEntityConfig(this.joinedType));
        }

        /// <summary>
        /// Gets the top clause.
        /// </summary>
        /// <returns>The top clause or empty if one doesn't exist.</returns>
        private string GetTopClause()
        {
            if (this.Top.HasValue)
            {
                return "TOP " + this.Top.Value + " ";
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the hint clause.
        /// </summary>
        /// <returns>The hint clause properly padded with spaces.</returns>
        private string GetHintClause()
        {
            if (string.IsNullOrEmpty(this.HintClause)) return string.Empty;
            return " " + this.HintClause + " ";
        }

        /// <summary>
        /// Returns the order by clause if it exists.
        /// </summary>
        /// <returns>
        /// The order by clause or empty if one doesn't exist.
        /// </returns>
        private string GetOrderByClause()
        {
            if (this.OrderByExpression != null)
            {
                return OrderByExpression.ParseOrderByClause(this.Config);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns a grouping clause using the provided grouping properties, if any.
        /// </summary>
        /// <returns>
        /// The group by clause or empty if one doesn't exist.
        /// </returns>
        private string GetGroupByClause()
        {
            var propertyExpressions = this.GroupingProperties;
            if (propertyExpressions != null)
            {
                var len = propertyExpressions.GetLength(0);
                if (len > 0)
                {
                    var buffer = new StringBuilder(len * 20);
                    for (int i = 0; i < propertyExpressions.Length; i++)
                    {
                        var expr = propertyExpressions[i]
                            .ParsePropertyName()
                            .AsQualifiedColumn(this.Alias, this.Config, this.ConcreteType);

                        buffer.Append(expr + ", ");
                    }

                    return "group by " + buffer.ToString(0, buffer.Length - 2);
                }
            }

            return string.Empty;
        }
    }
}
