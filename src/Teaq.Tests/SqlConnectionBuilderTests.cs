using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests
{
    [TestClass]
    public class SqlConnectionBuilderTests
    {
        [TestMethod]
        public void SqlConnectionBuilderCanBuildValidConnection()
        {
            var builder = new SqlConnectionBuilder();
            builder.Create("server=test;database=test;").Should().NotBe(null);
        }

        [TestMethod]
        public void SqlConnectionBuilderThrowsArgumentExceptionForInvalidConnection()
        {
            var builder = new SqlConnectionBuilder();
            Action test = () => builder.Create("invalid");
            test.ShouldThrow<ArgumentException>();
        }
    }
}
