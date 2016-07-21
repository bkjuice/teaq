using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Teaq.Configuration;

namespace Teaq
{
    /// <summary>
    /// Extensions to enable parsing of results using IL generated delegate calls.
    /// </summary>
    public static partial class DataReaderExtensions
    {
        /// <summary>
        /// Reads the specified data reader row by row using an iterator.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="dataModel">The optional data model.</param>
        /// <param name="onCompleteCallback">The method to invoke on completion of the enumeration.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// An enumerable stream of values.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the data is null and the target is not a nullable type.</exception>
        public static IEnumerable<T> EnumerateEntities<T>(
            this IDataReader reader,
            IDataModel dataModel = null,
            Action onCompleteCallback = null,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return reader?.EnumerateEntities<T>(null, dataModel, nullPolicy, onCompleteCallback) ?? EnumerateEmpty<T>(onCompleteCallback);
        }

        /// <summary>
        /// Reads the specified data reader row by row using an iterator.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="handler">The custom handler. If provided and the handler can read the
        /// result set, execution is delegated to the handler to produce the results.</param>
        /// <param name="onCompleteCallback">The method to invoke on completion of the enumeration.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// An enumerable stream of values.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the data is null and the target is not a nullable type.</exception>
        public static IEnumerable<T> EnumerateEntities<T>(
            this IDataReader reader,
            IDataHandler<T> handler,
            Action onCompleteCallback = null,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            Contract.Requires<ArgumentNullException>(handler != null);
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return reader.EnumerateEntities<T>(handler, null, nullPolicy, onCompleteCallback) ?? EnumerateEmpty<T>(onCompleteCallback);
        }

        /// <summary>
        /// Reads the specified data reader.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="estimatedRowCount">The optional estimated row count. Use this value to optimize the buffer size of the list.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// A list of values.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the data is null and the target is not a nullable type.</exception>
        public static List<T> ReadEntities<T>(
            this IDataReader reader,
            IDataModel dataModel = null,
            int estimatedRowCount = 64,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            Contract.Requires<ArgumentException>(estimatedRowCount >= 0);
            Contract.Ensures(Contract.Result<List<T>>() != null);

            return ReadEntities<T>(reader, null, dataModel, estimatedRowCount, nullPolicy);
        }

        /// <summary>
        /// Reads the specified data reader.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="handler">The custom handler. If provided and the handler can read the
        /// result set, execution is delegated to the handler to produce the results.</param>
        /// <param name="estimatedRowCount">The optional estimated row count. Use this value to optimize the buffer size of the list.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// A list of values.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the data is null and the target is not a nullable type.</exception>
        public static List<T> ReadEntities<T>(
            this IDataReader reader,
            IDataHandler<T> handler,
            int estimatedRowCount = 64,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            Contract.Requires<ArgumentNullException>(handler != null);
            Contract.Requires<ArgumentException>(estimatedRowCount >= 0);
            Contract.Ensures(Contract.Result<List<T>>() != null);

            return ReadEntities<T>(reader, handler, null, estimatedRowCount, nullPolicy);
        }

        /// <summary>
        /// Reads the specified data reader.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="handler">
        /// The custom handler. If provided and the handler can read the
        /// result set, execution is delegated to the handler to produce the results.
        /// </param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="estimatedRowCount">
        /// The optional estimated row count. Use this value to optimize the buffer size of the list.
        /// </param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <returns>
        /// A list of values.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the data is null and the target is not a nullable type.
        /// </exception>
        internal static List<T> ReadEntities<T>(
            this IDataReader reader,
            IDataHandler<T> handler,
            IDataModel dataModel,
            int estimatedRowCount,
            NullPolicyKind nullPolicy)
        {
            Contract.Requires(estimatedRowCount >= 0);
            Contract.Ensures(Contract.Result<List<T>>() != null);

            var results = new List<T>(estimatedRowCount);
            results.AddRange(reader?.EnumerateEntities<T>(handler, dataModel, nullPolicy, null) ?? EnumerateEmpty<T>(null));
            return results;
        }

        /// <summary>
        /// Reads the specified data reader row by row using an iterator.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="handler">The custom handler. If provided and the handler can read the
        /// result set, execution is delegated to the handler to produce the results.</param>
        /// <param name="dataModel">The data model.</param>
        /// <param name="nullPolicy">The null policy. Null values are included as default, by default.</param>
        /// <param name="onCompleteCallback">The method to invoke on completion of the enumeration.</param>
        /// <returns>
        /// An enumerable stream of values.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the data is null and the target is not a nullable type.</exception>
        internal static IEnumerable<T> EnumerateEntities<T>(
            this IDataReader reader,
            IDataHandler<T> handler,
            IDataModel dataModel,
            NullPolicyKind nullPolicy,
            Action onCompleteCallback)
        {
            Contract.Requires(reader != null);

            if (handler != null)
            {
                return new CustomHandlerIterator<T>(reader, handler, onCompleteCallback);
            }

            // TODO: EnumerateValues...T : struct
            // TODO: EnumerateStrings....
            if (typeof(T).TypeHandle.IsPrimitiveOrStringOrNullable())
            {
                return new SimpleTypeIterator<T>(reader, nullPolicy, onCompleteCallback);
            }

            return new ComplexTypeIterator<T>(reader, dataModel, onCompleteCallback);
        }

        /// <summary>
        /// Determines whether the value [is null or database null].
        /// </summary>
        /// <param name="value">The value. to test.</param>
        /// <returns>True if the value is considered null, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsNullOrDbNull(this object value)
        {
            return value == null || value is DBNull || (value as INullable)?.IsNull == true;
        }

        /// <summary>
        /// Throws the assignment exception.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="e">The inner exception.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowAssignmentException(string fieldName, string typeName, string propertyName, Exception e)
        {
            throw new InvalidOperationException($"The data type for field { fieldName} cannot be assigned to property '{ typeName }.{ propertyName }'. Verify the target property is nullable if null is allowed, and ensure the data type in the database matches the corresponding CLR type for the property.", e);
        }

        /// <summary>
        /// Null object pattern implementation.
        /// </summary>
        /// <typeparam name="T">The enumerated type.</typeparam>
        /// <returns>An empty collection.</returns>
        private static IEnumerable<T> EnumerateEmpty<T>(Action onCompletedCallback)
        {
            onCompletedCallback?.Invoke();
            yield break;
        }
    }
}
