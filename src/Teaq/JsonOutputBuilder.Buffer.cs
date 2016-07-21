using System;
using System.Diagnostics.Contracts;

namespace Teaq
{
    /// <summary>
    /// Output builder used to write JSON from raw data in a streaming manner.
    /// </summary>
    public abstract partial class JsonOutputBuilder
    {
        /// <summary>
        /// The buffer size
        /// </summary>
        private const int BufferSize = 4096;

        /// <summary>
        /// The buffer to use.
        /// </summary>
        private readonly char[] buffer = new char[BufferSize];

        /// <summary>
        /// The 0 based index position in the buffer.
        /// </summary>
        private int position;

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        protected abstract void Flush(char[] buffer, int offset, int count);

        /// <summary>
        /// Trims the comma delimiter at the end of the buffer, if one exists.
        /// </summary>
        private void TrimCommaDelimiter()
        {
            if (this.buffer[this.position - 1] == ',')
            {
                this.position--;
            }
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        private void Write(string value)
        {
            Contract.Ensures(this.position >= 0);

            var p = this.position;
            var b = buffer;
            var count = Math.Min(value.Length, BufferSize - p);
            var i = 0;
            while (value.Length - i > 0)
            {
                if (count == 0)
                {
                    this.Flush(b, 0, BufferSize);
                    p = 0;
                    count = Math.Min(value.Length - i, BufferSize - p);
                }

                Contract.Assume(p >= 0);
                value.CopyTo(i, b, p, count);
                i += count;
                p += count;
                count = Math.Min(value.Length - i, BufferSize - p);
            };

            this.position = p;
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        private void Write(char value)
        {
            var p = this.position;
            var b = buffer;
            if (BufferSize - p - 1 == 0)
            {
                this.Flush(b, 0, BufferSize);
                p = 0;
            }

            b[p] = value;
            this.position = p + 1;
        }

        /// <summary>
        /// Flushes this instance to the writer implementation.
        /// </summary>
        private void Flush()
        {
            var p = this.position;
            if (p > 0)
            {
                this.Flush(this.buffer, 0, p);
            }
        }
    }
}
