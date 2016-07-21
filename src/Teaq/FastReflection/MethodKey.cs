using System;

namespace Teaq.FastReflection
{
    /// <summary>
    /// Key structure to identify a method by its name, argument types and argument order.
    /// </summary>
    internal struct MethodKey : IEquatable<MethodKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodKey" /> struct for a method with no arguments.
        /// </summary>
        /// <param name="name">The method name.</param>
        public MethodKey(string name)
            : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodKey" /> struct for a method with no arguments.
        /// </summary>
        /// <param name="arguments">The argument list.</param>
        public MethodKey(RuntimeTypeHandle[] arguments)
            : this(string.Empty, arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodKey"/> struct.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <param name="arguments">The argument list.</param>
        public MethodKey(string name, RuntimeTypeHandle[] arguments)
            : this()
        {
            this.Name = name;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets the method name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the method argument list.
        /// </summary>
        public RuntimeTypeHandle[] Arguments { get; private set; }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="instance1">The first instance to compare.</param>
        /// <param name="instance2">The second instance to compare.</param>
        /// <returns>True if the instances are considered equal; false otherwise.</returns>
        public static bool operator ==(MethodKey instance1, MethodKey instance2)
        {
            return instance1.Equals(instance2);
        }

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <param name="instance1">The first instance to compare.</param>
        /// <param name="instance2">The second instance to compare.</param>
        /// <returns>False if the instances are considered equal; true otherwise.</returns>
        public static bool operator !=(MethodKey instance1, MethodKey instance2)
        {
            return !instance1.Equals(instance2);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(MethodKey other)
        {
            if (string.IsNullOrEmpty(this.Name) && string.IsNullOrEmpty(other.Name))
            {
                return ArgumentsEquals(this.Arguments, other.Arguments);
            }

            if (string.Compare(this.Name, other.Name, StringComparison.Ordinal) != 0)
            {
                return false;
            }

            return ArgumentsEquals(this.Arguments, other.Arguments);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(MethodKey))
            {
                return false;
            }

            return this.Equals((MethodKey)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = string.IsNullOrEmpty(this.Name) ? 0 : this.Name.GetHashCode();
            if (this.Arguments == null)
            {
                return hash;
            }

            for (int i = 0; i < this.Arguments.GetLength(0); i++)
            {
                hash = hash ^ this.Arguments[i].GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// Equalses the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>True if the array of types are equal, false otherwise.</returns>
        private static bool ArgumentsEquals(RuntimeTypeHandle[] x, RuntimeTypeHandle[] y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null && (y?.Length).GetValueOrDefault() == 0)
            {
                return true;
            }

            if (y == null && (x?.Length).GetValueOrDefault() == 0)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }
            
            for (int i = 0; i < x.Length && i < y.Length; i++)
            {
                if (!x[i].Equals(y[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
