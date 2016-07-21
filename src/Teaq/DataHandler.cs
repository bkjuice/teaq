using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;

namespace Teaq
{
    /// <summary>
    /// Base implementation of the data handler interface.
    /// </summary>
    /// <typeparam name="T">The type of result to be materialized.</typeparam>
    /// <remarks>Instances are not considered thread-safe by default.</remarks>
    /// <seealso cref="Teaq.IDataHandler{T}" />
    public abstract class DataHandler<T> : IDataHandler<T>
    {
        /// <summary>
        /// The current reader.
        /// </summary>
        private IDataReader currentReader;

        /// <summary>
        /// The known fields for the current reader.
        /// </summary>
        private Dictionary<string, int> knownFields;

        /// <summary>
        /// Gets the an estimated row count used to allocate the initial entity buffer. Default is 16.
        /// </summary>
        /// <remarks>
        /// Override this property to estimate result buffer size and minimize buffer re-allocations when processing results.
        /// </remarks>
        protected virtual int EstimatedRowCount
        {
            get
            {
                return 16;
            }
        }

        /// <summary>
        /// Called [after reading] has completed.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void OnAfterReading()
        {
            this.currentReader = null;
            this.knownFields = null;
        }

        /// <summary>
        /// Called [before reading] to materialize the underlying data.
        /// </summary>
        /// <param name="reader">The reader that will be read.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void OnBeforeReading(IDataReader reader)
        {
            this.currentReader = reader;
        }

        /// <summary>
        /// Reads the specified reader result set.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The list of entities that are read.</returns>
        public List<T> ReadEntities(IDataReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Ensures(Contract.Result<List<T>>() != null);

            var results = new List<T>(this.EstimatedRowCount);
            while(reader.Read())
            {
                results.Add(this.ReadEntity(reader));
            }

            return results;
        }

        /// <summary>
        /// Reads a single entity from the current reader result set.
        /// </summary>
        /// <param name="reader">The active data reader.</param>
        /// <returns>
        /// A single entity from the reader.
        /// </returns>
        public abstract T ReadEntity(IDataReader reader);

        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The ordinal position or -1 if not found.</returns>
        protected int GetOrdinal(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null);

            if (this.knownFields == null)
            {
                this.IndexFields();
            }

            int value;
            if (this.knownFields.TryGetValue(name, out value))
            {
                return value;
            }

            return -1;
        }

        /// <summary>
        /// Gets a value indicating if the column exists.
        /// </summary>
        /// <param name="name">The name to test, case sensitive.</param>
        /// <returns>True if the column exists, false otherwise.</returns>
        protected bool ColumnExists(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null);

            if (this.knownFields == null)
            {
                this.IndexFields();
            }

            return this.knownFields.ContainsKey(name);
        }

        private void IndexFields()
        {
            // TODO: Duplicate field names will fail here....but props cannot dup...
            this.knownFields = new Dictionary<string, int>(this.currentReader.FieldCount, StringComparer.Ordinal);
            for (int i = 0; i < this.currentReader.FieldCount; ++i)
            {
                // Catch dup key and wrap with better info:
                this.knownFields.Add(this.currentReader.GetName(i), i);
            }
        }
    }
}
