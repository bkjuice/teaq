using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Fluent interface to configure query table specifications.
    /// </summary>
    /// <typeparam name="T">The target entity type.</typeparam>
    public interface IQueryGroupedSpecification<T> : IQueryCompletion<T>
    {
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