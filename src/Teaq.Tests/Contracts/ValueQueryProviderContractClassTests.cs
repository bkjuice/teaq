using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests.Contracts
{
    [TestClass]
    public class ValueQueryProviderContractClassTests
    {
        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQueryNullableValues()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Action test = () => context.QueryNullableValues<int>(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQueryStringValues()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Action test = () => context.QueryStringValues(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQueryValues()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Action test = () => context.QueryValues<int>(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQueryNullableValuesAsync()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Func<Task> test = () => context.QueryNullableValuesAsync<int>(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQueryStringValuesAsync()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Func<Task> test = () => context.QueryStringValuesAsync(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestMethod]
        public void NullCommandThrowsArgumentNullExceptionForQueryValuesAsync()
        {
            using (var context = Repository.BuildContext("test"))
            {
                Func<Task> test = () => context.QueryValuesAsync<int>(null);
                test.ShouldThrow<ArgumentNullException>();
            }
        }
    }
}
