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
    public class BatchReaderTests
    {
        [TestMethod]
        public void EnumerateEntitySetReturnsEmptyWhenReaderIsNull()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;

            IDataReader reader = null;
            command.Setup(c => c.ExecuteReader()).Returns(reader);

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
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = Build2CustomerTable();
            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup(c => c.ExecuteReader()).Returns(tableHelper.GetReader());

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
        public async Task EnumerateEntitySetReturnsResultsWithBatchAsync()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = Build2CustomerTable();
            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup(c => c.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(tableHelper.GetReader());

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            using (var context = Repository.BuildBatchReader("test", batch, connectionBuilder.Object))
            {
                var hasResults = await context.NextResultAsync();
                hasResults.Should().BeTrue();
                var result = context.EnumerateEntitySet<Customer>();
                result.Count().Should().Be(2);
            }
        }

        [TestMethod]
        public async Task EnumerateEntitySetDoesNotInvokeHandlerWhenItemsNotRead()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = Build2CustomerTable();
            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup(c => c.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(tableHelper.GetReader());

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            var handlerInvoked = false;
            var handler = new DelegatingReaderHandler<Customer>(r =>
            {
                handlerInvoked = true;
                return new Customer(); // Don't care about actually reading anything here...
            });

            using (var context = Repository.BuildBatchReader("test", batch, connectionBuilder.Object))
            {
                var hasResults = await context.NextResultAsync();
                hasResults.Should().BeTrue();
                var result = context.EnumerateEntitySet(handler);
                result.Count().Should().Be(2);
                handlerInvoked.Should().BeFalse(); // <- Count() doesn't actually touch the items. If you step through the code in a debugger and check the current property of the iterator, you may trigger a false negative.
            }
        }

        [TestMethod]
        public async Task EnumerateEntitySetDoesInvokeHandlerWhenItemsRead()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = Build2CustomerTable();
            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup(c => c.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(tableHelper.GetReader());

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            var handlerInvoked = false;
            var handler = new DelegatingReaderHandler<Customer>(r =>
            {
                handlerInvoked = true;
                return new Customer(); // Don't care about actually reading anything here...
            });

            using (var context = Repository.BuildBatchReader("test", batch, connectionBuilder.Object))
            {
                var hasResults = await context.NextResultAsync();
                hasResults.Should().BeTrue();
                var result = context.EnumerateEntitySet(handler).ToList(); // <- forces materialization
                result.Count().Should().Be(2);
                handlerInvoked.Should().BeTrue(); 
            }
        }

        [TestMethod]
        public void EnumerateEntitySetReturnsResultsWithBatch()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = Build2CustomerTable();
            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup(c => c.ExecuteReader()).Returns(tableHelper.GetReader());

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
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

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
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = Build2CustomerTable();
            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup(c => c.ExecuteReader()).Returns(tableHelper.GetReader());

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

        [TestMethod]
        public void DataContextSqlQueryReturnsExpectedResults()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = Build2CustomerTable();
            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup(mockCommand => mockCommand.ExecuteReader()).Returns(tableHelper.GetReader());
            command.SetupGet(c => c.Connection).Returns(connectionStub);

            var context = Repository.BuildContext("test", connectionBuilder.Object);

            var query = Repository.Default.ForEntity<Customer>().BuildSelect().ToCommand();
            var result = context.Query(query);
            result.Count.Should().Be(2);
        }

        [TestMethod]
        public void DataContextOpenConnectionIsClosed()
        {
            var connectionStub = new DbConnectionStub();
            connectionStub.State = ConnectionState.Open;
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var tableHelper = new EntityTableHelper<Customer>();

            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup(c => c.ExecuteReader()).Returns(tableHelper.GetReader());
            command.SetupGet(c => c.Connection).Returns(connectionStub);

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

        private EntityTableHelper<Customer> Build2CustomerTable()
        {
            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            return tableHelper;
        }
    }
}
