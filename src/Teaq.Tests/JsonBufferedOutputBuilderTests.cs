using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Teaq.Tests
{
    [TestClass]
    public class JsonBufferedOutputBuilderTests
    {
        [TestMethod]
        public void JsonOutputBuilderThrowsInvalidOperationExceptionWhenCompleted()
        {
            var builder = JsonOutputBuilder.GetBuilder();
            builder.Close();
            Action test = () => builder.StartObject();
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void JsonOutputBuilderHandlesBufferExpansionAsExpected()
        {
            var builder = JsonOutputBuilder.GetBuilder(8);
            builder.StartArray("sevenCh");

            Action test = () =>
            {
                builder.StartObject();
                builder.WriteObjectMember("bufferMustExpand", "largeEnoughValue", JsonOutputValueKind.StringValue);
                builder.CloseScope();
                builder.Close();
            };

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void JsonOutputBuilderEmitsValidEmptyJson()
        {
            var builder = JsonOutputBuilder.GetBuilder();
            builder.Close();
            var result = builder.ToString();
            result.Should().Be("{}");
        }

        [TestMethod]
        public void JsonOutputBuilderEmitsValidEmptyJsonWithRootObject()
        {
            var builder = JsonOutputBuilder.GetBuilder();
            builder.StartObject("root");
            builder.Close();
            var result = builder.ToString();
            result.Should().Be("{\"root\":{}}");
        }

        [TestMethod]
        public void JsonOutputBuilderEmitsValidEmptyJsonWithRootArray()
        {
            var builder = JsonOutputBuilder.GetBuilder();
            builder.StartArray("root");
            builder.Close();
            var result = builder.ToString();
            result.Should().Be("{\"root\":[]}");
        }

        [TestMethod]
        public void JsonOutputBuilderWritesSingleValueAsExpectedInNakedRoot()
        {
            var builder = JsonOutputBuilder.GetBuilder();
            builder.WriteObjectMember("test", "value1", JsonOutputValueKind.StringValue);
            builder.Close();
            var result = builder.ToString();
            result.Should().Be("{\"test\":\"value1\"}");
        }

        [TestMethod]
        public void JsonOutputBuilderWritesMultipleValuesAsExpectedInNakedRoot()
        {
            var builder = JsonOutputBuilder.GetBuilder();
            builder.WriteObjectMember("test1", "value1", JsonOutputValueKind.StringValue);
            builder.WriteObjectMember("test2", "value2", JsonOutputValueKind.StringValue);
            builder.Close();
            var result = builder.ToString();
            result.Should().Be("{\"test1\":\"value1\",\"test2\":\"value2\"}");
        }

        [TestMethod]
        public void JsonOutputBuilderWritesMultipleValuesAsExpectedInNamedRoot()
        {
            var builder = JsonOutputBuilder.GetBuilder();
            builder.StartObject("root");
            builder.WriteObjectMember("test1", "value1", JsonOutputValueKind.StringValue);
            builder.WriteObjectMember("test2", "value2", JsonOutputValueKind.StringValue);
            builder.Close();
            var result = builder.ToString();
            result.Should().Be("{\"root\":{\"test1\":\"value1\",\"test2\":\"value2\"}}");
        }

        [TestMethod]
        public void JsonOutputBuilderWritesMultipleValuesInArrayAsExpectedInNamedRoot()
        {
            var builder = JsonOutputBuilder.GetBuilder();
            builder.StartArray("root");
            builder.WriteArrayMember("value1", JsonOutputValueKind.StringValue);
            builder.WriteArrayMember("value2", JsonOutputValueKind.StringValue);
            builder.Close();
            var result = builder.ToString();
            result.Should().Be("{\"root\":[\"value1\",\"value2\"]}");
        }
    }
}
