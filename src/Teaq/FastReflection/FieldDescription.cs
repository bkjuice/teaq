using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Teaq.FastReflection
{
    /// <summary>
    /// A lightwieght field descriptor.
    /// </summary>
    public sealed class FieldDescription : MemberDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDescription" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        internal FieldDescription(FieldInfo info)
            : base(info, info.FieldType)
        {
            Contract.Requires(info != null);

            this.FieldType = info.FieldType;
            this.FieldTypeHandle = info.FieldType.TypeHandle;
            this.GetValue = info.ReflectGetter();
            this.SetValue = info.ReflectSetter();

            this.MemberType = info.FieldType.GetTypeDescription();
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public Type FieldType { get; private set; }

        /// <summary>
        /// Gets the field type handle.
        /// </summary>
        public RuntimeTypeHandle FieldTypeHandle { get; private set; }

        /// <summary>
        /// Gets the get value method.
        /// </summary>
        /// <value>
        /// The get value method.
        /// </value>
        public Func<object, object> GetValue { get; private set; }

        /// <summary>
        /// Gets the set value method.
        /// </summary>
        /// <value>
        /// The set value method.
        /// </value>
        public Action<object, object> SetValue { get; private set; }
    }
}
