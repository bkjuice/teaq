using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using Microsoft.SqlServer.Server;

namespace Teaq
{
    /// <summary>
    /// Enables a collection of strings to be passed as a user defined table type to SQL Server.
    /// </summary>
    public class UdtStringEnumerator : IEnumerable<SqlDataRecord>
    {
        private SqlMetaData columnMetadata;

        private IEnumerable<string> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdtStringEnumerator" /> class.
        /// </summary>
        /// <param name="items">The strings to enumarate as a SQL Server user defined table.</param>
        /// <param name="columnName">The name of the single column in the string value table.</param>
        public UdtStringEnumerator(IEnumerable<string> items, string columnName)
            : this(items, columnName, Repository.DefaultStringType, Repository.DefaultStringSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdtStringEnumerator" /> class.
        /// </summary>
        /// <param name="items">The strings to enumarate as a SQL Server user defined table.</param>
        /// <param name="columnName">The name of the single column in the string value table.</param>
        /// <param name="type">The type of string.</param>
        /// <param name="size">The max size of the string.</param>
        public UdtStringEnumerator(IEnumerable<string> items, string columnName, SqlStringType type, int size)
        {
            Contract.Requires<ArgumentNullException>(items != null);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(columnName));

            this.items = items;
            this.columnMetadata =
                new SqlMetaData(
                    columnName,
                    type == SqlStringType.NVarchar ? SqlDbType.NVarChar : SqlDbType.VarChar,
                    size);
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
