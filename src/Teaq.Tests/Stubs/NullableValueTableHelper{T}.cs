using System;
using System.Data;

namespace Teaq.Tests.Stubs
{
    public class NullableValueTableHelper<T> : IDataHelper where T: struct
    {
        private DataTable table = CreateTable();

        public void AddRow(T? target)
        {
            var row = table.NewRow();
            row.ItemArray = new object[] { target.HasValue ? (object)target.Value : DBNull.Value };
            table.Rows.Add(row);
        }

        public IDataReader GetReader()
        {
            return this.table.CreateDataReader();
        }

        private static DataTable CreateTable()
        {
            var table = new DataTable();
            table.Columns.Add("value", typeof(T));
            return table;
        }
    }
}
