using System;
using System.Collections.Generic;
using FluentAssertions;
using Teaq.KeyGeneration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests.KeyGeneration
{
    [TestClass]
    public partial class SnowflakeTest
    {
        [TestMethod]
        public void DisposeDoesNotFailWithPartialConstruction()
        {
            Action test = () =>
                {
                    using (new Snowflake(256, SnowflakeWorkerIdRange.Max255))
                    { }
                };

            test.ShouldNotThrow<NullReferenceException>();
        }

        [TestMethod]
        public void DisposeDoesNotFail()
        {
            Action test = () =>
            {
                using (new Snowflake(250, SnowflakeWorkerIdRange.Max255))
                { }
            };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void SnowflakeThrowsSnowflakeConfigurationExceptionWhenRangeNotSet()
        {
            Action test = () => new Snowflake(0, SnowflakeWorkerIdRange.NotSet);
            test.ShouldThrow<SnowflakeConfigurationException>();
        }

        [TestMethod]
        public void SnowflakeThrowsSnowflakeConfigurationExceptionWhenRangeIsOutOfBoundsFor255()
        {
            Action test = () => new Snowflake(256, SnowflakeWorkerIdRange.Max255);
            test.ShouldThrow<SnowflakeConfigurationException>();
        }

        [TestMethod]
        public void SnowflakeThrowsSnowflakeConfigurationExceptionWhenRangeIsOutOfBoundsFor1023()
        {
            Action test = () => new Snowflake(1024, SnowflakeWorkerIdRange.Max1023);
            test.ShouldThrow<SnowflakeConfigurationException>();
        }

        [TestMethod]
        public void SnowflakeThrowsSnowflakeConfigurationExceptionWhenRangeIsOutOfBoundsFor4095()
        {
            Action test = () => new Snowflake(4096, SnowflakeWorkerIdRange.Max4095);
            test.ShouldThrow<SnowflakeConfigurationException>();
        }

        [TestMethod]
        public void SnowflakeSequencingTest65KUsing8BitWorkerId()
        {
            var maxCounter = 0xFFFF;
            var snowflake = new Snowflake(2);
            TestSnowflakeSequencing(snowflake, maxCounter);
        }

        [TestMethod]
        public void SnowflakeCollisionTest65KUsing8BitWorkerId()
        {
            var maxCounter = 0xFFFF;
            var snowflake = new Snowflake(1);
            TestSnowflakeCollisions(snowflake, maxCounter);
        }

        [TestMethod]
        public void SnowflakeSequencingTest65KUsingExplicit8BitWorkerId()
        {
            var maxCounter = 0xFFFF;
            var snowflake = new Snowflake(2, SnowflakeWorkerIdRange.Max255);
            TestSnowflakeSequencing(snowflake, maxCounter);
        }

        [TestMethod]
        public void SnowflakeCollisionTest65KUsingExplicit8BitWorkerId()
        {
            var maxCounter = 0xFFFF;
            var snowflake = new Snowflake(1, SnowflakeWorkerIdRange.Max255);
            TestSnowflakeCollisions(snowflake, maxCounter);
        }

        [TestMethod]
        public void SnowflakeSequencingTest65KUsing10BitWorkerId()
        {
            var maxCounter = 0xFFFF;
            var snowflake = new Snowflake(311, SnowflakeWorkerIdRange.Max1023);
            TestSnowflakeSequencing(snowflake, maxCounter);
        }

        [TestMethod]
        public void SnowflakeCollisionTest65KUsing10BitWorkerId()
        {
            var maxCounter = 0xFFFF;
            var snowflake = new Snowflake(301, SnowflakeWorkerIdRange.Max1023);
            TestSnowflakeCollisions(snowflake, maxCounter);
        }

        [TestMethod]
        public void SnowflakeSequencingTest65KUsing12BitWorkerId()
        {
            var maxCounter = 0xFFFF;
            var snowflake = new Snowflake(3011, SnowflakeWorkerIdRange.Max4095);
            TestSnowflakeSequencing(snowflake, maxCounter);
        }

        [TestMethod]
        public void SnowflakeCollisionTest65KUsing12BitWorkerId()
        {
            var maxCounter = 0xFFFF;
            var snowflake = new Snowflake(3001, SnowflakeWorkerIdRange.Max4095);
            TestSnowflakeCollisions(snowflake, maxCounter);
        }

        [TestMethod]
        public void SnowflakeCollisionTest256KUsing12BitWorkerId()
        {
            var maxCounter = 0xFFFF * 4;
            var snowflake = new Snowflake(3001, SnowflakeWorkerIdRange.Max4095);
            TestSnowflakeCollisions(snowflake, maxCounter);
        }

        [TestMethod]
        public void SnowflakesAreProducedInOrder()
        {
            var snowflake = new Snowflake(210, SnowflakeWorkerIdRange.Max255);
            long[] values = new long[10];
            for (int i = 0; i < 10; i++)
            {
                values[i] = snowflake.NextId();
            }

            long prev = 0;
            for (int i = 0; i < 10; i++)
            {
                values[i].Should().BeGreaterThan(prev);
                prev = values[i];
            }
        }

        private static void TestSnowflakeSequencing(Snowflake snowflake, int maxCounter)
        {
            var lastValue = 0L;
            var loops = maxCounter * 2;
            lastValue = snowflake.NextId();
            for (var i = 0; i < loops; i++)
            {
                var id = snowflake.NextId();
                Assert.IsTrue(id > lastValue);
                lastValue = id;
            }
        }

        private static void TestSnowflakeCollisions(Snowflake snowflake, int maxCounter)
        {
            HashSet<long> values = new HashSet<long>();
            var loops = maxCounter * 2;
            var id = snowflake.NextId();
            Assert.IsTrue(values.Add(id));

            for (var i = 0; i < loops; i++)
            {
                id = snowflake.NextId();
                Assert.IsTrue(values.Add(id));
            }
        }
    }
}
