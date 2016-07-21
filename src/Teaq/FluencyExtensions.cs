using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Teaq
{
    /// <summary>
    /// Sponsor class for basic extension methods to enable fluent syntax.
    /// </summary>
    internal static class FluencyExtensions
    {
        /// <summary>
        /// Appends the array to the specified target list.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items to be appended.</param>
        /// <param name="target">The target collection to which the items will be appended.</param>
        /// <remarks>This is a form of "AddRange" that allows use of null propagation.</remarks>
        public static void AppendTo<T>(this T[] items, List<T> target)
        {
            Contract.Requires(items != null);
            Contract.Requires(target != null);

            target.AddRange(items);
        }

        /// <summary>
        /// Appends the array to the specified target list.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items to be appended.</param>
        /// <param name="target">The target collection to which the items will be appended.</param>
        /// <remarks>This is a form of "AddRange" that allows use of null propagation.</remarks>
        public static void AppendTo<T>(this T[] items, FixedList<T> target)
        {
            Contract.Requires(items != null);
            Contract.Requires(target != null);

            for(int i = 0; i < items.Length; ++i)
            {
                target.Add(items[i]);
            }
        }

        /// <summary>
        /// Creates a dictionary from the collection of items using the provided key selector.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="items">The items that will populate the dictionary.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="comparer">The optional comparer to use when comparing keys.</param>
        /// <returns>
        /// The dictionary instance.
        /// </returns>
        /// <remarks>
        /// This is used in place of iterators for hot path scenarios.
        /// </remarks>
        public static Dictionary<TKey, TItem> ToDictionary<TItem, TKey>(this TItem[] items, Func<TItem, TKey> keySelector, IEqualityComparer<TKey> comparer = null)
        {
            Contract.Requires(items != null);
            Contract.Requires(keySelector != null);

            var result = comparer != null ? new Dictionary<TKey, TItem>(comparer) : new Dictionary<TKey, TItem>();
            for (int i = 0; i < items.GetLength(0); ++i)
            {
                var key = keySelector(items[i]);

                // The caller is responsible for a valid key, as always:
                Contract.Assume(key != null);
                result.Add(key, items[i]);
            }

            return result;
        }

        /// <summary>
        /// Converts the array of items to an equal sized array of other items by applying the provided projection.
        /// </summary>
        /// <typeparam name="T">The type of items</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="items">The items to be transformed.</param>
        /// <param name="projection">The transformation to invoke.</param>
        /// <returns>
        /// An array of transformed instances.
        /// </returns>
        /// <remarks>
        /// This is used in place of iterators for hot path scenarios.
        /// </remarks>
        public static TResult[] Transform<T, TResult>(this T[] items, Func<T, TResult> projection)
        {
            Contract.Requires(items != null);
            Contract.Requires(projection != null);
            Contract.Ensures(Contract.Result<TResult[]>().Length == items.Length);

            // x64 JIT eliminates both bounds checks...
            var result = new TResult[items.Length];
            for (int i = 0; i < items.Length; ++i)
            {
                Contract.Assert(i < result.Length);
                result[i] = projection(items[i]);
            }

            return result;
        }

        /// <summary>
        /// Wraps the provided function as an awaitable task ensuring the begin/end invoke pattern is properly used.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="method">The method to wrap as async.</param>
        /// <returns>An awaitable task.</returns>
        public static Task<T> InvokeAsAwaitable<T>(this Func<T> method)
        {
            return Task.Factory.FromAsync<T>((asyncCallBack, obj) => method.BeginInvoke(asyncCallBack, obj), method.EndInvoke, null);
        }

        /// <summary>
        /// Wraps the provided function as an awaitable task ensuring the begin/end invoke pattern is properly used.
        /// </summary>
        /// <param name="method">The action to wrap as async.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public static Task InvokeAsAwaitable(this Action method)
        {
            return Task.Factory.FromAsync((asyncCallBack, obj) => method.BeginInvoke(asyncCallBack, obj), method.EndInvoke, null);
        }

        /// <summary>
        /// Determines whether the target array [is null or empty].
        /// </summary>
        /// <typeparam name="T">The array type.</typeparam>
        /// <param name="target">The target array to test.</param>
        /// <returns>True if the array is null or has 0 elements; false otherwise.</returns>
        public static bool IsNullOrEmpty<T>(this T[] target)
        {
            if (target == null || target.GetLength(0) == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Fluent syntax helper to invoke <see cref="string.Format(string, object[])"/>.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>The formatted string value.</returns>
        public static string ToFormat(this string format, params object[] args)
        {
            Contract.Requires(format != null);
            Contract.Requires(args != null);

            return string.Format(format, args);
        }

        /// <summary>
        /// Provides null safe iteration of the target collection.
        /// </summary>
        /// <typeparam name="T">The collection item type.</typeparam>
        /// <param name="items">The items to be iterated, if any.</param>
        /// <param name="action">The action to be invoked on each item.</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items != null && action != null)
            {
                foreach (var item in items)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// Copies the specified part of the provided source array. 
        /// </summary>
        /// <typeparam name="T">The array element type.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size of the output array.</param>
        /// <returns>The subset array value.</returns>
        public static T[] CopyRange<T>(this T[] array, int offset, int size)
        {
            if (array == null)
            {
                return null;
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "The provided offset must be non-negative.");
            }

            if (offset + size >= array.GetLength(0))
            {
                size = array.GetLength(0) - offset;
            }

            if (size < 1)
            {
                return null;
            }

            var buffer = new T[size];
            Array.Copy(array, offset, buffer, 0, size);
            return buffer;
        }

        /// <summary>
        /// Expands the specified array. The array will not be resized if the new size is smaller than the current size.
        /// </summary>
        /// <typeparam name="T">The array element type.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="newSize">The new size.</param>
        /// <returns>The array if the new size is smaller or equal, or the resized array.</returns>
        public static T[] Expand<T>(this T[] array, int newSize)
        {
            if (array == null)
            {
                return null;
            }

            if (newSize <= array.GetLength(0))
            {
                return array;
            }

            Array.Resize(ref array, newSize);
            return array;
        }

        /// <summary>
        /// Combines the specified arrays.
        /// </summary>
        /// <typeparam name="T">The type of array item.</typeparam>
        /// <param name="array1">The array1.</param>
        /// <param name="array2">The array2.</param>
        /// <returns>The union of the 2 arrays in the provided order.</returns>
        /// <remarks>
        /// This method is null tolerant.
        /// </remarks>
        public static T[] Combine<T>(this T[] array1, T[] array2)
        {
            if (array2 == null)
            {
                return array1;
            }

            if (array1 == null)
            {
                return array2;
            }

            var array1Len = array1.GetLength(0);
            var array2Len = array2.GetLength(0);
            var newSize = array1Len + array2Len;
            var newArray = new T[newSize];
            Array.Copy(array1, newArray, array1Len);
            Array.Copy(array2, 0, newArray, array1Len, array2Len);
            return newArray;
        }
    }
}