using System;

namespace Teaq.Configuration
{
    /// <summary>
    /// Sponsor class to help with internalized handling of runtime configuration.
    /// </summary>
    internal static class RuntimeDataConfigurationExtensions
    {
        /// <summary>
        /// Ensures the provided type is a concrete class. This method is null-safe.
        /// </summary>
        /// <typeparam name="T">The target type to verify.</typeparam>
        /// <param name="dataModel">The data model.</param>
        /// <returns>
        /// The concrete mapped type or the provided type if not found.
        /// </returns>
        internal static Type EnsureConcreteType<T>(this IDataModel dataModel)
        {
            return dataModel?.GetConcreteType<T>() ?? typeof(T);
        }

        /// <summary>
        /// Tries to get the entity configuration. This method is null safe but may return null.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="dataModel">The data model.</param>
        /// <returns>The entity configuration or null if not defined.</returns>
        internal static IEntityConfiguration TryGetEntityConfig<T>(this IDataModel dataModel)
        {
            return dataModel?.GetEntityConfig(typeof(T));
        }
    }
}
