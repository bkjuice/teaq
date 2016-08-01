using System;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Teaq.QueryGeneration;
using Teaq.Tests.Stubs;

namespace Teaq.Tests.Contracts
{
    [TestClass]
    public class EntityQueryProviderContractClassTests
    {
        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQuery()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Action test = () => context.Query<Customer>(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQueryWithHandler()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Action test = () => context.Query(null, new Mock<IDataHandler<Customer>>().Object);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForHandler()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Action test = () => context.Query(new QueryCommand<Customer>("test"), default(IDataHandler<Customer>));
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQueryWithHandlerFunc()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Action test = () => context.Query(null, r => new Customer());
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForHandlerFunc()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Action test = () => context.Query(new QueryCommand<Customer>("test"), default(Func<IDataReader, Customer>));
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQueryAsync()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Func<Task> test = () => context.QueryAsync<Customer>(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQueryWithHandlerAsync()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Func<Task> test = () => context.QueryAsync(null, new Mock<IDataHandler<Customer>>().Object);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForHandlerAsync()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Func<Task> test = () => context.QueryAsync(new QueryCommand<Customer>("test"), default(IDataHandler<Customer>));
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQueryWithHandlerFuncAsync()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Func<Task> test = () => context.QueryAsync(null, r => new Customer());
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForHandlerFuncAsync()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Func<Task> test = () => context.QueryAsync(new QueryCommand<Customer>("test"), default(Func<IDataReader, Customer>));
                test.ShouldThrow<ArgumentNullException>();
            }
        }
    }
}
