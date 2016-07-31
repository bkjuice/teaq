using System;
using System.Data;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class BatchReaderContextTests
    {
        [TestMethod]
        public void EnumerateEntitySetReturnsEmptyWhenReaderIsNull()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup<IDbConnection>(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;

            IDataReader reader = null;
            command.Setup<IDataReader>(c => c.ExecuteReader()).Returns(reader);

            var model = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            var myConnection = "someConnectionString";
            using (var readerContext = Repository.BuildBatchReader(myConnection, batch, connectionBuilder.Object))
            {
                readerContext.EnumerateEntitySet<Customer>().Count().Should().Be(0);
            }
        }

        [TestMethod]
        public void EnumerateEntitySetAndThenReadEntitiesWithoutEnumeratingThrowsInvalidOperationException()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup<IDbConnection>(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });

            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup<IDataReader>(c => c.ExecuteReader()).Returns(tableHelper.GetReader());

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            var context = Repository.BuildBatchReader("test", batch, connectionBuilder.Object);
            context.NextResult();
            context.EnumerateEntitySet<Customer>();

            Action test = () => context.ReadEntitySet<Customer>();
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void EnumerateEntitySetReturnsResultsWithBatch()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup<IDbConnection>(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });

            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup<IDataReader>(c => c.ExecuteReader()).Returns(tableHelper.GetReader());

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            using (var context = Repository.BuildBatchReader("test", batch, connectionBuilder.Object))
            {
                context.NextResult();
                var result = context.EnumerateEntitySet<Customer>();
                result.Count().Should().Be(2);
            }
        }

        [TestMethod]
        public void InitializeDataContextSucceedsInUsingBlock()
        {
            var emptyModel = Repository.BuildModel(x => { });
            Action test = () =>
                {
                    using (var c = Repository.BuildBatchReader("test"))
                    {
                    }
                };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void DataContextSetQueryBatchSucceeds()
        {
            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            Action test = () =>
            {
                var c = Repository.BuildBatchReader("test", batch);
                c.QueryBatch.Should().BeSameAs(batch);
            };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void DataContextEntitySetThrowsArgumentNullExceptionWhenBatchIsNull()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var emptyModel = Repository.BuildModel(x => { });
            QueryBatch batch = null;
            Action test = () => Repository.BuildBatchReader("test", batch);
            test.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void DataContextEntitySetReturnsEmptyWithEmptyBatch()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup<IDbConnection>(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();

            using (var c = Repository.BuildBatchReader("test", batch))
            {
                var result = c.ReadEntitySet<Customer>();
                result.Count.Should().Be(0);
            }
        }

        [TestMethod]
        public void DataContextEntitySetReturnsResultsWithBatch()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup<IDbConnection>(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });

            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup<IDataReader>(c => c.ExecuteReader()).Returns(tableHelper.GetReader());

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            using (var context = Repository.BuildBatchReader("test", batch, connectionBuilder.Object))
            {
                context.NextResult();
                var result = context.ReadEntitySet<Customer>();
                result.Count.Should().Be(2);
            }
        }

        ////[TestMethod]
        ////public void DataContextSqlQueryReturnsExpectedResults()
        ////{
        ////    var connectionStub = new DbConnectionStub();
        ////    var connectionBuilder = new Mock<IConnectionBuilder>();
        ////    connectionBuilder.Setup<IDbConnection>(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

        ////    var tableHelper = new EntityTableHelper<Customer>();
        ////    tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
        ////    tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });

        ////    var command = new Mock<IDbCommand>();
        ////    connectionStub.MockCommand = command.Object;
        ////    command.Setup<IDataReader>(mockCommand => mockCommand.ExecuteReader()).Returns(tableHelper.GetReader());

        ////    var c = Repository.BuildContext("test", connectionBuilder.Object);
        ////    var result = c.Query<Customer>("test", (IDataHandler<Customer>)null);
        ////    result.Count.Should().Be(2);
        ////}

        [TestMethod]
        public void DataContextOpenConnectionIsClosed()
        {
            var connectionStub = new DbConnectionStub();
            connectionStub.State = ConnectionState.Open;
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup<IDbConnection>(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = new EntityTableHelper<Customer>();

            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup<IDataReader>(c => c.ExecuteReader()).Returns(tableHelper.GetReader());

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            using (var context = Repository.BuildBatchReader("test", batch, connectionBuilder.Object))
            {
                context.NextResult();
                var result = context.ReadEntitySet<Customer>();
            }

            connectionStub.CloseInvoked.Should().BeTrue();
        }
    }
}
