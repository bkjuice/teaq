using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace Teaq.Tests
{
    [TestClass]
    public class EntityModelBuilderTests
    {
        [TestMethod]
        public void DataConfigurationReturnsNullForUnknownType()
        {
            var model = Repository.BuildModel(
                modelBuilder =>
                {
                    modelBuilder.Entity<Customer>();
                });

            model.GetEntityConfig(typeof(IAbstractEntity)).Should().BeNull();
        }

        [TestMethod]
        public void DataConfigurationDefaultContextIsAsExpected()
        {
            var context = Repository.BuildContext("test");
            (context as DataContext).Should().NotBeNull();

            var context2 = Repository.BuildContext("test");
            context.Should().NotBeSameAs(context2);
        }

        [TestMethod]
        public void DataConfigurationDefaultContextIsAsExpectedWithConnectionBuilder()
        {
            var context = Repository.BuildContext("test", new SqlConnectionBuilder());
            (context as DataContext).Should().NotBeNull();

            var context2 = Repository.BuildContext("test", new SqlConnectionBuilder());
            context.Should().NotBeSameAs(context2);
        }

        [TestMethod]
        public void DataConfigurationThrowsNotSupportedExceptionForUnsupportedPropertyType()
        {
            Repository.BuildModel(
                builder =>
                {
                    Action test = () => builder.Entity<SomeType1>().Column(e => e.FailProperty).HasMapping("FAIL");
                    test.ShouldThrow<NotSupportedException>();
                });
        }

        [TestMethod]
        public void MappedPropertyDataTypeIsNVarCharWhenSpecifiedAsNVarChar()
        {
            var model = Repository.BuildModel(
                builder => 
                {
                    builder.Entity<SomeType1>()
                    .Column(e => e.MappedProperty)
                    .HasMapping("Column1")
                    .IsNVarChar(50);
                });

            model.GetEntityConfig(typeof(SomeType1)).ColumnDataType("MappedProperty").SqlDataType.Should().Be(SqlDbType.NVarChar);
        }

        [TestMethod]
        public void MappedPropertyDataTypeIsVarCharWhenSpecifiedAsVarChar()
        {
            var model = Repository.BuildModel(
                builder =>
                {
                    builder.Entity<SomeType1>()
                    .Column(e => e.MappedProperty)
                    .HasMapping("Column1")
                    .IsVarChar(50);
                });

            model.GetEntityConfig(typeof(SomeType1)).ColumnDataType("MappedProperty").SqlDataType.Should().Be(SqlDbType.VarChar);
        }

        [TestMethod]
        public void MappedPropertyCreatesForwardAndReverseMappings()
        {
            var model = Repository.BuildModel(
                builder =>
                {
                    builder.Entity<SomeType1>().Column(e => e.MappedProperty).HasMapping("Mapped");
                });

            model.GetEntityConfig(typeof(SomeType1)).Should().NotBeNull();
            model.GetEntityConfig(typeof(SomeType1)).ColumnMapping("MappedProperty").Should().Be("[Mapped]");
            model.GetEntityConfig(typeof(SomeType1)).PropertyMapping("Mapped").Should().Be("MappedProperty");
        }

        [TestMethod]
        public void DataConfigurationCreatesConcurrencyProperty()
        {
            var model = Repository.BuildModel(
                builder =>
                {
                    builder.Entity<SomeType2>()
                    .ConcurrencyToken(e => e.ConcurrencyToken)
                    .Column(e => e.Id).IsIdentity().IsKey().IsOfType(SqlDbType.Int);
                });

            model.GetEntityConfig(typeof(SomeType2)).Should().NotBeNull();
            model.GetEntityConfig(typeof(SomeType2)).ConcurrencyProperty.Should().Be("ConcurrencyToken");
        }

        [TestMethod]
        public void DataConfigurationCreatesConcurrencyPropertyWithMapping()
        {
            var model = Repository.BuildModel(
                builder =>
                {
                    builder.Entity<SomeType2>().ConcurrencyToken(e => e.ConcurrencyToken).HasMapping("Mapped");
                });

            model.GetEntityConfig(typeof(SomeType2)).Should().NotBeNull();
            model.GetEntityConfig(typeof(SomeType2)).ConcurrencyProperty.Should().Be("ConcurrencyToken");
            model.GetEntityConfig(typeof(SomeType2)).ColumnMapping("ConcurrencyToken").Should().Be("[Mapped]");
        }

        [TestMethod]
        public void UnmappedPropertyUsesPropertyNameAndColumnName()
        {
            var model = Repository.BuildModel(
                builder =>
                {
                    builder.Entity<SomeType1>().Column(e => e.MappedProperty).HasMapping("Mapped");
                });

            model.GetEntityConfig(typeof(SomeType1)).Should().NotBeNull();
            model.GetEntityConfig(typeof(SomeType1)).ColumnMapping("UnmappedProperty").Should().Be("UnmappedProperty");
            model.GetEntityConfig(typeof(SomeType1)).PropertyMapping("Unmapped").Should().Be("Unmapped");
        }

        [TestMethod]
        public void DataConfigurationCorrectlyConfiguresIdentity()
        {
            var model = Repository.BuildModel(
                builder =>
                {
                    builder.Entity<SomeType1>().Column(e => e.MappedProperty).IsIdentity();
                });

            model.GetEntityConfig(typeof(SomeType1)).HasIdentity.Should().BeTrue();
            model.GetEntityConfig(typeof(SomeType1)).IsKeyColumn("MappedProperty").Should().BeTrue();
            model.GetEntityConfig(typeof(SomeType1)).IsComputed("MappedProperty").Should().BeTrue();
        }

        private class SomeType1
        {
            public SomeType2 FailProperty { get; set; }

            public string MappedProperty { get; set; }

            public string UnmappedProperty { get; set; }
        }

        private class SomeType2
        {
            public byte[] ConcurrencyToken { get; set; }

            public int Id { get; set; }
        }
    }
}
