using System;
using System.Data;
using System.Reflection;
using Teaq.FastReflection;

namespace Teaq.Tests.Stubs
{
    public class EntityTableHelper<T> : IDataHelper where T : class
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
