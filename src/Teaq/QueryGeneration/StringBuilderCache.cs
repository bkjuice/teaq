using System;
using System.Diagnostics.Contracts;
using System.Text;

namespace Teaq.QueryGeneration
{
    /// <summary>
    /// Cache to minimize thrashing of large object heap for large query statements.
    /// </summary>
    internal static class StringBuilderCache
    {
        /// <summary>
        /// The large object heap threshold, estimated slightly under the 85KB limit to account for StringBuilder instance overhead.
        /// </summary>
        private const int LargeObjectHeapThreshold = 85000 - 24;

        /// <summary>
        /// The instance, per thread, to use. If the instance is acquired on a different thread than it is released,
        /// the cache is partialy defeated but not broken. Internal use only.
        /// </summary>
        [ThreadStatic]
        private static StringBuilder instance;

        /// <summary>
        /// Gets the <see cref="StringBuilder"/> instance.
        /// </summary>
        /// <param name="capacity">The required capacity.</param>
        /// <returns>The instance to use.</returns>
        public static StringBuilder GetInstance(int capacity)
        {
            Contract.Requires(capacity >= 0);

            if (instance == null)
            {
                return new StringBuilder(capacity);
            }

            if (capacity < LargeObjectHeapThreshold)
            {
                return new StringBuilder(capacity);
            }

            var instanceToUse = instance;
            instance = null;

            instanceToUse.Clear();

            // This could still thrash the LOH if the internal array must be re-allocated, but this is unavoidable.
            // Eventually, the buffer will be sized to handle most capacities. Possibly round up to nearest power of 2
            // if performance analysis shows this could help with batch workloads.
            instanceToUse.EnsureCapacity(capacity);
            return instanceToUse;
        }

        /// <summary>
        /// Returns the instance to the cache, which is kept only if it exceeds the LOH minimum size.
        /// </summary>
        /// <param name="instanceToReturn">The instance to return.</param>
        public static void ReturnInstance(StringBuilder instanceToReturn)
        {
            Contract.Requires(instanceToReturn != null);

            if (instanceToReturn.Capacity < LargeObjectHeapThreshold)
            {
                return;
            }

            instance = instanceToReturn;
        }
    }
}
