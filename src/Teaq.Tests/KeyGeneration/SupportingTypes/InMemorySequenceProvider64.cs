using System.Threading;
using Teaq.KeyGeneration;

namespace Teaq.Tests.KeyGeneration.SupportingTypes
{
    public class InMemorySequenceProvider64 : SequenceProvider<long>
    {
        private long counter;

        private int sleep;

        public bool DisposeInvoked { get; private set; }

        public InMemorySequenceProvider64(long startValue, int sleep = 0)
        {
            this.counter = startValue;
            this.sleep = sleep;
        }

        public override long Reserve(int rangeSize)
        {
            if (this.sleep > 0)
            {
                Thread.Sleep(this.sleep);
            }

            var value = this.counter;
            this.counter += rangeSize;
            return value;
        }

        protected override void Dispose(bool disposing)
        {
            this.DisposeInvoked = true;
        }
    }
}
