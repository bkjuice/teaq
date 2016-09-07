using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class DataContextTests
    {
        [TestMethod]
        public async Task QueryNullableValuesAsyncReturnsExpectedValuesWithNulls()
        {
            var tableHelper = BuildNullableValueTable<int>(1, null, 3, null, 5);
            DbConnectionStub connectionStub;
            var connectionBuilder = BuildConnectionMock(out connectionStub);

            var target = new DbCommandStub();
            connectionStub.MockCommand = target;
            target.ExecuteReaderWithBehaviorCallback = b => tableHelper.GetReader();

            using (var context = Repository.BuildContext("test", connectionBuilder.Object))
            {
                var results = await context.QueryNullableValuesAsync<int>("blah, blah blah", new { Id = 5 });
                var i = 1;
                foreach (var x in results)
                {
                    if (i % 2 == 0) x.Should().Be(null);
                    else x.Should().Be(i);
                    i++;
                }

                i.Should().BeGreaterThan(4);
            }
        }

        [TestMethod]
        public void QueryNullableValuesReturnsExpectedValuesWithNulls()
        {
            var tableHelper = BuildNullableValueTable<int>(1, null, 3, null, 5);
            DbConnectionStub connectionStub;
            var connectionBuilder = BuildConnectionMock(out connectionStub);

            var target = new DbCommandStub();
            connectionStub.MockCommand = target;
            target.ExecuteReaderCallback = () => tableHelper.GetReader();

            using (var context = Repository.BuildContext("test", connectionBuilder.Object))
            {
                var result = context.QueryNullableValues<int>("blah, blah blah", new { Id = 5 });
                result.Count.Should().Be(5);
                result[0] = 1;
                result[1] = null;
                result[2] = 3;
                result[3] = null;
                result[4] = 5;
            }
        }

        [TestMethod]
        public void QueryValuesReturnsExpectedValues()
        {
            var tableHelper = BuildValueTable<int>(1, 2, 3, 4, 5);
            DbConnectionStub connectionStub;
            var connectionBuilder = BuildConnectionMock(out connectionStub);

            var target = new DbCommandStub();
            connectionStub.MockCommand = target;
            target.ExecuteReaderCallback = () => tableHelper.GetReader();

            using (var context = Repository.BuildContext("test", connectionBuilder.Object))
            {
                var result = context.QueryValues<int>("blah, blah blah", new { Id = 5 });
                result.Count.Should().Be(5);
                result[0] = 1;
                result[1] = 2;
                result[2] = 3;
                result[3] = 4;
                result[4] = 5;
            }
        }

        [TestMethod]
        public async Task QueryValuesAsyncReturnsExpectedValues()
        {
            var tableHelper = BuildValueTable<int>(1, 2, 3, 4, 5);
            DbConnectionStub connectionStub;
            var connectionBuilder = BuildConnectionMock(out connectionStub);

            var target = new DbCommandStub();
            connectionStub.MockCommand = target;
            target.ExecuteReaderWithBehaviorCallback = b => tableHelper.GetReader();

            using (var context = Repository.BuildContext("test", connectionBuilder.Object))
            {
                var results = await context.QueryValuesAsync<int>("blah, blah blah", new { Id = 5 });

                // Once the results are enumerated, the enumeration doesn't reset. Forward only:
                var i = 1;
                foreach(var x in results)
                {
                    x.Should().Be(i);
                    i++;
                }

                i.Should().BeGreaterThan(4);
            }
        }

        [TestMethod]
        public void QueryUsingInlineStringInvokesQueryWithExpectedParameters()
        {
            var model = BuildTestModel();
            var tableHelper = BuildTestTable();

            DbConnectionStub connectionStub;
            var connectionBuilder = BuildConnectionMock(out connectionStub);

            var target = new DbCommandStub();
            connectionStub.MockCommand = target;
            using (var context = Repository.BuildContext("test", connectionBuilder.Object))
            {
                context.Query<Customer>("select * from customers where CustomerId = @Id", new { Id = 5 });
            }

            target.CommandText.Should().Be("select * from customers where CustomerId = @Id");
            target.Parameters.Count.Should().Be(1);

            var targetParam = target.Parameters[0] as IDbDataParameter;
            targetParam.Should().NotBeNull();
            targetParam.ParameterName.Should().Be("@Id");
            targetParam.Value.Should().Be(5);
        }

        [TestMethod]
        public async Task QueryAsyncUsingInlineStringInvokesQueryWithExpectedParameters()
        {
            var model = BuildTestModel();
            var tableHelper = BuildTestTable();

            DbConnectionStub connectionStub;
            var connectionBuilder = BuildConnectionMock(out connectionStub);

            var target = new DbCommandStub();
            connectionStub.MockCommand = target;
            using (var context = Repository.BuildContext("test", connectionBuilder.Object))
            {
                await context.QueryAsync<Customer>("select * from customers where CustomerId = @CustomerId", new { CustomerId = 15 });
            }

            target.CommandText.Should().Be("select * from customers where CustomerId = @CustomerId");
            target.Parameters.Count.Should().Be(1);

            var targetParam = target.Parameters[0] as IDbDataParameter;
            targetParam.Should().NotBeNull();
            targetParam.ParameterName.Should().Be("@CustomerId");
            targetParam.Value.Should().Be(15);
        }

        [TestMethod]
        public void QueryUsesQueryCommandModelForExplicitMappingImplicitly()
        {
            var model = BuildTestModel();
            var tableHelper = BuildTestTable();

            DbConnectionStub connectionStub;
            var connectionBuilder = BuildConnectionMock(out connectionStub);
            SetupCommandMock(tableHelper, connectionStub);

            using (var context = Repository.BuildContext("test", connectionBuilder.Object))
            {
                var query = model.ForEntity<CustomerWithMappedKey>().BuildSelect().ToCommand();
                var entities = context.Query(query);
                entities.Count.Should().Be(2);
                entities[0].Key.Should().Be("1");
                entities[1].Key.Should().Be("2");
            }
        }

        [TestMethod]
        public async Task QueryAsyncUsesQueryCommandModelForExplicitMappingImplicitly()
        {
            var model = BuildTestModel();
            var tableHelper = BuildTestTable();

            DbConnectionStub connectionStub;
            var connectionBuilder = BuildConnectionMock(out connectionStub);
            SetupCommandMock(tableHelper, connectionStub);

            using (var context = Repository.BuildContext("test", connectionBuilder.Object))
            {
                var query = model.ForEntity<CustomerWithMappedKey>().BuildSelect().ToCommand();
                var entities = (await context.QueryAsync(query)).ToList();
                entities.Count.Should().Be(2);
                entities[0].Key.Should().Be("1");
                entities[1].Key.Should().Be("2");
            }
        }

        private static Mock<IDbCommand> SetupCommandMock(EntityTableHelper<Customer> tableHelper, DbConnectionStub connectionStub)
        {
            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup(c => c.ExecuteReader()).Returns(tableHelper.GetReader()).Verifiable();
            command.Setup(c => c.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(tableHelper.GetReader()).Verifiable();
            command.SetupGet(c => c.Connection).Returns(connectionStub);
            return command;
        }

        private static Mock<IConnectionBuilder> BuildConnectionMock(out DbConnectionStub connectionStub)
        {
            connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);
            return connectionBuilder;
        }

        private static EntityTableHelper<Customer> BuildTestTable()
        {
            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            return tableHelper;
        }

        private static ValueTableHelper<T> BuildValueTable<T>(params T[] values)
        {
            var tableHelper = new ValueTableHelper<T>();
            foreach (var t in values)
            {
                tableHelper.AddRow(t);
            }

            return tableHelper;
        }

        private static NullableValueTableHelper<T> BuildNullableValueTable<T>(params T?[] values) where T: struct
        {
            var tableHelper = new NullableValueTableHelper<T>();
            foreach (var t in values)
            {
                tableHelper.AddRow(t);
            }

            return tableHelper;
        }

        private static IDataModel BuildTestModel()
        {
            return Repository.BuildModel(
                m =>
                {
                    m.Entity<CustomerWithMappedKey>()
                        .Column(e => e.Key).HasMapping("CustomerKey");
                });
        }
    }
}
