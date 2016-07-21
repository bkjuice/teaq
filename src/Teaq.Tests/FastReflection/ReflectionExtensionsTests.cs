using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.FastReflection;
using Teaq.Tests.FastReflection.SupportingTypes;

namespace Teaq.Tests.FastReflection
{
    [TestClass]
    public class ReflectionExtensionsTests
    {
        [TestMethod]
        public void ToRuntimeHandlesUsingTypesReturnsNullAsExpected()
        {
            Type[] target = null;
            target.ToRuntimeHandles().Should().BeNull();
        }

        [TestMethod]
        public void FromRuntimeHandlesUsingTypesReturnsNullAsExpected()
        {
            RuntimeTypeHandle[] target = null;
            target.FromRuntimeHandles().Should().BeNull();
        }

        [TestMethod]
        public void ToRuntimeHandlesUsingParameterInfoReturnsNullAsExpected()
        {
            ParameterInfo[] target = null;
            target.ToRuntimeHandles().Should().BeNull();
        }

        [TestMethod]
        public void ToRuntimeHandlesUsingObjectInstancesInfoReturnsNullAsExpected()
        {
            object[] target = null;
            target.ToRuntimeHandles().Should().BeNull();
        }

        [TestMethod]
        public void ToRuntimeHandlesUsingTypesIsExpectedTypeAndOrder()
        {
            var expected = new RuntimeTypeHandle[] { typeof(string).TypeHandle, typeof(int).TypeHandle, typeof(double).TypeHandle };
            (new Type[] { typeof(string), typeof(int), typeof(double) }).ToRuntimeHandles()
                .Should().ContainInOrder(expected);
        }

        [TestMethod]
        public void ToRuntimeHandleUsingParameterInfoIsExpectedTypeAndOrder()
        {
            var expected = new RuntimeTypeHandle[] { typeof(string).TypeHandle, typeof(string).TypeHandle, typeof(int).TypeHandle };
            
            typeof(DescribedType).GetMethod("PublicFunc").GetParameters().ToRuntimeHandles()
                .Should().ContainInOrder(expected);
        }

        [TestMethod]
        public void ToRuntimeHandleUsingObjectInstancesIsExpectedTypeAndOrder()
        {
            var expected = new RuntimeTypeHandle[] { typeof(string).TypeHandle, typeof(string).TypeHandle, typeof(int).TypeHandle };
            (new object[] { "test", "test", 5 }).ToRuntimeHandles()
                .Should().ContainInOrder(expected);
        }

        [TestMethod]
        public void ToFieldDescriptionsReturnsNullAsExpected()
        {
            FieldInfo[] infos = null;
            infos.ToFieldDescriptions().Should().BeNull();
        }

        [TestMethod]
        public void ToFieldDescriptionDictionaryReturnsNullAsExpected()
        {
            FieldDescription[] infos = null;
            infos.ToHashtable().Should().BeNull();
        }

        [TestMethod]
        public void ToMethodDescriptionDictionaryReturnsNullAsExpected()
        {
            MethodDescription[] infos = null;
            infos.ToHashtable().Should().BeNull();
        }

        [TestMethod]
        public void ToPropertyDescriptionsReturnsNullAsExpected()
        {
            PropertyInfo[] infos = null;
            infos.ToPropertyDescriptions(CommonBindings.PublicInstance).Should().BeNull();
        }

        [TestMethod]
        public void ToPropertyDescriptionsReturnsValidResult()
        {
            var infos = typeof(DescribedType).GetProperties();
            var result = infos.ToPropertyDescriptions(CommonBindings.PublicInstance);
            result.Should().HaveSameCount(infos);
        }

        [TestMethod]
        public void ToMethodDescriptionsReturnsNullAsExpected()
        {
            MethodInfo[] infos = null;
            infos.ToMethodDescriptions().Should().BeNull();
        }

        [TestMethod]
        public void ToMethodDescriptionsReturnsValidResult()
        {
            var infos = typeof(DescribedType).GetMethods();
            var result = infos.ToMethodDescriptions();
            result.Should().HaveSameCount(infos);
        }

        [TestMethod]
        public void ToDictionaryIgnoresNullMethodDescriptionItems()
        {
            var items = new MethodDescription[] { null, null, null };
            items.ToHashtable().Count.Should().Be(0);
        }

        [TestMethod]
        public void ToMethodDescriptionsIgnoresGenericMethodInfoItems()
        {
            var items = typeof(GenericMethodExample).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            var result = items.ToMethodDescriptions();
            result.GetLength(0).Should().Be(1);
        }
    }
}
