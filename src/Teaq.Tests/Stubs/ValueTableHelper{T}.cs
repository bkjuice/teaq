using System;
using System.Data;
using System.Reflection;
using Teaq.FastReflection;

namespace Teaq.Tests.Stubs
{
    public class ValueTableHelper<T> : IDataHelper
    {
        private DataTable table = CreateTable();

        public void AddRow(T target)
        {
            var row = table.NewRow();
            row.ItemArray = new object[] { target };
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
