using System;
using System.Diagnostics.Contracts;

namespace Teaq.FastReflection
{
    /// <summary>
    /// A lightwieght type description.
    /// </summary>
    public sealed class DictionaryDescription : TypeDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDescription" /> class without generating accessor IL.
        /// </summary>
        /// <param name="type">The type.</param>
        internal DictionaryDescription(Type type)
            : base(type)
        {
            Contract.Requires(type != null);
        }

        /// <summary>
        /// Gets the type of the dictionary key.
        /// </summary>
        public TypeDescription KeyType { get; internal set; }

        /// <summary>
        /// Gets the type of the dictionary value.
        /// </summary>
        public TypeDescription ValueType { get; internal set; }
    }
}