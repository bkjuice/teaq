using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.FastReflection;
using Teaq.Tests.FastReflection.SupportingTypes;

namespace Teaq.Tests.FastReflectionTests
{
    [TestClass]
    public class TypeDescriptionTests
    {
        [TestMethod]
        public void TypeDescriptionForListOfTReturnsExpectedGenericType()
        {
            var target = typeof(List<string>).GetTypeDescription();
            var genType = target.TryGetGenericArgumentDescription();
            genType.Should().NotBeNull();
            genType.ReflectedType.Should().Be(typeof(string));
        }

        [TestMethod]
        public void TypeDescriptionForListOfTReturnsExpectedGenericTypeWhenSpecifyingMembersAndBindings()
        {
            var target = typeof(List<string>).GetTypeDescription();
            var genType = target.TryGetGenericArgumentDescription(MemberTypes.Property, CommonBindings.PublicInstance);
            genType.Should().NotBeNull();
            genType.ReflectedType.Should().Be(typeof(string));
        }

        [TestMethod]
        public void TypeDescriptionIsEnumIsAsExpected()
        {
            var target = typeof(StringComparison).GetTypeDescription(MemberTypes.Constructor);
            target.IsEnum.Should().BeTrue();
            target.IsNullableEnum.Should().BeFalse();
            target.IsPrimitiveOrString.Should().BeTrue();
            target.IsPrimitiveOrStringOrNullable.Should().BeTrue();
            target.IsPrimitiveNullable.Should().BeFalse();
        }

        [TestMethod]
        public void TypeDescriptionIsNullableEnumIsAsExpected()
        {
            var target = typeof(StringComparison?).GetTypeDescription(MemberTypes.Constructor);
            target.IsEnum.Should().BeFalse();
            target.IsNullableEnum.Should().BeTrue();
            target.IsPrimitiveOrString.Should().BeFalse();
            target.IsPrimitiveOrStringOrNullable.Should().BeTrue();
            target.IsPrimitiveNullable.Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescriptionIsAbstractIsTrueForAbstractTypeFalseOtherwise()
        {
            typeof(AbstractType).GetTypeDescription(MemberTypes.Constructor).IsAbstract.Should().BeTrue();
            typeof(object).GetTypeDescription(MemberTypes.Constructor).IsAbstract.Should().BeFalse();
        }

        [TestMethod]
        public void TypeDescriptionIsSealedIsTrueForSealedTypeFalseOtherwise()
        {
            typeof(SealedType).GetTypeDescription(MemberTypes.Constructor).IsSealed.Should().BeTrue();
            typeof(object).GetTypeDescription(MemberTypes.Constructor).IsSealed.Should().BeFalse();
        }

        [TestMethod]
        public void TypeDescriptionHasExpectedPropertyAttribute()
        {
            TypeCache.Clear();
            var includePublic = BindingFlags.Public | BindingFlags.Instance;
            var t = typeof(TypeWithoutDefaultCtor).GetTypeDescription(MemberTypes.Property, includePublic);

            var p = t.GetProperty("SomeValue2");

            var isApplied = p.AttributeIsDefined("TestAttribute");
            Assert.IsTrue(isApplied);

            var attrs = p.GetAttributeData();
            Assert.IsTrue(attrs.Length == 2);
        }

        [TestMethod]
        public void TypeDescriptionIsPrimitiveOrNullableOrStringIsSetAsExpected()
        {
            typeof(string).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(int).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(uint).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(long).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(ulong).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(short).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(ushort).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(byte).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(bool).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(float).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(double).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(decimal).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(Guid).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(TimeSpan).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(DateTime).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(DateTimeOffset).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();

            typeof(int?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(uint?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(long?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(ulong?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(short?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(ushort?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(byte?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(bool?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(float?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(double?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(decimal?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(Guid?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(TimeSpan?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(DateTime?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
            typeof(DateTimeOffset?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrStringOrNullable.Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescriptionIsPrimitiveOrStringIsSetAsExpected()
        {
            typeof(string).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(int).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(uint).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(long).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(ulong).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(short).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(ushort).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(byte).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(bool).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(float).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(double).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(decimal).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(Guid).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(TimeSpan).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(DateTime).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();
            typeof(DateTimeOffset).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeTrue();

            typeof(int?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(uint?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(long?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(ulong?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(short?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(ushort?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(byte?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(bool?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(float?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(double?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(decimal?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(Guid?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(TimeSpan?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(DateTime?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
            typeof(DateTimeOffset?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveOrString.Should().BeFalse();
        }

        [TestMethod]
        public void TypeDescriptionIsPrimitiveNullableIsSetAsExpected()
        {
            typeof(string).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(int).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(uint).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(long).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(ulong).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(short).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(ushort).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(byte).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(bool).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(float).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(double).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(decimal).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(Guid).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(TimeSpan).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(DateTime).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();
            typeof(DateTimeOffset).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeFalse();

            typeof(int?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(uint?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(long?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(ulong?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(short?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(ushort?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(byte?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(bool?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(float?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(double?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(decimal?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(Guid?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(TimeSpan?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(DateTime?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
            typeof(DateTimeOffset?).GetTypeDescription(MemberTypes.Constructor).IsPrimitiveNullable.Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescriptionGetMethodByNameReturnsExpectedMethod()
        {
            TypeCache.Clear();

            // Targeting: public string PublicFunc(string value1, string value2, int value3)
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Method);
            var m = t.GetMethod("PublicFunc", typeof(string), typeof(string), typeof(int));
            m.Should().NotBeNull();
            m.MemberName.Should().Be("PublicFunc");
            m.ParameterCount.Should().Be(3);
            m.GetParameterTypes().Should().ContainInOrder(new Type[] { typeof(string), typeof(string), typeof(int) });
        }

        [TestMethod]
        public void TypeDescriptionGetPropertyThrowsIfPropertiesAreNotReflected()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Constructor);
            Action test = () => t.GetProperty("SomeProperty");
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescriptionGetFieldThrowsIfFieldsAreNotReflected()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Constructor);
            Action test = () => t.GetField("SomeField");
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescriptionGetMethodReturnsNullIfMethodDoesNotExist()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Method);
            t.GetMethod("SomeMethod").Should().BeNull();
        }

        [TestMethod]
        public void TypeDescriptionIsIDictionaryIsTrueForDictionary()
        {
            TypeCache.Clear();
            var t = typeof(Dictionary<string, string>).GetTypeDescription(MemberTypes.Constructor);
            t.IsIDictionary.Should().BeTrue();
            t.IsICollection.Should().BeTrue();
            t.IsIEnumerable.Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescriptionIsICollectionIsTrueForList()
        {
            TypeCache.Clear();
            var t = typeof(List<string>).GetTypeDescription(MemberTypes.Constructor);
            t.IsIDictionary.Should().BeFalse();
            t.IsICollection.Should().BeTrue();
            t.IsIEnumerable.Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescriptionIsIEnumerableIsTrueForEnumerableType()
        {
            TypeCache.Clear();
            var t = typeof(EnumerableType).GetTypeDescription(MemberTypes.Constructor);
            t.IsIDictionary.Should().BeFalse();
            t.IsICollection.Should().BeFalse();
            t.IsIEnumerable.Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescriptionGetMethodWithParameterTypesReturnsNullIfMethodDoesNotExist()
        {
            TypeCache.Clear();
            var args = (new object[] { new object(), new object(), new object() }).ToRuntimeHandles();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Method);
            t.GetMethod("SomeMethod", args).Should().BeNull();
        }

        [TestMethod]
        public void TypeDescriptionGetMethodWithParameterTypesReturnsMethodDescriptionIfMethodExists()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Constructor);
            t.GetMethod("NoArgsNoOp").Should().NotBeNull();
        }

        [TestMethod]
        public void TypeDescriptionGetMethodWithParameterTypesReturnsNullIfMethodsAreNotReflected()
        {
            TypeCache.Clear();
            var args = (new object[] { new object(), new object(), new object() }).ToRuntimeHandles();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Constructor);
            t.GetMethod("SomeMethod", args).Should().BeNull();
        }

        [TestMethod]
        public void TypeDescriptionGetHashCodeMatchesTypeHandleGetHashCode()
        {
            var expected = typeof(DescribedType).TypeHandle.GetHashCode();
            typeof(DescribedType).GetTypeDescription(MemberTypes.Constructor)
                .GetHashCode().Should().Be(expected);
        }

        [TestMethod]
        public void TypeDescriptionIsNotEqualNull()
        {
            typeof(DescribedType).GetTypeDescription(MemberTypes.Constructor)
                .Equals(null).Should().BeFalse();
        }

        [TestMethod]
        public void TypeDescriptionCreateInstanceWithWrongArgsReturnsNull()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Constructor);
            t.CreateInstance(new object[] { new object(), new object(), new object() }).Should().BeNull();
        }

        [TestMethod]
        public void TypeDescriptionGetMethodWithNoArgsReturnsExpectedMethod()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Method);
            var method = t.GetMethod("NoArgsNoOp");
            method.Should().NotBeNull();
            method.MemberName.Should().Be("NoArgsNoOp");
        }

        [TestMethod]
        public void TypeDescriptionHasReflectedPropertiesIsTrueWhenPropertiesAreReflected()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property);
            t.HasReflectedProperties.Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescriptionHasReflectedPropertiesIsFalseWhenPropertiesAreNotReflected()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Constructor);
            t.HasReflectedProperties.Should().BeFalse();
        }

        [TestMethod]
        public void TypeDescriptionHasReflectedFieldsIsTrueWhenPropertiesAreReflected()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Field, CommonBindings.PublicAndPrivateInstance);
            t.HasReflectedFields.Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescriptionHasReflectedFieldsIsFalseWhenPropertiesAreNotReflected()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Constructor);
            t.HasReflectedFields.Should().BeFalse();
        }

        [TestMethod]
        public void TypeDescriptionReflectMethodsIgnoresGenericMethods()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Constructor);
            t.ReflectMethods();
            t.GetMethod("GenericMethod", typeof(object)).Should().BeNull();
        }

        [TestMethod]
        public void TypeDescriptionIndexedPropertiesWillThrowIfAccessedIncorrectly()
        {
            TypeCache.Clear();
            var target = new IndexedPropertyExample();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
           
            // NOTE: This test is sensitve to reflection ordering from the BCL:
            Action test = () => t.GetProperties()[0].SetValue(target, 6);
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescriptiondPropertyWillThrowIfAccessedIncorrectly()
        {
            TypeCache.Clear();
            var target = new IndexedPropertyExample();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
            Action test = () => t.GetProperty("ReadOnlyProperty").SetValue(target, "Test", 6);
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescriptiondPropertyWillThrowIfSetterIsNotReflected()
        {
            TypeCache.Clear();
            var target = new IndexedPropertyExample();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
            Action test = () => t.GetProperty("ReadOnlyProperty").SetValue(target, "Test");
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescriptiondPropertyWillNotThrowIfEnsureSetterIsCalledWithPrivateAccess()
        {
            TypeCache.Clear();
            var target = new IndexedPropertyExample();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
            Action test = () => t.GetProperty("ReadOnlyProperty").SetValue(target, "Test");
            test.ShouldThrow<InvalidOperationException>();

            t.GetProperty("ReadOnlyProperty").EnsureSetter();
            test.ShouldNotThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescriptionHasDefaultCtorReturnsTrueAsExpected()
        {
            TypeCache.Clear();
            typeof(DescribedType).GetTypeDescription(MemberTypes.Property)
                .HasDefaultCtor.Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescriptionHasDefaultCtorReturnsFalseAsExpected()
        {
            TypeCache.Clear();
            typeof(TypeWithoutDefaultCtor).GetTypeDescription(MemberTypes.Property)
                .HasDefaultCtor.Should().BeFalse();
        }

        [TestMethod]
        public void TypeDescriptionCanInitializeWithoutDefaultCtor()
        {
            TypeCache.Clear();
            typeof(TypeWithoutDefaultCtor).GetTypeDescription(MemberTypes.Property)
                .CreateUninitializedInstance().Should().NotBeNull();
        }

        [TestMethod]
        public void TypeDescription_can_load_multiple_indexed_properties()
        {
            TypeCache.Clear();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
            t.GetProperties().Should().NotBeNull();
        }

        [TestMethod]
        public void TypeDescription_fields_are_reflected_on_demand()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property);
            t.GetFields().Should().NotBeNull();
        }

        [TestMethod]
        public void TypeDescription_fields_are_not_null_if_reflected()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            t.GetFields().Should().NotBeNull();
        }

        [TestMethod]
        public void TypeDescription_properties_are_reflected_on_demand()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Field);
            t.GetProperties().Should().NotBeNull();
        }

        [TestMethod]
        public void TypeDescription_properties_are_not_null_if_reflected()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property);
            t.GetProperties().Should().NotBeNull();
        }

        [TestMethod]
        public void TypeDescription_methods_are_reflected_on_demand()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property);
            t.GetMethods().Should().NotBeNull();
        }

        [TestMethod]
        public void TypeDescription_methods_are_not_null_if_reflected()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Method);
            t.GetMethods().Should().NotBeNull();
        }

        [TestMethod]
        public void TypeDescription_fields_are_not_replaced_when_bindings_match()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Field, BindingFlags.Public | BindingFlags.Instance);

            var items1 = t.GetFields();
            t.ReflectFields(BindingFlags.Public | BindingFlags.Instance);
            var items2 = t.GetFields();

            ReferenceEquals(items1, items2).Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescription_fields_are_replaced_when_bindings_are_not_default()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Field, BindingFlags.Public | BindingFlags.Instance);

            var items1 = t.GetFields();
            var items2 = t.GetFields(BindingFlags.Public | BindingFlags.Instance);

            ReferenceEquals(items1, items2).Should().BeFalse();
        }

        [TestMethod]
        public void TypeDescription_properties_are_not_replaced_when_bindings_match()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance);

            var items1 = t.GetProperties();
            t.ReflectProperties(BindingFlags.Public | BindingFlags.Instance);
            var items2 = t.GetProperties();

            ReferenceEquals(items1, items2).Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescription_properties_are_replaced_when_bindings_are_not_default()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance);

            var items1 = t.GetProperties();
            var items2 = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            ReferenceEquals(items1, items2).Should().BeFalse();
        }

        [TestMethod]
        public void TypeDescription_methods_are_not_replaced_when_bindings_match()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Method, BindingFlags.Public | BindingFlags.Instance);

            var items1 = t.GetMethods();
            t.ReflectMethods(BindingFlags.Public | BindingFlags.Instance);
            var items2 = t.GetMethods();

            ReferenceEquals(items1, items2).Should().BeTrue();
        }

        [TestMethod]
        public void TypeDescription_methods_are_replaced_when_bindings_are_not_default()
        {
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Method, BindingFlags.Public | BindingFlags.Instance);

            var items1 = t.GetMethods();
            var items2 = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            ReferenceEquals(items1, items2).Should().BeFalse();
        }

        [TestMethod]
        public void TypeDescription_create_instance_throws_InvalidOperation_if_no_default_constructor()
        {
            TypeCache.Clear();
            var t = typeof(TypeWithoutDefaultCtor).GetTypeDescription(MemberTypes.Property);

            Action target = () => t.CreateInstance();
            target.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescription_can_create_create_instance_with_known_ctor()
        {
            TypeCache.Clear();
            var t = typeof(TypeWithoutDefaultCtor).GetTypeDescription(MemberTypes.Property);
            var ctorExists = t.ReflectCtor(new[] { typeof(string), typeof(int) });
            ctorExists.Should().BeTrue();

            var target = t.CreateInstance(new object[] { "Test", 54 }) as TypeWithoutDefaultCtor;

            target.Should().NotBeNull();
            target.SomeValue1.Should().Be("Test");
            target.SomeValue2.Should().Be(54);
        }

        [TestMethod]
        public void TypeDescription_get_field_value_is_as_expected()
        {
            // Set up using .NET reflection:
            var includeNonPublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var instance = new DescribedType();
            instance.GetType().GetField("privateField", includeNonPublic).SetValue(instance, "TheTest");
            
            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Field, includeNonPublic);

            var result = t.GetField("privateField").GetValue(instance);
            result.Should().Be("TheTest");
        }

        [TestMethod]
        public void TypeDescription_set_field_value_is_as_expected()
        {
            var includeNonPublic = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var instance = new DescribedType();

            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Field, includeNonPublic);
            t.GetField("privateField").SetValue(instance, "TheSecondTest");

            // Verify using .NET reflection:
            var result = instance.GetType().GetField("privateField", includeNonPublic).GetValue(instance);
            result.Should().Be("TheSecondTest");
        }

        [TestMethod]
        public void TypeDescription_get_indexed_property_value_is_as_expected()
        {
            var instance = new IndexedPropertyExample();

            instance[1] = 5;
            instance["2"] = "TEST";

            TypeCache.Clear();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
            var indexedProperties = t.GetIndexedProperties();

            var result1 = (int)indexedProperties[0].GetValue(instance, 1);
            var result2 = (string)indexedProperties[1].GetValue(instance, "2");

            result1.Should().Be(5);
            result2.Should().Be("TEST");
        }

        [TestMethod]
        public void TypeDescription_set_indexed_property_value_is_as_expected()
        {
            var instance = new IndexedPropertyExample();

            TypeCache.Clear();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
            var indexedProperties = t.GetIndexedProperties();

            indexedProperties[0].SetValue(instance, 5, 1);
            indexedProperties[1].SetValue(instance, "TEST", "2");

            var result1 = (int)indexedProperties[0].GetValue(instance, 1);
            var result2 = (string)indexedProperties[1].GetValue(instance, "2");

            result1.Should().Be(5);
            result2.Should().Be("TEST");
        }

        [TestMethod]
        public void TypeDescription_set_indexed_property_throws_InvalidOperationException_with_no_index()
        {
            var instance = new IndexedPropertyExample();

            TypeCache.Clear();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
            var indexedProperties = t.GetIndexedProperties();

            Action test = () => indexedProperties[0].SetValue(instance, 5);
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescription_get_indexed_property_throws_with_no_index()
        {
            var instance = new IndexedPropertyExample();

            instance[1] = 5;

            TypeCache.Clear();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
            var indexedProperties = t.GetIndexedProperties();

            Action test = ()=> indexedProperties[0].GetValue(instance);
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescription_get_property_value_throws_with_index()
        {
            var includePublic = BindingFlags.Public | BindingFlags.Instance;

            var instance = new DescribedType();
            instance.PublicProperty = "TheTest";

            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property, includePublic);

            Action test = () => t.GetProperty("PublicProperty").GetValue(instance, 1);
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescription_get_property_value_is_as_expected()
        {
            var includePublic = BindingFlags.Public | BindingFlags.Instance;

            var instance = new DescribedType();
            instance.PublicProperty = "TheTest";

            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property, includePublic);

            var result = t.GetProperty("PublicProperty").GetValue(instance);
            result.Should().Be("TheTest");
        }

        [TestMethod]
        public void TypeDescription_get_property_index_types_is_as_expected()
        {
            var instance = new IndexedPropertyExample();

            TypeCache.Clear();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
            var indexedProperties = t.GetIndexedProperties();

            var indexType1 = indexedProperties[0].GetIndexParameterTypes();
            var indexType2 = indexedProperties[1].GetIndexParameterTypes();

            indexType1.GetLength(0).Should().Be(1);
            indexType1[0].Should().Be(typeof(int));

            indexType2.GetLength(0).Should().Be(1);
            indexType2[0].Should().Be(typeof(string));
        }

        [TestMethod]
        public void TypeDescription_read_only_property_is_read_only()
        {
            var instance = new IndexedPropertyExample();

            TypeCache.Clear();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
            var target = t.GetProperty("ReadOnlyProperty");
            target.IsReadOnly.Should().Be(true);
        }

        [TestMethod]
        public void TypeDescription_property_type_is_as_expected()
        {
            var instance = new IndexedPropertyExample();

            TypeCache.Clear();
            var t = typeof(IndexedPropertyExample).GetTypeDescription(MemberTypes.Property);
            var target = t.GetProperty("ReadOnlyProperty");
            target.PropertyType.Should().Be(typeof(string));
        }

        [TestMethod]
        public void TypeDescription_set_property_value_is_as_expected()
        {
            var includePublic = BindingFlags.Public | BindingFlags.Instance;
            var instance = new DescribedType();

            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property, includePublic);
            t.GetProperty("PublicProperty").SetValue(instance, "TheSecondTest");

            instance.PublicProperty.Should().Be("TheSecondTest");
        }

        [TestMethod]
        public void TypeDescription_set_property_value_throws_with_index()
        {
            var includePublic = BindingFlags.Public | BindingFlags.Instance;
            var instance = new DescribedType();

            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property, includePublic);
            Action test = () => t.GetProperty("PublicProperty").SetValue(instance, "TheSecondTest", 3);
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescription_get_index_types_throws_when_property_not_indexed()
        {
            var includePublic = BindingFlags.Public | BindingFlags.Instance;
            var instance = new DescribedType();

            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Property, includePublic);
            Action test = () => t.GetProperty("PublicProperty").GetIndexParameterTypes();
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescription_invoke_action_sets_value_as_expected()
        {
            var includePublic = BindingFlags.Public | BindingFlags.Instance;
            var instance = new DescribedType();

            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Method, includePublic);
            var method = t.GetMethods().First(m => m.MemberName == "PublicAction");

            method.IsVoid.Should().BeTrue();
            method.ReturnType.Should().Be(typeof(void));
            method.InvokeAction(instance, new object[] { 25 });

            instance.InternalValueProperty.Should().Be(25);
        }

        [TestMethod]
        public void TypeDescription_invoke_func_returns_value_as_expected()
        {
            var includePublic = BindingFlags.Public | BindingFlags.Instance;
            var instance = new DescribedType();

            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Method, includePublic);
            var method = t.GetMethods().First(m => m.MemberName =="PublicFunc");

            method.IsVoid.Should().BeFalse();
            method.ReturnType.Should().Be(typeof(string));
            var result = method.InvokeFunc(instance, new object[] { "The", "Test", 25 });

            result.Should().Be("TheTest25");
        }

        [TestMethod]
        public void TypeDescription_invoke_func_throws_InvalidOperationException_when_void()
        {
            var includePublic = BindingFlags.Public | BindingFlags.Instance;
            var instance = new DescribedType();

            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Method, includePublic);
            var method = t.GetMethods().First(m => m.MemberName == "PublicAction");

            method.IsVoid.Should().BeTrue();

            Action test = () => method.InvokeFunc(instance, new object[] { "The", "Test", 25 });
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void TypeDescription_invoke_action_throws_InvalidOperationException_when_not_void()
        {
            var includePublic = BindingFlags.Public | BindingFlags.Instance;
            var instance = new DescribedType();

            TypeCache.Clear();
            var t = typeof(DescribedType).GetTypeDescription(MemberTypes.Method, includePublic);
            var method = t.GetMethods().First(m => m.MemberName == "PublicFunc");

            method.IsVoid.Should().BeFalse();
            Action test = () => method.InvokeAction(instance, new object[] { 25 });
            test.ShouldThrow<InvalidOperationException>();
        }
    }
}
