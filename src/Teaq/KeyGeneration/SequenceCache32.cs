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
    public sealed class SequenceCache32 : IDisposable
    {
        /// <summary>
        /// The cache size. Set as a constant but may be tuned.
        /// </summary>
        private readonly int cacheSize;

        /// <summary>
        /// The current ID max value.
        /// </summary>
        private int max;

        /// <summary>
        /// The value.
        /// </summary>
        private int value;

        /// <summary>
        /// Indictor set when a sequence retrieval thread is in flight. This is the thread locking mechanism.
        /// </summary>
        private long rangeInFlight;

        /// <summary>
        /// The sequence provider.
        /// </summary>
        private SequenceProvider<int> provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceCache32" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        public SequenceCache32(SequenceProvider<int> provider, int cacheSize = 500)
        {
            if (cacheSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(cacheSize), "The provided cache size must be 1 or larger.");
            }

            this.cacheSize = cacheSize;
            this.provider = provider;
            this.max = int.MinValue;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.provider.Dispose();
        }

        /// <summary>
        /// Gets the next identifier. This is a thread-safe method.
        /// </summary>
        /// <returns>
        /// The next available ID either from memory. If the cached range is exceeded it is reset using the implementation
        /// provided range.
        /// </returns>
        public int NextId()
        {
            // For explanation of the synchronization primitives use, 
            // see SequenceCache64.NextId().
            for (;;)
            {
                var myMax = this.max;
                Thread.MemoryBarrier();
               
                var myValue = this.value;
                while (Interlocked.CompareExchange(ref this.value, myValue + 1, myValue) != myValue)
                {
                    myValue = this.value;
                }

                myValue++;
                if (myValue < myMax)
                {
                    return myValue;
                }

                if (Interlocked.CompareExchange(ref this.rangeInFlight, 1, 0) != 0)
                {
                    var wait = new SpinWait();
                    while (Interlocked.CompareExchange(ref this.rangeInFlight, 0, 0) == 1)
                    {
                        wait.SpinOnce();
                    }

                    continue;
                }

                try
                {
                    var sequence = this.provider.Reserve(this.cacheSize);
                    this.value = sequence;
                    Thread.MemoryBarrier();

                    this.max = unchecked((int)Math.Min((long)int.MaxValue, (long)(sequence + this.cacheSize)));
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