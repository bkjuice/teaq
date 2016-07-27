using System;
using System.Data;
using System.Linq.Expressions;

namespace Teaq.Configuration
{
    /// <summary>
    /// Fluent interface to configure an entity property mapping.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface IEntityPropertyBuilder<T>
    {
        /// <summary>
        /// Sets a value indicating the specified property is a key.
        /// </summary>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        IEntityPropertyBuilder<T> IsKey();

        /// <summary>
        /// Specifies the property is an identity column.
        /// </summary>
        /// <returns>
        /// The fluent interface to continue to specify property configuration.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures", 
            Justification = ".NET Framework design requires this signature to parse an expression tree.")]
        IEntityPropertyBuilder<T> IsIdentity();

        /// <summary>
        /// Sets a column mapping for the current property.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        IEntityPropertyBuilder<T> HasMapping(string columnName);

        /// <summary>
        /// Defines the SQL data type for the current property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="size">The size.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        IEntityPropertyBuilder<T> IsOfType(SqlDbType type, int? size = null, byte? precision = null, byte? scale = null);

        /// <summary>
        /// Defines the SQL data type as NVarChar for the current property.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        IEntityPropertyBuilder<T> IsNVarChar(int size);

        /// <summary>
        /// Defines the SQL data type as VarChar for the current property.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        IEntityPropertyBuilder<T> IsVarChar(int size);

        /// <summary>
        /// Defines the current column as computed.
        /// </summary>
        /// <returns></returns>
        IEntityPropertyBuilder<T> IsComputed();

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
        /// Specifies the concurrency token property.
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
