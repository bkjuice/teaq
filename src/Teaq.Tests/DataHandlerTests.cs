using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using Teaq.Tests;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class DataHandlerTests
    {
        [TestMethod]
        public void DataHandlerInitializesResultsToExpectedCapacityAndExecutesUntilReadIsFalse()
        {
            var stub = new DataHandlerStub<object>();
            var readerMock = new Mock<IDataReader>();
            var records = 0;
            readerMock.Setup(r => r.Read()).Returns(() => { records++; return records < 3; });

            stub.TestableEstimatedRowCount = 32;
            var results = stub.ReadEntities(readerMock.Object);
            results.Capacity.Should().Be(32);
            results.Count.Should().Be(2);
        }

        [TestMethod]
        public void BaseEstimatedRowCountIsArbitrarily16()
        {
            new DataHandlerStub<object>().BaseEstimatedRowCount.Should().Be(16);
        }
    }
}
