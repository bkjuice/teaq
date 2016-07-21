using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.FastReflection;
using Teaq.Tests.FastReflection.SupportingTypes;

namespace Teaq.Tests.FastReflection
{
    [TestClass]
    public class PropertyDescriptionTests
    {
        [TestMethod]
        public void PropertyDescriptionSetValueThrowsInvalidOperationExceptionWhenSetterNotReflected()
        {
            var instance = new DescribedType();
            var target = new PropertyDescription(typeof(DescribedType).GetProperty("ReadOnlyValueProperty"), CommonBindings.PublicInstance);
            Action test = () => target.SetValue(instance, "test");
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PropertyDescriptionSetValueThrowsInvalidOperationExceptionWhenIndexedSetterNotReflected()
        {
            var instance = new IndexedReadOnlyPropertyExample();
            var target = new PropertyDescription(typeof(IndexedReadOnlyPropertyExample).GetProperties()[0], CommonBindings.PublicInstance);
            Action test = () => target.SetValue(instance, 5, 0);
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PropertyDescriptionSetValueDoesNotThrowWhenSetterEnsured()
        {
            var instance = new DescribedType();
            var target = new PropertyDescription(typeof(DescribedType).GetProperty("ReadOnlyValueProperty"), CommonBindings.PublicInstance);
            target.EnsureSetter();
            Action test = () => target.SetValue(instance, "test");
            test.ShouldNotThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PropertyDescriptionSetValueDoesNotThrowWhenIndexedSetterEnsured()
        {
            var instance = new IndexedReadOnlyPropertyExample();
            var target = new PropertyDescription(typeof(IndexedReadOnlyPropertyExample).GetProperties()[0],  CommonBindings.PublicInstance);
            target.EnsureSetter();
            Action test = () => target.SetValue(instance, 5, 0);
            test.ShouldNotThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PropertyDescriptionThrowsWhenSetterEnsuredAndNoSetterExists()
        {
            var target = new PropertyDescription(typeof(DescribedType).GetProperty("PropertyWithNoSetter"), CommonBindings.PublicInstance);
            Action test = () => target.EnsureSetter();
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PropertyDescriptionThrowsWhenSetterEnsuredAndPrivateSetterExistsButDoesNotMatchBindings()
        {
            var target = new PropertyDescription(typeof(DescribedType).GetProperty("ReadOnlyValueProperty"), CommonBindings.PublicInstance);
            Action test = () => target.EnsureSetter(BindingFlags.Static);
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PropertyDescriptionEnsureSetterIgnoresBadBindingWhenSetterIsAlreadyReflected()
        {
            var target = new PropertyDescription(typeof(DescribedType).GetProperty("PublicProperty"), CommonBindings.PublicInstance);
            Action test =() => target.EnsureSetter(BindingFlags.Static);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void PropertyDescriptionEnsureSetterThrowsWhenIndexedPropertyDoesNotHaveSetter()
        {
            var target = new PropertyDescription(typeof(IndexedNoSetterPropertyExample).GetProperties()[0], CommonBindings.PublicInstance);
            Action test = () => target.EnsureSetter();
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void PropertyDescriptionTryEnsureSetterIsFalseWhenIndexedPropertyDoesNotHaveSetter()
        {
            var target = new PropertyDescription(typeof(IndexedNoSetterPropertyExample).GetProperties()[0], CommonBindings.PublicInstance);
            target.TryEnsureSetter().Should().BeFalse();
        }

        [TestMethod]
        public void PropertyDescriptionTryEnsureSetterIsTrueWhenIndexedSetterEnsured()
        {
            var instance = new IndexedReadOnlyPropertyExample();
            var target = new PropertyDescription(typeof(IndexedReadOnlyPropertyExample).GetProperties()[0], CommonBindings.PublicInstance);
            target.TryEnsureSetter().Should().BeTrue();
        }
    }
}
