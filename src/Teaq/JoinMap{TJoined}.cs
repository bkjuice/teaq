using System;
using System.Collections.Generic;
using System.Reflection;
using Teaq.Configuration;
using Teaq.FastReflection;

namespace Teaq
{
    /// <summary>
    /// Description of how to map a joined query to an entity.
    /// </summary>
    /// <typeparam name="T">The target entity type</typeparam>
    public class JoinMap<T> 
    {
        /// <summary>
        /// The split positions ordered list.
        /// </summary>
        private readonly List<Split> splits = new List<Split>();

        /// <summary>
        /// The complex property assignments ordered list.
        /// </summary>
        private readonly List<Action<T, object>> complexPropertyAssignments = new List<Action<T, object>>();

        /// <summary>
        /// The next start value which is the current split position.
        /// </summary>
        private int nextStart;

        /// <summary>
        /// The data model
        /// </summary>
        private IDataModel dataModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinMap{T}" /> class.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        private JoinMap(IDataModel dataModel)
        {
            this.dataModel = dataModel;
        }

        /// <summary>
        /// Gets the total fields.
        /// </summary>
        public int TotalFields
        {
            get
            {
                return this.nextStart;
            }
        }

        /// <summary>
        /// Gets the configured splits.
        /// </summary>
        internal List<Split> Splits
        {
            get
            {
                return this.splits;
            }
        }

        /// <summary>
        /// Adds the join mapping to split columns into a complex type.
        /// </summary>
        /// <typeparam name="TJoined">The type of the joined.</typeparam>
        /// <param name="assignmentAction">The assignment action.</param>
        /// <param name="expectedColumnCount">The expected column count for custom select lists.</param>
        public void AddJoinSplit<TJoined>(Action<T, TJoined> assignmentAction, int? expectedColumnCount = null)
        {
            var t = typeof(TJoined);
            var description = t.GetTypeDescription(MemberTypes.Property | MemberTypes.Field | MemberTypes.Constructor);

            IEntityConfiguration config = null;
            if (this.dataModel != null)
            {
                config = this.dataModel.GetEntityConfig(t);
            }

            var countOfExpectedProperties = 0;
            if (expectedColumnCount.HasValue)
            {
                this.splits.Add(new Split(this.nextStart, expectedColumnCount.Value, description));
                countOfExpectedProperties = expectedColumnCount.Value;
            }
            else
            {
                var properties = description.GetProperties();
                for (int i = 0; i < properties.GetLength(0); i++)
                {
                    if (config != null && config.IsExcluded(properties[i].MemberName))
                    {
                        continue;
                    }

                    if (properties[i].PropertyTypeHandle.IsPrimitiveOrStringOrNullable())
                    {
                        countOfExpectedProperties++;
                    }
                }
            }

            this.splits.Add(new Split (this.nextStart, countOfExpectedProperties, description));
            this.nextStart += countOfExpectedProperties;
            this.complexPropertyAssignments.Add((e, o) => assignmentAction(e, (TJoined)o));
        }

        /// <summary>
        /// Creates the specified join map.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="expectedColumnCount">The column count.</param>
        /// <returns>The join map instance.</returns>
        internal static JoinMap<T> Create(IDataModel model = null, int? expectedColumnCount = null)
        {
            var map = new JoinMap<T>(model);
            map.AddJoinSplit<T>(null, expectedColumnCount);
            return map;
        }

        /// <summary>
        /// Sets the child object.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="child">The child.</param>
        /// <param name="index">The index.</param>
        internal void SetChildObject(T root, object child, int index)
        {
            this.complexPropertyAssignments[index](root, child);
        }

        /// <summary>
        /// Helper class for split positions.
        /// </summary>
        internal struct Split
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Split"/> struct.
            /// </summary>
            /// <param name="startColumn">The start column.</param>
            /// <param name="fieldCount">The field count.</param>
            /// <param name="targetType">Type of the target.</param>
            internal Split(int startColumn, int fieldCount, TypeDescription targetType): this()
            {
                this.StartColumn = startColumn;
                this.FieldCount = fieldCount;
                this.Description = targetType;
            }

            /// <summary>
            /// Gets or sets the start column.
            /// </summary>
            /// <value>
            /// The start column.
            /// </value>
            public int StartColumn { get; private set; }

            /// <summary>
            /// Gets or sets the field count.
            /// </summary>
            /// <value>
            /// The field count.
            /// </value>
            public int FieldCount { get; private set; }

            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            public TypeDescription Description { get; private set; }
        }
    }
}
