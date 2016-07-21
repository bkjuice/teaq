using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using Teaq.KeyGeneration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests.KeyGeneration
{
    [TestClass]
    public class SnowflakeClockRegressionExceptionTests
    {
        [TestMethod]
        public void SnowflakeClockRegressionExceptionInitializesAsExpectedViaDefaultCtor()
        {
            Action test = () => new SnowflakeClockRegressionException();
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void SnowflakeClockRegressionExceptionInitializesWithMesssage()
        {
            var e = new SnowflakeClockRegressionException("ERROR!");
            e.Message.Should().Be("ERROR!");
        }

        [TestMethod]
        public void SnowflakeClockRegressionExceptionInitializesWithMesssageAndInnerException()
        {
            var inner = new Exception();
            var e = new SnowflakeClockRegressionException("ERROR!", inner);
            e.Message.Should().Be("ERROR!");
            e.InnerException.Should().BeSameAs(inner);
        }

        [TestMethod]
        public void SnowflakeClockRegressionExceptionCanSerializeAsExpected()
        {
            var formatter = new BinaryFormatter();
            var inner = new Exception();
            Action test = () =>
            {
                var e = new SnowflakeClockRegressionException("Test", inner);
                e.HasBeenLogged = true;

                using (var m = new MemoryStream())
                {
                    formatter.Serialize(m, e);
                    m.Flush();
                    m.Position = 0;
                    e = formatter.Deserialize(m) as SnowflakeClockRegressionException;
                }
            };

            test.ShouldNotThrow();
        }
    }
}
