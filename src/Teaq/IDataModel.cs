using System;
using Teaq.Configuration;
using Teaq.QueryGeneration;

namespace Teaq
{
    /// <summary>
    /// Data model configuration.
    /// </summary>
    public interface IDataModel
    {
        /// <summary>
        /// Provides entry into a fluent query builder API for the specified concrete type.
        /// </summary>
        /// <typeparam name="T">The target entity type.</typeparam>
        /// <returns>
        /// Fluent builder interface.
        /// </returns>
        IQueryBuilder<T> ForEntity<T>();

        /// <summary>
        /// Gets the entity configuration the specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The entity configuration if it exists, or null.</returns>
        IEntityConfiguration GetEntityConfig(Type entityType);

        /// <summary>
        /// Gets a concrete type for the provided type if it is not a concrete class.
        /// </summary>
        /// <typeparam name="T">The type to verify.</typeparam>
        /// <returns>
        /// The concrete mapped type or the provided type if not found.
        /// </returns>
        Type GetConcreteType<T>();
    }
}
