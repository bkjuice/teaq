using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class DataConnectionExtensionTests
    {
        [TestMethod]
        public async Task EnumerateEntitiesAsyncReturnsEnumerableEntitiesAsExpected()
        {
            var tableHelper = new TableHelper("Id");
            tableHelper.AddRow(1);
            tableHelper.AddRow(2);
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            command.Setup(c => c.ExecuteReader(It.IsAny<CommandBehavior>()))
                .Returns(() =>
                    tableHelper.GetReader()
                );

            connection.Setup(c => c.CreateCommand()).Returns(command.Object);
            command.SetupGet(c => c.Connection).Returns(connection.Object);

            var enumerable = await command.Object.EnumerateEntitiesAsync<int>();
            enumerable.Count().Should().Be(2);
        }

        [TestMethod]
        public void EnumerateEntitiesReturnsEnumerableEntitiesAsExpected()
        {
            var tableHelper = new TableHelper("Id");
            tableHelper.AddRow(1);
            tableHelper.AddRow(2);
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            command.Setup(c => c.ExecuteReader())
                .Returns(()=>
                    tableHelper.GetReader()
                );

            connection.Setup(c => c.CreateCommand()).Returns(command.Object);
            command.SetupGet(c => c.Connection).Returns(connection.Object);

            command.Object.EnumerateEntities<int>().Count().Should().Be(2);
        }

        [TestMethod]
        public void BuildCommandSetsCommandTextAsExpected()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.SetupProperty(c => c.CommandText);

            var actual = connection.Object.BuildTextCommand("some query");
            actual.CommandText.Should().Be("some query");
        }

        [TestMethod]
        public void BuildCommandIgnoresNullParametersAsExpected()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            var parameters = new Mock<IDataParameterCollection>();
            var actualParameters = new List<object>();

            parameters.Setup(p => p.Add(It.IsAny<object>())).Callback<object>(o => actualParameters.Add(o));
            connection.Setup(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.SetupGet(c => c.Parameters).Returns(parameters.Object);

            connection.Object.BuildTextCommand("test", null);
            actualParameters.Count.Should().Be(0);
        }

        [TestMethod]
        public void BuildCommandAddsParametersAsExpected()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            var parameters = new Mock<IDataParameterCollection>();
            var actualParameters = new List<object>();

            parameters.Setup(p => p.Add(It.IsAny<object>())).Callback<object>(o => actualParameters.Add(o));
            connection.Setup(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.SetupGet(c => c.Parameters).Returns(parameters.Object);

            connection.Object.BuildTextCommand("some query", (new object[] { 1, "two" }).Select((o, i) => o.AsDbParameter($"@p{i}")));
            actualParameters.Count.Should().Be(2);
        }

        [TestMethod]
        public void CommandOpenChecksConnectionState()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.SetupGet(c => c.State).Returns(ConnectionState.Open).Verifiable();
            command.SetupGet(c => c.Connection).Returns(connection.Object);
            command.Object.Open();

            connection.VerifyGet(c => c.State, Times.Once());
        }

        [TestMethod]
        public void CommandOpenOpensConnectionIfNotOpen()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.SetupGet(c => c.State).Returns(ConnectionState.Closed);
            connection.Setup(c => c.Open()).Verifiable();
            command.SetupGet(c => c.Connection).Returns(connection.Object);

            command.Object.Open();
            connection.Verify(c => c.Open(), Times.Once());
        }

        [TestMethod]
        public void CommandOpenDoesNotOpenConnectionIfOpen()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.SetupGet(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.Open()).Verifiable();
            command.SetupGet(c => c.Connection).Returns(connection.Object);

            command.Object.Open();
            connection.Verify(c => c.Open(), Times.Never());
        }

        [TestMethod]
        public void ReadEntityOfTInvokesExecuteReader()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.Setup(c => c.ExecuteReader()).Verifiable();
            command.SetupGet(c => c.Connection).Returns(connection.Object);

            command.Object.ReadEntities<object>();
            command.Verify(c => c.ExecuteReader(), Times.Once());
        }

        [TestMethod]
        public void ReadFirstEntityReturnsDefault()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            var reader = new Mock<IDataReader>();

            connection.Setup(c => c.CreateCommand()).Returns(command.Object);
            command.Setup(c => c.ExecuteReader()).Returns(reader.Object);
            command.SetupGet(c => c.Connection).Returns(connection.Object);
            command.Object.ReadFirstEntity<object>().Should().BeNull();
        }

        [TestMethod]
        public void ReadFirstEntityReturnsFirstRow()
        {
            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });

            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup(c => c.CreateCommand()).Returns(command.Object);
            command.Setup(c => c.ExecuteReader()).Returns(tableHelper.GetReader());
            command.SetupGet(c => c.Connection).Returns(connection.Object);

            var entity = command.Object.ReadFirstEntity<Customer>();
            entity.Should().NotBeNull();
            entity.CustomerId.Should().Be(1);
        }
    }
}