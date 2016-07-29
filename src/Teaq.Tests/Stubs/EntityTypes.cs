using System;
using System.Collections;
using System.Collections.Generic;

namespace Teaq.Tests.Stubs
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

    public class CustomerWithMappedKey
    {
        public int CustomerId { get; set; }

        public string Key { get; set; }

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

    public class UserTenancy
    {
        public string UserName { get; set; }

        public string TenancyKey { get; set; }
    }
}
