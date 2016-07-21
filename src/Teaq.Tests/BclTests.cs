using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Teaq.Tests
{
    [TestClass]
    public class BclTests
    {
        [TestMethod]
        public void VerifyAnUndecoreatedTypeHasAUniqueGuid()
        {
            typeof(Undecorated1).GUID.Should().NotBe(Guid.Empty);
            typeof(Undecorated1).GUID.Should().NotBe(typeof(Undecorated2).GUID);
        }

        private class Undecorated1 { }

        private class Undecorated2 { }
    }
}
