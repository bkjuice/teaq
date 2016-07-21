using System;
using System.Threading;

namespace Teaq.KeyGeneration
{
    /// <summary>
    /// Cache handler for a block of reserved SEQUENCE values. 
    /// </summary>
    /// <remarks>
    /// This class is thread-safe and will synchronize use of the given provider (which does not have to be thread-safe).
    /// The thread safety for this class requires that a SEQUENCE function not cycle and always increase.
    /// </remarks>
    public sealed class SequenceCache64 : IDisposable
    {
        /// <summary>
        /// The cache size. Set as a constant but may be tuned.
        /// </summary>
        private readonly int cacheSize;

        /// <summary>
        /// The current ID max value.
        /// </summary>
        private long max;

        /// <summary>
        /// The value.
        /// </summary>
        private long value;

        /// <summary>
        /// Indictor set when a sequence retrieval thread is in flight. This is the thread locking mechanism.
        /// </summary>
        private long rangeInFlight;

        /// <summary>
        /// The sequence provider.
        /// </summary>
        private SequenceProvider<long> provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceCache64" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        public SequenceCache64(SequenceProvider<long> provider, int cacheSize = 1000)
        {
            if (cacheSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(cacheSize), "The provided cache size must be 1 or larger.");
            }

            this.cacheSize = cacheSize;
            this.provider = provider;
            this.max = long.MinValue;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.provider.Dispose();
        }

        /// <summary>
        /// Gets the next identifier. This is a threadsafe method.
        /// </summary>
        /// <returns>
        /// The next available ID either from memory. If the cached range is exceeded it is reset using the implementation 
        /// provided range.
        /// </returns>
        public long NextId()
        {
            for (;;)
            {
                // Must get the max value reliably before getting the counter value.
                // CLR assignments are atomic but we must disallow instruction re-ordering:
                var myMax = this.max;
                Thread.MemoryBarrier();

                var myValue = this.value;
                while (Interlocked.CompareExchange(ref this.value, myValue + 1, myValue) != myValue)
                {
                    myValue = this.value;
                }

                // The current thread "owns" the +1 value, and the value was acquired after the max value was interrogated.
                // If this thread was pre-empted and another thread changed the value and max, the following statement is 
                // still valid for the local values, because the max value and counter value can only ever increase:
                myValue++;
                if (myValue < myMax)
                {
                    return myValue;
                }

                // Threads now compete to check the underlying sequence implementation for the next lower bound.
                // Once max is exceeded, threads will "pile up" here. Only one will go to the database:
                if (Interlocked.CompareExchange(ref this.rangeInFlight, 1, 0) != 0)
                {
                    var wait = new SpinWait();
                    while (Interlocked.CompareExchange(ref this.rangeInFlight, 0, 0) == 1)
                    {
                        wait.SpinOnce();
                    }

                    // Another thread handled the next range; re-enter the loop to acquire another value:
                    continue;
                }

                try
                {
                    long sequence = this.provider.Reserve(this.cacheSize);
                    
                    // Assignment of the low range value BEFORE max is important for correctness.
                    // It is the unchanged max value that will ensure other threads retry
                    // until this operation is complete.
                    this.value = sequence;
                    Thread.MemoryBarrier();

                    // At this point, other threads are spinning because the max has not been raised,
                    // and the command is still in flight. If a thread enters the loop just after this 
                    // assignment, it will be unblocked and take value + 1:
                    this.max = sequence + this.cacheSize;
                    Thread.MemoryBarrier();
                    return sequence;
                }
                finally
                {
                    this.rangeInFlight = 0;
                }
            }
        }
    }
}
