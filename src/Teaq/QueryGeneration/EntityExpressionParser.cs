using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using Teaq.Configuration;

namespace Teaq.QueryGeneration
{
    internal static class EntityExpressionParser
    {
        private static readonly Dictionary<string, Func<string, string>> orderByBuilders = new Dictionary<string, Func<string, string>>
            {
                [ "OrderBy"] =  s => "order by " + s,
                [ "OrderByDescending"] = s=> $"order by {s} desc" 
            };

        public static SqlParameter[] Parameterize<T>(
            this Expression<Func<T, bool>> entityExpression, 
            string parameterName,
            QueryBatch batch, 
            out string queryClause, 
            IDataModel model = null,
            SqlParameter[] locals = null, 
            string columnQualifier = null)
        {
            var visitor = new EntitySelectorVisitor(parameterName, model, batch, locals);
            if (!string.IsNullOrEmpty(columnQualifier))
            {
                visitor.AddTableAlias(typeof(T), columnQualifier);
            }

            visitor.Visit(entityExpression);
            visitor.FinalizeExpression();
            queryClause = visitor.QueryClause.ToString();
            return visitor.Parameters.ToArray();
        }

        public static string ParsePropertyName(this LambdaExpression expression)
        {
            Contract.Requires(expression != null);

            var memberExpr = expression.Body as MemberExpression;
            if (memberExpr == null)
            {
                if (expression.Body.NodeType == ExpressionType.Convert)
                {
                    memberExpr = (expression.Body as UnaryExpression).Operand as MemberExpression;
                }

                if (memberExpr == null)
                {
                    throw new NotSupportedException(
                        "Only simple property expressions are supported. The provided expression was not a MemberExpression.");
                }
            }

            var subExpr = memberExpr.Expression as MemberExpression;
            if (subExpr != null)
            {
                throw new NotSupportedException(
                    "The provided expression must be a property that is an immediate child of the entity. " +
                    "You cannot define descendent properties of complex types from the parent entity " +
                    "(ex. MyClass.Property1.Child1 is illegal).");
            }

            return memberExpr.Member.Name;
        }

        public static string ParseJoinExpression<T, TJoined>(
            this Expression<Func<T, TJoined, bool>> onExpression, 
            IDataModel model = null, 
            string aliasForT = null, 
            string aliasForTJoined = null)
        {
            Contract.Requires(onExpression != null);

            var binaryExpr = onExpression.Body as BinaryExpression;
            if (binaryExpr == null)
            {
                throw new NotSupportedException(
                    "Only simple binary expressions are supported. The provided expression was not a BinaryExpression.");
            }

            var leftExpr = binaryExpr.Left as MemberExpression;
            if (leftExpr == null)
            {
                throw new NotSupportedException("The provided lefthand expression must be a property expression.");
            }

            var t = typeof(T);
            var config = model?.GetEntityConfig(t);
            var table1Column = leftExpr.Member.Name.AsQualifiedColumn(aliasForT, config, t);
            var rightExpr = binaryExpr.Right as MemberExpression;
            if (rightExpr == null)
            {
                var constRightExpr = binaryExpr.Right as ConstantExpression;
                if (constRightExpr == null)
                {
                    throw new NotSupportedException(
                        "The provided righthand expression must be a property expression or a constant.");
                }

                if (constRightExpr.Value == null)
                {
                    return table1Column + EntitySelectorVisitor.SupportedDbNullSymbol(binaryExpr.NodeType) + "NULL";
                }
                else
                {
                    return table1Column + 
                        EntitySelectorVisitor.SupportedSymbol(binaryExpr.NodeType) + 
                        constRightExpr.Value.ToString();
                }
            }
            else
            {
                var tJoined = typeof(TJoined);
                var joinedConfig = model?.GetEntityConfig(tJoined);
                return table1Column +
                    EntitySelectorVisitor.SupportedSymbol(binaryExpr.NodeType) +
                    rightExpr.Member.Name.AsQualifiedColumn(aliasForTJoined, joinedConfig, tJoined);
            }
        }

        public static string ParseOrderByClause<T>(
            this Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> orderByExpression, 
            IEntityConfiguration config = null)
        {
            Contract.Requires(orderByExpression != null);

            var methodExpr = orderByExpression.Body as MethodCallExpression;
            Func<string, string> builder;
            if (!orderByBuilders.TryGetValue(methodExpr.Method.Name, out builder))
            {
                throw new NotSupportedException("Expected OrderBy or OrderByDescending method call.");
            }

            var arguments = methodExpr.Arguments;
            if (arguments.Count != 2)
            {
                throw new NotSupportedException(
                    "Expected 2 arguments for a standard Linq OrderBy or OrderByDescending method call.");
            }

            var targetProperty = ((LambdaExpression)arguments[1]).ParsePropertyName();
            var target = typeof(T);
            return builder(target.AsUnqualifiedTable(config) + "." + targetProperty.EnsureBracketedIdentifier());
        }
    }
}