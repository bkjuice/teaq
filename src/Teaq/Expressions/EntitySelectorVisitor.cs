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
    /// <summary>
    /// Visitor implementation to leverage framework class to navigate expression tree.
    /// </summary>
    internal sealed class EntitySelectorVisitor : ExpressionVisitor
    {
        #region Expression Lookups

        /// <summary>
        /// The known symbols for creating a string representation of the entity expression.
        /// </summary>
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

        /// <summary>
        /// The known symbols for creating a string representation of a null comparison expression.
        /// </summary>
        private static readonly Dictionary<ExpressionType, string> supportedDbNullSymbols = new Dictionary<ExpressionType, string>
            {
                { ExpressionType.Equal, " Is " },
                { ExpressionType.NotEqual, " Is Not " },
            };

        /// <summary>
        /// The known symbols for creating a string representation of a compound logical entity expression.
        /// </summary>
        private static readonly HashSet<ExpressionType> compoundExpressions = new HashSet<ExpressionType>
            {
                ExpressionType.AndAlso,
                ExpressionType.OrElse,
            };

        #endregion

        /// <summary>
        /// The base parameter name.
        /// </summary>
        private readonly string baseParameterName;
       
        /// <summary>
        /// The current batch index, if the expression will be used in a query batch.
        /// </summary>
        private readonly int? currentBatchIndex;

        /// <summary>
        /// The batch specification, if applicable.
        /// </summary>
        private readonly QueryBatch batch;

        /// <summary>
        /// Any predefined local query parameters that may be used when building the selector expression.
        /// </summary>
        private readonly SqlParameter[] predefinedLocals;

        /// <summary>
        /// The parameter qualifier.
        /// </summary>
        private int parameterQualifier;

        /// <summary>
        /// Value indicating the current member expression is nullable and uses "HasValue";
        /// </summary>
        private bool currentMemberExpressionNullableWithHasValue;

        /// <summary>
        /// The count of open nested groupings.
        /// </summary>
        private int openNestedGroupings;

        /// <summary>
        /// The data model entity config, if one exists.
        /// </summary>
        private readonly IDataModel model;

        /// <summary>
        /// The table aliases.
        /// </summary>
        private Dictionary<Type, string> tableAliases;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySelectorVisitor" /> class.
        /// </summary>
        /// <param name="baseParameterName">Name of the base parameter.</param>
        /// <param name="model">The entity config.</param>
        /// <param name="batch">The batch spec.</param>
        /// <param name="predefinedLocals">The predefined locals.</param>
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

        /// <summary>
        /// Gets the query clause.
        /// </summary>
        public StringBuilder QueryClause { get; private set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public List<SqlParameter> Parameters { get; private set; }

        /// <summary>
        /// Gets the symbol for the specified expression type.
        /// </summary>
        /// <param name="exprType">Type of the expr.</param>
        /// <returns>The symbol that matches the expression type.</returns>
        public static string SupportedSymbol(ExpressionType exprType)
        {
            return supportedSymbols[exprType];
        }

        /// <summary>
        /// Gets the symbol for the specified expression type for use in NULL comparison.
        /// </summary>
        /// <param name="exprType">Type of the expr.</param>
        /// <returns>The symbol that matches the expression type.</returns>
        public static string SupportedDbNullSymbol(ExpressionType exprType)
        {
            return supportedDbNullSymbols[exprType];
        }

        /// <summary>
        /// Adds a table alias for the specified type.
        /// </summary>
        /// <param name="target">The target type.</param>
        /// <param name="alias">The alias.</param>
        public void AddTableAlias(Type target, string alias)
        {
            Contract.Requires(target != null);

            if (this.tableAliases == null)
            {
                this.tableAliases = new Dictionary<Type, string>();
            }

            this.tableAliases.Add(target, alias);
        }

        /// <summary>
        /// Finalizes the expression and closes any open groupings.
        /// </summary>
        public void FinalizeExpression()
        {
            while (this.CloseIfGrouped())
            { 
            }
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        /// Static or extension methods are not supported. For example, if using 'Contains', be sure the expression refers
        /// to a concrete List{T} and not IEnumerable{T}.
        /// </exception>
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
            this.QueryClause.Append(sourceColumn).Append(" in ");
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

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            Type tableType;
            var name = ExtractMemberName(node, this.model, out tableType);
            if (this.currentMemberExpressionNullableWithHasValue)
            {
                this.QueryClause.Append(this.GetColumnQualifier(tableType) + ".");
                this.QueryClause.Append(name).Append(" Is Not NULL");
                this.currentMemberExpressionNullableWithHasValue = false;
                this.CloseIfGrouped();
            }
            else if (string.Compare(name, "HasValue", StringComparison.Ordinal) == 0)
            {
                this.currentMemberExpressionNullableWithHasValue = true;
            }

            return base.VisitMember(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.UnaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
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

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.BinaryExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (compoundExpressions.Contains(node.NodeType))
            {
                // Binary expressions are always grouped to ensure correctness. 
                // SQL Server ignores redundant parenthesis and there is no impact on query execution.
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
                this.QueryClause.Append(sourceColumn).Append(supportedSymbols[node.NodeType]).Append(predefinedName);
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
                    this.QueryClause.Append(sourceColumn).Append(supportedSymbols[node.NodeType]).Append(parameter.ParameterName);
                }
                else
                {
                    this.QueryClause.Append(sourceColumn).Append(supportedDbNullSymbols[node.NodeType]).Append("NULL");
                }
            }

            return node;
        }

        /// <summary>
        /// Checks the property name mapping, if one exists.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="config">The entity model configuration.</param>
        /// <returns>The mapped property name if a mapping exists, otherwise the property name.</returns>
        private static string MapColumn(string propertyName, IEntityConfiguration config)
        {
            if (config == null)
            {
                return propertyName;
            }

            return config.ColumnMapping(propertyName);
        }

        /// <summary>
        /// Evaluates the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>The expression value.</returns>
        private static object Evaluate(Expression operation)
        {
            object value;
            if (!TryEvaluate(operation, out value))
            {
                // use compile / invoke as a fall-back, which has poor perf:                
                value = Expression.Lambda(operation).Compile().DynamicInvoke(); 
            } 
            
            return value;
        }

        /// <summary>
        /// Attempts to evaluate the expression value using reflection, to avoid a costly Lambda.Compile invocation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="value">The value.</param>
        /// <returns>True if the value was able to be extracted via reflection, false if the expression must be compiled and then evaluated.</returns>
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

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="info">The reflected field info for the expression element.</param>
        /// <returns>The object value.</returns>
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

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="info">The reflected property info for the expression element.</param>
        /// <returns>The object value.</returns>
        private static object GetPropertyValue(object target, PropertyInfo info)
        {
            var t = target?.GetType().GetTypeDescription(MemberTypes.Field | MemberTypes.Property);
            return t?.GetProperty(info.Name)?.GetValue(target);
        }

        /// <summary>
        /// Extracts the name of the member.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="model">The optional configuration.</param>
        /// <param name="tableType">The column type.</param>
        /// <returns>
        /// The member name.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Expected a member or unary expression.</exception>
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
            return MapColumn(member.Name, GetEntityConfig(model, tableType));
        }

        /// <summary>
        /// Gets the entity configuration.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <returns>The entity configuration or null.</returns>
        private static IEntityConfiguration GetEntityConfig(IDataModel model, Type tableType)
        {
            if (model == null)
            {
                return null;
            }

            return model.GetEntityConfig(tableType);
        }

        /// <summary>
        /// Matches the name to the specified global name.
        /// </summary>
        /// <param name="sourceName">Name of the source.</param>
        /// <param name="globalName">Name of the global.</param>
        /// <returns>True if the names match, false otherwise.</returns>
        private static bool MatchName(string sourceName, string globalName)
        {
            return string.Compare(sourceName, globalName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Tries to get the the column data type if one is specified.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="sourceColumn">The source column.</param>
        /// <returns>
        /// The column data type specification or null if none is specified.
        /// </returns>
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

        /// <summary>
        /// Tries to get the the column data type if one is specified.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The column data type specification or null if none is specified.
        /// </returns>
        private string GetColumnQualifier(Type entityType)
        {
            Contract.Requires(entityType != null);

            if ((this.tableAliases?.ContainsKey(entityType)).GetValueOrDefault())
            {
                return this.tableAliases[entityType];
            }

            return entityType.AsUnqualifiedTable(this.model?.GetEntityConfig(entityType));
        }

        /// <summary>
        /// Tries to get the predefined parameter if one exists.
        /// </summary>
        /// <param name="sourceColumnName">Name of the source column.</param>
        /// <param name="predefinedName">Name of the predefined.</param>
        /// <returns>True if the parameter is predefined, false otherwise, indicating the parameter must be created.</returns>
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

        /// <summary>
        /// Closes any open grouping expressions.
        /// </summary>
        /// <returns>True if there are any additional open groupings; false otherwise.</returns>
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