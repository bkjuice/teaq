﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Teaq.Fakes;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class DataContextTests
    {
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
