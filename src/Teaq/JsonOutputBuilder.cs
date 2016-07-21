using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Teaq
{
    /// <summary>
    /// Output builder used to write JSON from raw data in a streaming manner.
    /// </summary>
    public abstract partial class JsonOutputBuilder
    {
        /// <summary>
        /// The open scopes to be closed on completion or by the caller.
        /// </summary>
        private readonly Stack<char> openScopes = new Stack<char>();

        /// <summary>
        /// Value indicating this instance is immutable.
        /// </summary>
        private bool immutable;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonOutputBuilder"/> class.
        /// </summary>
        internal JsonOutputBuilder()
        {
        }

        /// <summary>
        /// Starts the array.
        /// </summary>
        public void StartArray()
        {
            this.ThrowIfImmutable();
            this.OpenScope('[', ']');
        }

        /// <summary>
        /// Starts the object.
        /// </summary>
        /// <param name="name">The name of the objects.</param>
        public void StartArray(string name)
        {
            Contract.Requires(name != null);

            this.ThrowIfImmutable();
            this.WriteName(name);
            this.StartArray();
        }

        /// <summary>
        /// Starts the object.
        /// </summary>
        public void StartObject()
        {
            this.ThrowIfImmutable();
            this.OpenScope('{', '}');
        }

        /// <summary>
        /// Starts the object.
        /// </summary>
        /// <param name="name">The name of the objects.</param>
        public void StartObject(string name)
        {
            Contract.Requires(name != null);

            this.ThrowIfImmutable();
            this.WriteName(name);
            this.StartObject();
        }

        /// <summary>
        /// Closes the scope.
        /// </summary>
        public void CloseScope()
        {
            this.ThrowIfImmutable();
            if (this.openScopes.Count > 0)
            {
                this.TrimCommaDelimiter();
                var c = this.openScopes.Pop();
                this.Write(c);
                this.Write(",");
            }
        }

        /// <summary>
        /// Writes the object member.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <param name="value">The member value.</param>
        /// <param name="valueKind">The kind of value.</param>
        public void WriteObjectMember(string name, string value, JsonOutputValueKind valueKind)
        {
            Contract.Requires(name != null);
            Contract.Requires(value != null);

            this.ThrowIfImmutable();
            this.WriteName(name);
            this.WriteValue(valueKind, value);
        }

        /// <summary>
        /// Writes the array member.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueKind">Kind of the value.</param>
        public void WriteArrayMember(string value, JsonOutputValueKind valueKind)
        {
            Contract.Requires(value != null);

            this.ThrowIfImmutable();
            this.WriteValue(valueKind, value);
        }

        /// <summary>
        /// Closes all open scopes in this instance.
        /// </summary>
        public void Close()
        {
            this.ThrowIfImmutable();
            while (this.openScopes.Count > 0)
            {
                this.CloseScope();
            }

            this.TrimCommaDelimiter();
            this.Flush();
            this.immutable = true;
        }

        /// <summary>
        /// Writes the name.
        /// </summary>
        /// <param name="name">The name th write.</param>
        private void WriteName(string name)
        {
            this.Write('"');
            this.Write(name);
            this.Write("\":");
        }

        /// <summary>
        /// Writes the token.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="kind">if set to <c>true</c> [quoted].</param>
        private void WriteValue(JsonOutputValueKind kind, string value)
        {
            if (kind == JsonOutputValueKind.StringValue)
            {
                this.Write('"');
                this.Write(value);
                this.Write('"');
                this.Write(',');
                return;
            }

            if (kind == JsonOutputValueKind.NullValue || value == null)
            {
                value = "null";
            }

            this.Write(value);
            this.Write(",");
        }

        /// <summary>
        /// Opens the document root node.
        /// </summary>
        private JsonOutputBuilder OpenRoot()
        {
            this.Write('{');
            this.openScopes.Push('}');
            return this;
        }

        /// <summary>
        /// Opens the scope.
        /// </summary>
        /// <param name="startToken">The start token.</param>
        /// <param name="endToken">The end token.</param>
        private void OpenScope(char startToken, char endToken)
        {
            this.Write(startToken);
            this.openScopes.Push(endToken);
        }

        /// <summary>
        /// Throws if this instance is immutable.
        /// </summary>
        private void ThrowIfImmutable()
        {
            if (this.immutable)
            {
                throw new InvalidOperationException("The JSON output builder instance is immutable and cannot be modified once completed.");
            }
        }
    }
}
