using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class DataReaderJsonExtensionTests
    {
        [TestMethod]
        public async Task ReadToJsonAsyncHandlesPrimitiveNumberAsExpected()
        {
            IDataReader reader = this.CreateReaderFromTableWithValue<int>(43);
            var builder = JsonOutputBuilder.GetBuilder();
            builder.StartArray("j");
            await reader.ReadToJsonAsync(builder);

            builder.Close();
            builder.ToString().Should().Be("{\"j\":[43]}");
        }

        [TestMethod]
        public void ReadToJsonInitializesValidJsonWhenReaderIsNull()
        {
            IDataReader reader = null;
            var builder = JsonOutputBuilder.GetBuilder();
            builder.StartArray("j");
            reader.ReadToJson(builder);

            builder.Close();
            builder.ToString().Should().Be("{\"j\":[]}");
        }

        [TestMethod]
        public void ReadToJsonHandlesPrimitiveNumberAsExpected()
        {
            IDataReader reader = this.CreateReaderFromTableWithValue<int>(43);
            var builder = JsonOutputBuilder.GetBuilder();
            builder.StartArray("j");
            reader.ReadToJson(builder);

            builder.Close();
            builder.ToString().Should().Be("{\"j\":[43]}");
        }

        [TestMethod]
        public void DataReaderReadEntityReturnsArrayOfJsonCustomersAsArrayOfObjectsWithStringValuesFromStream()
        {
            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.Parse("10/10/15 1:27:29 AM"), Change = 2, Modified = DateTimeOffset.Parse("10/10/15 1:27:29 AM +00:00") });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.Parse("10/10/15 1:27:29 AM"), Change = 2, Modified = DateTimeOffset.Parse("10/10/15 1:27:29 AM +00:00") });

            string json = null;
            using (var s = new MemoryStream(4096))
            using (var w = new StreamWriter(s))
            {
                var builder = JsonOutputBuilder.GetBuilder(w);
                builder.StartArray("customers");
                tableHelper.GetReader().ReadToJson(builder);
                builder.Close();
                w.Flush();
                s.Position = 0;
                using (var r = new StreamReader(s))
                {
                    json = r.ReadToEnd();
                }
            }
           
            json.Should().NotBeNullOrEmpty();
            json.Should().Be(
                "{\"customers\":[{\"CustomerId\":1,\"CustomerKey\":\"1\",\"Inception\":\"2015-10-10T01:27:29.0000000\",\"Modified\":\"2015-10-10T01:27:29.0000000+00:00\",\"Change\":2},{\"CustomerId\":2,\"CustomerKey\":\"2\",\"Inception\":\"2015-10-10T01:27:29.0000000\",\"Modified\":\"2015-10-10T01:27:29.0000000+00:00\",\"Change\":2}]}");
        }

        [TestMethod]
        public void DataReaderReadEntityReturnsArrayOfJsonCustomersAsArrayOfObjectsWithStringValues()
        {
            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.Parse("10/10/15 1:27:29 AM"), Change = 2, Modified = DateTimeOffset.Parse("10/10/15 1:27:29 AM +00:00") });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.Parse("10/10/15 1:27:29 AM"), Change = 2, Modified = DateTimeOffset.Parse("10/10/15 1:27:29 AM +00:00") });

            var builder = JsonOutputBuilder.GetBuilder();
            builder.StartArray("customers");
            tableHelper.GetReader().ReadToJson(builder);
            builder.Close();
            var result = builder.ToString();

            result.Should().NotBeNullOrEmpty();
            result.Should().Be(
                "{\"customers\":[{\"CustomerId\":1,\"CustomerKey\":\"1\",\"Inception\":\"2015-10-10T01:27:29.0000000\",\"Modified\":\"2015-10-10T01:27:29.0000000+00:00\",\"Change\":2},{\"CustomerId\":2,\"CustomerKey\":\"2\",\"Inception\":\"2015-10-10T01:27:29.0000000\",\"Modified\":\"2015-10-10T01:27:29.0000000+00:00\",\"Change\":2}]}");
        }

        [TestMethod]
        public void DataReaderReadEntityReturnsArrayOfJsonCustomersAsArrayOfObjectsWithMappedValues()
        {
            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.Parse("10/10/15 1:27:29 AM"), Change = 2, Modified = DateTimeOffset.Parse("10/10/15 1:27:29 AM +00:00") });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.Parse("10/10/15 1:27:29 AM"), Change = 2, Modified = DateTimeOffset.Parse("10/10/15 1:27:29 AM +00:00") });

            var mapping = new JsonMapping();
            mapping.AddFieldMapping("CustomerKey", "customerKey", JsonOutputValueKind.NumberValue);
            mapping.AddFieldMapping("CustomerId", "customerId", JsonOutputValueKind.NumberValue);
            var builder = JsonOutputBuilder.GetBuilder("customers", true);
            tableHelper.GetReader().ReadToJson(builder, mapping);
            builder.Close();
            var result = builder.ToString();

            result.Should().NotBeNullOrEmpty();
            result.Should().Be(
                "{\"customers\":[{\"customerId\":1,\"customerKey\":1,\"Inception\":\"2015-10-10T01:27:29.0000000\",\"Modified\":\"2015-10-10T01:27:29.0000000+00:00\",\"Change\":2},{\"customerId\":2,\"customerKey\":2,\"Inception\":\"2015-10-10T01:27:29.0000000\",\"Modified\":\"2015-10-10T01:27:29.0000000+00:00\",\"Change\":2}]}");
        }

        public IDataReader CreateReaderFromTableWithValue<TValue>(object value)
        {
            var table = new DataTable();
            table.Columns.Add(new DataColumn("Target", typeof(TValue)));
            var row = table.NewRow()[0] = value;
            table.Rows.Add(row);
            return table.CreateDataReader();
        }
    }
}
