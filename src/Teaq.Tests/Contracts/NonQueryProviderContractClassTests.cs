using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests.Contracts
{
    [TestClass]
    public class NonQueryProviderContractClassTests
    {
        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForExecuteNonQuery()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Action test = () => context.ExecuteNonQuery(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForExecuteScalarString()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Action test = () => context.ExecuteScalar(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForExecuteScalar()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Action test = () => context.ExecuteScalar<int>(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForExecuteNonQueryAsync()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Func<Task> test = () => context.ExecuteNonQueryAsync(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForExecuteScalarStringAsync()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Func<Task> test = () => context.ExecuteScalarAsync(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForExecuteScalarAsync()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Func<Task> test = () => context.ExecuteScalarAsync<int>(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }
    }
}
