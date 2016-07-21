using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests
{
    [TestClass]
    public class FluencyExtensionsTests
    {
        [TestMethod]
        public void ExpandIncreasesSizeWhenNewSizeIsGreater()
        {
            new int[5].Expand(15).Length.Should().Be(15);
        }

        [TestMethod]
        public void ExpandReturnsOriginalArrayIfNotExpandedWithSmallerNewSize()
        {
            var target = new int[5];
            ReferenceEquals(target.Expand(4), target).Should().BeTrue();
        }

        [TestMethod]
        public void ExpandReturnsOriginalArrayIfNotExpandedWithEqualNewSize()
        {
            var target = new int[5];
            ReferenceEquals(target.Expand(5), target).Should().BeTrue();
        }

        [TestMethod]
        public void ExpandNullReturnsNull()
        {
            int[] target = null;
            target.Expand(100).Should().BeNull();
        }

        [TestMethod]
        public void IsNullOrEmptyIsTrueForNullArray()
        {
            FluencyExtensions.IsNullOrEmpty<int>(null).Should().BeTrue();    
        }

        [TestMethod]
        public void IsNullOrEmptyIsTrueForEmptyArray()
        {
            (new int[] { }).IsNullOrEmpty().Should().BeTrue();
        }

        [TestMethod]
        public void IsNullOrEmptyIsFalseForArrayWithElements()
        {
            (new int[] { 1, 2 }).IsNullOrEmpty().Should().BeFalse();
        }

        [TestMethod]
        public void ForEachIsNullTolerantForCollection()
        {
            IEnumerable<object> items = null;
            Action test = () => items.ForEach(o => { });
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void ForEachIsNullTolerantForAction()
        {
            IEnumerable<object> items = new object[] { new object(), new object() };
            Action test = () => items.ForEach(null);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void ForEachIteratesAllItems()
        {
            IEnumerable<object> items = new object[] { new object(), new object() };
            var counter = 0;
            items.ForEach(o => { counter = counter + 1; });
            counter.Should().Be(2);
        }

        [TestMethod]
        public void ToFormatMatchesStringFormat()
        {
            "Test {0}".ToFormat(1).Should().Be(string.Format("Test {0}", 1));
        }

        [TestMethod]
        public void ArrayCombineWorksAsExpectedWith2ValidArrays()
        {
            var expected = new int[] {1,2,3,4,5,6};
            var array1 = new int[] { 1, 2, 3 };
            var array2 = new int[] { 4, 5, 6 };

            array1.Combine(array2).Should().ContainInOrder(expected);
        }

        [TestMethod]
        public void ArrayCombineReturnsArray2WithArray1Null()
        {
            var array1 = default(int[]);
            var array2 = new int[] { 4, 5, 6 };

            array1.Combine(array2).Should().ContainInOrder(array2);
            ReferenceEquals(array2, array1.Combine(array2)).Should().BeTrue();
        }

        [TestMethod]
        public void ArrayCombineReturnsArray1WithArray2Null()
        {
            var array1 = new int[] { 1, 2, 3 };
            var array2 = default(int[]);

            array1.Combine(array2).Should().ContainInOrder(array1);
            ReferenceEquals(array1, array1.Combine(array2)).Should().BeTrue();
        }

        [TestMethod]
        public void CopyRangeReturnsNullWhenSourceArrayIsNull()
        {
            int[] a = null;
            a.CopyRange(0, 0).Should().BeNull();
        }

        [TestMethod]
        public void CopyRangeThrowsArgumentOutOfRangeExceptionWhenOffsetIsLessThanZero()
        {
            var a = new int[5];
            Action test = () => a.CopyRange(-1, 3);
            test.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void CopyRangeReturnsNullIfOffsetPlusSizeExceedsLength()
        {
            var a = new int[5];
            a.CopyRange(5, 1).Should().BeNull();
        }

        [TestMethod]
        public void CopyRangeReturnsExpectedRangeWithoutOverflow()
        {
            var a = new int[] { 1, 2, 3, 4, 5, 6 };
            var b = a.CopyRange(2, 5);
            b.Length.Should().Be(4);
            b.Should().ContainInOrder(3, 4, 5, 6);
        }

        [TestMethod]
        public void CopyRangeReturnsExpectedRangeOfWholeArray()
        {
            var a = new int[] { 1, 2, 3, 4, 5, 6 };
            var b = a.CopyRange(0, 6);
            b.Length.Should().Be(6);
            b.Should().ContainInOrder(1, 2, 3, 4, 5, 6);
        }

        [TestMethod]
        public async Task InvokeAsAwaitableInvokesFunctionAsyncAndReturnsExpectedValue()
        {
            var invoked = false;
            Func<string> func = () =>
            {
                invoked = true; return "invoked";
            };

            var result = await func.InvokeAsAwaitable();
            result.Should().Be("invoked");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public async Task InvokeAsAwaitableInvokesActionAsync()
        {
            var invoked = false;
            Action sub = () =>
            {
                invoked = true;
            };

            await sub.InvokeAsAwaitable();
            invoked.Should().BeTrue();
        }
    }
}
