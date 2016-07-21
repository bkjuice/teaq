using System;
using System.Linq;

namespace Teaq.Profiling.Scenarios
{
    internal class DefaultObjectMaterialization
    {
        private readonly static EntityTableHelper<Customer> table = BuildDataSet();

        public bool Run()
        {
            return
                this.Materialize100EntitiesUsingReadEntities() && this.Materialize100EntitiesUsingEnumerateEntities();
        }

        public bool Materialize100EntitiesUsingReadEntities()
        {
            var reader = table.GetReader();
            return reader.ReadEntities<Customer>().Count == 100;
        }

        public bool Materialize100EntitiesUsingEnumerateEntities()
        {
            var reader = table.GetReader();
            return reader.EnumerateEntities<Customer>().Count() == 100;
        }

        private static EntityTableHelper<Customer> BuildDataSet()
        {
            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "19999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "29999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 3, CustomerKey = "39999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 4, CustomerKey = "49999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 5, CustomerKey = "59999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 6, CustomerKey = "69999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 7, CustomerKey = "79999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 8, CustomerKey = "89999", Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 9, CustomerKey = "99999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 10, CustomerKey = "109999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });

            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "19999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "29999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 3, CustomerKey = "39999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 4, CustomerKey = "49999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 5, CustomerKey = "59999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 6, CustomerKey = "69999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 7, CustomerKey = "79999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 8, CustomerKey = "89999", Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 9, CustomerKey = "99999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 10, CustomerKey = "109999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });

            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "19999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "29999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 3, CustomerKey = "39999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 4, CustomerKey = "49999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 5, CustomerKey = "59999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 6, CustomerKey = "69999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 7, CustomerKey = "79999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 8, CustomerKey = "89999", Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 9, CustomerKey = "99999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 10, CustomerKey = "109999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });

            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "19999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "29999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 3, CustomerKey = "39999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 4, CustomerKey = "49999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 5, CustomerKey = "59999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 6, CustomerKey = "69999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 7, CustomerKey = "79999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 8, CustomerKey = "89999", Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 9, CustomerKey = "99999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 10, CustomerKey = "109999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });

            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "19999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "29999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 3, CustomerKey = "39999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 4, CustomerKey = "49999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 5, CustomerKey = "59999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 6, CustomerKey = "69999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 7, CustomerKey = "79999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 8, CustomerKey = "89999", Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 9, CustomerKey = "99999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 10, CustomerKey = "109999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });

            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "19999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "29999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 3, CustomerKey = "39999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 4, CustomerKey = "49999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 5, CustomerKey = "59999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 6, CustomerKey = "69999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 7, CustomerKey = "79999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 8, CustomerKey = "89999", Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 9, CustomerKey = "99999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 10, CustomerKey = "109999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });

            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "19999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "29999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 3, CustomerKey = "39999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 4, CustomerKey = "49999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 5, CustomerKey = "59999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 6, CustomerKey = "69999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 7, CustomerKey = "79999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 8, CustomerKey = "89999", Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 9, CustomerKey = "99999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 10, CustomerKey = "109999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });

            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "19999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "29999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 3, CustomerKey = "39999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 4, CustomerKey = "49999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 5, CustomerKey = "59999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 6, CustomerKey = "69999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 7, CustomerKey = "79999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 8, CustomerKey = "89999", Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 9, CustomerKey = "99999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 10, CustomerKey = "109999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });

            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "19999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "29999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 3, CustomerKey = "39999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 4, CustomerKey = "49999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 5, CustomerKey = "59999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 6, CustomerKey = "69999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 7, CustomerKey = "79999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 8, CustomerKey = "89999", Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 9, CustomerKey = "99999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 10, CustomerKey = "109999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });

            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "19999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "29999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 3, CustomerKey = "39999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 4, CustomerKey = "49999", Inception = DateTime.UtcNow, Change = 0, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 5, CustomerKey = "59999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 6, CustomerKey = "69999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 7, CustomerKey = "79999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 8, CustomerKey = "89999", Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 9, CustomerKey = "99999", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 10, CustomerKey = "109999", Inception = DateTime.UtcNow, Change = 1, Modified = DateTimeOffset.UtcNow });

            return tableHelper;
        }
    }
}
