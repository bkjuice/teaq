using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            var enumerable = await connection.Object.EnumerateEntitiesAsync<int>("test", new object[0]);
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

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object);
            connection.Object.EnumerateEntities<int>("test", new object[0]).Count().Should().Be(2);
        }

        [TestMethod]
        public void ExecuteNonQueryCreatesCommandAsExpected()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            connection.Object.ExecuteNonQuery("some query", null);
            connection.Verify<IDbCommand>(c => c.CreateCommand(), Times.Once());
        }

        [TestMethod]
        public void ExecuteNonQuerySetsCommandTextAsExpected()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.SetupProperty<string>(c => c.CommandText);

            connection.Object.ExecuteNonQuery("some query", null);
            command.Object.CommandText.Should().Be("some query");
        }

        [TestMethod]
        public void ExecuteNonQueryIgnoresNullParametersAsExpected()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            var parameters = new Mock<IDataParameterCollection>();
            var actualParameters = new List<object>();

            parameters.Setup(p => p.Add(It.IsAny<object>())).Callback<object>(o => actualParameters.Add(o));
            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.SetupGet<IDataParameterCollection>(c => c.Parameters).Returns(parameters.Object);

            connection.Object.ExecuteNonQuery("some query", null);
            actualParameters.Count.Should().Be(0);
        }

        [TestMethod]
        public void ExecuteNonQueryAddsParametersAsExpected()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            var parameters = new Mock<IDataParameterCollection>();
            var actualParameters = new List<object>();

            parameters.Setup(p => p.Add(It.IsAny<object>())).Callback<object>(o => actualParameters.Add(o));
            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.SetupGet<IDataParameterCollection>(c => c.Parameters).Returns(parameters.Object);

            connection.Object.ExecuteNonQuery("some query", new object[] { 1, "two" });
            actualParameters.Count.Should().Be(2);
        }

        [TestMethod]
        public void ExecuteNonQueryChecksConnectionState()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            connection.SetupGet<ConnectionState>(c => c.State).Returns(ConnectionState.Open).Verifiable();
            connection.Object.ExecuteNonQuery("some query", null);

            connection.VerifyGet<ConnectionState>(c => c.State, Times.Once());
        }

        [TestMethod]
        public void ExecuteNonQueryOpensConnectionIfNotOpen()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            connection.SetupGet<ConnectionState>(c => c.State).Returns(ConnectionState.Closed);
            connection.Setup(c => c.Open()).Verifiable();

            connection.Object.ExecuteNonQuery("some query", null);

            connection.Verify(c => c.Open(), Times.Once());
        }

        [TestMethod]
        public void ExecuteNonQueryDoesNotOpenConnectionIfOpen()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            connection.SetupGet<ConnectionState>(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.Open()).Verifiable();

            connection.Object.ExecuteNonQuery("some query", null);

            connection.Verify(c => c.Open(), Times.Never());
        }

        [TestMethod]
        public void ExecuteNonQueryInvokesCommand()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.Setup<int>(c => c.ExecuteNonQuery()).Verifiable();

            connection.Object.ExecuteNonQuery("some query", null);
            command.Verify(c => c.ExecuteNonQuery(), Times.Once());
        }

        [TestMethod]
        public void ReadEntityOfTCreatesCommandAsExpected()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            connection.Object.ReadEntities<object>("some query", null);
            connection.Verify<IDbCommand>(c => c.CreateCommand(), Times.Once());
        }

        [TestMethod]
        public void ReadEntityOfTSetsCommandTextAsExpected()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.SetupProperty<string>(c => c.CommandText);

            connection.Object.ReadEntities<object>("some query", null);
            command.Object.CommandText.Should().Be("some query");
        }

        [TestMethod]
        public void ReadEntityOfTIgnoresNullParametersAsExpected()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            var parameters = new Mock<IDataParameterCollection>();
            var actualParameters = new List<object>();

            parameters.Setup(p => p.Add(It.IsAny<object>())).Callback<object>(o => actualParameters.Add(o));
            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.SetupGet<IDataParameterCollection>(c => c.Parameters).Returns(parameters.Object);


            connection.Object.ReadEntities<object>("some query", null);
            actualParameters.Count.Should().Be(0);
        }

        [TestMethod]
        public void ReadEntityOfTAddsParametersAsExpected()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            var parameters = new Mock<IDataParameterCollection>();
            var actualParameters = new List<object>();

            parameters.Setup(p => p.Add(It.IsAny<object>())).Callback<object>(o => actualParameters.Add(o));
            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.SetupGet<IDataParameterCollection>(c => c.Parameters).Returns(parameters.Object);

            connection.Object.ReadEntities<object>("some query", new object[] { 1, "two" });
            actualParameters.Count.Should().Be(2);
        }

        [TestMethod]
        public void ReadEntityOfTChecksConnectionState()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            connection.SetupGet<ConnectionState>(c => c.State).Returns(ConnectionState.Open).Verifiable();
            connection.Object.ReadEntities<object>("some query", null);

            connection.VerifyGet<ConnectionState>(c => c.State, Times.Once());
        }

        [TestMethod]
        public void ReadEntityOfTOpensConnectionIfNotOpen()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            connection.SetupGet<ConnectionState>(c => c.State).Returns(ConnectionState.Closed);
            connection.Setup(c => c.Open()).Verifiable();

            connection.Object.ReadEntities<object>("some query", null);

            connection.Verify(c => c.Open(), Times.Once());
        }

        [TestMethod]
        public void ReadEntityOfTDoesNotOpenConnectionIfOpen()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            connection.SetupGet<ConnectionState>(c => c.State).Returns(ConnectionState.Open);
            connection.Setup(c => c.Open()).Verifiable();

            connection.Object.ReadEntities<object>("some query", null);

            connection.Verify(c => c.Open(), Times.Never());
        }

        [TestMethod]
        public void ReadEntityOfTInvokesExecuteReader()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object).Verifiable();
            command.Setup(c => c.ExecuteReader()).Verifiable();

            connection.Object.ReadEntities<object>("some query", null);
            command.Verify(c => c.ExecuteReader(), Times.Once());
        }

        [TestMethod]
        public void ReadFirstEntityReturnsDefault()
        {
            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            var reader = new Mock<IDataReader>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object);
            command.Setup(c => c.ExecuteReader()).Returns(reader.Object);
            connection.Object.ReadFirstEntity<object>("some query", null).Should().BeNull();
        }

        [TestMethod]
        public void ReadFirstEntityReturnsFirstRow()
        {
            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });

            var connection = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();

            connection.Setup<IDbCommand>(c => c.CreateCommand()).Returns(command.Object);
            command.Setup(c => c.ExecuteReader()).Returns(tableHelper.GetReader());
            var entity = connection.Object.ReadFirstEntity<Customer>("some query", null);
            entity.Should().NotBeNull();
            entity.CustomerId.Should().Be(1);
        }
    }
}