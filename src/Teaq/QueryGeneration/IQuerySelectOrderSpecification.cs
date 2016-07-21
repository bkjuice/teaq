using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Teaq.QueryGeneration
{   
    /// <summary>
    /// Fluent interface to specify ordering or complete the query.
    /// </summary>
    /// <typeparam name="T">The target entity type.</typeparam>
    public interface IQuerySelectOrderSpecification<T> : IQueryCompletion<T>
    {
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