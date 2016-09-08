using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace Teaq.Tests
{
    [TestClass]
    public class UdtEntityEnumeratorTests
    {
        [TestMethod]
        public void UdtEntityEnumeratorCanReflectColumnMetadataForNullableDoubleWithValue()
        {
            var items = new List<DtoWithNullablePrimitives>();
            items.Add(new DtoWithNullablePrimitives { ANullableDouble = 1 });
            items.Add(new DtoWithNullablePrimitives { ANullableDouble = 2 });

            Action test = () => new UdtEntityEnumerator<DtoWithNullablePrimitives>(items);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void UdtEntityEnumeratorCanReflectColumnMetadataForNullableDoubleWithNoValue()
        {
            var items = new List<DtoWithNullablePrimitives>();
            items.Add(new DtoWithNullablePrimitives { ANullableDouble = 1 });
            items.Add(new DtoWithNullablePrimitives { ANullableDouble = null });

            Action test = () => new UdtEntityEnumerator<DtoWithNullablePrimitives>(items);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void UdtEntityEnumeratorCanReflectColumnMetadataForDouble()
        {
            var items = new List<DtoWithPrimitives>();
            items.Add(new DtoWithPrimitives { ADouble = 1 });
            items.Add(new DtoWithPrimitives { ADouble = 2 });

            Action test = () => new UdtEntityEnumerator<DtoWithPrimitives>(items);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void UdtEntityEnumeratorRespectsGlobalStringTypeAndSizeAndDoesNotThrow()
        {
            var listOfDtos = new List<SimpleCustomerDto>
            {
                new SimpleCustomerDto
                {
                    Change = 1,
                    CustomerId = 2,
                    CustomerKey = "123",
                    Inception = DateTime.Now,
                    Modified = DateTimeOffset.Now
                },
            };

            var lastSetting = Repository.DefaultStringType;
            Repository.DefaultStringType = SqlStringType.NVarchar;
            Action test = () =>
            {
                var udt = new UdtEntityEnumerator<SimpleCustomerDto>(listOfDtos);
                var record = udt.First();
                record.Should().NotBeNull();
            };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void UdtEntityEnumeratorEnumeratesListOfSimpleCustomerDTOsAsExpected()
        {
            var listOfDtos = new List<SimpleCustomerDto>
            {
                new SimpleCustomerDto
                {
                    Change = 1,
                    CustomerId = 2,
                    CustomerKey = "123",
                    Inception = DateTime.Now,
                    Modified = DateTimeOffset.Now
                },

                new SimpleCustomerDto
                {
                    Change = 2,
                    CustomerId = 1,
                    CustomerKey = "456",
                    Inception = DateTime.Now,
                    Modified = DateTimeOffset.Now
                },
            };

            var model = Repository.BuildModel(config => {
                config.Entity<SimpleCustomerDto>()
                .Column(e => e.CustomerKey)
                .IsVarChar(50);
            });

            var udt = new UdtEntityEnumerator<SimpleCustomerDto>(listOfDtos, model);
            var count = 0;
            foreach(var record in udt)
            {
                count++;
            }

            count.Should().Be(2);
        }

        private class SimpleCustomerDto
        {
            public int CustomerId { get; set; }

            public string CustomerKey { get; set; }

            public DateTime Inception { get; set; }

            public DateTimeOffset Modified { get; set; }

            public byte Change { get; set; }
        }

        private class DtoWithNullablePrimitives
        {
            public double? ANullableDouble { get; set; } 
        }

        private class DtoWithPrimitives
        {
            public double ADouble { get; set; }
        }
    }
}
