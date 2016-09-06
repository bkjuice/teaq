using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.QueryGeneration;
using FluentAssertions;

namespace Teaq.Tests.QueryGeneration
{
    [TestClass]
    public class StringBuilderCacheTests
    {
        private const int LargeObjectHeapThreshold = 85000 - 24;

        [TestMethod]
        public void GetInstanceReturnsNewInstanceIfReturnedButNotLOHSize()
        {
            var b1 = StringBuilderCache.GetInstance(4096);
            b1.Append("test");
            b1.Length.Should().Be(4);
            b1.Capacity.Should().Be(4096);

            StringBuilderCache.ReturnInstance(b1);
            var b2 = StringBuilderCache.GetInstance(4096);
            b2.Length.Should().Be(0);
            b2.Capacity.Should().Be(4096);

            ReferenceEquals(b1, b2).Should().BeFalse();
        }

        [TestMethod]
        public void GetInstanceReturnsSameInstanceIfReturnedAndLOHSize()
        {
            var b1 = StringBuilderCache.GetInstance(LargeObjectHeapThreshold);
            b1.Append("test");
            b1.Length.Should().Be(4);
            b1.Capacity.Should().Be(LargeObjectHeapThreshold);

            StringBuilderCache.ReturnInstance(b1);
            var b2 = StringBuilderCache.GetInstance(LargeObjectHeapThreshold);
            b2.Length.Should().Be(0);
            b2.Capacity.Should().Be(LargeObjectHeapThreshold);

            ReferenceEquals(b1, b2).Should().BeTrue();
        }

        [TestMethod]
        public void GetInstanceReturnsNewInstanceIfReturnedAndLOHSizeForFirstAllocationButNotSecond()
        {
            var b1 = StringBuilderCache.GetInstance(LargeObjectHeapThreshold);
            b1.Append("test");
            b1.Length.Should().Be(4);
            b1.Capacity.Should().Be(LargeObjectHeapThreshold);

            StringBuilderCache.ReturnInstance(b1);
            var b2 = StringBuilderCache.GetInstance(4096);
            b2.Length.Should().Be(0);
            b2.Capacity.Should().Be(4096);

            ReferenceEquals(b1, b2).Should().BeFalse();
        }
    }
}
