using System;
using System.Data;
using System.Reflection;
using Teaq.FastReflection;

namespace Teaq.Tests.Stubs
{
    public class TableHelper : IDisposable, IDataHelper
    {
        private DataTable table;

        public TableHelper(params string[] columnNames)
        {
            this.table = new DataTable();
            for (int i = 0; i < columnNames.GetLength(0); i++)
            {
                this.table.Columns.Add(columnNames[i], typeof(object));
            }
        }

        public void AddRow(params object[] items)
        {
            var row = table.NewRow();
            row.ItemArray = items;
            table.Rows.Add(row);
        }

        public IDataReader GetReader()
        {
            return this.table.CreateDataReader();
        }

        protected virtual void Dispose(bool flag)
        {
            if (flag)
            {
                table.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}