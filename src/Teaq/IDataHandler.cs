using System.Data;

namespace Teaq
{
    /// <summary>
    /// Interface to support pluggable read handlers for special query cases or performance sensitive code.
    /// </summary>
    /// <typeparam name="T">The type of result that will be materialized.</typeparam>
    public interface IDataHandler<T>
    {
        /// <summary>
        /// Called [before reading] to materialize the underlying data.
        /// </summary>
        /// <param name="reader">The reader that will be read.</param>
        void OnBeforeReading(IDataReader reader);

        /// <summary>
        /// Called [after reading] has completed.
        /// </summary>
        void OnAfterReading();

        /// <summary>
        /// Reads a single entity from the current reader result set.
        /// </summary>
        /// <param name="reader">The active data reader.</param>
        /// <returns>
        /// A single entity materialized from the data reader.
        /// </returns>
        T ReadEntity(IDataReader reader);
    }
}
