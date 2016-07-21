using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using Teaq.FastReflection;
using Teaq.Tests.FastReflection.SupportingTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests
{
    [TestClass]
    public class TypeCacheTests
    {
        [TestMethod]
        public void Expression_has_unique_hash_code()
        {
            Expression<Func<bool>> expr1 = () => true;
            Expression<Func<bool>> expr2 = () => true;
            Expression<Func<bool>> expr3 = () => false;
            Expression<Func<bool>> expr4 = () => false;

            Assert.AreNotEqual(expr1.GetHashCode(), expr2.GetHashCode());
            Assert.AreNotEqual(expr3.GetHashCode(), expr4.GetHashCode());

            Assert.AreNotEqual(expr1.GetHashCode(), expr3.GetHashCode());
            Assert.AreNotEqual(expr2.GetHashCode(), expr4.GetHashCode());
        }

        [TestMethod]
        public void Func_does_not_have_unique_hash_code()
        {
            Func<bool> expr1 = () => true;
            Func<bool> expr2 = () => true;
            Func<bool> expr3 = () => false;
            Func<bool> expr4 = () => false;

            Assert.AreEqual(expr1.GetHashCode(), expr2.GetHashCode());
            Assert.AreEqual(expr3.GetHashCode(), expr4.GetHashCode());

            Assert.AreEqual(expr1.GetHashCode(), expr3.GetHashCode());
            Assert.AreEqual(expr2.GetHashCode(), expr4.GetHashCode());
        }

        [TestMethod]
        public void RuntimeTypeHandle_not_equal_to_object_of_same_type()
        {
            var t = typeof(DescribedType);
            var handle = typeof(DescribedType).TypeHandle;

            Assert.IsFalse(handle.Equals(t));
            Assert.IsFalse(handle.Equals(new DescribedType()));
        }

        [TestMethod]
        public void RuntimeTypeHandle_equal_to_handle_of_same_type()
        {
            var t = typeof(DescribedType);
            var handle1 = typeof(DescribedType).TypeHandle;
            var handle2 = typeof(DescribedType).TypeHandle;
            var handle3 = new DescribedType().GetType().TypeHandle;

            Assert.IsTrue(handle1.Equals(handle2));
            Assert.IsTrue(handle2.Equals(handle3));
        }

        [TestMethod]
        public void TypeDescription_equal_to_same_type()
        {
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property);
            Assert.IsTrue(t.Equals(typeof(DescribedType)));
        }

        [TestMethod]
        public void TypeCacheCoverage1()
        {
            // covers obsolete method:
            TypeCache.Clear();
            Action test = ()=> typeof(string).GetTypeDescription();
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void TypeCacheCoverage2()
        {
            // covers obsolete method:
            TypeCache.Clear();
            Action test = () =>
                typeof(string).GetTypeDescription(MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void TypeCacheObjectDescriptionIsObjectDescription()
        {
            TypeCache.Clear();
            TypeCache.ObjectDescription.Should().NotBeNull();
        }
    }
}
