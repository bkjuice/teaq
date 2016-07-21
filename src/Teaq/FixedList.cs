using System;
using System.Diagnostics.Contracts;

namespace Teaq
{
    /// <summary>
    /// Helper buffer class to handle "add" semantics when the maximum number of items is known.
    /// </summary>
    /// <remarks>
    /// This class is used to reduce the number of array buffer copies that 
    /// occur when using <see cref="System.Collections.Generic.List{T}"/> and converting to arrays.
    /// This structure removes the need to have counters and array pointers in various classes.
    /// </remarks>
    /// <typeparam name="T">The type of item to use.</typeparam>
    internal class FixedList<T>
    {
        /// <summary>
        /// The buffer.
        /// </summary>
        private T[] buffer;

        /// <summary>
        /// The count of items in the buffer.
        /// </summary>
        private int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedList{T}"/> class.
        /// </summary>
        /// <param name="size">The exect size of the list.</param>
        public FixedList(int size)
        {
            Contract.Requires(size >= 0);
            this.buffer = new T[size];
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if adding the item will exceed the fixed size.
        /// </exception>
        public void Add(T item)
        {
            if (!(count < this.buffer.Length))
            {
                throw new InvalidOperationException($"You cannot exceed the size of a fixed list. Max size for this instance: { this.buffer.Length }");
            }

            buffer[count] = item;
            this.count ++;
        }

        /// <summary>
        /// Gets the count of items in the list.
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// Gets the underlying raw buffer.
        /// </summary>
        /// <returns>The underlying raw buffer.</returns>
        public T[] GetRawBuffer()
        {
            return this.buffer;
        }
    }
}
