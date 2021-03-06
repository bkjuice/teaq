﻿using System;
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
        /// Gets a JSON output builder initialized as an empty object with the provided string builder as a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use when building the JSON output.</param>
        /// <returns>
        /// The buffering builder to use.
        /// </returns>
        public static JsonOutputBuilder GetBuilder(StringBuilder buffer)
        {
            Contract.Requires<ArgumentNullException>(buffer != null);
            Contract.Ensures(Contract.Result<JsonOutputBuilder>() != null);

            return new BufferedJsonWriter(buffer).OpenRoot();
        }

        /// <summary>
        /// Gets a JSON output builder initialized as a named object or array with the provided string builder as a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use when building the JSON output.</param>
        /// <param name="rootName">Name of the root object.</param>
        /// <param name="isArray">if set to <c>true</c> [is array].</param>
        /// <returns>
        /// The buffering builder to use.
        /// </returns>
        public static JsonOutputBuilder GetBuilder(StringBuilder buffer, string rootName, bool isArray)
        {
            Contract.Requires<ArgumentNullException>(buffer != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(rootName));
            Contract.Ensures(Contract.Result<JsonOutputBuilder>() != null);

            return InitInstance(GetBuilder(buffer), rootName, isArray);
        }

        /// <summary>
        /// Gets a JSON output builder initialized as an object with the provided text writer as an output stream.
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
        /// Gets a JSON output builder initialized as a named object or array with the provided text writer as an output stream.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="rootName">Name of the root.</param>
        /// <param name="isArray">if set to <c>true</c> [is array].</param>
        /// <returns>
        /// The JSON output writer that will write to the backing stream.
        /// </returns>
        public static JsonOutputBuilder GetBuilder(TextWriter output, string rootName, bool isArray)
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

            public BufferedJsonWriter(StringBuilder buffer) : base()
            {
                this.output = buffer;
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
