using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Teaq.Configuration;
using Teaq.FastReflection;

namespace Teaq.QueryGeneration
{
    internal sealed class QueryPredicateVisitor : ExpressionVisitor
    {
        #region Expression Lookups

        private static readonly Dictionary<ExpressionType, string> supportedSymbols = new Dictionary<ExpressionType, string>
        {
            [ExpressionType.Equal] = " = ",
            [ExpressionType.GreaterThan] = " > ",
            [ExpressionType.GreaterThanOrEqual] = " >= ",
            [ExpressionType.LessThan] = " < ",
            [ExpressionType.LessThanOrEqual] = " <= ",
            [ExpressionType.NotEqual] = " != ",
            [ExpressionType.AndAlso] = " AND ",
            [ExpressionType.OrElse] = " OR ",
        };

        private static readonly Dictionary<ExpressionType, string> supportedDbNullSymbols = new Dictionary<ExpressionType, string>
        {
            [ExpressionType.Equal] = " IS ",
            [ExpressionType.NotEqual] = " IS NOT ",
        };

        private static readonly HashSet<ExpressionType> compoundExpressions = new HashSet<ExpressionType>
        {
            ExpressionType.AndAlso,
            ExpressionType.OrElse,
        };

        #endregion

        private readonly string baseParameterName;

        private readonly int? currentBatchIndex;

        private readonly QueryBatch batch;

        private readonly SqlParameter[] predefinedLocals;

        private int parameterQualifier;

        private bool expressionIsNullableHasValue;

        private int openNestedGroupings;

        private readonly IDataModel model;

        private Dictionary<Type, string> tableAliases;

        public QueryPredicateVisitor(
            string baseParameterName,
            IDataModel model,
            QueryBatch batch,
            SqlParameter[] predefinedLocals)
        {
            this.QueryClause = new StringBuilder(1024);
            this.Parameters = new List<SqlParameter>(20);
            this.baseParameterName = baseParameterName;
            this.batch = batch;
            this.model = model;

            this.currentBatchIndex = batch?.NextBatchIndex();
            this.predefinedLocals = predefinedLocals;
        }

        public StringBuilder QueryClause { get; private set; }

        public List<SqlParameter> Parameters { get; private set; }

        public static string SupportedSymbol(ExpressionType exprType)
        {
            return supportedSymbols[exprType];
        }

        public static string SupportedDbNullSymbol(ExpressionType exprType)
        {
            return supportedDbNullSymbols[exprType];
        }

        public void AddTableAlias(Type target, string alias)
        {
            Contract.Requires(target != null);

            if (this.tableAliases == null)
            {
                this.tableAliases = new Dictionary<Type, string>();
            }

            this.tableAliases.Add(target, alias);
        }

        public void FinalizeExpression()
        {
            while (this.CloseIfGrouped()) ;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "Contains":
                    return this.VisitContainsMethod(node);

                case "IsNullOrEmpty":
                    return this.VisitStringIsNullOrEmptyMethod(node);

                default:
                    throw new NotSupportedException("Only .Contains or string.IsNullOrEmpty methods are supported.");
            }
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Type tableType;
            var name = ExtractMemberName(node, this.model, out tableType);
            if (this.expressionIsNullableHasValue)
            {
                this.QueryClause.Append(this.GetColumnQualifier(tableType) + ".");
                this.QueryClause.AppendSqlIdentifier(name).Append(" IS NOT NULL");
                this.expressionIsNullableHasValue = false;
                this.CloseIfGrouped();
            }
            else if (string.Compare(name, "HasValue", StringComparison.OrdinalIgnoreCase) == 0)
            {
                this.expressionIsNullableHasValue = true;
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                var binaryExpr = node.Operand as BinaryExpression;
                if (binaryExpr == null)
                {
                    this.QueryClause.Append("NOT (");
                    this.openNestedGroupings++;
                }
                else
                {
                    this.QueryClause.Append("NOT ");
                }
            }

            return base.VisitUnary(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (compoundExpressions.Contains(node.NodeType))
            {
                // Binary expressions are always grouped. SQL Server ignores redundant parenthesis.
                this.QueryClause.Append("(");
                this.Visit(node.Left);
                this.QueryClause.Append(supportedSymbols[node.NodeType]);
                this.parameterQualifier++;
                this.Visit(node.Right);
                this.QueryClause.Append(")");
                return node;
            }

            Type tableType;
            string sourceColumn = ExtractMemberName(node.Left, this.model, out tableType);
            this.QueryClause.Append(this.GetColumnQualifier(tableType) + ".");
            string predefinedName;
            if (this.TryPredefinedParameter(sourceColumn, out predefinedName))
            {
                this.QueryClause.AppendSqlIdentifier(sourceColumn).Append(supportedSymbols[node.NodeType]).Append(predefinedName);
            }
            else
            {
                var value = Evaluate(node.Right);
                if (value != null)
                {
                    var columnDataType = this.GetColumnDataType(tableType, sourceColumn);
                    var parameter = value.MakeQualifiedParameter(
                        sourceColumn,
                        columnDataType,
                        this.baseParameterName,
                        this.currentBatchIndex,
                        this.parameterQualifier,
                        -1);

                    this.Parameters.Add(parameter);
                    this.QueryClause.AppendSqlIdentifier(sourceColumn).Append(supportedSymbols[node.NodeType]).Append(parameter.ParameterName);
                }
                else
                {
                    this.QueryClause.AppendSqlIdentifier(sourceColumn).Append(supportedDbNullSymbols[node.NodeType]).Append("NULL");
                }
            }

            return node;
        }

        private static object Evaluate(Expression operation)
        {
            object value;
            if (!TryEvaluate(operation, out value))
            {
                // use compile / invoke as a fall-back, which has relatively poor perf:                
                value = Expression.Lambda(operation).Compile().DynamicInvoke();
            }

            return value;
        }

        private static bool TryEvaluate(Expression operation, out object value)
        {
            if (operation == null)
            {
                // used for static fields:                 
                value = null;
                return true;
            }

            switch (operation.NodeType)
            {
                case ExpressionType.Constant:
                    {
                        value = ((ConstantExpression)operation).Value;
                        return true;
                    }

                case ExpressionType.MemberAccess:
                    {
                        var me = (MemberExpression)operation;
                        object target;
                        if (TryEvaluate(me.Expression, out target))
                        {
                            switch (me.Member.MemberType)
                            {
                                case MemberTypes.Field:
                                    {
                                        value = GetFieldValue(target, (FieldInfo)me.Member);
                                        return true;
                                    }

                                case MemberTypes.Property:
                                    {
                                        value = GetPropertyValue(target, (PropertyInfo)me.Member);
                                        return true;
                                    }
                            }
                        }

                        break;
                    }
            }

            value = null;
            return false;
        }

        private static object GetFieldValue(object target, FieldInfo info)
        {
            Contract.Requires(info != null);

            if (info.IsStatic)
            {
                return info.GetValue(null);
            }

            var t = target?.GetType().GetTypeDescription(MemberTypes.Field | MemberTypes.Property);
            return t?.GetField(info.Name)?.GetValue(target);
        }

        private static object GetPropertyValue(object target, PropertyInfo info)
        {
            var t = target?.GetType().GetTypeDescription(MemberTypes.Field | MemberTypes.Property);
            return t?.GetProperty(info.Name)?.GetValue(target);
        }

        private static string ExtractMemberName(Expression expression, IDataModel model, out Type tableType)
        {
            Contract.Ensures(Contract.ValueAtReturn(out tableType) != null);

            var left = (expression as MemberExpression) ?? (expression as UnaryExpression)?.Operand as MemberExpression;
            if (left == null)
            {
                throw new InvalidOperationException("Expected a member or unary expression.");
            }

            var member = left.Member;
            tableType = member.DeclaringType;

            return model?.GetEntityConfig(tableType)?.ColumnMapping(member.Name) ?? member.Name;
        }

        private static bool MatchName(string sourceName, string globalName)
        {
            return string.Equals(sourceName, globalName, StringComparison.OrdinalIgnoreCase);
        }

        private static Type GetTableTypeAndSourceColumn(MemberExpression arg, out string sourceColumn)
        {
            Contract.Ensures(Contract.Result<Type>() != null);

            var memberAccess = arg.Expression as MemberExpression;
            if (memberAccess != null)
            {
                sourceColumn = memberAccess.Member.Name;
                return memberAccess.Member.DeclaringType;
            }
            else
            {
                sourceColumn = arg.Member.Name;
                return arg.Member.DeclaringType;
            }
        }

        private Expression VisitStringIsNullOrEmptyMethod(MethodCallExpression node)
        {
            Contract.Ensures(Contract.Result<Expression>() != null);

            var arg = (MemberExpression)node.Arguments[0];

            string sourceColumn;
            var tableType = GetTableTypeAndSourceColumn(arg, out sourceColumn);
            sourceColumn = sourceColumn.EnsureBracketedIdentifier();
            var qualifier = this.GetColumnQualifier(tableType) + ".";

            this.QueryClause
                .Append(qualifier + sourceColumn)
                .Append(" IS NULL OR LEN(")
                .Append(qualifier + sourceColumn)
                .Append(") = 0");

            return node;
        }

        private Expression VisitContainsMethod(MethodCallExpression node)
        {
            Contract.Ensures(Contract.Result<Expression>() != null);

            if (node.Object == null)
            {
                throw new NotSupportedException(
                    "Static or extension methods are not supported for '.Contains'. " +
                        "Be sure the expression refers to a concrete collection type that is enumerable, such as List<T>.");
            }

            var arg = (MemberExpression)node.Arguments[0];

            string sourceColumn;
            var tableType = GetTableTypeAndSourceColumn(arg, out sourceColumn);

            this.QueryClause.Append(this.GetColumnQualifier(tableType) + ".");
            this.QueryClause.AppendSqlIdentifier(sourceColumn).Append(" IN ");
            this.QueryClause.Append("(");

            var target = Evaluate(node.Object);
            var columnDataType = this.GetColumnDataType(tableType, sourceColumn);

            var enumerable = target as IEnumerable;
            int i = 0;
            foreach (var item in enumerable)
            {
                var parameter = item.MakeQualifiedParameter(
                    sourceColumn,
                    columnDataType,
                    this.baseParameterName,
                    this.currentBatchIndex,
                    this.parameterQualifier,
                    i);

                this.Parameters.Add(parameter);
                this.QueryClause.Append(parameter.ParameterName).Append(", ");
                i++;
            }

            this.QueryClause.Length = this.QueryClause.Length - 2;
            this.QueryClause.Append(")");

            this.CloseIfGrouped();
            return node;
        }

        private ColumnDataType GetColumnDataType(Type entityType, string sourceColumn)
        {
            ColumnDataType columnDataType = null;
            if (this.model != null)
            {
                var config = this.model.GetEntityConfig(entityType);
                if (config != null)
                {
                    columnDataType = config.ColumnDataType(sourceColumn);
                }
            }

            return columnDataType;
        }

        private string GetColumnQualifier(Type entityType)
        {
            Contract.Requires(entityType != null);

            if ((this.tableAliases?.ContainsKey(entityType)).GetValueOrDefault())
            {
                return this.tableAliases[entityType];
            }

            return entityType.AsUnqualifiedTable(this.model?.GetEntityConfig(entityType));
        }

        private bool TryPredefinedParameter(string sourceColumnName, out string predefinedName)
        {
            predefinedName = null;

            var locals = this.predefinedLocals;
            for (int i = 0; i < locals?.Length; i++)
            {
                var p = locals[i];
                if (MatchName(p.SourceColumn, sourceColumnName))
                {
                    predefinedName = p.ParameterName;
                    return true;
                }
            }

            if (this.batch?.GlobalNameExists(sourceColumnName) == true)
            {
                var parameters = this.batch.EmbeddedParameters();
                for (int i = 0; i < parameters.Count; i++)
                {
                    var p = parameters[i];
                    if (MatchName(sourceColumnName, p.SourceColumnName))
                    {
                        predefinedName = p.ParameterName;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CloseIfGrouped()
        {
            if (this.openNestedGroupings > 0)
            {
                this.QueryClause.Append(")");
                this.openNestedGroupings--;
            }

            return this.openNestedGroupings > 0;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.QueryClause != null);
        }
    }
}