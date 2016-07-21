using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Teaq.QueryGeneration;

namespace Teaq.Configuration
{
    /// <summary>
    /// Configuration and implementation class for data model fluent configuration.
    /// </summary>
    internal class DataModel : IDataModelBuilder, IDataModel
    {
        /// <summary>
        /// The entity configuration mapped to types.
        /// </summary>
        private Dictionary<Type, IEntityConfiguration> entityConfigurations;

        /// <summary>
        /// The abstract mappings, used to resolve abstract types to concrete types when materializing objects.
        /// </summary>
        private Dictionary<Type, Type> abstractMappings;

        /// <summary>
        /// Entry point into fluent configuration for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        /// The fluent entity configuration builder.
        /// </returns>
        public IEntityModelBuilder<TEntity> Entity<TEntity>(string schemaName = null, string tableName = null)
        {
            if (string.IsNullOrEmpty(schemaName))
            {
                schemaName = "dbo";
            }

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = typeof(TEntity).Name;
            }

            var builder = new EntityModelBuilder<TEntity>(schemaName, tableName);
            if (this.entityConfigurations == null)
            {
                this.entityConfigurations = new Dictionary<Type, IEntityConfiguration>();
            }

            this.entityConfigurations.Add(typeof(TEntity), builder.Config);
            return builder;
        }

        /// <summary>
        /// Gets the entity configuration the specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The entity configuration if it exists, or null.
        /// </returns>
        public IEntityConfiguration GetEntityConfig(Type entityType)
        {
            if (this.entityConfigurations == null)
            {
                return null;
            }

            IEntityConfiguration targetConfig = null;
            if (!this.entityConfigurations.TryGetValue(entityType, out targetConfig))
            {
                if (this.abstractMappings == null)
                {
                    return null;
                }

                if (this.abstractMappings.TryGetValue(entityType, out entityType))
                {
                    this.entityConfigurations.TryGetValue(entityType, out targetConfig);
                }
            }

            return targetConfig;
        }

        /// <summary>
        /// Maps an abstract type to a concrete type that will be used for persistence operations.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>
        /// The fluent entity configuration builder.
        /// </returns>
        public IDataModelBuilder MapAbstractType<TInterface, TEntity>() where TEntity : TInterface
        {
            if (this.abstractMappings == null)
            {
                this.abstractMappings = new Dictionary<Type, Type>();
            }

            this.abstractMappings.Add(typeof(TInterface), typeof(TEntity));
            return this;
        }

        /// <summary>
        /// Ensures the provided type is a concrete class.
        /// </summary>
        /// <typeparam name="T">The type to verify.</typeparam>
        /// <returns>
        /// The concrete mapped type or the provided type if not found.
        /// </returns>
        public Type GetConcreteType<T>()
        {
            var target = typeof(T);
            this.abstractMappings?.TryGetValue(target, out target);
            return target;
        }

        /// <summary>
        /// Provides entry into a fluent query builder API for the specified concrete type.
        /// </summary>
        /// <typeparam name="T">The target entity type.</typeparam>
        /// <returns>
        /// Fluent builder interface.
        /// </returns>
        public IQueryBuilder<T> ForEntity<T>()
        {
            Contract.Ensures(Contract.Result<IQueryBuilder<T>>() != null);

            return new QueryBuilder<T>(this);
        }       
    }
}
