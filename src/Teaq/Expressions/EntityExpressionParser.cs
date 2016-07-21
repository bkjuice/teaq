using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using Teaq.Configuration;
using Teaq.QueryGeneration;

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Design", 
    "CA1020:AvoidNamespacesWithFewTypes", 
    Scope = "namespace", 
    Target = "Teaq.Expressions")]

namespace Teaq.Expressions
{
    /// <summary>
    /// Sponsor class for simple entity expression parsing.
    /// </summary>
    internal static class EntityExpressionParser
    {
        /// <summary>
        /// The order by tokens.
        /// </summary>
        private static readonly Dictionary<string, string> orderByTokens = new Dictionary<string, string>
            {
                { "OrderBy", "order by {0}" },
                { "OrderByDescending", "order by {0} desc" }
            };

        /// <summary>
        /// Combines the expressions using the and operator.
        /// </summary>
        /// <typeparam name="T">The target auditable entity type.</typeparam>
        /// <param name="expression1">The initial expression.</param>
        /// <param name="expression2">The expresssion to be combined.</param>
        /// <returns>
        /// The combined expression.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures", 
            Justification = "Expression type requires nested delegate. Common pattern.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1011:ConsiderPassingBaseTypesAsParameters", 
            Justification = "Using generic derived type for compile type checking.")]
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expression1, 
            Expression<Func<T, bool>> expression2)
        {
            Contract.Requires(expression1 != null);
            Contract.Requires(expression2 != null);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(expression1.Body, expression2.Body), 
                expression1.Parameters);
        }

        /// <summary>
        /// Combines the expressions using the and operator.
        /// </summary>
        /// <typeparam name="T">The target auditable entity type.</typeparam>
        /// <param name="expression1">The initial expression.</param>
        /// <param name="expression2">The expresssion to be combined.</param>
        /// <returns>
        /// The combined expression.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Expression type requires nested delegate. Common pattern.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1011:ConsiderPassingBaseTypesAsParameters", 
            Justification = "Using generic derived type for compile type checking.")]
        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expression1, 
            Expression<Func<T, bool>> expression2)
        {
            Contract.Requires(expression1 != null);
            Contract.Requires(expression2 != null);

            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(expression1.Body, expression2.Body),
                expression1.Parameters);
        }

        /// <summary>
        /// Parameterizes the specified entity expression.
        /// </summary>
        /// <typeparam name="T">The target entity type.</typeparam>
        /// <param name="entityExpression">The entity expression.</param>
        /// <param name="parameterName">The base name of the parameter.</param>
        /// <param name="queryClause">The query clause.</param>
        /// <param name="model">The model.</param>
        /// <param name="locals">The local predefined parameters.</param>
        /// <param name="columnQualifier">The table alias.</param>
        /// <returns>
        /// The array of initialized SQL parameters.
        /// </returns>
        /// <exception cref="System.NotSupportedException">Thrown if an unsupported expression is passed.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures", 
            Justification = "Expression type requires nested delegate. Common pattern.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1021:AvoidOutParameters",
            MessageId = "2#", 
            Justification = "Design does not justify a class definition for return values.")]
        public static SqlParameter[] Parameterize<T>(
            this Expression<Func<T, bool>> entityExpression, 
            string parameterName, 
            out string queryClause, 
            IDataModel model = null, 
            SqlParameter[] locals = null, 
            string columnQualifier = null)
        {
            return entityExpression.Parameterize(parameterName, null, out queryClause, model, locals, columnQualifier);
        }

        /// <summary>
        /// Parameterizes the specified entity expression.
        /// </summary>
        /// <typeparam name="T">The target entity type.</typeparam>
        /// <param name="entityExpression">The entity expression.</param>
        /// <param name="parameterName">The base name of the parameter.</param>
        /// <param name="batch">The batch specification.</param>
        /// <param name="queryClause">The query clause.</param>
        /// <param name="model">The configuration.</param>
        /// <param name="locals">The local predefined parameters.</param>
        /// <param name="columnQualifier">The table alias.</param>
        /// <returns>
        /// The array of initialized SQL parameters.
        /// </returns>
        /// <exception cref="System.NotSupportedException">Thrown if an unsupported expression is passed.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1011:ConsiderPassingBaseTypesAsParameters", 
            Justification = "Compiler verification is desired.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures", 
            Justification = "Expression type requires nested delegate. Common pattern.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1021:AvoidOutParameters", 
            MessageId = "3#", 
            Justification = "Design does not justify a class definition for return values.")]
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

        /// <summary>
        /// Extracts the name of the property.
        /// </summary>
        /// <param name="expression">The e.</param>
        /// <returns>
        /// The property name.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        /// Only simple property expressions are supported. The provided expression was not a MemberExpression.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The provided expression must be a property that is an immediate child of the entity. 
        /// You cannot define descendent properties of complex types from the parent entity 
        /// (ex. MyClass.Property1.Child1 is illegal).
        /// </exception>
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

        /// <summary>
        /// Parses the join clause.
        /// </summary>
        /// <typeparam name="T">The target entity type.</typeparam>
        /// <typeparam name="TJoined">The type of the joined.</typeparam>
        /// <param name="onExpression">The on expression.</param>
        /// <param name="model">The model.</param>
        /// <param name="aliasForT">The alias for T.</param>
        /// <param name="aliasForTJoined">The alias for T joined.</param>
        /// <returns>
        /// The join by clause that may be appended to the working query.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        /// Only simple binary expressions are supported. The provided expression was not a BinaryExpression.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures", 
            Justification = "Required by .NET framework design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1011:ConsiderPassingBaseTypesAsParameters", 
            Justification = "Compile time type safety is desirable to encourage correct usages.")]
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
                        "The provided righthand expression must be a property expression or a constant");
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

        /// <summary>
        /// Parses the order by clause.
        /// </summary>
        /// <typeparam name="T">The target entity type.</typeparam>
        /// <param name="orderByExpression">The order by expression.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>
        /// The order by clause that may be appended to the working query.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Expected Linq OrderBy or OrderByDescending method call.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Expected a Linq OrderBy or OrderByDescending method call. This method call will have 2 arguments. 
        /// The provided method call has (n) arguments.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1006:DoNotNestGenericTypesInMemberSignatures", 
            Justification = "Required by .NET framework design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design", 
            "CA1011:ConsiderPassingBaseTypesAsParameters", 
            Justification = "Compile time type safety is desirable to encourage correct usages.")]
        public static string ParseOrderByClause<T>(
            this Expression<Func<IEnumerable<T>, IOrderedEnumerable<T>>> orderByExpression, 
            IEntityConfiguration config = null)
        {
            Contract.Requires(orderByExpression != null);

            // Compile type check ensures this is not null and has 2 arguments:
            var methodExpr = orderByExpression.Body as MethodCallExpression;

            string token;
            if (!orderByTokens.TryGetValue(methodExpr.Method.Name, out token))
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
            return token.Replace("{0}", target.AsUnqualifiedTable(config) + "." + targetProperty);
        }
    }
}