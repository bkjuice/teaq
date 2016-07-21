using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Fluent interface to specify a query filter predicate.
    /// </summary>
    /// <typeparam name="T">The target entity type.</typeparam>
    public interface IQuerySelectFilterSpecification<T> : IQueryCompletion<T>
    {
        /// <summary>
        /// Adds a join clause to the query.
        /// </summary>
        /// <typeparam name="TJoined">The type of the joined table.</typeparam>
        /// <param name="joinType">Type of the join.</param>
        /// <param name="onExpression">The join on expression.</param>
        /// <param name="joinColumnList">The join column list.</param>
        /// <returns>
        /// The fluent interface to build the remaining query specification.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures", 
            Justification = ".NET Framework design requires this signature.")]
        IQuerySelectFilterSpecification<T> Join<TJoined>(
            JoinType joinType, 
            Expression<Func<T, TJoined, bool>> onExpression, 
            params string[] joinColumnList);

        /// <summary>
        /// Specifies a grouping clause using the provided properties.
        /// </summary>
        /// <param name="propertyExpressions">The property expression array.</param>
        /// <returns>
        /// The fluent interface to build the remaining query specification.
        /// </returns>
        IQueryGroupedSpecification<T> GroupBy(params Expression<Func<T, object>>[] propertyExpressions);

        /// <summary>
        /// Applies the specified filter expression to the query as a where clause.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        /// <returns>The fluent interface to build the remaining query specification.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures", 
            Justification = ".NET Framework design requires this signature.")]
        IQuerySelectOrderSpecification<T> Where(Expression<Func<T, bool>> filterExpression);

        /// <summary>
        /// Adds an order by clause to the working query.
        /// </summary>
        /// <param name="orderByExpression">The order by expression.</param>
        /// <returns>The fluent completion interface.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = ".NET Framework design requires this signature.")]
        IQueryCompletion<T> OrderBy(Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> orderByExpression);
    }
}