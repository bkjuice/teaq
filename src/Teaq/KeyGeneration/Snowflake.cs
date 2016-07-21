using System;
using System.Threading;

namespace Teaq.KeyGeneration
{
    /// <summary>
    /// Implementation of the Twitter 'Snowflake' algorithm which is a 64 bit 'shared nothing' generated ID. This class is thread-safe
    /// and is designed to be used as a singleton.
    /// </summary>
    /// <remarks>Twitter snowflake is described here: https://blog.twitter.com/2010/announcing-snowflake. 
    /// The 64 bit ID is composed of:
    /// <list type="bullet">
    /// <item><description>42 bits of time in milliseconds with a custom epoch to provide 69 years before keys roll over.</description></item>
    /// <item><description>22 bits that are dedicated to preventing key collisions.</description></item>
    /// </list>
    /// For this implementation, the high order 42 bits are driven by time measured in milliseconds from January 1st 1970. A fixed 'epoch' 
    /// value is subtracted from this tick count to provide additional time before keys roll beyond the bounds of a 64 bits (roughly 70 years).
    /// The 22 bits dedicated to avoiding key collisions are divided between a thread specific sequence counter and a global worker ID. The 
    /// collision avoidance algorithim may be configured using the the <see cref="SnowflakeWorkerIdRange"/> enumerated values.
    /// </remarks>
    public sealed class Snowflake : IDisposable
    {
        /// <summary>
        /// The timestamp mask. Use this value to remove the timestamp from the ID, and then shift the worker id bits.
        /// </summary>
        private const long TimestampMask = 0x3FFFFF;

        /// <summary>
        /// The fixed epoch.
        /// </summary>
        private const long Epoch = 1351606710465L;

        /// <summary>
        /// The timestamp left shift, which is always 22 bits.
        /// </summary>
        private const int TimestampShift = 22;

        /// <summary>
        /// The worker identifier.
        /// </summary>
        private readonly long workerId;

        /// <summary>
        /// The number of bits allocated for a worker id in the generated identifier. 8 bits indicates values from 0 to 255, 
        /// 10 bits indicates a value from 0-1023, 12 bits indicates 0-4095. Reduce this value to improve performance but reducing overall
        /// number of machines allowed to generate keys.
        /// </summary>
        private readonly int workerIdBits;

        /// <summary>
        /// Gets the worker id shift
        /// </summary>
        private readonly int workerIdShift;

        /// <summary>
        /// Gets the sequence mask
        /// </summary>
        private readonly int sequenceMask;

        /// <summary>
        /// The cached generator that manages thread synchronization.
        /// </summary>
        private readonly SequenceCache64 cachedGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowflake" /> class.
        /// </summary>
        /// <param name="workerId">The worker identifier.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability", 
            "CA2000:Dispose objects before losing scope",
            Justification = "Class scope. Handled via IDisposable implementation.")]
        public Snowflake(byte workerId)
        {
            this.workerId = workerId;
            this.workerIdBits = 8;
            var sequenceBits = 22 - this.workerIdBits;
            this.workerIdShift = sequenceBits;
            this.sequenceMask = (int)(-1L ^ (-1L << sequenceBits));
            this.cachedGenerator = new SequenceCache64(new Generator(), this.sequenceMask);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Snowflake" /> class.
        /// </summary>
        /// <param name="workerId">The worker identifier.</param>
        /// <param name="range">The worker identifier range.</param>
        /// <exception cref="SnowflakeConfigurationException">
        /// Thrown if the provided range value is not set or the provided worker ID exceeds the maximum allowed value.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Class scope. Handled via IDisposable implementation.")]
        public Snowflake(short workerId, SnowflakeWorkerIdRange range)
        {
            var workerBits = (int)range;
            if (workerBits == 0)
            {
                throw new SnowflakeConfigurationException(
                       "The provided range value is not set. Choose a valid range value other than {0}".ToFormat(range.ToString()));
            }

            var max = (1L << (int)range) - 1;
            if (workerId > max)
            {
                throw new SnowflakeConfigurationException(
                       "The provided worker ID {0} exceeds the maximum alllowed value of {1}.".ToFormat(workerId, max));
            }

            this.workerId = workerId;
            this.workerIdBits = workerBits;
            var sequenceBits = 22 - this.workerIdBits;
            this.workerIdShift = sequenceBits;
            this.sequenceMask = (int)(-1L ^ (-1L << sequenceBits));
            this.cachedGenerator = new SequenceCache64(new Generator(), this.sequenceMask);
        }

        /// <summary>
        /// Gets the next snowflake id.
        /// </summary>
        /// <returns>
        /// The next snowflake ID.
        /// </returns>
        public long NextId()
        {
            var timestamp = this.cachedGenerator.NextId() - Epoch;
            var sequence = timestamp & this.sequenceMask;
            timestamp = timestamp & ~this.sequenceMask;

            return
                (timestamp << TimestampShift) |
                (this.workerId << this.workerIdShift) |
                sequence;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.cachedGenerator != null)
            {
                this.cachedGenerator.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The sequence generator used to provide the underlying sequence count based on time.
        /// </summary>
        private class Generator : SequenceProvider<long>
        {
            /// <summary>
            /// 1 January 1970. Used to calculate timestamp (in milliseconds)
            /// </summary>
            private const long Jan1Of1970 = 621355968000000000;

            /// <summary>
            /// The maximum ahead tolerance for the counter to run ahead of the system clock before being blocked.
            /// </summary>
            private const long MaxAheadTolerance = 128;

            /// <summary>
            /// The minimum ahead tolerance for the counter to run ahead of the system clock before being blocked.
            /// </summary>
            private const long MinAheadTolerance = 32;

            /// <summary>
            /// The last generated timestamp used to test for collisions. This is static to ensure
            /// that in any application domain, the provided sequence always increases. Thread safety
            /// is ensured by the wrapping <see cref="SequenceCache64"/> class.
            /// </summary>
            private static long lastTimestamp;

            /// <summary>
            /// The number of ranges the counter has progressed before clock sync.
            /// </summary>
            private long aheadRanges;

            /// <summary>
            /// The ahead tolerance.
            /// </summary>
            private long aheadTolerance;

            /// <summary>
            /// Initializes a new instance of the <see cref="Generator"/> class.
            /// </summary>
            public Generator()
            {
                this.aheadTolerance = MaxAheadTolerance;
            }

            /// <summary>
            /// Reserves the specified range size.
            /// </summary>
            /// <param name="rangeSize">Size of the range.</param>
            /// <returns>The minimum value of the next range.</returns>
            /// <remarks>
            /// Thread safety is managed by the <see cref="SequenceCache64"/> class that wraps this instance.
            /// </remarks>
            public override long Reserve(int rangeSize)
            {
                var timestamp = (long)(DateTime.UtcNow.Ticks - Jan1Of1970) / TimeSpan.TicksPerMillisecond;
                var next = lastTimestamp;
                if (timestamp > next)
                {
                    next = timestamp + rangeSize;
                    this.aheadTolerance = MaxAheadTolerance;
                }
                else
                {
                    // This is an intentional performance bottleneck for bursts of high volume.
                    // The range size represents the counter that is used in between clock milliseconds.
                    // A tolerance is managed so the throttling effect is not at every range boundary.
                    // The tradeoff is raw performance vs. correctness if a node serving IDs fails.
                    // The number of "ahead" ranges represents the number of milliseconds the node must 
                    // take to recover, in order to not generate duplicate IDs. 
                    // Assuming recovery will always take at least 128ms.
                    if (this.aheadRanges >= this.aheadTolerance)
                    {
                        Thread.Sleep(1);
                        this.aheadTolerance = Math.Max(MinAheadTolerance, this.aheadTolerance / 2);
                        this.aheadRanges -= 1;
                        if (this.aheadRanges < MinAheadTolerance)
                        {
                            this.aheadRanges = 0;
                        }
                    }
                    else
                    {
                        this.aheadRanges++;
                    }

                    next += rangeSize;
                }

                lastTimestamp = next;
                return next;
            }
        }
    }
}