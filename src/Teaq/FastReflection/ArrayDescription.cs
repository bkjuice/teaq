using System;
using System.Diagnostics.Contracts;

namespace Teaq.FastReflection
{
    /// <summary>
    /// A lightwieght type description.
    /// </summary>
    public sealed class ArrayDescription : TypeDescription
    {
        /// <summary>
        /// The array constructor.
        /// </summary>
        private Func<int, object> arrayCtor;

        /// <summary>
        /// The array setter.
        /// </summary>
        private Action<Array, int, object> arraySetter;

        /// <summary>
        /// The array getter.
        /// </summary>
        private Func<Array, int, object> arrayGetter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayDescription" /> class without generating accessor IL.
        /// </summary>
        /// <param name="type">The type.</param>
        internal ArrayDescription(Type type)
            : base(type)
        {
            Contract.Requires(type != null);
        }

        /// <summary>
        /// Gets the array element type.
        /// </summary>
        public TypeDescription ElementType { get; internal set; }

        /// <summary>
        /// Creates the array instance.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>The initialized array instance.</returns>
        public Array CreateArrayInstance(int length)
        {
            return this.arrayCtor(length) as Array;
        }

        /// <summary>
        /// Gets the array value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="index">The index.</param>
        /// <returns>The array value.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the underlying type is not an array.</exception>
        public object GetArrayValue(Array instance, int index)
        {
            return this.arrayGetter(instance, index);
        }

        /// <summary>
        /// Sets the array value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if the underlying type is not an array.</exception>
        public void SetArrayValue(Array instance, int index, object value)
        {
            this.arraySetter(instance, index, value);
        }

        /// <summary>
        /// Initializes the array initializer, getter and setter methods.
        /// </summary>
        internal void InitializeAccessors()
        {
            Contract.Requires(this.ElementType != null);

            this.arrayCtor = this.ElementType.ReflectedType.ReflectArrayInitializer();
            this.arrayGetter = this.ElementType.ReflectedType.ReflectArrayGetValue();
            this.arraySetter = this.ElementType.ReflectedType.ReflectArraySetValue();
        }
    }
}
