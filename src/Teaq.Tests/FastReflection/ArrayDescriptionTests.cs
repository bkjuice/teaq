using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.FastReflection;
using FluentAssertions;

namespace Teaq.Tests.FastReflection
{
    [TestClass]
    public class ArrayDescriptionTests
    {
        [TestMethod]
        public void ArrayReflectsToArrayDescription()
        {
            var target = new int[] { 1, 3, 3, 4 };
            var description = target.GetType().GetTypeDescription() as ArrayDescription;
            description.Should().NotBeNull();
        }

        [TestMethod]
        public void ArrayDescriptionGetsCorrectValueAtSpecifiedIndex()
        {
            var target = new int[] { 1, 3, 3, 4 };
            var description = target.GetType().GetTypeDescription() as ArrayDescription;
            var value = description.GetArrayValue(target, 1);

            ((int)value).Should().Be(target[1]);
        }

        [TestMethod]
        public void ArrayDescriptionSetsCorrectValueAtSpecifiedIndex()
        {
            var target = new int[] { 1, 3, 3, 4 };
            var description = target.GetType().GetTypeDescription() as ArrayDescription;
            description.SetArrayValue(target, 1, 2);

            target[1].Should().Be(2);
        }

        [TestMethod]
        public void ArrayDescriptionCreatesArrayOfSpecifiedTypeAndSize()
        {
            var description = typeof(int[]).GetTypeDescription() as ArrayDescription;
            var instance = description.CreateArrayInstance(10);
            instance.GetType().Should().Be(typeof(int[]));
            instance.Length.Should().Be(10);
        }
    }
}
