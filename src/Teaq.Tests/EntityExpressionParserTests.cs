using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.QueryGeneration;

namespace Teaq.Tests
{
    [TestClass]
    public class EntityExpressionParserTests
    {
        [TestMethod]
        public void ParseJoinExpressionThrowsNotSupportedExceptionWhenLeftNotPropertyExpression()
        {
            var stub = new InvalidJoinExpressionStub();
            stub.OnExpression = (c, a) => true == stub.MethodExpression(c, a);
            Action test = () => stub.OnExpression.ParseJoinExpression();
            test.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void ParseJoinExpressionThrowsNotSupportedExceptionWhenLeftNull()
        {
            Expression<Func<Customer, Address, bool>> onExpression = (c, a) => null == c.CustomerKey;
            Action test = () => onExpression.ParseJoinExpression();
            test.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void ParseJoinExpressionThrowsNotSupportedExceptionWhenRightNotPropertyExpressionOrConstant()
        {
            Expression<Func<Customer, Address, bool>> onExpression = (c, a) => c.CustomerId == int.Parse("2");
            Action test = () => onExpression.ParseJoinExpression();
            test.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void ParseJoinExpressionUsesRightNullAsExpected()
        {
            Expression<Func<Customer, Address, bool>> onExpression = (c, a) => c.CustomerKey == null;
            var joinExpr = onExpression.ParseJoinExpression();
            joinExpr.Should().Contain("NULL");
        }

        [TestMethod]
        public void ParseJoinExpressionUsesRightConstantAsExpected()
        {
            Expression<Func<Customer, Address, bool>> onExpression = (c, a) => c.CustomerId == 5;
            var joinExpr = onExpression.ParseJoinExpression();
            joinExpr.Should().Contain("5");
        }

        [TestMethod]
        public void ParseJoinExpressionUsesEqualityAsExpected()
        {
            Expression<Func<Customer, Address, bool>> onExpression = (c, a) => c.CustomerId == a.CustomerId;
            var joinExpr = onExpression.ParseJoinExpression();
            joinExpr.Should().Contain("[Customer].[CustomerId] = [Address].[CustomerId]");
        }

        [TestMethod]
        public void ParseJoinExpressionThrowsNotSupportedExceptionWhenNotBinaryExpression()
        {
            var stub = new InvalidJoinExpressionStub();
            stub.OnExpression = (c, a) => stub.MethodExpression(c, a);
            Action test = () => stub.OnExpression.ParseJoinExpression();
            test.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void OrderByExpressionAsUnknownMethodThrowsNotSupportedException()
        {
            var stub = new InvalidOrderByExpressionStub();
            stub.OrderByExpression = items => stub.UnknownMethod(items);
            Action test = () => stub.OrderByExpression.ParseOrderByClause();
            test.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void OrderByExpressionAsKnownMethodWithWrongArgumentsThrowsNotSupportedException()
        {
            var stub = new InvalidOrderByExpressionStub();
            stub.OrderByExpression = items => stub.OrderBy(items);
            Action test = () => stub.OrderByExpression.ParseOrderByClause();
            test.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void OrderByExpressionProducesExpectedQueryClause()
        {
            Expression<Func<IEnumerable<Customer>, IOrderedEnumerable<Customer>>> orderByExpression = items => items.OrderBy(c => c.CustomerId);
            var clause = orderByExpression.ParseOrderByClause();
            clause.Should().Be("order by [Customer].[CustomerId]");
        }

        [TestMethod]
        public void OrderByDescendingExpressionProducesExpectedQueryClause()
        {
            Expression<Func<IEnumerable<Customer>, IOrderedEnumerable<Customer>>> orderByExpression = items => items.OrderByDescending(c => c.CustomerId);
            var clause = orderByExpression.ParseOrderByClause();
            clause.Should().Be("order by [Customer].[CustomerId] desc");
        }

        [TestMethod]
        public void ExtractPropertyNameExtractsNameAsExpected()
        {
            Expression<Func<CustomerWithNullable, DateTimeOffset?>> complexExpression = c => c.Deleted;
            var property = complexExpression.ParsePropertyName();
            property.Should().Be("Deleted");
        }

        [TestMethod]
        public void ExtractPropertNameThrowsNotSupportedExceptionWithComplexProperty()
        {
            Expression<Func<CustomerWithNullable, DateTimeOffset>> complexExpression = c => c.Deleted.Value;
            Action test = () => complexExpression.ParsePropertyName();
            test.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void ExtractPropertyNameThrowsNotSupportedExceptionWithNonMemberExpressionLambda()
        {
            // Nonsensical, not the test subject:
            Expression<Func<Customer, bool>> binaryExpression = c => c.CustomerId > 2 && c.CustomerId < 2;
            Action test = () => binaryExpression.ParsePropertyName();
            test.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void NullableValueProducesExpectedResultWhenUsedWithContains()
        {
            var ids = new List<int> { 1, 2, 3 };
            Expression<Func<CustomerWithNullableId, bool>> expr = c => ids.Contains(c.RelatedCustomerId.Value);

            string queryClause;
            var parameters = expr.Parameterize("@p", null, out queryClause);

            queryClause.Should().NotBeNullOrEmpty();
            queryClause.Should().Be("[CustomerWithNullableId].[RelatedCustomerId] in (@p, @pn1, @pn2)");
            parameters.GetLength(0).Should().Be(3);
        }

        [TestMethod]
        public void NullableHasValueProducesExpectedResult()
        {
            var client = new CustomerWithNullable();
            Expression<Func<CustomerWithNullable, bool>> expr = c => c.Deleted.HasValue;

            string queryClause;
            var parameters = expr.Parameterize("@p", null, out queryClause);

            queryClause.Should().NotBeNullOrEmpty();
            queryClause.Should().Be("[CustomerWithNullable].[Deleted] Is Not NULL");
            parameters.GetLength(0).Should().Be(0);
        }

        [TestMethod]
        public void NullableNotHasValueProducesExpectedResult()
        {
            var client = new CustomerWithNullable();
            Expression<Func<CustomerWithNullable, bool>> expr = c => !c.Deleted.HasValue;

            string queryClause;
            var parameters = expr.Parameterize("@p", null, out queryClause);

            queryClause.Should().NotBeNullOrEmpty();
            queryClause.Should().Be("not ([CustomerWithNullable].[Deleted] Is Not NULL)");
            parameters.GetLength(0).Should().Be(0);
        }

        [TestMethod]
        public void NullableNotHasValueProducesExpectedResultWithCompoundGrouping()
        {
            var client = new CustomerWithNullable();
            Expression<Func<CustomerWithNullable, bool>> expr = c => !(c.CustomerId == 5 && c.Deleted.HasValue);

            string queryClause;
            var parameters = expr.Parameterize("@p", null, out queryClause);

            queryClause.Should().NotBeNullOrEmpty();
            queryClause.Should().Be("not ([CustomerWithNullable].[CustomerId] = @p and [CustomerWithNullable].[Deleted] Is Not NULL)");
            parameters.GetLength(0).Should().Be(1);
        }

        [TestMethod]
        public void NullableNotHasValueProducesExpectedResultWithNestedCompoundGrouping()
        {
            var client = new CustomerWithNullable();
            Expression<Func<CustomerWithNullable, bool>> expr = c => !(c.CustomerId == 5 && !(c.Deleted.HasValue || c.CustomerKey == null));

            string queryClause;
            var parameters = expr.Parameterize("@p", null, out queryClause);

            queryClause.Should().NotBeNullOrEmpty();
            queryClause.Should().Be("not ([CustomerWithNullable].[CustomerId] = @p and not ([CustomerWithNullable].[Deleted] Is Not NULL or [CustomerWithNullable].[CustomerKey] Is NULL))");
            parameters.GetLength(0).Should().Be(1);
        }

        [TestMethod]
        public void NullableNotHasValueProducesExpectedResultWithNestedNegatedCompoundGrouping()
        {
            var client = new CustomerWithNullable();
            Expression<Func<CustomerWithNullable, bool>> expr = c => !(c.CustomerId == 5 && !(!c.Deleted.HasValue || c.CustomerKey == null));

            string queryClause;
            var parameters = expr.Parameterize("@p", null, out queryClause);

            queryClause.Should().NotBeNullOrEmpty();
            queryClause.Should().Be("not ([CustomerWithNullable].[CustomerId] = @p and not (not ([CustomerWithNullable].[Deleted] Is Not NULL) or [CustomerWithNullable].[CustomerKey] Is NULL))");
            parameters.GetLength(0).Should().Be(1);
        }

        [TestMethod]
        public void NullableNotHasValueProducesExpectedResultWithMultipleNestedCompoundGrouping()
        {
            var client = new CustomerWithNullable();
            Expression<Func<CustomerWithNullable, bool>> expr = c => !((c.CustomerId == 5 || c.CustomerId == 4) && !(c.Deleted.HasValue || c.CustomerKey == null));

            string queryClause;
            var parameters = expr.Parameterize("@p", null, out queryClause);

            queryClause.Should().NotBeNullOrEmpty();
            queryClause.Should().Be("not (([CustomerWithNullable].[CustomerId] = @p or [CustomerWithNullable].[CustomerId] = @px1) and not ([CustomerWithNullable].[Deleted] Is Not NULL or [CustomerWithNullable].[CustomerKey] Is NULL))");
            parameters.GetLength(0).Should().Be(2);
        }

        [TestMethod]
        public void ParameterizeUsingStaticExtensionMethodThrowsNotSupportedException()
        {
            IEnumerable<int> items = new List<int>();
            Expression<Func<Customer, bool>> expression = c => items.Contains(c.CustomerId);
            string queryClause;
            Action test = () => expression.Parameterize("@p", null, out queryClause);
            test.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void ParameterizeUsingNonIListCollectionSucceeds()
        {
            var items = new NonIListCollection<int>(new List<int> { 1, 2 });
            Expression<Func<Customer, bool>> expression = c => items.Contains(c.CustomerId);
            string queryClause;
            var parameters = expression.Parameterize("@p", null, out queryClause);
            parameters.Should().NotBeNull();
            parameters.GetLength(0).Should().Be(2);
        }

        [TestMethod]
        public void ParameterizeUsingMethodOtherThanContainsThrowsNotSupportedException()
        {
            var items = new NonIListCollection<int>(new List<int> { 1, 2 });
            Expression<Func<Customer, bool>> expression = c => items.InvalidContains(c.CustomerId);
            string queryClause;
            Action test = () => expression.Parameterize("@p", null, out queryClause);
            test.ShouldThrow<NotSupportedException>();
        }

        [TestMethod]
        public void ParameterizeUsingNestedComplexPropertyDoesNotThrow()
        {
            var client = new CustomerWithNestedProperty();
            Expression<Func<CustomerWithNestedProperty, bool>> expression = c => c.Keys.CustomerId == 5;
            string queryClause;
            Action test = () => expression.Parameterize("@p", null, out queryClause);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void ParameterizeExpressionWithCompoundUnarySucceedsWithImplicitCast()
        {
            Expression<Func<Address, bool>> expr = e => e.CustomerId == 55 && e.Change == 1;
            
            string queryClause;
            Action test = () => expr.Parameterize("@test", null, out queryClause);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void ParameterizeExpressionWithCompoundUnarySucceedsWithExplicitCast()
        {
            Expression<Func<Address, bool>> expr = e => e.CustomerId == 55 && e.Change == (byte)1;

            string queryClause;
            Action test = () => expr.Parameterize("@test", null, out queryClause);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void ParameterizeExpressionTreeWithCompoundExpression()
        {
            var lastModifyDateTime = new DateTimeOffset(new DateTime(1999, 9, 9), TimeSpan.FromHours(-5));

            Expression<Func<Customer, bool>> e = c => c.Modified <= lastModifyDateTime && c.Inception <= DateTime.MaxValue;

            string queryClause;
            var parameters = e.Parameterize("compositeExpr", null, out queryClause);

            parameters.GetLength(0).Should().Be(2);
        }

        [TestMethod]
        public void ParameterizeExpressionTreeWithContainsAndTableAlias()
        {
            var items = new List<int> { 5, 6, 7, 8 };

            Expression<Func<Customer, bool>> e = c => items.Contains(c.CustomerId) && c.Inception <= DateTime.MaxValue;

            string queryClause;
            var parameters = e.Parameterize("@compositeExpr", null, out queryClause, columnQualifier: "Extent1");

            parameters.GetLength(0).Should().Be(5);
        }

        [TestMethod]
        public void ParameterizeCustomerIdEqualsExpressionWithConstant()
        {
            Expression<Func<Customer, bool>> e = c => c.CustomerId == 50;

            string queryClause;
            var parameters = e.Parameterize("@clientId", null, out queryClause);
            queryClause.Should().Be("[Customer].[CustomerId] = @clientId");

            parameters.GetLength(0).Should().Be(1);
            parameters[0].Value.Should().Be(50);
        }

        [TestMethod]
        public void ParameterizeCustomerIdEqualsExpression()
        {
            var id = 50;
            Expression<Func<Customer, bool>> e = c => c.CustomerId == id;

            string queryClause;
            var parameters = e.Parameterize("@clientId", null, out queryClause);
            queryClause.Should().Be("[Customer].[CustomerId] = @clientId");

            parameters.GetLength(0).Should().Be(1);
            parameters[0].Value.Should().Be(50);
        }

        [TestMethod]
        public void ParameterizeCustomerKeyEqualsExpression()
        {
            var key = "TestKey";
            Expression<Func<Customer, bool>> e = c => c.CustomerKey == key;

            string queryClause;
            var parameters = e.Parameterize("@clientKey", null, out queryClause);
            queryClause.Should().Be("[Customer].[CustomerKey] = @clientKey");

            parameters.GetLength(0).Should().Be(1);
            parameters[0].Value.Should().Be("TestKey");
        }

        [TestMethod]
        public void ParameterizeCustomerKeyEqualsExpressionUsingPredefinedLocalParameter()
        {
            var key = "TestKey";
            Expression<Func<Customer, bool>> e = c => c.CustomerKey == key;

            var locals = new SqlParameter[1];
            locals[0] = new SqlParameter("@clientKey", "TestKey");
            locals[0].SourceColumn = "CustomerKey";

            string queryClause;
            var parameters = e.Parameterize("@p", null, out queryClause, locals: locals);
            queryClause.Should().Be("[Customer].[CustomerKey] = @clientKey");
            parameters.GetLength(0).Should().Be(0);
        }

        [TestMethod]
        public void ParameterizeCustomerKeyEqualsExpressionUsingMultiplePredefinedLocalParameters()
        {
            var key = "TestKey";
            Expression<Func<Customer, bool>> e = c => c.CustomerId == 4 && c.CustomerKey == key;

            var locals = new SqlParameter[2];
            locals[0] = new SqlParameter("@clientKey", "TestKey");
            locals[0].SourceColumn = "CustomerKey";

            locals[1] = new SqlParameter("@clientId", 4);
            locals[1].SourceColumn = "CustomerId";

            string queryClause;
            var parameters = e.Parameterize("@p", null, out queryClause, locals: locals);
            queryClause.Should().Be("([Customer].[CustomerId] = @clientId and [Customer].[CustomerKey] = @clientKey)");
            parameters.GetLength(0).Should().Be(0);
        }

        [TestMethod]
        public void ParameterizeInceptionGreaterThanOrEqualsExpression()
        {
            var Inception = new DateTime(1999, 9, 9);
            Expression<Func<Customer, bool>> e = c => c.Inception >= Inception;

            string queryClause;
            var parameters = e.Parameterize("@Inception", null, out queryClause);
            queryClause.Should().Be("[Customer].[Inception] >= @Inception");

            parameters.GetLength(0).Should().Be(1);
            parameters[0].Value.Should().Be(Inception);

        }

        [TestMethod]
        public void ParameterizeModifiedLessThanOrEqualsExpression()
        {
            var lastModifyDateTime = new DateTimeOffset(new DateTime(1999, 9, 9), TimeSpan.FromHours(-5));
            Expression<Func<Customer, bool>> e = c => c.Modified >= lastModifyDateTime;

            string queryClause;
            var parameters = e.Parameterize("@lastModifyDateTime", null, out queryClause);
            queryClause.Should().Be("[Customer].[Modified] >= @lastModifyDateTime");

            parameters.GetLength(0).Should().Be(1);
            parameters[0].Value.Should().Be(lastModifyDateTime);
        }

        [TestMethod]
        public void ParameterizeModifiedLessThanOrEqualsExpressionUsingVisitor()
        {
            var lastModifyDateTime = new DateTimeOffset(new DateTime(1999, 9, 9), TimeSpan.FromHours(-5));
            Expression<Func<Customer, bool>> e = c => c.Modified >= lastModifyDateTime;

            string queryClause;
            var parameters = e.Parameterize("@lastModifyDateTime", null, out queryClause);
            queryClause.Should().Be("[Customer].[Modified] >= @lastModifyDateTime");

            parameters.GetLength(0).Should().Be(1);
            parameters[0].Value.Should().Be(lastModifyDateTime);
        }
    }
}
