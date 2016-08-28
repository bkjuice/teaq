using System;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Xml;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Teaq.Configuration;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class DataReaderExtensionTests
    {
        [TestMethod]
        public void ReadEntitiesCanReadStringToNullableBool()
        {
            var items = this.CreateReaderFromTableWithValue<string>("true")
              .ReadEntities<EntityWithProperty<bool?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(true);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToBool()
        {
            var items = this.CreateReaderFromTableWithValue<string>("false")
              .ReadEntities<EntityWithProperty<bool>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(false);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToNullableByte()
        {
            var items = this.CreateReaderFromTableWithValue<string>("210")
              .ReadEntities<EntityWithProperty<byte?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(210);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToByte()
        {
            var items = this.CreateReaderFromTableWithValue<string>("111")
              .ReadEntities<EntityWithProperty<byte>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(111);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToNullableFloat()
        {
            var items = this.CreateReaderFromTableWithValue<string>("100")
              .ReadEntities<EntityWithProperty<float?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(100);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToFloat()
        {
            var items = this.CreateReaderFromTableWithValue<string>("100")
              .ReadEntities<EntityWithProperty<float>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(100);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToNullableDecimal()
        {
            var items = this.CreateReaderFromTableWithValue<string>("100")
              .ReadEntities<EntityWithProperty<decimal?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(100);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToDecimal()
        {
            var items = this.CreateReaderFromTableWithValue<string>("100")
              .ReadEntities<EntityWithProperty<decimal>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(100);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToNullableInt64()
        {
            var items = this.CreateReaderFromTableWithValue<string>("100")
              .ReadEntities<EntityWithProperty<long?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(100);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToInt64()
        {
            var items = this.CreateReaderFromTableWithValue<string>("100")
              .ReadEntities<EntityWithProperty<long>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(100);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToNullableInt16()
        {
            var items = this.CreateReaderFromTableWithValue<string>("100")
              .ReadEntities<EntityWithProperty<short?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(100);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToInt16()
        {
            var items = this.CreateReaderFromTableWithValue<string>("100")
              .ReadEntities<EntityWithProperty<short>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(100);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToNullableInt32()
        {
            var items = this.CreateReaderFromTableWithValue<string>("100")
              .ReadEntities<EntityWithProperty<int?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(100);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToInt32()
        {
            var items = this.CreateReaderFromTableWithValue<string>("100")
              .ReadEntities<EntityWithProperty<int>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(100);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToNullableDouble()
        {
            var items = this.CreateReaderFromTableWithValue<string>("1.77")
              .ReadEntities<EntityWithProperty<double?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(1.77);
        }

        [TestMethod]
        public void ReadEntitiesCanReadStringToDouble()
        {
            var items = this.CreateReaderFromTableWithValue<string>("1.77")
              .ReadEntities<EntityWithProperty<double>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(1.77);
        }

        [TestMethod]
        public void ReadEntitiesIgnoresEmptyColumName()
        {
            var stub = new DataReaderStub();
            stub.FieldCountGetter = () => 1;
            stub.GetFieldTypeFunc = i => typeof(int);
            stub.GetNameFunc = i => string.Empty;

            Action test = () => stub
             .ReadEntities<EntityWithProperty<int>>()
             .ToList();

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void ReadEntitiesIgnoresNullColumName()
        {
            var stub = new DataReaderStub();
            stub.FieldCountGetter = () => 1;
            stub.GetFieldTypeFunc = i => typeof(int);
            stub.GetNameFunc = i => null;

            Action test = () => stub
             .ReadEntities<EntityWithProperty<int>>()
             .ToList();

            test.ShouldNotThrow();
        }

        [TestMethod]
        public void ReadEntitiesCanReadDoubleAsNullableDecimal()
        {
            var items = this.CreateReaderFromTableWithValue<double>(1.77)
               .ReadEntities<EntityWithProperty<decimal?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(1.77M);
        }

        [TestMethod]
        public void ReadEntitiesCanReadDoubleAsNullableFloat()
        {
            var items = this.CreateReaderFromTableWithValue<double>(1.77)
               .ReadEntities<EntityWithProperty<float?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(1.77F);
        }

        [TestMethod]
        public void ReadEntitiesCanReadDoubleAsFloat()
        {
            var items = this.CreateReaderFromTableWithValue<double>(1.66)
               .ReadEntities<EntityWithProperty<float>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(1.66F);
        }

        [TestMethod]
        public void ReadEntitiesCanReadDecimalAsNullableBool()
        {
            var items = this.CreateReaderFromTableWithValue<decimal>(1M)
               .ReadEntities<EntityWithProperty<bool?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(true);
        }

        [TestMethod]
        public void ReadEntitiesCanReadDecimalAsBool()
        {
            var items = this.CreateReaderFromTableWithValue<decimal>(1M)
               .ReadEntities<EntityWithProperty<bool>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(true);
        }

        [TestMethod]
        public void ReadEntitiesCanReadDoubleAsNullableBool()
        {
            var items = this.CreateReaderFromTableWithValue<double>(1)
               .ReadEntities<EntityWithProperty<bool?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(true);
        }

        [TestMethod]
        public void ReadEntitiesCanReadDoubleAsBool()
        {
            var items = this.CreateReaderFromTableWithValue<double>(1)
               .ReadEntities<EntityWithProperty<bool>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(true);
        }

        [TestMethod]
        public void ReadEntitiesCanReadIntegerAsNullableBool()
        {
            var items = this.CreateReaderFromTableWithValue<int>(1)
               .ReadEntities<EntityWithProperty<bool?>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(true);
        }

        [TestMethod]
        public void ReadEntitiesCanReadIntegerAsBool()
        {
            var items = this.CreateReaderFromTableWithValue<int>(1)
               .ReadEntities<EntityWithProperty<bool>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be(true);
        }

        [TestMethod]
        public void ReadEntitiesReadEntitiesAsyncCanReadSqlXml()
        {
            var items =  this.CreateReaderFromTableWithValue<SqlXml>(
                new SqlXml(new XmlTextReader("<test />", XmlNodeType.Document, null)))
                .ReadEntities<EntityWithProperty<string>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be("<test />");
        }

        [TestMethod]
        public void EnumerateEntitiesAsyncEnumeratesAsStringForwardOnly()
        {
            var items = this.CreateReaderFromTableWithValue<string>("a value to test")
               .EnumerateEntities<string>();

            items.First().Should().Be("a value to test");
            items.Any().Should().BeFalse();
        }

        [TestMethod]
        public void EnumerateEntitiesReturnsEmptyCollectionWhenReaderIsNull()
        {
            IDataReader reader = null;
            reader.EnumerateEntities<object>().Any().Should().BeFalse();
        }
        
        [TestMethod]
        public void EnumerateEntitiesEnumeratesAllItemsInOrderUsingDataHandler()
        {
            var handlerStub = new DataHandlerStub<Customer>();

            var handlerInvokedCount = 0;
            handlerStub.Builder = i =>
            {
                handlerInvokedCount++;
                return new Customer { CustomerId = i };
            };

            var readerMock = new Mock<IDataReader>();
            var records = 0;
            readerMock.Setup(r => r.Read()).Returns(() => { records++; return records < 8; });

            var count = 0;
            readerMock.Object.EnumerateEntities<Customer>(handlerStub).ForEach(c =>
            {
                c.CustomerId.Should().Be(count);
                count++;
            });

            count.Should().Be(7);
            handlerInvokedCount.Should().Be(7);
        }

        [TestMethod]
        public void EnumerateEntitiesEnumeratesAsEntityForwardOnly()
        {
            var items = this.CreateReaderFromTableWithValue<SqlXml>(
               new SqlXml(new XmlTextReader("<test />", XmlNodeType.Document, null)))
               .EnumerateEntities<EntityWithProperty<string>>();

            items.First().Target.Should().Be("<test />");
            items.Any().Should().BeFalse();
        }

        [TestMethod]
        public void EnumerateEntitiesEnumeratesAsStringForwardOnly()
        {
            var items = this.CreateReaderFromTableWithValue<string>("a value to test")
               .EnumerateEntities<string>();

            items.First().Should().Be("a value to test");
            items.Any().Should().BeFalse();
        }

        [TestMethod]
        public void EnumerateEntitiesEnumeratesAsIntForwardOnly()
        {
            var items = this.CreateReaderFromTableWithValue<int>(43)
               .EnumerateEntities<int>();

            items.First().Should().Be(43);
            items.Any().Should().BeFalse();
        }

        [TestMethod]
        public void EnumerateEntitiesEnumeratesAsNullableIntForwardOnly()
        {
            var items = this.CreateReaderFromTableWithValue<int>(43).EnumerateEntities<int?>();
            items.First().Value.Should().Be(43);
            items.Any().Should().BeFalse();
        }

        [TestMethod]
        public void EnumerateEntitiesSkipsNullValuesForNullableTypeWhenPolicyIsSkip()
        {
            var items = this.CreateReaderFromTableWithNullValue<int>().EnumerateEntities<int?>(nullPolicy: NullPolicyKind.Skip);
            items.Any().Should().BeFalse();
        }

        [TestMethod]
        public void EnumerateEntitiesIncludesNullValuesForNullableTypeByDefault()
        {
            var items = this.CreateReaderFromTableWithNullValue<int>().EnumerateEntities<int?>();
            items.Any().Should().BeTrue();
        }

        [TestMethod]
        public void EnumerateEntitiesSkipsNullValuesForPrimitiveTypeWhenPolicyIsSkip()
        {
            var items = this.CreateReaderFromTableWithNullValue<int>().EnumerateEntities<int>(nullPolicy: NullPolicyKind.Skip);
            items.Any().Should().BeFalse();
        }

        [TestMethod]
        public void EnumerateEntitiesIncludedNullValuesForPrimitiveTypeByDefault()
        {
            var items = this.CreateReaderFromTableWithNullValue<int>().EnumerateEntities<int>();
            items.Any().Should().BeTrue();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlXml()
        {
            var items = this.CreateReaderFromTableWithValue<SqlXml>(
                new SqlXml(new XmlTextReader("<test />", XmlNodeType.Document, null)))
                .ReadEntities<EntityWithProperty<string>>();

            items.Count.Should().Be(1);
            items[0].Target.Should().Be("<test />");
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlXmlAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlXml>().ReadEntities<EntityWithProperty<string>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().BeNull();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlString()
        {
            var items = this.CreateReaderFromTableWithValue<SqlString>("test").ReadEntities<EntityWithProperty<string>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().Be("test");
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlStringAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlString>().ReadEntities<EntityWithProperty<string>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().BeNull();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlSingle()
        {
            var items = this.CreateReaderFromTableWithValue<SqlSingle>(10f).ReadEntities<EntityWithProperty<float>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().BeGreaterOrEqualTo(10);
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlSingleAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlSingle>().ReadEntities<EntityWithProperty<float?>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlMoney()
        {
            var items = this.CreateReaderFromTableWithValue<SqlMoney>(1m).ReadEntities<EntityWithProperty<decimal>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().Be(1);
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlMoneyAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlMoney>().ReadEntities<EntityWithProperty<decimal?>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlInt64()
        {
            var items = this.CreateReaderFromTableWithValue<SqlInt64>(1L).ReadEntities<EntityWithProperty<long>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().Be(1);
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlInt64AsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlInt64>().ReadEntities<EntityWithProperty<long?>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlInt32()
        {
            var items = this.CreateReaderFromTableWithValue<SqlInt32>(1).ReadEntities<EntityWithProperty<int>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().Be(1);
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlInt32AsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlInt32>().ReadEntities<EntityWithProperty<int?>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlInt16()
        {
            var items = this.CreateReaderFromTableWithValue<SqlInt16>((short)1).ReadEntities<EntityWithProperty<short>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().Be(1);
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlInt16AsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlInt16>().ReadEntities<EntityWithProperty<short?>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlGuid()
        {
            var items = this.CreateReaderFromTableWithValue<SqlGuid>(Guid.Empty).ReadEntities<EntityWithProperty<Guid>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().Be(Guid.Empty);
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlGuidAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlGuid>().ReadEntities<EntityWithProperty<Guid?>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlDouble()
        {
            var items = this.CreateReaderFromTableWithValue<SqlDouble>(10d).ReadEntities<EntityWithProperty<double>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().BeGreaterOrEqualTo(10d);
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlDoubleAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlDouble>().ReadEntities<EntityWithProperty<double?>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlDecimal()
        {
            var items = this.CreateReaderFromTableWithValue<SqlDecimal>(10m).ReadEntities<EntityWithProperty<decimal>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().Be(10m);
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlDecimalAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlDecimal>().ReadEntities<EntityWithProperty<decimal?>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlChars()
        {
            var items = this.CreateReaderFromTableWithValue<SqlChars>(new SqlChars(new char[] { '1', '2' })).ReadEntities<EntityWithProperty<char[]>>();
            items.Count.Should().Be(1);
            items[0].Target.GetLength(0).Should().Be(2);
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlCharsAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlChars>().ReadEntities<EntityWithProperty<char[]>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().BeNull();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlByte()
        {
            var items = this.CreateReaderFromTableWithValue<SqlByte>(new SqlByte(1)).ReadEntities<EntityWithProperty<byte>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().Be(1);
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlByteAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlByte>().ReadEntities<EntityWithProperty<byte?>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlBoolean()
        {
            var items = this.CreateReaderFromTableWithValue<SqlBoolean>(true).ReadEntities<EntityWithProperty<bool>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().BeTrue();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlBooleanAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlBoolean>().ReadEntities<EntityWithProperty<bool?>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlBinary()
        {
            var items = this.CreateReaderFromTableWithValue<SqlBinary>(new byte[] { 1, 2 }).ReadEntities<EntityWithProperty<byte[]>>();
            items.Count.Should().Be(1);
            items[0].Target.GetLength(0).Should().Be(2);
        }

        [TestMethod]
        public void ReadEntitiesCanReadSqlBinaryAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlBinary>().ReadEntities<EntityWithProperty<byte[]>>();
            items.Count.Should().Be(1);
            items[0].Target.Should().BeNull();
        }

        [TestMethod]
        public void ReadEntitiesCanReadNullableDateTime()
        {
            var items = this.CreateReaderFromTableWithValue<SqlDateTime>(DateTime.UtcNow.Date).ReadEntities<EntityWithNullableProperty<DateTime>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void ReadEntitiesCanReadNullableDateTimeAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<SqlDateTime>().ReadEntities<EntityWithNullableProperty<DateTime>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesThrowsInvalidOperationExceptionWhenDataTypeCannotBeConverted()
        {
            var reader = this.CreateReaderFromTableWithValue<DateTimeOffset>(DateTimeOffset.UtcNow);
            Action test = () => reader.ReadEntities<EntityWithNullableProperty<uint>>();
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void ReadEntitiesCanReadNullableDateTimeOffset()
        {
            var items = this.CreateReaderFromTableWithValue<DateTimeOffset>(DateTimeOffset.UtcNow).ReadEntities<EntityWithNullableProperty<DateTimeOffset>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void ReadEntitiesCanReadNullableDateTimeOffsetAsNull()
        {
            var items = this.CreateReaderFromTableWithNullValue<DateTimeOffset>().ReadEntities<EntityWithNullableProperty<DateTimeOffset>>();
            items.Count.Should().Be(1);
            items[0].Target.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesReturnsAllRows()
        {
            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });

            var entities = tableHelper.GetReader().ReadEntities<Customer>();
            entities.Count.Should().Be(2);
            entities[0].CustomerId.Should().Be(1);
            entities[1].CustomerId.Should().Be(2);
        }

        [TestMethod]
        public void ReadEntitiesHandlesNullableIntIgnoresNullValues()
        {
            var tableHelper = new ValueTableHelper<object>();
            int value1 = 1;
            object value2 = null;

            tableHelper.AddRow(value1);
            tableHelper.AddRow(value2);

            var items = tableHelper.GetReader().ReadEntities<int?>();
            items.Count.Should().Be(2);
            items[0].Should().Be(1);
            items[1].HasValue.Should().BeFalse();
        }

        [TestMethod]
        public void ReadEntitiesHandlesIntAsDecimal()
        {
            var tableHelper = new ValueTableHelper<decimal>();
            decimal value1 = 1;
            decimal value2 = 2;

            tableHelper.AddRow(value1);
            tableHelper.AddRow(value2);

            var items = tableHelper.GetReader().ReadEntities<int>();
            items.Count.Should().Be(2);
            items[0].Should().Be(1);
            items[1].Should().Be(2);
        }

        [TestMethod]
        public void ReadEntitiesUsesMapping()
        {
            var config = new Mock<IEntityConfiguration>();
            config.Setup<string>(c => c.PropertyMapping(It.IsAny<string>())).Returns<string>(s => s);

            var model = new Mock<IDataModel>();
            model.Setup(h => h.GetEntityConfig(It.IsAny<Type>())).Returns(config.Object).Verifiable();

            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            var items = tableHelper.GetReader().ReadEntities<Customer>(dataModel: model.Object);
            model.Verify(h => h.GetEntityConfig(It.IsAny<Type>()), Times.AtLeastOnce());
        }

        [TestMethod]
        public void ReadEntitiesIgnoresUnmatchedColumns()
        {
            var config = new Mock<IEntityConfiguration>();

            // introduce a bad column name for all properties:
            config.Setup<string>(c => c.PropertyMapping(It.IsAny<string>())).Returns("unmatched");

            var model = new Mock<IDataModel>();
            model.Setup(h => h.GetEntityConfig(It.IsAny<Type>())).Returns(config.Object).Verifiable();

            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            var item = tableHelper.GetReader().ReadEntities<Customer>(dataModel: model.Object).First();
            item.CustomerId.Should().Be(default(int));
            item.Inception.Should().Be(default(DateTime));
            item.Change.Should().Be(default(byte));
            item.Modified.Should().Be(default(DateTimeOffset));
        }

        [TestMethod]
        public void ReadEntitiesThrowsInvalidCastExceptionWhenNullCannotBeMapped()
        {
            var config = new Mock<IEntityConfiguration>();

            var model = new Mock<IDataModel>();
            model.Setup<IEntityConfiguration>(h => h.GetEntityConfig(It.IsAny<Type>())).Returns(config.Object).Verifiable();

            var tableHelper = new EntityTableHelper<Customer>(true);
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new object[] { 2, "2", null, 2, DateTimeOffset.UtcNow });
            
            Action test = ()=> tableHelper.GetReader().ReadEntities<Customer>();
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void ReadEntitiesInvokesHandler()
        {
            var handler = new Mock<IDataHandler<Customer>>();
            handler.Setup(h => h.ReadEntity(It.IsAny<IDataReader>())).Verifiable();

            var tableHelper = new EntityTableHelper<Customer>();
            tableHelper.AddRow(new Customer { CustomerId = 1, CustomerKey = "1", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });
            tableHelper.AddRow(new Customer { CustomerId = 2, CustomerKey = "2", Inception = DateTime.UtcNow, Change = 2, Modified = DateTimeOffset.UtcNow });

            tableHelper.GetReader().ReadEntities(handler.Object);
            handler.Verify<Customer>(h => h.ReadEntity(It.IsAny<IDataReader>()), Times.Exactly(2));
        }

        public IDataReader CreateReaderFromTableWithValue<TValue>(object value)
        {
            return CreateReaderFromTableWithValue<TValue>(value, "Target");
        }

        public IDataReader CreateReaderFromTableWithNullValue<TValue>()
        {
            return CreateReaderFromTableWithValue<TValue>(DBNull.Value, "Target");
        }

        public IDataReader CreateReaderFromTableWithValue<TValue>(object value, string columnName)
        {
            var table = new DataTable();
            var column = new DataColumn(columnName, typeof(TValue));

            // DataColumn will default the name if null or empty, defeating some of the test scenarios:
            column.ColumnName = columnName;

            table.Columns.Add(new DataColumn(columnName, typeof(TValue)));
            var row = table.NewRow()[0] = value;
            table.Rows.Add(row);
            return table.CreateDataReader();
        }

        public class EntityWithNullableProperty<TValue> where TValue : struct
        {
            public TValue? Target { get; set; }
        }

        public class EntityWithProperty<TValue>
        {
            public TValue Target { get; set; }
        }
    }
}
