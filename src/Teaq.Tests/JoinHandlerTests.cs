using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class JoinHandlerTests
    {
        [TestMethod]
        public void JoinConfigurationIsMappedAsExpected()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<CustomerWithAddress>()
                    .Exclude(c => c.Inception)
                    .Exclude(c => c.Modified);

                builder.Entity<Address>()
                    .Column(a => a.CustomerId).HasMapping("AddressCustomerId")
                    .Column(a => a.Change).HasMapping("AddressChange");
            });

            var handler = new JoinHandler<CustomerWithAddress>(model);
            var map = handler.GetMap();
            map.AddJoinSplit<Address>((c, a) => c.Address = a);

            var tableHelper = new TableHelper("CustomerId", "CustomerKey", "Change", "AddressId", "AddressCustomerId", "AddressChange", "AddressLine1");
            tableHelper.AddRow(1, "test1", 2, 5, 1, 3, "78 run 9");
            tableHelper.AddRow(2, "test2", 2, 6, 2, 3, "55 elm st");

            List<CustomerWithAddress> results;
            using (var reader = tableHelper.GetReader())
            {
                results = reader.ReadEntities(handler);
            }

            results.Count.Should().Be(2);
            results[0].CustomerId.Should().Be(1);
            results[0].Address.Should().NotBeNull();
            results[0].Address.CustomerId.Should().Be(1);

            results[1].CustomerId.Should().Be(2);
            results[1].Address.Should().NotBeNull();
            results[1].Address.CustomerId.Should().Be(2);
        }
    }
}
