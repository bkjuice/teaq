using System;
using System.Data;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Teaq.QueryGeneration;

namespace Teaq.Tests
{
    [TestClass]
    public class QueryBuilderTests
    {
        [TestMethod]
        public void SelectQueryParameterWithColumnMappingDoesNotAddBracketsToParameterName()
        {
            var model = Repository.BuildModel(
                builder =>
                {
                    builder.Entity<Customer>()
                    .Column(e => e.CustomerId).HasMapping("ID");
                });


            var q = model.ForEntity<Customer>().BuildSelect().Where(e => e.CustomerId == 1).ToCommand();
            q.GetParameters()[0].ParameterName.Should().NotContain("[");
            q.GetParameters()[0].ParameterName.Should().NotContain("]");
        }

        [TestMethod]
        public void UpdateQueryParameterWithColumnMappingDoesNotAddBracketsToParameterName()
        {
            var model = Repository.BuildModel(m =>
            {
                m.Entity<UserTenancy>("dbo", "UsersToTenancies")
                    .Column(e => e.UserName).IsNVarChar(256)
                    .Column(e => e.TenancyKey).HasMapping("Key");
            });

            var modified = new UserTenancy { UserName = "modified", TenancyKey = "123" };
            var userTenancy = new UserTenancy { UserName = "original", TenancyKey = "123" };

            var command = model.ForEntity<UserTenancy>()
               .BuildUpdate(modified, e => e.UserName == userTenancy.UserName)
               .ToCommand();

            
            command.GetParameters()[1].ParameterName.Should().NotContain("[");
            command.GetParameters()[1].ParameterName.Should().NotContain("]");
        }

        [TestMethod]
        public void InsertQueryParameterWithColumnMappingDoesNotAddBracketsToParameterName()
        {
            var model = Repository.BuildModel(m =>
            {
                m.Entity<UserTenancy>("dbo", "UsersToTenancies")
                    .Column(e => e.UserName).IsNVarChar(256)
                    .Column(e => e.TenancyKey).HasMapping("Key");
            });

            var modified = new UserTenancy { UserName = "modified", TenancyKey = "123" };
            var userTenancy = new UserTenancy { UserName = "original", TenancyKey = "123" };

            var command = model.ForEntity<UserTenancy>()
               .BuildInsert(modified)
               .ToCommand();

            command.GetParameters()[1].ParameterName.Should().NotContain("[");
            command.GetParameters()[1].ParameterName.Should().NotContain("]");
        }


        [TestMethod]
        public void QueryBuilderStringsDefaultToNVarcharWhenSetGlobally()
        {
            Repository.SetDefaultStringType(Repository.StringDataType.NVarchar);

            var command = Repository.Default.ForEntity<Customer>().BuildSelect()
                .Where(c => c.CustomerKey == "XYZ").ToCommand();

            command.GetParameters()[0].SqlDbType.Should().Be(SqlDbType.NVarChar);

            Repository.SetDefaultStringType(Repository.StringDataType.Varchar);
        }


        [TestMethod]
        public void QueryBuilderHandlesNullNotEqualExpression()
        {
            var command = Repository.Default.ForEntity<Customer>().BuildSelect().Where(c => c.CustomerId == 1 && c.CustomerKey != null)
               .ToCommand();

            command.CommandText.Should().Contain("Is Not NULL");
        }

        [TestMethod]
        public void QueryBuilderHandlesNullEqualExpression()
        {
            var command = Repository.Default.ForEntity<Customer>().BuildSelect().Where(c => c.CustomerId == 1 && c.CustomerKey == null)
               .ToCommand();

            command.CommandText.Should().Contain("Is NULL");
        }

        [TestMethod]
        public void QueryFactoryProducesValidQueryBuilder()
        {
            var stub = Repository.BuildModel(x => { });
            stub.ForEntity<Customer>().Should().NotBeNull();
        }

        [TestMethod]
        public void QueryFactoryPassesModelToQueryBuilder()
        {
            var model = Repository.BuildModel(builder => { });
            var queryBuilder = new QueryBuilderStub<Customer>(model); 
            queryBuilder.VerifiableDataModel.Should().NotBeNull();
            queryBuilder.VerifiableDataModel.Should().BeSameAs(model);
        }

        [TestMethod]
        public void QueryFactoryProducesValidQueryForAbstractEntity()
        {
            var model = Repository.BuildModel(builder => { });
            Action test = () => model.ForEntity<IAbstractEntity>();
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void QueryFactoryProducesValidQueryWithWhereClauseForAbstractEntity()
        {
            var model = Repository.BuildModel(
                builder => 
                {
                    builder.Entity<ConcreteEntity>(tableName: "ConcreteEntity");
                    builder.MapAbstractType<IAbstractEntity, ConcreteEntity>();
                });

            var command = model.BuildSelectCommand<IAbstractEntity>(a => a.Id == 6);
            command.CommandText.Should().Contain("where [ConcreteEntity].[Id]");
        }

        [TestMethod]
        public void ContextSqlQueryCanMapResultToAbstractEntity()
        {
            var model = Repository.BuildModel(
                builder =>
                {
                    builder.Entity<ConcreteEntity>(tableName: "ConcreteEntity");
                    builder.MapAbstractType<IAbstractEntity, ConcreteEntity>();
                });

            var connectionStub = new DbConnectionStub();

            var tableHelper = new EntityTableHelper<ConcreteEntity>();
            tableHelper.AddRow(new ConcreteEntity { Id = 1, Data = "1" });
            tableHelper.AddRow(new ConcreteEntity { Id = 2, Data = "2" });
            tableHelper.AddRow(new ConcreteEntity { Id = 3, Data = "3" });

            var commandStub = new DbCommandStub();
            commandStub.DataHelper = tableHelper;
            connectionStub.MockCommand = commandStub;

            var connectionBuilder = new Mock<IConnectionBuilder>();
            connectionBuilder.Setup(b => b.Create(It.IsAny<string>())).Returns(connectionStub);

            var selectCommand = model.BuildSelectCommand<IAbstractEntity>(c => c.Id == 10);
            var context = Repository.BuildContext("test", connectionBuilder.Object);
            Action test = () => context.Query<IAbstractEntity>(selectCommand);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void ColumnInScopeReturnsTrueForDelete()
        {
            QueryBuilderStub<Customer>.VerifiableColumnInScope("anything", typeof(string), QueryType.Delete, null).Should().BeTrue();
        }

        [TestMethod]
        public void ColumnInScopeIsFalseForUnsupportedType()
        {
            QueryBuilderStub<Customer>.VerifiableColumnInScope("test", typeof(Address), QueryType.Insert, null).Should().BeFalse();
        }

        [TestMethod]
        public void ColumnInScopeIsFalseForComputedColumn()
        {
            var model = Repository.BuildModel(builder =>
                {
                    builder.Entity<Customer>().Column(c => c.CustomerId).IsIdentity();
                });

            QueryBuilderStub<Customer>.VerifiableColumnInScope("CustomerId", typeof(int), QueryType.Insert, model.GetEntityConfig(typeof(Customer))).Should().BeFalse();
        }

        [TestMethod]
        public void FormatColumnsWithEmptyColumnListReturnsEmpty()
        {
            QueryBuilderStub<Customer>.VerifiableFormatColumns(null, typeof(Customer), null, new string[] { }).Should().BeEmpty();
        }

        [TestMethod]
        public void BuildInsertQueryWithIdentityResultsInSelectScopeIdentityPostfix()
        {
            var model = Repository.BuildModel(
              modelBuilder =>
              {
                  modelBuilder.Entity<Customer>("someSchema", "CustomerTable").Column(c => c.CustomerId).IsIdentity();
              });

            var builder = new QueryBuilder<Customer>(model);
            var client = new Customer { CustomerId = 5 };
            var command = builder.BuildInsert(client).ToCommand();
            command.CommandText.Should().Contain("select SCOPE_IDENTITY()");
        }

        [TestMethod]
        public void BuildSelectAsWithAliasAndTopProducesExpectedStatement()
        {
            var builder = new QueryBuilder<Customer>();
            var command = builder.BuildSelectAs("A", 5).ToCommand();
            command.CommandText.Should().Contain("TOP 5");
        }

        [TestMethod]
        public void BuildSelectAsWithOptionProducesExpectedStatement()
        {
            var builder = new QueryBuilder<Customer>();
            var command = builder.BuildSelectAs("A", 5)
                .WithOption("OPTION RECOMPILE")
                .ToCommand();
            command.CommandText.Should().EndWith("OPTION RECOMPILE");
        }

        [TestMethod]
        public void BuildSelectAsWithOptionNormalizesClauseAsExpected()
        {
            var builder = new QueryBuilder<Customer>();
            var command = builder.BuildSelectAs("A", 5)
                .WithOption("RECOMPILE")
                .ToCommand();
            command.CommandText.Should().EndWith("OPTION RECOMPILE");
        }

        [TestMethod]
        public void BuildSelectAsWithOptionNormalizesClausePreservingCase()
        {
            var builder = new QueryBuilder<Customer>();
            var command = builder.BuildSelectAs("A", 5)
                .WithOption("option recompile")
                .ToCommand();
            command.CommandText.Should().EndWith("option recompile");
        }

        [TestMethod]
        public void BuildSelectAsWithOptionNormalizesClauseAsCaseInsensitive()
        {
            var builder = new QueryBuilder<Customer>();
            var command = builder.BuildSelectAs("A", 5)
                .WithOption("recompile")
                .ToCommand();
            command.CommandText.Should().EndWith("OPTION recompile");
        }

        [TestMethod]
        public void BuildSelectWithGroupingClauseResultsInExpectedQuery()
        {
            var builder = new QueryBuilder<Address>();
            var command = builder.BuildSelect("AddressId", "CustomerId").GroupBy(a => a.AddressId, a => a.CustomerId)
                .Where(a => a.CustomerId == 50).OrderBy(items => items.OrderByDescending(a => a.AddressId)).ToCommand();

            command.CommandText.Should().NotBeNullOrEmpty();
            command.CommandText.Should().Contain("group by [Address].[AddressId], [Address].[CustomerId]\r\n");
        }

        [TestMethod]
        public void DeleteWithoutWhereClauseIsAsExpected()
        {
            new QueryBuilder<Customer>().BuildDelete().ToCommand().CommandText.Should().NotContain("where");
        }

        [TestMethod]
        public void UpdateWothoutWhereClauseIsAsExpected()
        {
            var client = new Customer { CustomerKey = "ALL" };
            new QueryBuilder<Customer>().BuildUpdate(client).ToCommand().CommandText.Should().NotContain("where");
        }

        [TestMethod]
        public void BuildQueryWithDataModelResultsInExpectedState()
        {
            var model = Repository.BuildModel(modelBuilder => { });
            var builder = new QueryBuilderStub<Customer>(model);
            builder.VerifiableDataModel.Should().NotBeNull();
            builder.VerifiableDataModel.Should().BeSameAs(model);
        }

        [TestMethod]
        public void BuildQueryWithDataModelUsingAbstractEntityResultsInExpectedState()
        {
            var model = Repository.BuildModel(
                modelBuilder => 
                {
                    modelBuilder.Entity<ConcreteEntity>(tableName: "Entity");
                    modelBuilder.MapAbstractType<IAbstractEntity, ConcreteEntity>();
                });

            var builder = new QueryBuilder<IAbstractEntity>(model);
            var queryCommand = builder
                .BuildSelect(top: 5)
                .Where(c => c.Id == 5).ToCommand();

            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(1);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("select TOP 5 [Entity].[Id]");
        }

        [TestMethod]
        public void BuildQueryWithTopProducesExpectedResult()
        {
            var clientKey = default(string);
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder
                .BuildSelect(top: 5, columns: "CustomerKey")
                .Where(c => c.CustomerKey == clientKey).ToCommand();

            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(0);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("select TOP 5 [Customer].[CustomerKey]\r\n from");
        }

        [TestMethod]
        public void BuildQueryWithExplicitTableNameProducesExpectedResult()
        {
            var model = Repository.BuildModel(
                modelBuilder =>
                {
                    modelBuilder.Entity<Customer>("someSchema", "CustomerTable");
                });

            var builder = new QueryBuilder<Customer>(model);
            var queryCommand = builder.BuildSelect().Where(c => c.CustomerId == 3).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(1);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("from [someSchema].[CustomerTable]");
        }

        [TestMethod]
        public void BuildQueryWithExplicitDataTypeProducesExpectedResult()
        {
            var model = Repository.BuildModel(
                modelBuilder =>
                {
                    modelBuilder
                        .Entity<Customer>().Column(c => c.CustomerId).IsKey()
                        .Column(c => c.CustomerKey).IsOfType(SqlDbType.Char, 20);
                });

            var clientKey = "XXXX5";
            var builder = new QueryBuilder<Customer>(model);
            var queryCommand = builder.BuildSelect().Where(c => c.CustomerKey == clientKey).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(1);
            queryCommand.GetParameters()[0].SqlDbType.Should().Be(SqlDbType.Char);
            queryCommand.GetParameters()[0].Size.Should().Be(20);
        }

        [TestMethod]
        public void BuildQueryWithOrderByResultsInExpectedClauseAndParameters()
        {
            var clientId = 5;
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildSelect().Where(c => c.CustomerId == clientId).OrderBy(items => items.OrderBy(c => c.CustomerId)).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(1);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("select ");
            queryCommand.CommandText.Should().Contain("where ");
            queryCommand.CommandText.Should().Contain("[Customer].[CustomerId] = @p");
            queryCommand.CommandText.Should().Contain("\r\n order by [Customer].[CustomerId]");
        }

        [TestMethod]
        public void BuildQueryResultsInExpectedClauseAndParameters()
        {
            var clientId = 5;
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildSelect().Where(c => c.CustomerId == clientId).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(1);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("select ");
            queryCommand.CommandText.Should().Contain("where ");
            queryCommand.CommandText.Should().Contain("[Customer].[CustomerId] = @p");
        }

        [TestMethod]
        public void BuildQueryWithSelectListAndGlobalParameter()
        {
            var clientId = 5;
            var batch = new QueryBatch();
            batch.AddEmbeddedParameter("CustomerId", "@clientId");
            batch.AddEmbeddedParameter("CustomerId2", "@clientId2");
            batch.AddEmbeddedParameter("CustomerId3", "@clientId3");
            
            //var builder = new QueryBuilder<Customer>(batch);
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildSelect(columns: "CustomerKey").Where(c => c.CustomerId == clientId)
                .AddToBatch(batch);

            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(0);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("select [Customer].[CustomerKey]\r\n from");
            queryCommand.CommandText.Should().Contain("where ");
            queryCommand.CommandText.Should().Contain("[Customer].[CustomerId] = @clientId");
        }

        [TestMethod]
        public void BuildQueryWithSelectListAndNull()
        {
            var clientKey = default(string);
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildSelect(columns: "CustomerKey").Where(c => c.CustomerKey == clientKey).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(0);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("select [Customer].[CustomerKey]\r\n from");
            queryCommand.CommandText.Should().Contain("where ");
            queryCommand.CommandText.Should().Contain("[Customer].[CustomerKey] Is NULL");
        }

        [TestMethod]
        public void BuildQueryWithSelectListAndNonMatchingGlobalParameter()
        {
            var clientId = 5;
            var batch = new QueryBatch();
            batch.AddEmbeddedParameter("CustomerId2", "@clientId2");
            batch.AddEmbeddedParameter("CustomerId3", "@clientId3");

            //var builder = new QueryBuilder<Customer>(batch);
            var builder = new QueryBuilder<Customer>();

            var queryCommand = builder.BuildSelect("CustomerKey").Where(c => c.CustomerId == clientId).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(1);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("select [Customer].[CustomerKey]\r\n from");
            queryCommand.CommandText.Should().Contain("where ");
            queryCommand.CommandText.Should().Contain("[Customer].[CustomerId] = @p");
        }

        [TestMethod]
        public void BuildQueryWithSelectListResultsInExpectedClauseAndParameters()
        {
            var clientId = 5;
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildSelect(columns:"CustomerKey").Where( c => c.CustomerId == clientId).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(1);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("select [Customer].[CustomerKey]\r\n from");
            queryCommand.CommandText.Should().Contain("where ");
            queryCommand.CommandText.Should().Contain("[Customer].[CustomerId] = @p");
        }

        [TestMethod]
        public void BuildQueryWithSelectListAndTableAliasResultsInExpectedClauseAndParameters()
        {
            var clientId = 5;
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder
                .BuildSelectAs("A", "CustomerKey" )
                .WithNoLock()
                .Where(c => c.CustomerId == clientId)
                .ToCommand();

            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(1);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("select [A].[CustomerKey]\r\n from [dbo].[Customer] [A] (nolock)");
            queryCommand.CommandText.Should().Contain("where ");
            queryCommand.CommandText.Should().Contain("[A].[CustomerId] = @p");
        }

        [TestMethod]
        public void BuildQueryWithMultipleSelectListAndTableAliasResultsInExpectedClauseAndParameters()
        {
            var clientId = 5;
            var builder = new QueryBuilder<Customer>();

            var queryCommand = builder
                .BuildSelectAs("A", "CustomerKey", "Inception")
                .WithNoLock()
                .Where(c => c.CustomerId == clientId)
                .ToCommand();

            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(1);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("select [A].[CustomerKey], [A].[Inception]\r\n from [dbo].[Customer] [A] (nolock)");
            queryCommand.CommandText.Should().Contain("where ");
            queryCommand.CommandText.Should().Contain("[A].[CustomerId] = @p");
        }

        [TestMethod]
        public void BuildDeleteResultsInExpectedClauseAndParameters()
        {
            var CustomerId = 5;
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildDelete(c => c.CustomerId == CustomerId).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(1);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("delete ");
            queryCommand.CommandText.Should().Contain("where ");
            queryCommand.CommandText.Should().Contain("[Customer].[CustomerId] = @p");
        }

        [TestMethod]
        public void BuildInsertResultsInExpectedClauseAndParameters()
        {
            var Customer = new Customer { CustomerId = 5 };
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildInsert(Customer).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(5);
            queryCommand.GetParameters()[0].Value.Should().Be(5);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("insert ");
            queryCommand.CommandText.Should().Contain("[CustomerId]");
        }

        [TestMethod]
        public void BuildInsertResultsInExpectedClauseAndParametersWithBatch()
        {
            var batch = new QueryBatch();
            var Customer = new Customer { CustomerId = 5 };
            //var builder = new QueryBuilder<Customer>(batch);
            var builder = new QueryBuilder<Customer>();
            var queryCommand = builder.BuildInsert(Customer).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(5);
            queryCommand.GetParameters()[0].Value.Should().Be(5);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("insert ");
            queryCommand.CommandText.Should().Contain("[CustomerId]");
        }

        [TestMethod]
        public void BuildUpdateResultsInExpectedClauseAndParameters()
        {
            var model = Repository.BuildModel(
                config =>
                {
                    config.Entity<Customer>()
                        .Column(c => c.CustomerId).IsKey()
                        .Exclude(c => c.CustomerKey);
                });

            // Get count of properties less the suppressed properties:
            var expectedParamsCount = typeof(Customer).GetProperties().Count() - 2;

            //Add 1 for the filter:
            expectedParamsCount++;

            var client = new Customer { CustomerId = 5 };
            var builder = new QueryBuilder<Customer>(model);
            var queryCommand = builder.BuildUpdate(client, c => c.CustomerId == client.CustomerId).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(expectedParamsCount);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("update ");
            queryCommand.CommandText.Should().Contain("where ");
            queryCommand.CommandText.Should().Contain("[CustomerId] = @");
            queryCommand.CommandText.Should().NotContain("CustomerId,");
        }

        [TestMethod]
        public void BuildUpdateResultsInExpectedClauseAndParametersWithBatch()
        {
            var model = Repository.BuildModel( 
                config =>
                    {
                        config.Entity<Customer>()
                            .Column(c => c.CustomerId).IsKey()
                            .Column(c => c.CustomerKey).IsKey();
                    });

            // Get count of properties less the suppressed properties:
            var expectedParamsCount = typeof(Customer).GetProperties().Count() - 2;

            //Add 1 for the filter:
            expectedParamsCount++;

            var batch = new QueryBatch();
            var client = new Customer { CustomerId = 5 };
            //var builder = new QueryBuilder<Customer>(model, batch);
            var builder = new QueryBuilder<Customer>(model);
            var queryCommand = builder.BuildUpdate(client, c => c.CustomerId == client.CustomerId).ToCommand();
            queryCommand.GetParameters().Should().NotBeNull();
            queryCommand.GetParameters().GetLength(0).Should().Be(expectedParamsCount);
            queryCommand.CommandText.Should().NotBeNullOrEmpty();
            queryCommand.CommandText.Should().Contain("update ");
            queryCommand.CommandText.Should().Contain("[Inception]");
            queryCommand.CommandText.Should().Contain("[Modified]");
            queryCommand.CommandText.Should().Contain("[Change]");
            queryCommand.CommandText.Should().Contain("where ");
            queryCommand.CommandText.Should().Contain("[CustomerId] = @");
            queryCommand.CommandText.Should().NotContain("CustomerId,");
        }

        [TestMethod]
        public void InnerJoinResultsInExpectedStatement()
        {
            var model = Repository.BuildModel(
                config =>
                {
                    config.Entity<Customer>()
                        .Column(c => c.CustomerId).IsKey()
                        .Column(c => c.CustomerKey).IsKey();

                    config.Entity<Address>()
                        .Column(a => a.CustomerId).IsKey();
                });

            var builder = new QueryBuilder<Customer>(model);
            var command = builder
                .BuildSelect()
                .Join<Address>(JoinType.Inner, (c, a) => a.CustomerId == c.CustomerId)
                .Where(c => c.CustomerId == 5)
                .ToCommand();
                
            command.CommandText.Should().NotBeNullOrEmpty();
            command.CommandText.Should().Contain(", [Address].[CustomerId]");
            command.CommandText.Should().Contain("[dbo].[Customer]\r\nInner Join [dbo].[Address] on [Customer].[CustomerId] = [Address].[CustomerId]");
        }
    }
}
