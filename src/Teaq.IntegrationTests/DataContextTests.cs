using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.Tests;
using Teaq.Tests.Stubs;

namespace Teaq.IntegrationTests
{
    [TestClass]
    public class DataContextTests
    {
        [TestMethod]
        public void ExecuteInsertSelectsIdentity()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>()
                    .Column(c => c.CustomerId).IsIdentity()
                    .Column(c => c.CustomerKey).IsKey();
            });

            var customerKey = MakeTestKey();
            var customer = new Customer { CustomerKey = customerKey, Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow };

            var context = Repository.BuildContext(ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString);
            var command = model.ForEntity<Customer>().BuildInsert(customer).ToCommand();
            var customerId = context.ExecuteScalar<int>(command).GetValueOrDefault();
            customerId.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void ExecuteInsertThenSelect()
        {
            var model = Repository.BuildModel(builder =>
                {
                    builder.Entity<Customer>()
                        .Column(c => c.CustomerId).IsIdentity()
                        .Column(c => c.CustomerKey).IsKey();
                });

            var customerKey = MakeTestKey();
            var customer = new Customer { CustomerKey = customerKey, Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow };
            var customer2 = default(Customer);

            var context = Repository.BuildContext(ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString);
            var command = model.ForEntity<Customer>().BuildInsert(customer).ToCommand();
            context.ExecuteNonQuery(command);

            var query = model.BuildSelectCommand<Customer>(c => c.CustomerKey == customerKey);
            customer2 = context.Query<Customer>(query).FirstOrDefault();
            customer2.CustomerId.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void ExecuteInsertThenSelectWithJoin()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<CustomerWithAddress>(tableName: "Customer")
                    .Column(c => c.CustomerId).IsIdentity()
                    .Column(c => c.CustomerKey).IsKey();

                builder.Entity<Address>()
                    .Column(a => a.AddressId).IsIdentity();
            });

            var customerKey = MakeTestKey();
            var customer = new CustomerWithAddress
            {
                CustomerKey = customerKey,
                Inception = DateTime.UtcNow,
                Change = 3,
                Modified = DateTimeOffset.UtcNow
            };

            var address = new Address { Change = 3 };
            var context = Repository.BuildContext(ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString);
            var insertCustomer = model.ForEntity<CustomerWithAddress>().BuildInsert(customer).ToCommand();
            customer.CustomerId = context.ExecuteScalar<int>(insertCustomer).GetValueOrDefault();
            address.CustomerId = customer.CustomerId;

            var insertAddress = model.ForEntity<Address>().BuildInsert(address).ToCommand();
            address.AddressId = context.ExecuteScalar<int>(insertAddress).GetValueOrDefault();

            var joinHandler = new JoinHandler<CustomerWithAddress>(model);
            joinHandler.GetMap().AddJoinSplit<Address>((c, a) => c.Address = a);

            var selectCommand = model
                .ForEntity<CustomerWithAddress>()
                .BuildSelect()
                .Join<Address>(JoinType.Inner, (c, a) => a.CustomerId == c.CustomerId)
                .Where(c => c.CustomerId == customer.CustomerId)
                .ToCommand();

            // join handler has a handle on a model...use the query command overload
            var customer2 =
                context.Query(selectCommand, joinHandler)
                .FirstOrDefault();

            customer2.Should().NotBeNull();
            customer2.CustomerId.Should().BeGreaterThan(0);
            customer2.Address.Should().NotBeNull();
        }

        [TestMethod]
        public void ExecuteInsertThenSelectUsingAlternateSchema()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>("dbo2", "Customer2")
                    .Column(c => c.CustomerId).IsIdentity()
                    .Column(c => c.CustomerKey).IsKey()
                    .Exclude(c => c.Inception)
                    .Exclude(c => c.Change)
                    .Exclude(c => c.Modified);
            });

            var customerKey = MakeTestKey();
            var customer = new Customer { CustomerKey = customerKey, Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow };

            var context = Repository.BuildContext(ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString);
            var command = model.ForEntity<Customer>().BuildInsert(customer).ToCommand();
            context.ExecuteNonQuery(command);

            var query = model.BuildSelectCommand<Customer>(c => c.CustomerKey == customerKey);
            var customer2 = context.Query(query, model).FirstOrDefault();
            customer2.CustomerId.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task EnumerateEntitiesAsyncCanEnumerateWithoutReaderClosingUnexpectedly()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>()
                    .Column(c => c.CustomerId).IsIdentity()
                    .Column(c => c.CustomerKey).IsKey();

                builder.Entity<Address>()
                    .Column(a => a.CustomerId).IsKey()
                    .Column(a => a.AddressId).IsIdentity();
            });

            var customerKey = MakeTestKey();
            var connection = ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString;
            using (var context = Repository.BuildContext(connection))
            {
                var customer1 = new Customer { CustomerKey = customerKey, Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow };
                var command = model.ForEntity<Customer>().BuildInsert(customer1).ToCommand();
                context.ExecuteNonQuery(command);

                var select = model.BuildSelectCommand<Customer>(c => c.CustomerKey == customerKey);
                var e = await context.QueryAsync(select);
                var customer2 = e.FirstOrDefault();

                customer2.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void ExecuteInsertThenSelectInBatch()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>()
                    .Column(c => c.CustomerId).IsIdentity()
                    .Column(c => c.CustomerKey).IsKey();

                builder.Entity<Address>()
                    .Column(a => a.CustomerId).IsKey()
                    .Column(a => a.AddressId).IsIdentity();
            });

            var customerKey = MakeTestKey();
            var batch = new QueryBatch();
            var customer = new Customer { CustomerKey = customerKey, Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow };
            var customer2 = default(Customer);

            var connection = ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString;
            var context = Repository.BuildContext(connection);
            var command = model.ForEntity<Customer>().BuildInsert(customer).ToCommand();
            context.ExecuteNonQuery(command);

            // Should the query already have the model associated?
            var select = model.BuildSelectCommand<Customer>(c => c.CustomerKey == customerKey);
            customer2 = context.Query(select).FirstOrDefault();

            customer2.Should().NotBeNull();
            customer2.CustomerId.Should().BeGreaterThan(0);

            var address = new Address { CustomerId = customer2.CustomerId, Change = 3 };
            var addressInsert = model.ForEntity<Address>().BuildInsert(address).ToCommand();
            context.ExecuteNonQuery(addressInsert);

            model.ForEntity<Customer>()
                .BuildSelect()
                .Where(c => c.CustomerId == customer2.CustomerId)
                .AddToBatch(batch);

            model.ForEntity<Address>()
                .BuildSelect()
                .Where(a => a.CustomerId == customer2.CustomerId)
                .AddToBatch(batch);

            using (var readerContext = Repository.BuildBatchReader(connection, batch))
            {
                readerContext.NextResult();
                var customers = readerContext.ReadEntitySet<Customer>(model);
                customers.Count.Should().Be(1);
                readerContext.NextResult();
                var addresses = readerContext.ReadEntitySet<Address>(model);
                addresses.Count.Should().Be(1);
                addresses[0].CustomerId.Should().Be(customers[0].CustomerId);
            }
        }

        [TestMethod]
        public void ExecuteInsertThenSelectThenUpdateThenSelect()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>()
                    .Column(c => c.CustomerId).IsIdentity()
                    .Column(c => c.CustomerKey).IsKey();
            });

            var customerKey = MakeTestKey();
            var customer = new Customer { CustomerKey = customerKey, Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow };
            var customer2 = default(Customer);

            var context = Repository.BuildContext(ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString);
            var command = model.ForEntity<Customer>().BuildInsert(customer).ToCommand();
            context.ExecuteNonQuery(command);

            var query = model.BuildSelectCommand<Customer>(c => c.CustomerKey == customerKey);
            customer2 = context.Query(query).FirstOrDefault();

            customer2.CustomerId.Should().BeGreaterThan(0);

            customer2.Change = 1;
            var update = model.ForEntity<Customer>().BuildUpdate(customer2, c => c.CustomerId == customer2.CustomerId).ToCommand();
            context.ExecuteNonQuery(update);

            var updatedCustomer = context.Query(query).FirstOrDefault();
            updatedCustomer.Change.Should().Be(1);
        }

        [TestMethod]
        public void ExecuteInsertThenSelectThenDeleteThenSelect()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>()
                    .Column(c => c.CustomerId).IsIdentity()
                    .Column(c => c.CustomerKey).IsKey().IsOfType(SqlDbType.Char, 20);
            });

            var customerKey = MakeTestKey();
            var customer = new Customer { CustomerKey = customerKey, Inception = DateTime.UtcNow, Change = 3, Modified = DateTimeOffset.UtcNow };
            var customer2 = default(Customer);

            var context = Repository.BuildContext(ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString);
            var command = model.ForEntity<Customer>().BuildInsert(customer).ToCommand();
            context.ExecuteNonQuery(command);

            var query = model.BuildSelectCommand<Customer>(c => c.CustomerKey == customerKey);
            customer2 = context.Query(query).FirstOrDefault();
            customer2.CustomerId.Should().BeGreaterThan(0);

            var delete = model.BuildDeleteCommand<Customer>(c => c.CustomerId == customer2.CustomerId);
            context.ExecuteNonQuery(delete);

            var deletedCustomer = context.Query(query).FirstOrDefault();
            deletedCustomer.Should().BeNull();
        }

        private static string MakeTestKey()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 20).ToUpper();
        }
    }
}
