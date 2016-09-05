using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using Microsoft.SqlServer.Server;

namespace Teaq
{
    /// <summary>
    /// Enables a collection of strings to be passed as a user defined table type to SQL Server.
    /// </summary>
    public class UdtStringEnumerator : UdtEnumerator<string>
    {
        private SqlMetaData columnMetadata;

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
            : base(items)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(columnName));

            this.columnMetadata =
                new SqlMetaData(
                    columnName,
                    type == SqlStringType.NVarchar ? SqlDbType.NVarChar : SqlDbType.VarChar,
                    size);
        }

        /// <summary>
        /// Transforms the specified value to a <see cref="SqlDataRecord" />.
        /// </summary>
        /// <param name="value">The value to transform.</param>
        /// <returns>
        /// The resulting <see cref="SqlDataRecord" /> instance.
        /// </returns>
        protected override SqlDataRecord Transform(string value)
        {
            var record = new SqlDataRecord(this.columnMetadata);
            record.SetValue(0, value);
            return record;
        }
    }
}
