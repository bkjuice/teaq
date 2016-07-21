using System;
using System.Linq.Expressions;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Fluent query builder entry interface.
    /// </summary>
    /// <typeparam name="T">The target entity type.</typeparam>
    public interface IQueryBuilder<T>
    {
        /// <summary>
        /// Builds the select query.
        /// </summary>
        /// <param name="tableAlias">Optional table alias.</param>
        /// <param name="top">Optional top.</param>
        /// <param name="columnList">The column list.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        IQuerySelectSpecification<T> BuildSelectAs(string tableAlias, int top, params string[] columnList);

        /// <summary>
        /// Builds the select query.
        /// </summary>
        /// <param name="tableAlias">Optional table alias.</param>
        /// <param name="columnList">The column list.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        IQuerySelectSpecification<T> BuildSelectAs(string tableAlias, params string[] columnList);

        /// <summary>
        /// Builds the select query.
        /// </summary>
        /// <param name="top">Optional top.</param>
        /// <param name="columnList">The column list.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        IQuerySelectSpecification<T> BuildSelect(int top, params string[] columnList);

        /// <summary>
        /// Builds the select query.
        /// </summary>
        /// <param name="columnList">The column list.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        IQuerySelectSpecification<T> BuildSelect(params string[] columnList);

        /// <summary>
        /// Builds an update query.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = ".NET Framework design requires this signature.")]
        IQueryCompletion<T> BuildUpdate(T target, Expression<Func<T, bool>> filter = null);

        /// <summary>
        /// Builds the insert.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        IQueryCompletion<T> BuildInsert(T target);

        /// <summary>
        /// Builds the delete query.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// The fluent interface for query completion.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = ".NET Framework design requires this signature.")]
        IQueryCompletion<T> BuildDelete(Expression<Func<T, bool>> filter = null);
    }
}
