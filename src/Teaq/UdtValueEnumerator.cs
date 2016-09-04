using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.SqlServer.Server;

namespace Teaq
{
    /// <summary>
    /// Enables a collection of simple value types to be passed as a user defined table type to SQL Server.
    /// </summary>
    /// <typeparam name="T">The target value type to be enumerated.</typeparam>
    public class UdtValueEnumerator<T> : IEnumerable<SqlDataRecord> where T : struct
    {
        private SqlMetaData columnMetadata;

        private IEnumerable<T> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdtValueEnumerator{T}" /> class.
        /// </summary>
        /// <param name="items">The items to enumarate as a SQL Server user defined table.</param>
        /// <param name="columnName">The name of the single column in the value table.</param>
        public UdtValueEnumerator(IEnumerable<T> items, string columnName)
        {
            Contract.Requires<ArgumentNullException>(items != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(columnName));

            this.items = items;
            this.columnMetadata = typeof(T).GetUdtMetaData(columnName);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection of values as a user defined table parameter.
        /// </returns>
        public IEnumerator<SqlDataRecord> GetEnumerator()
        {
            foreach (var item in items)
            {
                var record = new SqlDataRecord(this.columnMetadata);
                record.SetValue(0, item);
                yield return record;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
