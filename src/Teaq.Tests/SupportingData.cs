using System;
using System.Data;
using System.Reflection;
using Teaq.FastReflection;

namespace Teaq.Tests
{
    public interface IDataHelper
    {
        IDataReader GetReader();
    }

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

    public class EntityTableHelper<T> : IDataHelper where T: class
    {
        private DataTable table;

        public EntityTableHelper(bool looseTypes = false)
        {
            this.table = CreateTable(looseTypes);
        }

        public void AddRow(T target)
        {
            var type = typeof(T).GetTypeDescription(MemberTypes.Property);
            var properties = type.GetProperties();
            var items = new object[properties.GetLength(0)];
            for (int i = 0; i < properties.GetLength(0); i++)
            {
                items[i] = properties[i].GetValue(target);
            }

            var row = table.NewRow();
            row.ItemArray = items;
            table.Rows.Add(row);
        }

        public void AddRow(object[] items)
        {
            var row = table.NewRow();
            row.ItemArray = items;
            table.Rows.Add(row);
        }

        public IDataReader GetReader()
        {
            return this.table.CreateDataReader();
        }

        private static DataTable CreateTable(bool looseTypes = false)
        {
            var table = new DataTable();
            var type = typeof(T).GetTypeDescription(MemberTypes.Property);
            var properties = type.GetProperties();
            for (int i = 0; i < properties.GetLength(0); i++)
            {
                table.Columns.Add(properties[i].MemberName, looseTypes ? typeof(object) : properties[i].PropertyType);
            }

            return table;
        }
    }
}
