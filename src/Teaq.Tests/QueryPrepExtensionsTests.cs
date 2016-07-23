using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.QueryGeneration;

namespace Teaq.Tests
{
    [TestClass]
    public class QueryPrepExtensionsTests
    {
        [TestMethod]
        public void ExtractSelectColumnListUsesExplicitColumns()
        {
            QueryPrepExtensions.ExtractSelectColumnList(typeof(Customer), null, new string[] { "Id" })
                .Should()
                .NotContain("CustomerKey");
        }

        [TestMethod]
        public void ExtractSelectColumnListIngoresNullExplicitColumns()
        {
            QueryPrepExtensions.ExtractSelectColumnList(typeof(Customer), null, null)
                .Should()
                .Contain("CustomerKey");
        }

        [TestMethod]
        public void ExtractSelectColumnListIngoresEmptyExplicitColumns()
        {
            QueryPrepExtensions.ExtractSelectColumnList(typeof(Customer), null, new string[] { })
                .Should()
                .Contain("CustomerKey");
        }

        [TestMethod]
        public void AsQualifiedColumnReturnsColumnNameWithTypeQualifier()
        {
            "MyColumn".AsQualifiedColumn(null, null, typeof(Customer)).Should().Be("[Customer].[MyColumn]");
        }

        [TestMethod]
        public void AsQualifiedColumnUsesTableAliasAsExpected()
        {
            "MyColumn".AsQualifiedColumn("X", null, typeof(Customer)).Should().Be("[X].[MyColumn]");
        }

        [TestMethod]
        public void AsQualifiedColumnUsesTableMappingAsExpected()
        {
            var model = Repository.BuildModel(builder =>
                {
                    builder.Entity<Customer>("dbo", "Customer2");
                });

            "MyColumn".AsQualifiedColumn(null, model.GetEntityConfig(typeof(Customer)), typeof(Customer)).Should().Be("[Customer2].[MyColumn]");
        }

        [TestMethod]
        public void AsQualifiedColumnPrefersTableAliasOverMappingAsExpected()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>("dbo", "Customer2");
            });

            "MyColumn".AsQualifiedColumn("X", model.GetEntityConfig(typeof(Customer)), typeof(Customer)).Should().Be("[X].[MyColumn]");
        }

        [TestMethod]
        public void AsQualifiedTableDefaultsToDbo()
        {
            typeof(Customer).AsQualifiedTable(null).Should().Be("[dbo].[Customer]");
        }

        [TestMethod]
        public void AsQualifiedTableUsesModelSchema()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>("dbo2");
            });

            typeof(Customer).AsQualifiedTable(model.GetEntityConfig(typeof(Customer))).Should().Be("[dbo2].[Customer]");
        }

        [TestMethod]
        public void AsQualifiedTableUsesModelSchemaAndTableName()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>("dbo2", "Customer2");
            });

            typeof(Customer).AsQualifiedTable(model.GetEntityConfig(typeof(Customer))).Should().Be("[dbo2].[Customer2]");
        }

        [TestMethod]
        public void AsUnqualifiedTableUsesModelTableName()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>("dbo2", "Customer2");
            });

            typeof(Customer).AsUnqualifiedTable(model.GetEntityConfig(typeof(Customer))).Should().Be("[Customer2]");
        }

        [TestMethod]
        public void AsUnqualifiedTableUsesTypeNameWhenTableNameNullOrEmpty()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>("dbo2", string.Empty);
            });

            typeof(Customer).AsUnqualifiedTable(model.GetEntityConfig(typeof(Customer))).Should().Be("[Customer]");
        }

        [TestMethod]
        public void AsUnqualifiedTableUsesTypeNameWhenTableNameNotProvided()
        {
            var model = Repository.BuildModel(builder =>
            {
                builder.Entity<Customer>("dbo2");
            });

            typeof(Customer).AsUnqualifiedTable(model.GetEntityConfig(typeof(Customer))).Should().Be("[Customer]");
        }

        [TestMethod]
        public void AsUnqualifiedTableUsesTypeNameWhenEntityMappingNotProvided()
        {
            var model = Repository.BuildModel(builder =>
            {
            });

            typeof(Customer).AsUnqualifiedTable(model.GetEntityConfig(typeof(Customer))).Should().Be("[Customer]");
        }
    }
}
