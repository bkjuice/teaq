using System;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using Teaq.QueryGeneration;

namespace Teaq.Configuration
{
    /// <summary>
    /// Builder class implementation.
    /// </summary>
    /// <typeparam name="T">The target entity type.</typeparam>
    internal class EntityModelBuilder<T> : 
        IEntityModelBuilder<T>, 
        IEntityPropertyBuilder<T>, 
        IEntityConcurrencyPropertyBuilder<T>
    {
        /// <summary>
        /// The supported type list used as error information.
        /// </summary>
        private static readonly string supportedTypeList = PrimitiveExtensions.SupportedPrimitiveTypes();

        /// <summary>
        /// The current property.
        /// </summary>
        private string currentProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityModelBuilder{T}" /> class.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        public EntityModelBuilder(string schemaName, string tableName)
        {
            this.Config = new EntityConfiguration(schemaName, tableName);
        }

        /// <summary>
        /// Gets the entity configuration.
        /// </summary>
        public EntityConfiguration Config { get; private set; }

        /// <summary>
        /// Excludes the specified property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyExpression">The property.</param>
        /// <returns>The fluent interface to continue to specify exclusions.</returns>
        public IEntityModelBuilder<T> Exclude<TValue>(Expression<Func<T, TValue>> propertyExpression)
        {
            ValidateType(typeof(TValue));
            var name = propertyExpression.ParsePropertyName();
            this.Config.ExcludeProperty(name);
            return this;
        }

        /// <summary>
        /// Defines a property configuration for the specified entity type.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>A fluent interface to define the entity property.</returns>
        public IEntityPropertyBuilder<T> Column<TValue>(Expression<Func<T, TValue>> propertyExpression)
        {
            ValidateType(typeof(TValue));
            this.currentProperty = propertyExpression.ParsePropertyName();
            return this;
        }

        /// <summary>
        /// Specified the concurrency token property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>The current builder instance.</returns>
        /// <returns></returns>
        public IEntityConcurrencyPropertyBuilder<T> ConcurrencyToken(Expression<Func<T, byte[]>> propertyExpression)
        {
            this.currentProperty = propertyExpression.ParsePropertyName();
            this.Config.ConcurrencyProperty = this.currentProperty;
            this.Config.AddComputedColumn(this.currentProperty);
            return this;
        }

        /// <summary>
        /// Sets a value indicating the specified property is a key.
        /// </summary>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        public IEntityPropertyBuilder<T> IsComputed()
        {
            this.Config.AddComputedColumn(this.currentProperty);
            return this;
        }

        /// <summary>
        /// Sets a value indicating the specified property is a key.
        /// </summary>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        public IEntityPropertyBuilder<T> IsKey()
        {
            this.Config.AddKeyColumn(this.currentProperty);
            return this;
        }

        /// <summary>
        /// Determines whether this instance is identity.
        /// </summary>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        public IEntityPropertyBuilder<T> IsIdentity()
        {
            this.IsKey();
            this.IsComputed();
            this.Config.HasIdentity = true;
            return this;
        }

        /// <summary>
        /// Sets a column mapping for the current property.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        public IEntityPropertyBuilder<T> HasMapping(string columnName)
        {
            this.Config.AddPropertyColumnMapping(this.currentProperty, columnName);
            return this;
        }

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
        public IEntityPropertyBuilder<T> IsOfType(
            SqlDbType type,
            int? size = null,
            byte? precision = null,
            byte? scale = null)
        {
            this.Config.AddColumnDataType(
                this.currentProperty, 
                new ColumnDataType
                {
                    SqlDataType = type,
                    Size = size,
                    Precision = precision,
                    Scale = scale
                });

            return this;
        }

        /// <summary>
        /// Defines the SQL data type as NVarChar for the current property.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        public IEntityPropertyBuilder<T> IsNVarChar(int size)
        {
            this.Config.AddColumnDataType(
                this.currentProperty, 
                new ColumnDataType
                {
                    SqlDataType = SqlDbType.NVarChar,
                    Size = size
                });

            return this;
        }

        /// <summary>
        /// Defines the SQL data type as VarChar for the current property.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        /// The fluent interface to continue property definition.
        /// </returns>
        public IEntityPropertyBuilder<T> IsVarChar(int size)
        {
            this.Config.AddColumnDataType(
                this.currentProperty, 
                new ColumnDataType
                {
                    SqlDataType = SqlDbType.VarChar,
                    Size = size
                });

            return this;
        }

        /// <summary>
        /// Determines whether the specified column name has mapping.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The current builder instance.</returns>
        IEntityModelBuilder<T> IEntityConcurrencyPropertyBuilder<T>.HasMapping(string columnName)
        {
            this.HasMapping(columnName);
            return this;
        }

        /// <summary>
        /// Validates the type.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <exception cref="System.NotSupportedException">Thrown if the type is not supported.</exception>
        private static void ValidateType(Type t)
        {
            Contract.Requires(t != null);

            if (!t.TypeHandle.IsPrimitiveOrStringOrNullable())
            {
                throw new NotSupportedException(
                    string.Format("The provided type {0} is not supported. Supported types:\r\n" + supportedTypeList, t.Name));
            }
        }
    }
}