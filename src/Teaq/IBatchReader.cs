using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Teaq
{
    /// <summary>
    /// Testable surrogate interface used to abstract data access control. 
    /// This interface reduces the data context surface that must be mocked for unit testing.
    /// </summary>
    public interface IBatchReader : IBatchContext, IDisposable
    {
        /// <summary>
        /// Gets the current reader. Use this for direct access to result sets in cases where ORM is not helpful.
        /// </summary>
        IDataReader CurrentReader { get; }

        /// <summary>
        /// Gets the next data reader result, if available.
        /// </summary>
        /// <returns>
        /// True if there is a next result set or next result set from the next batch of queries to run, false otherwise if all results are iterated.
        /// </returns>
        bool NextResult();

        /// <summary>
        /// Gets the next data reader result, if available.
        /// </summary>
        /// <returns>
        /// True if there is a next result set or next result set from the next batch of queries to run, false otherwise if all results are iterated.
        /// </returns>
        Task<bool> NextResultAsync();

        /// <summary>
        /// Reads the set of entities from the query batch.
        /// </summary>
        /// <typeparam name="TEntity">The concrete entity type.</typeparam>
        /// <param name="model">The optional data model.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}" /> collection of entities.
        /// </returns>
        List<TEntity> ReadEntitySet<TEntity>(IDataModel model = null);

        /// <summary>
        /// Reads the set of entities from the query batch.
        /// </summary>
        /// <typeparam name="TEntity">The concrete entity type.</typeparam>
        /// <param name="handler">The explicit data handler.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}" /> collection of entities.
        /// </returns>
        List<TEntity> ReadEntitySet<TEntity>(IDataHandler<TEntity> handler);

        /// <summary>
        /// Enumerates the entities in the batch for the current row set.
        /// </summary>
        /// <typeparam name="TEntity">The concrete entity type.</typeparam>
        /// <param name="model">The optional data model.</param>
        /// An <see cref="IEnumerable{T}" /> collection of entities.
        IEnumerable<TEntity> EnumerateEntitySet<TEntity>(IDataModel model = null);

        /// <summary>
        /// Enumerates the entities in the batch for the current row set.
        /// </summary>
        /// <typeparam name="TEntity">The concrete entity type.</typeparam>
        /// <param name="handler">The explicit data handler.</param>
        /// An <see cref="IEnumerable{T}" /> collection of entities.
        IEnumerable<TEntity> EnumerateEntitySet<TEntity>(IDataHandler<TEntity> handler);
    }
}
