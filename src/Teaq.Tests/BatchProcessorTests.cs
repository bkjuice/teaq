using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Teaq.Tests
{
    [TestClass]
    public class BatchProcessorTests
    {
        [TestMethod]
        public void AddCommandThrowsWhenCommandIsNull()
        {
            var processor = new BatchProcessor();
            SqlCommand command = null;
            Action test = () => processor.AddCommand(command, r => true);
            test.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void AddCommandThrowsWhenHandlerFuncIsNull()
        {
            var processor = new BatchProcessor();
            SqlCommand command = new SqlCommand();
            Action test = () => processor.AddCommand(command, null);
            test.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void AddCommandAddsExecStatementForStoredProcedureBatch()
        {
            var processor = new BatchProcessor();
            SqlCommand command = new SqlCommand("select_betterMouseTrap");
            command.CommandType = CommandType.StoredProcedure;
            processor.AddCommand(command, r => true);

            processor.QueryBatch.NextBatch().CommandText.Should().Contain("exec select_betterMouseTrap");
        }

        [TestMethod]
        public void AddCommandHandlesMixOfExecAndTextCommands()
        {
            var processor = new BatchProcessor();
            SqlCommand command = new SqlCommand("select_betterMouseTrap");
            command.CommandType = CommandType.StoredProcedure;
            processor.AddCommand(command, r => true);
            SqlCommand command2 = new SqlCommand("select iron from mars");
            processor.AddCommand(command2, r => true);
            var batch = processor.QueryBatch.NextBatch();
            batch.CommandText.Should().Contain("exec select_betterMouseTrap");
            batch.CommandText.Should().Contain("select iron from mars");
            batch.CommandText.Should().NotContain("exec select iron from mars");
        }

        [TestMethod]
        public void ExecuteBatchThrowsArgumentNullExceptionWhenConnectionIsNull()
        {
            var processor = new BatchProcessor();
            Action test = () => processor.ExecuteBatch(null);
            test.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void ExecuteBatchThrowsArgumentNullExceptionWhenConnectionIsEmpty()
        {
            var processor = new BatchProcessor();
            Action test = () => processor.ExecuteBatch(string.Empty);
            test.ShouldThrow<ArgumentNullException>();
        }
    }
}
