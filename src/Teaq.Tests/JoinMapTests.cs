using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.FastReflection;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class JoinMapTests
    {
        [TestMethod]
        public void JoinMapInitializesAsExpected()
        {
            var map = JoinMap<CustomerWithAddress>.Create();
            var propertyCount = typeof(CustomerWithAddress).GetTypeDescription(MemberTypes.Property).GetProperties().GetLength(0);
            map.TotalFields.Should().Be(propertyCount -1);
        }

        [TestMethod]
        public void JoinMapExcludesPropertiesAsExpected()
        {
            var model = Repository.BuildModel(builder =>
                {
                    builder.Entity<CustomerWithAddress>().Exclude(c => c.Inception);
                });

            var map = JoinMap<CustomerWithAddress>.Create(model);
            var propertyCount = typeof(CustomerWithAddress).GetTypeDescription(MemberTypes.Property).GetProperties().GetLength(0);
            map.TotalFields.Should().Be(propertyCount - 2);
        }

        [TestMethod]
        public void JoinMapUsesExplicitPropertyCountAsExpected()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<CustomerWithAddress>().Exclude(c => c.Inception);
            });

            var map = JoinMap<CustomerWithAddress>.Create(model, 3);
            map.TotalFields.Should().Be(3);
        }

        [TestMethod]
        public void JoinMapCanParseComplexProperty()
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
            tableHelper.AddRow(1, "test1", (byte)2, 5, 1, (byte)3, "123 main street");
            tableHelper.AddRow(2, "test2", (byte)2, 6, 2, (byte)3, "456 park pl");

            var results = new List<CustomerWithAddress>();
            
            using (var reader = tableHelper.GetReader())
            {
                handler.OnBeforeReading(reader);
                while (reader.Read())
                {
                    var result = handler.ReadEntity(reader);
                    results.Add(result);
                }
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
