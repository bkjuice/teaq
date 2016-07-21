using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.FastReflection;
using Teaq.Tests.FastReflection.SupportingTypes;

namespace Teaq.Tests.FastReflection
{
    [TestClass]
    public class MethodDescriptionTests
    {
        [TestMethod]
        public void MethodDescriptionArgumentsAreExpectedTypeAndOrder()
        {
            var info = typeof(DescribedType).GetMethod("PublicFunc");
            var method = new MethodDescription(info);

            var args = (new Type[] { typeof(string), typeof(string), typeof(int) });
            method.GetParameterTypes().Should().ContainInOrder(args);
        }
    }
}
