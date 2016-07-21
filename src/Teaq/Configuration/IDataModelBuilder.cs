using System;

namespace Teaq.Configuration
{
    /// <summary>
    /// Builder interface to set up a data model representation of a set of entities.
    /// </summary>
    public interface IDataModelBuilder
    {
        /// <summary>
        /// Entry point into fluent configuration for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        /// The fluent entity configuration builder.
        /// </returns>
        IEntityModelBuilder<TEntity> Entity<TEntity>(string schemaName = null, string tableName = null);

        /// <summary>
        /// Maps an abstract type to a concrete type that will be used for persistence operations.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>
        /// The fluent entity configuration builder.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "Fluent interface to ensure compiler type safety in usage. Arguments do not apply here.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = ".NET Framework design requires this signature to parse an expression tree.")]
        IDataModelBuilder MapAbstractType<TInterface, TEntity>() where TEntity : TInterface;
    }
}
