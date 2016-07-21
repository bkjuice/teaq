using System;
using System.Linq.Expressions;

namespace Teaq.Configuration
{
    /// <summary>
    /// Fluent interface to configure a concurrency token entity property mapping.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface IEntityConcurrencyPropertyBuilder<T>
    {
        /// <summary>
        /// Sets a column mapping for the current property.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        IEntityModelBuilder<T> HasMapping(string columnName);

        /// <summary>
        /// Defines a property configuration for the specified entity type.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>
        /// A fluent interface to define the entity property.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = ".NET Framework design requires this signature to parse an expression tree.")]
        IEntityPropertyBuilder<T> Column<TValue>(Expression<Func<T, TValue>> propertyExpression);

        /// <summary>
        /// Excludes the specified property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyExpression">The property.</param>
        /// <returns>
        /// The fluent interface to continue to specify exclusions.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = ".NET Framework design requires this signature to parse an expression tree.")]
        IEntityModelBuilder<T> Exclude<TValue>(Expression<Func<T, TValue>> propertyExpression);

        /// <summary>
        /// Specified the concurrency token property.
        /// </summary>
        /// <param name="propertyExpression">The property.</param>
        /// <returns>
        /// The fluent interface to continue to specify property configuration.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures", 
            Justification = ".NET Framework design requires this signature to parse an expression tree.")]
        IEntityConcurrencyPropertyBuilder<T> ConcurrencyToken(Expression<Func<T, byte[]>> propertyExpression);
    }
}
