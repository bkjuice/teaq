using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests
{
    [TestClass]
    public class UdtTypeEnumeratorTests
    {
        [TestMethod]
        public void UdtTypeEnumeratorEnumeratesListOfSimpleCustomerDTOsAsExpected()
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

            var udt = new UdtTypeEnumerator<SimpleCustomerDto>(listOfDtos, model);
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
    }
}
