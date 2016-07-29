using System;
using System.Data;
using System.Data.SqlClient;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.QueryGeneration;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class QueryExtensionsTests
    {
        [TestMethod]
        public void ExtensionBuildUnboundedQueryMatchesResultsForExpectedClauseAndParameters()
        {
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildSelect().ToCommand();
            var queryCommand2 = Repository.Default.BuildUnboundedSelectCommand<Customer>();
            queryCommand2.CommandText.Should().Be(queryCommand.CommandText);
            queryCommand2.ParameterCount.Should().Be(queryCommand.ParameterCount);
        }

        [TestMethod]
        public void ExtensionBuildQueryMatchesResultsForExpectedClauseAndParameters()
        {
            var clientId = 5;
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildSelect().Where(c => c.CustomerId == clientId).ToCommand();
            var queryCommand2 = Repository.Default.BuildSelectCommand<Customer>(c => c.CustomerId == clientId);
            queryCommand2.CommandText.Should().Be(queryCommand.CommandText);
            queryCommand2.ParameterCount.Should().Be(queryCommand.ParameterCount);
        }

        [TestMethod]
        public void ExtensionBuildQueryWithColumnListMatchesResultsForExpectedClauseAndParameters()
        {
            var clientId = 5;
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildSelect(columns: "CustomerKey").Where(c => c.CustomerId == clientId).ToCommand();
            var queryCommand2 = Repository.Default.BuildSelectCommand<Customer>(c => c.CustomerId == clientId, "CustomerKey");
            queryCommand2.CommandText.Should().Be(queryCommand.CommandText);
            queryCommand2.ParameterCount.Should().Be(queryCommand.ParameterCount);
        }

        [TestMethod]
        public void ExtensionBuildDeleteResultsForExpectedClauseAndParameters()
        {
            var clientId = 5;
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildDelete(c => c.CustomerId == clientId).ToCommand();
            var queryCommand2 = Repository.Default.BuildDeleteCommand<Customer>(c => c.CustomerId == clientId);
            queryCommand2.CommandText.Should().Be(queryCommand.CommandText);
            queryCommand2.ParameterCount.Should().Be(queryCommand.ParameterCount);
        }

        [TestMethod]
        public void ExtensionAddToQueryBatchSucceeds()
        {
            var batch = new QueryBatch();
            batch.Add("test", true);

            batch.HasBatch.Should().BeTrue();
            var command = batch.NextBatch();

            command.CommandText.Should().Be("test");
            command.ParameterCount.Should().Be(0);
        }

        [TestMethod]
        public void ExtensionAddToQueryReaderBatchSucceeds()
        {
            var batch = new QueryBatch();
            batch.Add<Customer>("test", true);

            batch.HasBatch.Should().BeTrue();
            var command = batch.NextBatch();

            command.CommandText.Should().Be("test");
            command.ParameterCount.Should().Be(0);
        }

        [TestMethod]
        public void BuildTextQueryCommandReturnsExpectedQueryTextWithLeadingDelimiter()
        {
            var commandText = "select * from mytable where Id=@EMPLOYEEID and DealerId=@DealerId";
            var expected = "\r\nselect * from mytable where Id=@EMPLOYEEID1 and DealerId=@DealerId1";
            var command = new SqlCommand(commandText);
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@EMPLOYEEID", 1);
            command.Parameters.AddWithValue("@DealerId", 1);

            var batch = new QueryBatch();
            var result = batch.BuildTextQueryCommand(command, true);
            result.CommandText.Should().Be(expected);
        }

        [TestMethod]
        public void BuildSprocQueryCommandReturnsExpectedQueryTextWithLeadingDelimiter()
        {
            var expected = "\r\nexec sp_bw_dropdown_EmployeeEmail @EMPLOYEEID=@EMPLOYEEID1, @DealerId=@DealerId1";
            var command = new SqlCommand("sp_bw_dropdown_EmployeeEmail");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@EMPLOYEEID", 1);
            command.Parameters.AddWithValue("@DealerId", 1);

            var batch = new QueryBatch();
            var result = batch.BuildSprocQueryCommand(command, true);

            result.CommandText.Should().Be(expected);
        }

        [TestMethod]
        public void BuildSprocQueryCommandThrowsInvalidOperationExceptionWhenCommandIsNotStoredProcedure()
        {
            var command = new SqlCommand("sp_bw_dropdown_EmployeeEmail");
            command.CommandType = CommandType.Text;
            var batch = new QueryBatch();
            Action test = () => batch.BuildSprocQueryCommand(command, true);
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void BuildSprocQueryCommandBuildsParametersAsExpected()
        {
            var command = new SqlCommand("sp_bw_dropdown_EmployeeEmail");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@EMPLOYEEID", 99);
            command.Parameters.AddWithValue("@DealerId", 100);

            var batch = new QueryBatch();
            var result = batch.BuildSprocQueryCommand(command, true);
            result.ParameterCount.Should().Be(2);
            var qualifier = batch.CurrentBatchIndex;
            var parameters = result.GetParameters();
            parameters[0].ParameterName.Should().Be("@EMPLOYEEID1");
            parameters[0].Value.Should().BeOfType<int>();
            ((int)parameters[0].Value).Should().Be(99);
            parameters[1].ParameterName.Should().Be("@DealerId1");
            parameters[1].Value.Should().BeOfType<int>();
            ((int)parameters[1].Value).Should().Be(100);
        }
    }
}
