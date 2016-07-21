using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests
{
    [TestClass]
    public class CLRVerificationTests
    {
        [TestMethod]
        public void StructFieldAssignmentIsHandledAsIfByReference()
        {
            var subject = new BadMutableStructThatSomeoneMightUse();
            subject.Value = 23;
            subject.Value.Should().Be(23);
        }

        [TestMethod]
        public void StructFieldAssignmentIsHandledAsIfByReferenceButNotOnCopiedInstances()
        {
            var subject = new BadMutableStructThatSomeoneMightUse();
            var otherSubject = subject;
            subject.Value = 23;
            otherSubject.Value.Should().NotBe(23);
        }

        [TestMethod]
        public void StructFieldAssignmentIsHandledByValueWhenMethodCalled()
        {
            var subject = new BadMutableStructThatSomeoneMightUse();
            SetValueByMethod(subject, 99);
            subject.Value.Should().NotBe(99);
        }

        [TestMethod]
        public void StructFieldAssignmentByReflectionIsNotAsIfByReference()
        {
            var info = typeof(BadMutableStructThatSomeoneMightUse).GetField("Value", BindingFlags.Public | BindingFlags.Instance);
            var subject = new BadMutableStructThatSomeoneMightUse();
            info.SetValue(subject, 23);
            subject.Value.Should().NotBe(23);
        }

        [TestMethod]
        public void StructFieldAssignmentByReflectionIsAsIfByReferenceIfBoxed()
        {
            var info = typeof(BadMutableStructThatSomeoneMightUse).GetField("Value", BindingFlags.Public | BindingFlags.Instance);
            object subject = new BadMutableStructThatSomeoneMightUse();
            info.SetValue(subject, 23);
            ((BadMutableStructThatSomeoneMightUse)subject).Value.Should().Be(23);
        }

        private static void SetValueByMethod(BadMutableStructThatSomeoneMightUse subject, int value)
        {
            subject.Value = value;
        }

        public struct BadMutableStructThatSomeoneMightUse
        {
            public int Value;
        }
    }
}
