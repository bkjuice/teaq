using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Teaq
{
    /// <summary>
    /// Sponsor class for reflection extension methods used to identify what are considered primitive .NET data types.
    /// </summary>
    public static class PrimitiveExtensions
    {
        /// <summary>
        /// The primitive types including string and corresponding nullable types.
        /// </summary>
        private static readonly HashSet<RuntimeTypeHandle> supportedPrimitiveAndNullableTypes = new HashSet<RuntimeTypeHandle>
        {
            typeof(string).TypeHandle,
            typeof(int).TypeHandle,
            typeof(uint).TypeHandle,
            typeof(long).TypeHandle,
            typeof(ulong).TypeHandle,
            typeof(short).TypeHandle,
            typeof(ushort).TypeHandle,
            typeof(byte).TypeHandle,
            typeof(bool).TypeHandle,
            typeof(float).TypeHandle,
            typeof(double).TypeHandle,
            typeof(decimal).TypeHandle,
            typeof(Guid).TypeHandle,
            typeof(TimeSpan).TypeHandle,
            typeof(DateTime).TypeHandle,
            typeof(DateTimeOffset).TypeHandle,
            typeof(int?).TypeHandle,
            typeof(uint?).TypeHandle,
            typeof(long?).TypeHandle,
            typeof(ulong?).TypeHandle,
            typeof(short?).TypeHandle,
            typeof(ushort?).TypeHandle,
            typeof(byte?).TypeHandle,
            typeof(bool?).TypeHandle,
            typeof(float?).TypeHandle,
            typeof(double?).TypeHandle,
            typeof(decimal?).TypeHandle,
            typeof(Guid?).TypeHandle,
            typeof(TimeSpan?).TypeHandle,
            typeof(DateTime?).TypeHandle,
            typeof(DateTimeOffset?).TypeHandle,
        };

        /// <summary>
        /// The primitive types including string.
        /// </summary>
        private static readonly HashSet<Type> supportedPrimitiveTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort),
            typeof(byte),
            typeof(bool),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(Guid),
            typeof(TimeSpan),
            typeof(DateTime),
            typeof(DateTimeOffset),
        };

        /// <summary>
        /// The primitive nullable types.
        /// </summary>
        private static readonly HashSet<Type> supportedNullableTypes = new HashSet<Type>
        {
            // Include nullable versions of the same types
            typeof(int?),
            typeof(uint?),
            typeof(long?),
            typeof(ulong?),
            typeof(short?),
            typeof(ushort?),
            typeof(byte?),
            typeof(bool?),
            typeof(float?),
            typeof(double?),
            typeof(decimal?),
            typeof(Guid?),
            typeof(TimeSpan?),
            typeof(DateTime?),
            typeof(DateTimeOffset?),
        };

        /// <summary>
        /// Determines whether the specified type is considered a primitive, string, or nullable primitive type.
        /// </summary>
        /// <param name="target">The target type to be tested.</param>
        /// <returns>
        ///   <c>true</c> if the provided type [is primitive or string]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrimitiveOrStringOrNullable(this RuntimeTypeHandle target)
        {
            Contract.Requires(target != null);

            return supportedPrimitiveAndNullableTypes.Contains(target);
        }

        /// <summary>
        /// Determines whether the specified type is considered a non-nullable primitive type or string.
        /// </summary>
        /// <param name="target">The target type to be tested.</param>
        /// <returns>
        ///   <c>true</c> if the provided type [is primitive or string]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrimitiveOrString(this Type target)
        {
            Contract.Requires(target != null);

            return supportedPrimitiveTypes.Contains(target);
        }

        /// <summary>
        /// Determines whether the specified type is a nullable type where the generic parameter is considered a primitive type.
        /// </summary>
        /// <param name="target">The target type to be tested.</param>
        /// <returns>
        ///   <c>true</c> if the provided type [is a nullable primitive]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrimitiveNullable(this Type target)
        {
            Contract.Requires(target != null);

            return supportedNullableTypes.Contains(target);
        }

        /// <summary>
        /// Gets the supported primitive type list for error messages.
        /// </summary>
        /// <returns>The delimited list of supported types.</returns>
        public static string SupportedPrimitiveTypes()
        {
            Contract.Ensures(Contract.Result<string>() != null);

            return string.Join("\r\n", supportedPrimitiveAndNullableTypes.Select(t => Type.GetTypeFromHandle(t).Name));
        }
    }
}