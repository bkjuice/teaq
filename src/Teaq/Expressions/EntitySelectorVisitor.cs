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
using Teaq.QueryGeneration;

namespace Teaq.Expressions
{
    internal sealed class EntitySelectorVisitor : ExpressionVisitor
    {
        #region Expression Lookups

        private static readonly Dictionary<ExpressionType, string> supportedSymbols = new Dictionary<ExpressionType, string>
            {
                { ExpressionType.Equal, " = " },
                { ExpressionType.GreaterThan, " > " },
                { ExpressionType.GreaterThanOrEqual, " >= " },
                { ExpressionType.LessThan, " < " },
                { ExpressionType.LessThanOrEqual, " <= " },
                { ExpressionType.NotEqual, " != " },
                { ExpressionType.AndAlso, " and " },
                { ExpressionType.OrElse, " or " },
            };

        private static readonly Dictionary<ExpressionType, string> supportedDbNullSymbols = new Dictionary<ExpressionType, string>
            {
                { ExpressionType.Equal, " Is " },
                { ExpressionType.NotEqual, " Is Not " },
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

        private bool currentMemberExpressionNullableWithHasValue;

        private int openNestedGroupings;

        private readonly IDataModel model;

        private Dictionary<Type, string> tableAliases;

        public EntitySelectorVisitor(
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

            if (batch != null)
            {
                this.currentBatchIndex = batch.NextBatchIndex();
            }

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
            while (this.CloseIfGrouped())
            {
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object == null)
            {
                throw new NotSupportedException(
                    "Static or extension methods are not supported. For example, if using 'Contains', " +
                        "be sure the expression refers to a concrete collection type that is enumerable, such as List<T>.");
            }

            // NOTE: Move this to a dictionary lookup if supporting several methods.
            if (node.Method.Name != "Contains")
            {
                throw new NotSupportedException("Only 'Contains' is currently supported.");
            }

            // TODO: Support IsNullOrEmpty / Whitespace as special cases...
            var arg = (MemberExpression)node.Arguments[0];

            Type tableType = null;
            string sourceColumn;
            var memberAccess = arg.Expression as MemberExpression;
            if (memberAccess != null)
            {
                sourceColumn = memberAccess.Member.Name;
                tableType = memberAccess.Member.DeclaringType;
            }
            else
            {
                sourceColumn = arg.Member.Name;
                tableType = arg.Member.DeclaringType;
            }

            this.QueryClause.Append(this.GetColumnQualifier(tableType) + ".");
            this.QueryClause.AppendIdentifier(sourceColumn).Append(" in ");
            this.QueryClause.Append("(");

            // IList to avoid yet another enumerator, if possible:
            object target = Evaluate(node.Object);
            var listTarget = target as IList;

            var columnDataType = this.GetColumnDataType(tableType, sourceColumn);
            if (listTarget != null)
            {
                for (var i = 0; i < listTarget.Count; i++)
                {
                    var parameter = listTarget[i].MakeParameter(
                        sourceColumn,
                        columnDataType,
                        this.baseParameterName,
                        this.currentBatchIndex,
                        this.parameterQualifier,
                        i);

                    this.Parameters.Add(parameter);
                    this.QueryClause.Append(parameter.ParameterName).Append(", ");
                }
            }
            else
            {
                var enumerable = target as IEnumerable;
                int i = 0;
                foreach (var item in enumerable)
                {
                    var parameter = item.MakeParameter(
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
            }

            this.QueryClause.Length = this.QueryClause.Length - 2;
            this.QueryClause.Append(")");

            this.CloseIfGrouped();
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Type tableType;
            var name = ExtractMemberName(node, this.model, out tableType);
            if (this.currentMemberExpressionNullableWithHasValue)
            {
                this.QueryClause.Append(this.GetColumnQualifier(tableType) + ".");
                this.QueryClause.AppendIdentifier(name).Append(" Is Not NULL");
                this.currentMemberExpressionNullableWithHasValue = false;
                this.CloseIfGrouped();
            }
            else if (string.Compare(name, "HasValue", StringComparison.Ordinal) == 0)
            {
                this.currentMemberExpressionNullableWithHasValue = true;
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
                    this.QueryClause.Append("not (");
                    this.openNestedGroupings++;
                }
                else
                {
                    this.QueryClause.Append("not ");
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
                this.QueryClause.AppendIdentifier(sourceColumn).Append(supportedSymbols[node.NodeType]).Append(predefinedName);
            }
            else
            {
                var value = Evaluate(node.Right);
                if (value != null)
                {
                    var columnDataType = this.GetColumnDataType(tableType, sourceColumn);
                    var parameter = value.MakeParameter(
                        sourceColumn,
                        columnDataType,
                        this.baseParameterName,
                        this.currentBatchIndex,
                        this.parameterQualifier,
                        -1);

                    this.Parameters.Add(parameter);
                    this.QueryClause.AppendIdentifier(sourceColumn).Append(supportedSymbols[node.NodeType]).Append(parameter.ParameterName);
                }
                else
                {
                    this.QueryClause.AppendIdentifier(sourceColumn).Append(supportedDbNullSymbols[node.NodeType]).Append("NULL");
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
            return string.Compare(sourceName, globalName, StringComparison.OrdinalIgnoreCase) == 0;
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

            // Try local scope first:
            var locals = this.predefinedLocals;
            if (locals != null)
            {
                for (int i = 0; i < this.predefinedLocals.GetLength(0); i++)
                {
                    if (MatchName(locals[i].SourceColumn, sourceColumnName))
                    {
                        predefinedName = locals[i].ParameterName;
                        return true;
                    }
                }
            }

            if (this.batch == null)
            {
                return false;
            }

            if (!this.batch.GlobalNameExists(sourceColumnName))
            {
                return false;
            }

            var parameters = this.batch.EmbeddedParameters();
            var len = parameters.Count;
            for (int i = 0; i < len; i++)
            {
                if (MatchName(sourceColumnName, parameters[i].SourceColumnName))
                {
                    predefinedName = parameters[i].ParameterName;
                }
            }

            return true;
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