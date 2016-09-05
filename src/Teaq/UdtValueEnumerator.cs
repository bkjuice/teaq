using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.SqlServer.Server;

namespace Teaq
{
    /// <summary>
    /// Enables a collection of simple value types to be passed as a user defined table type to SQL Server.
    /// </summary>
    /// <typeparam name="T">The target value type to be enumerated.</typeparam>
    public class UdtValueEnumerator<T> : UdtEnumerator<T> where T : struct
    {
        private SqlMetaData columnMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdtValueEnumerator{T}" /> class.
        /// </summary>
        /// <param name="items">The items to enumarate as a SQL Server user defined table.</param>
        /// <param name="columnName">The name of the single column in the value table.</param>
        public UdtValueEnumerator(IEnumerable<T> items, string columnName)
            : base(items)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(columnName));

            this.columnMetadata = typeof(T).GetUdtMetaData(columnName);
        }

        /// <summary>
        /// Transforms the specified value to a <see cref="SqlDataRecord" />.
        /// </summary>
        /// <param name="value">The value to transform.</param>
        /// <returns>
        /// The resulting <see cref="SqlDataRecord" /> instance.
        /// </returns>
        protected override SqlDataRecord Transform(T value)
        {
            var record = new SqlDataRecord(this.columnMetadata);
            record.SetValue(0, value);
            return record;
        }
    }
}
