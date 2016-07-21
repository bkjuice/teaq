using System;
using System.Linq.Expressions;

namespace Teaq.Configuration
{
    /// <summary>
    /// Fluent interface to configure an entity mapping.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface IEntityModelBuilder<T>
    {
        /// <summary>
        /// Defines a property configuration for the specified entity type.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>A fluent interface to define the entity property.</returns>
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
        /// The fluent interface to continue to specify entity configuration.
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
