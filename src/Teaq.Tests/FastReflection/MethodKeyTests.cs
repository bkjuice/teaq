using System;
using System.Collections.Generic;
using Teaq.FastReflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Teaq.Tests.FastReflection
{
    [TestClass]
    public class MethodKeyTests
    {
        [TestMethod]
        public void MethodKeyValueDefaultIsNull()
        {
            var target = new MethodKey();
            target.Name.Should().BeNull();
            target.Arguments.Should().BeNull();
        }

        [TestMethod]
        public void MethodKeyNameIsAsExpected()
        {
            var target = new MethodKey("test");
            target.Name.Should().Be("test");
            target.Arguments.Should().BeNull();
        }

        [TestMethod]
        public void MethodKeyArgsAndNameIsAsExpected()
        {
            var args = (new Type[] { typeof(string), typeof(string), typeof(int) }).ToRuntimeHandles();
            var target = new MethodKey(args);
            
            target.Name.Should().Be(string.Empty);
            target.Arguments.Should().ContainInOrder(args);
        }

        [TestMethod]
        public void MethodKeyDefaultEqualityIsAsExpected()
        {
            (new MethodKey() == new MethodKey()).Should().BeTrue();
        }

        [TestMethod]
        public void MethodKeyDefaultInequalityIsAsExpected()
        {
            (new MethodKey() != new MethodKey()).Should().BeFalse();
        }

        [TestMethod]
        public void MethodKeyDefaultEqualityIsAsExpectedWhenBoxed()
        {
            new MethodKey().Equals((object)new MethodKey()).Should().BeTrue();
        }

        [TestMethod]
        public void MethodKeyIsNotEqualToOtherType()
        {
            new MethodKey().Equals(new object()).Should().BeFalse();
        }

        [TestMethod]
        public void MethodKeyIsNotEqualToNull()
        {
            new MethodKey().Equals(null).Should().BeFalse();
        }

        [TestMethod]
        public void MethodKeysAreEqualIfNameAndArgumentsMatchExactly()
        {
            var args1 = (new Type[] { typeof(string), typeof(string), typeof(int) }).ToRuntimeHandles();
            var args2 = (new Type[] { typeof(string), typeof(string), typeof(int) }).ToRuntimeHandles();
            (new MethodKey("X", args1) == new MethodKey("X", args2)).Should().BeTrue();
        }

        [TestMethod]
        public void MethodKeysAreNotEqualIfNameDoesNotMatch()
        {
            var args1 = (new Type[] { typeof(string), typeof(string), typeof(int) }).ToRuntimeHandles();
            var args2 = (new Type[] { typeof(string), typeof(string), typeof(int) }).ToRuntimeHandles();
            (new MethodKey("X", args1) == new MethodKey("Y", args2)).Should().BeFalse();
        }

        [TestMethod]
        public void MethodKeysAreNotEqualIfArgumentsDoNotMatchExactly()
        {
            var args1 = (new Type[] { typeof(string), typeof(string), typeof(int) }).ToRuntimeHandles();
            var args2 = (new Type[] { typeof(int), typeof(string), typeof(string) }).ToRuntimeHandles();
            (new MethodKey("X", args1) == new MethodKey("X", args2)).Should().BeFalse();
        }

        [TestMethod]
        public void MethodKeysAreNotEqualIfArgumentsDoNotMatchByNumberWhenLess()
        {
            var args1 = (new Type[] { typeof(string), typeof(string), typeof(int) }).ToRuntimeHandles();
            var args2 = (new Type[] { typeof(int), typeof(string) }).ToRuntimeHandles();
            (new MethodKey("X", args1) == new MethodKey("X", args2)).Should().BeFalse();
        }

        [TestMethod]
        public void MethodKeysAreNotEqualIfArgumentsDoNotMatchByNumberWhenGreater()
        {
            var args1 = (new Type[] { typeof(int), typeof(string) }).ToRuntimeHandles();
            var args2 = (new Type[] { typeof(string), typeof(string), typeof(int) }).ToRuntimeHandles();
            (new MethodKey("X", args1) == new MethodKey("X", args2)).Should().BeFalse();
        }

        [TestMethod]
        public void MethodKeysAreNotEqualIfLeftArgumentsAreNull()
        {
            RuntimeTypeHandle[] args1 = null;
            var args2 = (new Type[] { typeof(int), typeof(string) }).ToRuntimeHandles();
            (new MethodKey("X", args1) == new MethodKey("X", args2)).Should().BeFalse();
        }

        [TestMethod]
        public void MethodKeysAreNotEqualIfRightArgumentsAreNull()
        {
            var args1 = (new Type[] { typeof(string), typeof(string), typeof(int) }).ToRuntimeHandles();
            RuntimeTypeHandle[] args2 = null;
            (new MethodKey("X", args1) == new MethodKey("X", args2)).Should().BeFalse();
        }

        [TestMethod]
        public void MethodKeyWithNoArgsIsMatchingDictionaryKey()
        {
            var dictionary = new Dictionary<MethodKey, string>();
            dictionary.Add(new MethodKey("Test", new RuntimeTypeHandle[] { }), "test");
            dictionary.ContainsKey(new MethodKey("Test")).Should().BeTrue();

            dictionary.Clear();
            dictionary.Add(new MethodKey("Test"), "test");
            dictionary.ContainsKey(new MethodKey("Test", new RuntimeTypeHandle[] { })).Should().BeTrue();
        }

        [TestMethod]
        public void MethodKeyWithNoNameAndNoArgsIsMatchingDictionaryKey()
        {
            var dictionary = new Dictionary<MethodKey, string>();
            dictionary.Add(new MethodKey(new RuntimeTypeHandle[] { }), "test");
            dictionary.ContainsKey(new MethodKey()).Should().BeTrue();

            dictionary.Clear();
            dictionary.Add(new MethodKey(), "test");
            dictionary.ContainsKey(new MethodKey(new RuntimeTypeHandle[] { })).Should().BeTrue();
        }
    }
}
