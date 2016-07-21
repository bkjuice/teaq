using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.Tests;
using FluentAssertions;
using System.Configuration;
using System.Threading.Tasks;

namespace Teaq.IntegrationTests
{
    [TestClass]
    public class DataBatchProcessorTests
    {
        [TestMethod]
        public void ExecSelectAddressAndCustomerDoesNotFailAndInvokesHandlersInOrderWhenTrueIsReturned()
        {
            var count = 0;
            var processor = new BatchProcessor();
            var execCommand = new SqlCommand("select_address");
            execCommand.CommandType = CommandType.StoredProcedure;

            // Not testing the results, just the invocation and the handler callback order:
            execCommand.Parameters.AddWithValue("@addressId", 0);
            processor.AddCommand(execCommand,
                r =>
                {
                    count.Should().Be(0);
                    count++;
                    return true;
                });

            processor.AddCommand(execCommand,
               r =>
               {
                   count.Should().Be(1);
                   count++;
                   return true;
               });

            count.Should().Be(0);
            processor.ExecuteBatch(GetConnection());
            count.Should().Be(2);
        }

        [TestMethod]
        public void AddFluentQueryCommandResultsInExecutedQuery()
        {
            var count = 0;
            var processor = new BatchProcessor();

            var command = Repository.Default
                .ForEntity<Address>()
                .BuildSelect()
                .Where(a => a.AddressId == 0)
                .ToCommand();

            processor.AddCommand(command,
                r =>
                {
                    count.Should().Be(0);
                    count++;
                    return true;
                });

            count.Should().Be(0);
            processor.ExecuteBatch(GetConnection());
            count.Should().Be(1);
        }

        [TestMethod]
        public async Task ExecAsyncSelectAddressAndCustomerDoesNotFailAndInvokesHandlersInOrderWhenTrueIsReturned()
        {
            var count = 0;
            var processor = new BatchProcessor();
            var execCommand = new SqlCommand("select_address");
            execCommand.CommandType = CommandType.StoredProcedure;

            // Not testing the results, just the invocation and the handler callback order:
            execCommand.Parameters.AddWithValue("@addressId", 0);
            processor.AddCommand(execCommand,
                r =>
                {
                    count.Should().Be(0);
                    count++;
                    return true;
                });

            processor.AddCommand(execCommand,
               r =>
               {
                   count.Should().Be(1);
                   count++;
                   return true;
               });

            count.Should().Be(0);
            await processor.ExecuteBatchAsync(GetConnection());
            count.Should().Be(2);
        }

        [TestMethod]
        public void ExecSelectAddressAndCustomerDoesNotFailAndStopsWhenFalseIsReturned()
        {
            var count = 0;
            var processor = new BatchProcessor();
            var execCommand = new SqlCommand("select_address");
            execCommand.CommandType = CommandType.StoredProcedure;

            // Not testing the results, just the invocation and the handler callback order:
            execCommand.Parameters.AddWithValue("@addressId", 0);
            processor.AddCommand(execCommand,
                r =>
                {
                    count.Should().Be(0);
                    count++;
                    return false;
                });

            processor.AddCommand(execCommand,
               r =>
               {
                   count++;
                   return true;
               });

            count.Should().Be(0);
            processor.ExecuteBatch(GetConnection());
            count.Should().Be(1);
        }

        private static string GetConnection()
        {
            return ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString;
        }
    }

    
}
