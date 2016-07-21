using System;

namespace Teaq.KeyGeneration
{
    /// <summary>
    /// Provides reservations for Sequence ranges.
    /// </summary>
    /// <typeparam name="T">The sequence type.</typeparam>
    public abstract class SequenceProvider<T> : IDisposable where T : struct
    {
        /// <summary>
        /// Reserves the next available range.
        /// </summary>
        /// <param name="rangeSize">Size of the range.</param>
        /// <returns>The first value in the reserved range.</returns>
        /// <remarks>
        /// Implementations must provide ever increasing values that do not overlap.
        /// </remarks>
        public abstract T Reserve(int rangeSize);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}