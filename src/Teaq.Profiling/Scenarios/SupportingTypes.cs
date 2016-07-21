using System;
using System.Collections;
using System.Collections.Generic;

namespace Teaq.Profiling.Scenarios
{
    public interface IAbstractEntity : ICommonAbstractEntity
    {
        new int Id { get; set; }

        string Data { get; set; }
    }

    public interface ICommonAbstractEntity
    {
        int Id { get; set; }
    }

    public class ConcreteEntity : IAbstractEntity
    {
        public int Id { get; set; }

        public string Data { get; set; }
    }

    public class NonIListCollection<T> : IEnumerable<T>
    {
        private List<T> items;

        public NonIListCollection(List<T> items)
        {
            this.items = items;
        }

        public bool InvalidContains(T item)
        {
            return items.Contains(item);
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }
    }

    public class Address
    {
        public int AddressId { get; set; }

        public int CustomerId { get; set; }

        public byte Change { get; set; }
    }

    public class Customer
    {
        public int CustomerId { get; set; }

        public string CustomerKey { get; set; }

        public DateTime Inception { get; set; }

        public DateTimeOffset Modified { get; set; }

        public byte Change { get; set; }
    }

    public class CustomerWithNullable
    {
        public int CustomerId { get; set; }

        public string CustomerKey { get; set; }

        public DateTime Inception { get; set; }

        public DateTimeOffset Modified { get; set; }

        public byte Change { get; set; }

        public DateTimeOffset? Deleted { get; set; }
    }

    public class CustomerWithNullableId
    {
        public int CustomerId { get; set; }

        public int? RelatedCustomerId { get; set; }
    }

    public class CustomerWithNestedProperty
    {
        public CustomerWithNestedProperty()
        {
            this.Keys = new NestedKeys();
        }

        public NestedKeys Keys { get; private set; }

        public DateTime Inception { get; set; }

        public DateTimeOffset Modified { get; set; }

        public byte Change { get; set; }

        public DateTimeOffset? Deleted { get; set; }
    }

    public class CustomerWithAddress
    {
        public int CustomerId { get; set; }

        public string CustomerKey { get; set; }

        public DateTime Inception { get; set; }

        public DateTimeOffset Modified { get; set; }

        public byte Change { get; set; }

        public Address Address { get; set; } 
    }

    public class CustomerWithAddresses
    {
        public int CustomerId { get; set; }

        public string CustomerKey { get; set; }

        public DateTime Inception { get; set; }

        public DateTimeOffset Modified { get; set; }

        public byte Change { get; set; }

        public List<Address> Addresses { get; set; }
    }

    public class NestedKeys
    {
        public int CustomerId { get; set; }

        public string CustomerKey { get; set; }
    }

    public static class CustomerData
    {
        public readonly static EntityTableHelper<Customer> OneHundredCustomers = BuildDataSet();

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
