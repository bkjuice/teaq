using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Teaq.QueryGeneration;

namespace Teaq.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public void BuildBatchWriterContextWithConnectionStringReturnsValidInstance()
        {
            Repository.BuildBatchWriter("test").Should().NotBeNull();
        }

        [TestMethod]
        public void BuildBatchWriterContextWithConnectionStringAndBuilderAndBatchReturnsValidInstanceWithReferencedTypes()
        {
            var batch = new QueryBatch();
            var context = Repository.BuildBatchWriter("test", new SqlConnectionBuilder(), batch);
            context.Should().NotBeNull();
            context.QueryBatch.Should().BeSameAs(batch);
        }

        [TestMethod]
        public void TryGetEntityConfigReturnsNullWhenModelIsNull()
        {
            IDataModel model = null;
            model.TryGetEntityConfig<object>().Should().BeNull();
        }

        [TestMethod]
        public void BuildContextReturnsValidInstanceWithProvidedBatch()
        {
            var batch = new QueryBatch();
            var context = Repository.BuildBatchReader("test", batch);
            context.Should().NotBeNull();
            context.QueryBatch.Should().BeSameAs(batch);
        }
    }
}
