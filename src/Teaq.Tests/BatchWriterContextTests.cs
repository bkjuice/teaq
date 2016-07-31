using System.Data;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class BatchWriterContextTests
    {
        [TestMethod]
        public void BatchWriterContextExecutesNonQuery()
        {
            Mock<IDbCommand> command;
            var builder = SetupConnectionBuilder(out command);
            command.Setup(c => c.ExecuteNonQuery()).Returns(0).Verifiable();

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            using (var context = Repository.BuildBatchWriter("test", batch, builder.Object))
            {
                context.SaveChanges();
            }

            command.Verify(c => c.ExecuteNonQuery(), Times.Once());
        }

        [TestMethod]
        public async Task BatchWriterContextExecutesNonQueryAsync()
        {
            Mock<IDbCommand> command;
            var builder = SetupConnectionBuilder(out command);
            command.Setup(c => c.ExecuteNonQuery()).Returns(0).Verifiable();

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            using (var context = Repository.BuildBatchWriter("test", batch, builder.Object))
            {
                await context.SaveChangesAsync();
            }

            command.Verify<int>(c => c.ExecuteNonQuery(), Times.Once());
        }

        private Mock<IConnectionBuilder> SetupConnectionBuilder(out Mock<IDbCommand> command)
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.SetupGet(c => c.Connection).Returns(connectionStub);

            return connectionBuilder;
        }
    }
}
