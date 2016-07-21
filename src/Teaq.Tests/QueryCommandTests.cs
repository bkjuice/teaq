using System.Data.SqlClient;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.QueryGeneration;

namespace Teaq.Tests
{
    /// <summary>
    /// Summary description for CommandTests
    /// </summary>
    [TestClass]
    public class QueryCommandTests
    {
        [TestMethod]
        public void QueryCommandInitializesAsExpected()
        {
            var command = new QueryCommand("test", new SqlParameter[] { new SqlParameter() }, false);
            command.CommandText.Should().Be("test");
            command.CanSplitBatch.Should().BeFalse();
            command.ParameterCount.Should().Be(1);
        }

        [TestMethod]
        public void QueryCommandEmptyEqualsEmptyInstance()
        {
            var command = new QueryCommand();
            command.IsEmpty.Should().BeTrue();
        }

        [TestMethod]
        public void QueryCommandWithTextNotEmpty()
        {
            var command = new QueryCommand("test");
            command.IsEmpty.Should().BeFalse();
        }

        [TestMethod]
        public void QueryCommandDoesNotEqualNull()
        {
            var command = new QueryCommand();
            command.Equals(null).Should().BeFalse();
        }

        [TestMethod]
        public void QueryCommandDoesNotEqualCommandWithTextDifference()
        {
            var command1 = new QueryCommand("test1");
            var command2 = new QueryCommand("test2");
            command1.Equals(command2).Should().BeFalse();
        }

        [TestMethod]
        public void QueryCommandDoesNotEqualCommandWithSplitBatchDifference()
        {
            var command1 = new QueryCommand("", false);
            var command2 = new QueryCommand("", true);
            command1.Equals(command2).Should().BeFalse();
        }
    }
}
