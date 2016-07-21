using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Teaq.FastReflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests.FastReflection
{
    [TestClass]
    public class CollectionDescriptionTests
    {
        [TestMethod]
        public void IReadOnlyCollectionDoesNotOfferAddMethod()
        {
            var collectionDesc = typeof(IReadOnlyCollection<int>).GetTypeDescription()
                as CollectionDescription;

            collectionDesc.Should().NotBeNull();
            collectionDesc.CanAddItems.Should().BeFalse();
        }

        [TestMethod]
        public void ReadOnlyCollectionDoesNotOfferAddMethod()
        {
            var collectionDesc = typeof(ReadOnlyCollection<int>).GetTypeDescription()
                as CollectionDescription;

            collectionDesc.Should().NotBeNull();
            collectionDesc.CanAddItems.Should().BeFalse();
        }

        [TestMethod]
        public void ReadOnlyObservableCollectionDoesNotOfferAddMethod()
        {
            var collectionDesc = typeof(ReadOnlyObservableCollection<int>).GetTypeDescription()
                as CollectionDescription;

            collectionDesc.Should().NotBeNull();
            collectionDesc.CanAddItems.Should().BeFalse();
        }

        [TestMethod]
        public void CollectionResultsInCollectionDescription()
        {
            typeof(List<int>).GetTypeDescription()
                .Should().BeOfType<CollectionDescription>();
        }

        [TestMethod]
        public void EnumerableResultsInCollectionDescription()
        {
            typeof(IEnumerable<int>).GetTypeDescription()
                .Should().BeOfType<CollectionDescription>();
        }

        [TestMethod]
        public void CollectionDescriptionIdentifiesItemTypeAsGenericType()
        {
            (typeof(List<int>).GetTypeDescription() as CollectionDescription)
                .ItemType.ReflectedType.Should().Be(typeof(int));

        }

        [TestMethod]
        public void CollectionListCanAddItemsShouldBeTrue()
        {
            (typeof(List<int>).GetTypeDescription() as CollectionDescription)
                .CanAddItems.Should().BeTrue();
        }

        [TestMethod]
        public void CollectionCanAddItemsShouldBeFalseForEnumerableWithNoAdd()
        {
            (typeof(EnumerableTypeWithoutAddMethod).GetTypeDescription() as CollectionDescription)
                .CanAddItems.Should().BeFalse();
        }

        [TestMethod]
        public void CollectionAddItemThrowsInvalidOperationExceptionWhenAddNotAvailable()
        {
            var description = typeof(EnumerableTypeWithoutAddMethod).GetTypeDescription() as CollectionDescription;
            var instance = new EnumerableTypeWithoutAddMethod(new List<int>());
            Action test = ()=> description.AddItem(instance, 5);
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void CollectionAddItemAddsWithUntypedAddMethod()
        {
            var description = typeof(EnumerableTypeWithUntypedAddMethod).GetTypeDescription() as CollectionDescription;
            var instance = new EnumerableTypeWithUntypedAddMethod(new List<int>());
            Action test = () => description.AddItem(instance, 5);
            test.ShouldNotThrow();
            instance.Count().Should().Be(1);
        }

        [TestMethod]
        public void CollectionAddItemAddsWithUntypedVoidAddMethod()
        {
            var description = typeof(EnumerableTypeWithUntypedVoidAddMethod).GetTypeDescription() as CollectionDescription;
            var instance = new EnumerableTypeWithUntypedVoidAddMethod(new List<int>());
            Action test = () => description.AddItem(instance, 5);
            test.ShouldNotThrow();
            instance.Count().Should().Be(1);
        }
    }
}
