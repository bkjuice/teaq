using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.Configuration;

namespace Teaq.Tests
{
    [TestClass]
    public class DataContextBaseTests
    {
        [TestMethod]
        public void DataContextBaseInitializesWithProvidedValuesCtor()
        {
            var builder = new SqlConnectionBuilder();
            var stub = new DataContextStub("test", builder);
            stub.ConnectionString.Should().Be("test");
            stub.TestableBuilder.Should().BeSameAs(builder);
        }

        private class DataContextStub : DataContextBase
        {
            internal DataContextStub(string connectionString)
            : base(connectionString)
            {
            }

            internal DataContextStub(string connectionString, IConnectionBuilder connectionBuilder)
                : base(connectionString, connectionBuilder)
            {
            }

            public IConnectionBuilder TestableBuilder
            {
                get
                {
                    return base.ConnectionBuilder;
                }
            }
        }
    }
}
