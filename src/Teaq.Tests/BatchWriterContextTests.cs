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
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup<IDbConnection>(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup<int>(c => c.ExecuteNonQuery()).Returns(0).Verifiable();

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            using (var context = Repository.BuildBatchWriter("test", batch, connectionBuilder.Object))
            {
                context.SaveChanges();
            }

            command.Verify<int>(c => c.ExecuteNonQuery(), Times.Once());
        }

        [TestMethod]
        public async Task BatchWriterContextExecutesNonQueryAsync()
        {
            var connectionStub = new DbConnectionStub();
            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup<IDbConnection>(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var command = new Mock<IDbCommand>();
            connectionStub.MockCommand = command.Object;
            command.Setup<int>(c => c.ExecuteNonQuery()).Returns(0).Verifiable();

            var emptyModel = Repository.BuildModel(x => { });
            var batch = new QueryBatch();
            batch.Add<Customer>("test");

            using (var context = Repository.BuildBatchWriter("test", batch, connectionBuilder.Object))
            {
                await context.SaveChangesAsync();
            }

            command.Verify<int>(c => c.ExecuteNonQuery(), Times.Once());
        }
    }
}
