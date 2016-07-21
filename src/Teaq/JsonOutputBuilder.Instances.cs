using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace Teaq
{
    /// <summary>
    /// Output builder used to write JSON from raw data in a streaming manner.
    /// </summary>
    public abstract partial class JsonOutputBuilder
    {
        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <returns>The buffering builder to use.</returns>
        public static JsonOutputBuilder GetBuilder(int initialCapacity = 512)
        {
            Contract.Ensures(Contract.Result<JsonOutputBuilder>() != null);

            return new BufferedJsonWriter(initialCapacity).OpenRoot();
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <param name="rootName">Name of the root.</param>
        /// <param name="isArray">if set to <c>true</c> [is array].</param>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <returns>
        /// The buffering builder to use.
        /// </returns>
        public static JsonOutputBuilder GetBuilder(string rootName, bool isArray, int initialCapacity = 512)
        {
            Contract.Requires(!string.IsNullOrEmpty(rootName));
            Contract.Ensures(Contract.Result<JsonOutputBuilder>() != null);

            return InitInstance(GetBuilder(initialCapacity), rootName, isArray);
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <param name="output">The output writer to use.</param>
        /// <returns>The JSON output writer that will write to the backing stream.</returns>
        public static JsonOutputBuilder GetBuilder(TextWriter output)
        {
            Contract.Requires(output != null);
            Contract.Ensures(Contract.Result<JsonOutputBuilder>() != null);

            return new StreamingJsonWriter(output).OpenRoot();
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <param name="rootName">Name of the root.</param>
        /// <param name="isArray">if set to <c>true</c> [is array].</param>
        /// <param name="output">The output.</param>
        /// <returns>The JSON output writer that will write to the backing stream.</returns>
        public static JsonOutputBuilder GetBuilder(string rootName, bool isArray, TextWriter output)
        {
            Contract.Requires(!string.IsNullOrEmpty(rootName));
            Contract.Ensures(Contract.Result<JsonOutputBuilder>() != null);

            return InitInstance(GetBuilder(output), rootName, isArray);
        }

        /// <summary>
        /// Initializes the instance.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="rootName">Name of the root.</param>
        /// <param name="isArray">if set to <c>true</c> the instance is initialized as as array.</param>
        /// <returns>The initialized instance.</returns>
        private static JsonOutputBuilder InitInstance(JsonOutputBuilder builder, string rootName, bool isArray)
        {
            if (isArray)
            {
                builder.StartArray(rootName);
            }
            else
            {
                builder.StartObject(rootName);
            }

            return builder;
        }

        private class StreamingJsonWriter : JsonOutputBuilder
        {
            private readonly TextWriter output;

            public StreamingJsonWriter(TextWriter output)
            {
                Contract.Requires(output != null);
                this.output = output;
            }

            protected override void Flush(char[] buffer, int offset, int count)
            {
                this.output.Write(buffer, offset, count);
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.output != null);
            }
        }

        private class BufferedJsonWriter : JsonOutputBuilder
        {
            private readonly StringBuilder output;

            public BufferedJsonWriter(int initialCapacity) : base()
            {
                this.output = new StringBuilder(Math.Max(initialCapacity, 8));
            }

            public override string ToString()
            {
                return this.output.ToString();
            }

            protected override void Flush(char[] buffer, int offset, int count)
            {
                this.output.Append(buffer, offset, count);
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.output != null);
            }
        }
    }
}
