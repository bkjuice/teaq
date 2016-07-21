using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.FastReflection;
using Teaq.Tests.FastReflection.SupportingTypes;

namespace Teaq.Tests.FastReflection
{
    [TestClass]
    public class FieldDescriptionTests
    {
        [TestMethod]
        public void FieldDescriptionSetFieldValueSucceedsForValidField()
        {
            var target = new FieldDescription(typeof(DescribedType).GetField("PublicField"));
            var subject = new DescribedType();
            subject.PublicField.Should().BeNull();

            target.SetValue(subject, "Value");
            subject.PublicField.Should().Be("Value");
        }

        [TestMethod]
        public void FieldDescriptionFieldTypeMatchesActualType()
        {
            var target = new FieldDescription(typeof(DescribedType).GetField("PublicField"));
            target.FieldType.Should().Be(typeof(string));
        }

        [TestMethod]
        public void FieldDescriptionFieldTypeHandleTypeMatchesActualType()
        {
            var target = new FieldDescription(typeof(DescribedType).GetField("PublicField"));
            Type.GetTypeFromHandle(target.FieldTypeHandle).Should().Be(typeof(string));
        }
    }
}