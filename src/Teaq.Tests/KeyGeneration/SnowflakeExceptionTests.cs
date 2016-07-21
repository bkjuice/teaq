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
    public class SnowflakeExceptionTests
    {
        [TestMethod]
        public void SnowflakeConfigurationExceptionCanSerializeAsExpected()
        {
            var formatter = new BinaryFormatter();
            var inner = new Exception();
            Action test = () =>
            {
                var e = new SnowflakeConfigurationException("Test", inner);
                e.HasBeenLogged = true;

                using (var m = new MemoryStream())
                {
                    formatter.Serialize(m, e);
                    m.Flush();
                    m.Position = 0;
                    e = formatter.Deserialize(m) as SnowflakeConfigurationException;
                }
            };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void SnowflakeExceptionCanSerializeAsExpected()
        {
            var formatter = new BinaryFormatter();
            var inner = new Exception();
            Action test = () =>
            {
                var e = new SnowflakeException("Test", inner);
                e.HasBeenLogged = true;
                
                using (var m = new MemoryStream())
                {
                    formatter.Serialize(m, e);
                    m.Flush();
                    m.Position = 0;
                    e = formatter.Deserialize(m) as SnowflakeException;
                }

                e.HasBeenLogged.Should().BeTrue();
                e.ExceptionId.Should().NotBe(Guid.Empty);
                e.InnerException.Should().NotBeNull();
            };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void SnowflakeExceptionInitializesAsExpected()
        {
            Action test = () =>
            {
                var e = new SnowflakeException();
                e.ExceptionId.Should().NotBe(Guid.Empty);
                e.Message.Should().NotBeEmpty();
            };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void SnowflakeExceptionInitializesAsExpectedWithMessage()
        {
            Action test = () =>
            {
                var e = new SnowflakeException("Test");
                e.Message.Should().Be("Test");
            };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void SnowflakeExceptionInitializesAsExpectedWithInnerException()
        {
            var inner = new Exception();
            Action test = () =>
            {
                var e = new SnowflakeException("Test", inner);
                e.InnerException.Should().BeSameAs(inner);
            };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void SnowflakeConfigurationExceptionInitializesAsExpected()
        {
            Action test = () =>
            {
                var e = new SnowflakeConfigurationException();
                e.ExceptionId.Should().NotBe(Guid.Empty);
                e.Message.Should().NotBeEmpty();
            };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void SnowflakeConfigurationExceptionInitializesAsExpectedWithMessage()
        {
            Action test = () =>
            {
                var e = new SnowflakeConfigurationException("Test");
                e.Message.Should().Be("Test");
            };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void SnowflakeConfigurationExceptionInitializesAsExpectedWithInnerException()
        {
            var inner = new Exception();
            Action test = () =>
            {
                var e = new SnowflakeConfigurationException("Test", inner);
                e.InnerException.Should().BeSameAs(inner);
            };

            test.ShouldNotThrow();
        }
    }
}
