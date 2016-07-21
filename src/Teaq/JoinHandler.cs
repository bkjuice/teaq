using System;
using System.Data;
using System.Diagnostics.Contracts;
using Teaq.Configuration;
using Teaq.FastReflection;

namespace Teaq
{
    /// <summary>
    /// Join handler to enable splitting and mapping joined tables to complex properties.
    /// </summary>
    /// <typeparam name="T">The type of entity to be read.</typeparam>
    /// <seealso cref="Teaq.IDataHandler{T}" />
    public class JoinHandler<T> : DataHandler<T>
    {
        /// <summary>
        /// The join maps indexed by type.
        /// </summary>
        private readonly JoinMap<T> joinMap;

        /// <summary>
        /// The data model.
        /// </summary>
        private IDataModel dataModel;

        /// <summary>
        /// The current index.
        /// </summary>
        private DataFieldSetter[] currentIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinHandler{T}" /> class.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        /// <param name="expectedColumnCount">The expected column count (optional).</param>
        public JoinHandler(IDataModel dataModel = null, int? expectedColumnCount = null)
        {
            this.dataModel = dataModel;
            this.joinMap = JoinMap<T>.Create(this.dataModel, expectedColumnCount);
        }

        /// <summary>
        /// Reads a single entity from the current reader result set.
        /// </summary>
        /// <param name="reader">The active data reader.</param>
        /// <returns>
        /// A single entity from the reader.
        /// </returns>
        public override T ReadEntity(IDataReader reader)
        {
            var map = this.joinMap;
            var values = new object[reader.FieldCount];
            reader.GetValues(values);
            
            // TODO: Test if empty splits is even possible:
            // TODO: How to identify a 0 entity is the same? List<> and Id, as identity.
            var root = map.Splits[0];
            var entity = (T)root.Description.CreateInstance();
            SetValues(reader, entity, values, this.currentIndex, root.StartColumn, root.FieldCount);

            for (int i = 1; i < map.Splits.Count; i++)
            {
                var split = map.Splits[i];
                var target = split.Description.CreateInstance();
                SetValues(reader, target, values, this.currentIndex, split.StartColumn, split.FieldCount);
                map.SetChildObject(entity, target, i);
            }

            return entity;
        }

        /// <summary>
        /// Gets an existing join map or creates a map if one is not already specified for the target type.
        /// </summary>
        /// <returns>
        /// The join map.
        /// </returns>
        public JoinMap<T> GetMap()
        {
            Contract.Ensures(Contract.Result<JoinMap<T>>() != null);

            return this.joinMap;
        }

        /// <summary>
        /// Called [before reading] to materialize the underlying data.
        /// </summary>
        /// <param name="reader">The reader that will be read.</param>
        public override void OnBeforeReading(IDataReader reader)
        {
            // Loop through the splits
            // expand each type for each split
            var index = new DataFieldSetter[reader.FieldCount];
            foreach (var split in this.joinMap.Splits)
            {
                MapFieldRange(reader, index, split.Description, split.Description.ReflectedType, this.dataModel, split.StartColumn, split.FieldCount);
            }

            this.currentIndex = index;
        }

        /// <summary>
        /// Indexes a range of data reader fields.
        /// </summary>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="index">The index to fill.</param>
        /// <param name="description">The reflected type description.</param>
        /// <param name="t">The type being reflected.</param>
        /// <param name="model">The model to use (optional).</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count of fields to index using the specified type.</param>
        internal static void MapFieldRange(
            IDataReader reader,
            DataFieldSetter[] index,
            TypeDescription description,
            Type t,
            IDataModel model,
            int startIndex,
            int count)
        {
            Contract.Requires(reader != null);
            Contract.Requires(index != null);
            Contract.Requires(description != null);
            Contract.Requires(t != null);
            Contract.Requires(startIndex > -1);
            Contract.Requires(count > -1);

            var config = model?.GetEntityConfig(t);
            var max = Math.Min(reader.FieldCount, startIndex + count);
            for (int i = startIndex; i < index.Length && i < max; ++i)
            {
                var name = reader.GetName(i);
                if (config != null)
                {
                    name = config.PropertyMapping(name);
                }
                var property = description.GetProperty(name);
                if (property != null)
                {
                    var actualHandle = reader.GetFieldType(i).TypeHandle;
                    var expectedHandle = property.PropertyTypeHandle;
                    if (actualHandle.Equals(expectedHandle))
                    {
                        index[i] = new DataFieldSetter(property);
                    }
                    else
                    {
                        var converter = DataReaderExtensions.GetConverter(actualHandle, expectedHandle, property.PropertyType);
                        index[i] = new DataFieldSetter(property, converter);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the complex type property values for the given range of fields.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="values">The values.</param>
        /// <param name="index">The index.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="fieldCount">The field count.</param>
        /// <returns>
        /// The entity instance.
        /// </returns>
        internal static object SetValues(
            IDataReader reader,
            object entity,
            object[] values,
            DataFieldSetter[] index,
            int startColumn,
            int fieldCount)
        {
            var max = startColumn + fieldCount;
            for (int i = startColumn; i < max; i++)
            {
                var rowMap = index[i];
                if (rowMap.Property != null)
                {
                    var value = values[i];
                    if (!value.IsNullOrDbNull())
                    {
                        try
                        {
                            rowMap.SetValue(entity, values[i]);
                        }
                        catch (InvalidCastException e)
                        {
                            DataReaderExtensions.ThrowAssignmentException(reader.GetName(i), entity.GetType().Name, rowMap.Property.MemberName, e);
                        }
                    }
                }
            }

            return entity;
        }
    }
}
