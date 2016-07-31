using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        /// Enumerates the primitive values from the result set at column position 0, exluding null values.
        /// </summary>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="onCompleteCallback">The callback used once iteration is complete.</param>
        /// <returns>
        /// The collection of string values.
        /// </returns>
        public static IEnumerable<string> EnumerateStringValues(this IDataReader reader, Action onCompleteCallback = null) 
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            return reader.EnumerateStringValuesInternal(onCompleteCallback);
        }

        /// <summary>
        /// Enumerates the primitive values from the result set at column position 0, exluding null values.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="onCompleteCallback">The callback used once iteration is complete.</param>
        /// <returns>The collection of values with null values ommitted.</returns>
        public static IEnumerable<T> EnumerateValues<T>(this IDataReader reader, Action onCompleteCallback = null) where T: struct
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return reader.EnumerateValuesInternal<T>(null, onCompleteCallback);
        }

        /// <summary>
        /// Enumerates the primitive values from the result set at column position 0.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="onCompleteCallback">The callback used once iteration is complete.</param>
        /// <returns>The collection of values with null values included.</returns>
        public static IEnumerable<T?> EnumerateNullableValues<T>(this IDataReader reader, Action onCompleteCallback = null) where T : struct
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Ensures(Contract.Result<IEnumerable<T?>>() != null);

            return reader.EnumerateNullableValuesInternal<T>(null, onCompleteCallback);
        }

        /// <summary>
        /// Enumerates the primitive values from the result set using the provided handler function, exluding null values.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="handler">The handler to use to retrieve the target value.</param>
        /// <param name="onCompleteCallback">The callback used once iteration is complete.</param>
        /// <returns>
        /// The collection of values with null values ommitted.
        /// </returns>
        public static IEnumerable<T> EnumerateValues<T>(this IDataReader reader, Func<IDataReader, T?> handler, Action onCompleteCallback = null) where T : struct
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Requires<ArgumentNullException>(handler != null);
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return reader.EnumerateValuesInternal(handler, onCompleteCallback);
        }

        /// <summary>
        /// Enumerates the primitive values from the result set at column position 0.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="handler">The handler to use to retrieve the target value.</param>
        /// <param name="onCompleteCallback">The callback used once iteration is complete.</param>
        /// <returns>The collection of values with null values included.</returns>
        public static IEnumerable<T?> EnumerateNullableValues<T>(this IDataReader reader, Func<IDataReader, T?> handler, Action onCompleteCallback = null) where T : struct
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Requires<ArgumentNullException>(handler != null);
            Contract.Ensures(Contract.Result<IEnumerable<T?>>() != null);

            return reader.EnumerateNullableValuesInternal(handler, onCompleteCallback);
        }

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
            Contract.Requires<ArgumentNullException>(reader != null);
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
            Contract.Requires<ArgumentNullException>(reader != null);
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
            Contract.Requires<ArgumentNullException>(reader != null);
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
        public static List<T> ReadEntities<T>(
            this IDataReader reader,
            IDataHandler<T> handler,
            int estimatedRowCount = 64,
            NullPolicyKind nullPolicy = NullPolicyKind.IncludeAsDefaultValue)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Requires<ArgumentNullException>(handler != null);
            Contract.Requires<ArgumentException>(estimatedRowCount >= 0);
            Contract.Ensures(Contract.Result<List<T>>() != null);

            return ReadEntities<T>(reader, handler, null, estimatedRowCount, nullPolicy);
        }

        internal static IEnumerable<string> EnumerateStringValuesInternal(this IDataReader reader, Action onCompleteCallback = null)
        {
            Contract.Requires(reader != null);
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);

            return new SimpleTypeIterator<string>(reader, NullPolicyKind.IncludeAsDefaultValue, onCompleteCallback);
        }

        internal static IEnumerable<T?> EnumerateNullableValuesInternal<T>(this IDataReader reader, Func<IDataReader, T?> handler, Action onCompleteCallback = null) where T : struct
        {
            Contract.Requires(reader != null);
            Contract.Ensures(Contract.Result<IEnumerable<T?>>() != null);

            return new ValueIterator<T>(reader, handler, onCompleteCallback);
        }

        internal static IEnumerable<T> EnumerateValuesInternal<T>(this IDataReader reader, Func<IDataReader, T?> handler, Action onCompleteCallback = null) where T : struct
        {
            Contract.Requires(reader != null);
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return (new ValueIterator<T>(reader, handler, onCompleteCallback)).Where(v => v.HasValue).Select(v => v.Value);
        }

        internal static List<T> ReadEntities<T>(
            this IDataReader reader,
            IDataHandler<T> handler,
            IDataModel dataModel,
            int estimatedRowCount,
            NullPolicyKind nullPolicy)
        {
            Contract.Requires(reader != null);
            Contract.Requires(estimatedRowCount >= 0);
            Contract.Ensures(Contract.Result<List<T>>() != null);

            var results = new List<T>(estimatedRowCount);
            results.AddRange(reader?.EnumerateEntities<T>(handler, dataModel, nullPolicy, null) ?? EnumerateEmpty<T>(null));
            return results;
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsNullOrDbNull(this object value)
        {
            return value == null || value is DBNull || (value as INullable)?.IsNull == true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ThrowAssignmentException(string fieldName, string typeName, string propertyName, Exception e)
        {
            throw new InvalidOperationException($"The data type for field { fieldName} cannot be assigned to property '{ typeName }.{ propertyName }'. Verify the target property is nullable if null is allowed, and ensure the data type in the database matches the corresponding CLR type for the property.", e);
        }

        private static IEnumerable<T> EnumerateEmpty<T>(Action onCompletedCallback)
        {
            onCompletedCallback?.Invoke();
            yield break;
        }
    }
}
