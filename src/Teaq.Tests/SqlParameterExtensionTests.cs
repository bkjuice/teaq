using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.Configuration;
using Teaq.QueryGeneration;

namespace Teaq.Tests
{
    [TestClass]
    public class SqlParameterExtensionTests
    {
        [TestMethod]
        public void CopyParametersReturnsNullIfSourceIsNull()
        {
            List<SqlParameter> parameters = null;
            SqlParameterExtensions.Copy(parameters).Should().BeNull();
        }

        [TestMethod]
        public void MakeParameterCreatesDbNullWhenNull()
        {
            string value = null;
            var parameter = value.MakeQualifiedParameter("value", null, "", -1, -1, -1);
            parameter.Value.Should().Be(DBNull.Value);
        }

        [TestMethod]
        public void MakeParameterCreatesVarCharByDefault()
        {
            string value = "test";
            var parameter = value.MakeQualifiedParameter("value", null, "", -1, -1, -1);
            parameter.Value.Should().Be("test");
            parameter.SqlDbType.Should().Be(SqlDbType.VarChar);
        }

        [TestMethod]
        public void MakeParameterUsesBatchQualifier()
        {
            string value = "test";
            var parameter = value.MakeQualifiedParameter("test", null, "@test", 1, -1, -1);
            parameter.Value.Should().Be("test");
            parameter.ParameterName.Should().Be("@test1");
        }

        [TestMethod]
        public void MakeParameterUsesBatchAndParameterQualifiers()
        {
            string value = "test";
            var parameter = value.MakeQualifiedParameter("test", null, "@test", 1, 2, -1);
            parameter.Value.Should().Be("test");
            parameter.ParameterName.Should().Be("@test1x2");
        }

        [TestMethod]
        public void MakeParameterUsesProvidedColumnDataType()
        {
            string value = "test";
            var columnDataType = new ColumnDataType { SqlDataType = SqlDbType.Char, Size = 20 };
            var parameter = value.MakeQualifiedParameter("test", columnDataType, "@test", 1, 2, -1);
            parameter.Value.Should().Be("test");
            parameter.ParameterName.Should().Be("@test1x2");
            parameter.SqlDbType.Should().Be(SqlDbType.Char);
            parameter.Size.Should().Be(20);
        }

        [TestMethod]
        public void MakeParameterUsesProvidedPrecisionAndScale()
        {
            string value = "test";
            var columnDataType = new ColumnDataType { SqlDataType = SqlDbType.Decimal, Precision = 8, Scale = 2 };
            var parameter = value.MakeQualifiedParameter("test", columnDataType, "@test", 1, 2, -1);
            parameter.Value.Should().Be("test");
            parameter.ParameterName.Should().Be("@test1x2");
            parameter.SqlDbType.Should().Be(SqlDbType.Decimal);
            parameter.Precision.Should().Be(8);
            parameter.Scale.Should().Be(2);
        }

        [TestMethod]
        public void MakeParameterUsesBatchAndParameterAndIndexQualifiers()
        {
            string value = "test";
            var parameter = value.MakeQualifiedParameter("test", null, "@test", batchQualifier: 1, parameterQualifier: 2, indexer: 5);
            parameter.Value.Should().Be("test");
            parameter.ParameterName.Should().Be("@test1x2n5");
        }

        [TestMethod]
        public void MakeParameterCreatesNVarCharWhenUnicodeIsSpecified()
        {
            string value = "test";
            var parameter = value.MakeQualifiedParameter("test", new ColumnDataType { SqlDataType = SqlDbType.NVarChar, Size = 50 }, "", -1, -1, -1);
            parameter.SqlDbType.Should().Be(SqlDbType.NVarChar);
            parameter.Size.Should().Be(50);
            parameter.Value.Should().Be("test");
        }

        [TestMethod]
        public void CombineCombinesArraysAsExpected()
        {
            var strings1 = new string[2] { "test1", "test2" };
            var strings2 = new string[2] { "test3", "test4" };

            var result = strings1.Combine(strings2);
            result.GetLength(0).Should().Be(4);
            result[0].Should().Be("test1");
            result[1].Should().Be("test2");
            result[2].Should().Be("test3");
            result[3].Should().Be("test4");
        }

        [TestMethod]
        public void CombineCombinesArraysAsExpectedWhenLeftIsNull()
        {
            string[] strings1 = null;
            var strings2 = new string[2] { "test3", "test4" };

            var result = strings1.Combine(strings2);
            result.GetLength(0).Should().Be(2);
            result[0].Should().Be("test3");
            result[1].Should().Be("test4");
            ReferenceEquals(strings2, result).Should().BeTrue();
        }

        [TestMethod]
        public void CombineCombinesArraysAsExpectedWhenRightIsNull()
        {
            var strings1 = new string[2] { "test1", "test2" };
            string[] strings2 = null;

            var result = strings1.Combine(strings2);
            result.GetLength(0).Should().Be(2);
            result[0].Should().Be("test1");
            result[1].Should().Be("test2");

            ReferenceEquals(strings1, result).Should().BeTrue();
        }

        [TestMethod]
        public void CopyParametersReturnsNullWhenSourceIsNull()
        {
            SqlParameter[] parameters = null;
            parameters.Copy().Should().BeNull();
        }

        [TestMethod]
        public void CopyParametersMakesValueBasedCopy()
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@p1", "test") };
            var copy = parameters.Copy();

            ReferenceEquals(copy, parameters).Should().BeFalse();
            CompareParameterLists(parameters, copy).Should().BeTrue();
        }

        [TestMethod]
        public void QualifyForBatchRenamesParametersAsExpected()
        {
            var batch = new QueryBatch();
            var sqlCommand = new SqlCommand();
            sqlCommand.Parameters.AddWithValue("@parameterA", "test");
            sqlCommand.Parameters.AddWithValue("@parameterB", "test");

            var qualifiedParams = sqlCommand.Parameters.QualifyForBatch(batch);
            var qualifier = batch.CurrentBatchIndex;
            qualifiedParams.Count().Should().Be(2);
            qualifiedParams.Skip(0).First().ParameterName.Should().Be("@parameterA" + qualifier);
            qualifiedParams.Skip(1).First().ParameterName.Should().Be("@parameterB" + qualifier);
        }

        [TestMethod]
        public void QualifyForBatchPreservesTypeInfoAsExpected()
        {
            var batch = new QueryBatch();
            var sqlCommand = new SqlCommand();
            sqlCommand.Parameters.AddWithValue("@parameterA", "test").TypeName = "Test1";
            sqlCommand.Parameters.AddWithValue("@parameterB", "test").TypeName = "Test2";

            var qualifiedParams = sqlCommand.Parameters.QualifyForBatch(batch);
            var qualifier = batch.CurrentBatchIndex;
            qualifiedParams.Count().Should().Be(2);
            qualifiedParams.Skip(0).First().TypeName.Should().Be("Test1");
            qualifiedParams.Skip(1).First().TypeName.Should().Be("Test2");
        }

        [TestMethod]
        public void QualifyForBatchThrowsArgumentNullExceptionWhenParametersAreNull()
        {
            Action test = () => SqlParameterExtensions.QualifyForBatch(null, new QueryBatch());
            test.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void QualifyForBatchCreatesCopiedParameters()
        {
            var batch = new QueryBatch();
            var sqlCommand = new SqlCommand();
            var p1 = sqlCommand.Parameters.AddWithValue("@parameterA", "test");
            var p2 = sqlCommand.Parameters.AddWithValue("@parameterB", "test");

            var qualifiedParams = sqlCommand.Parameters.QualifyForBatch(batch);
            qualifiedParams[0].Should().NotBeSameAs(p1);
            qualifiedParams[1].Should().NotBeSameAs(p2);
        }

        /// <summary>
        /// Compares the parameters.
        /// </summary>
        /// <param name="parametersA">The parameters A.</param>
        /// <param name="parametersB">The parameters B.</param>
        /// <returns>True if the array of parameters is considered equivelent.</returns>
        private static bool CompareParameterLists(SqlParameter[] parametersA, SqlParameter[] parametersB)
        {
            if (parametersA == null && parametersB == null)
            {
                return true;
            }

            if (parametersA == null)
            {
                return false;
            }

            if (parametersB == null)
            {
                return false;
            }

            if (ReferenceEquals(parametersA, parametersB))
            {
                return true;
            }

            if (parametersA.GetLength(0) != parametersB.GetLength(0))
            {
                return false;
            }

            for (int i = 0; i < parametersA.GetLength(0); i++)
            {
                if (string.Compare(
                    parametersA[i].ParameterName,
                    parametersB[i].ParameterName,
                    StringComparison.OrdinalIgnoreCase) != 0)
                {
                    return false;
                }

                if (string.Compare(
                    parametersA[i].SourceColumn,
                    parametersB[i].SourceColumn,
                    StringComparison.OrdinalIgnoreCase) != 0)
                {
                    return false;
                }

                if (parametersA[i].Value != parametersB[i].Value)
                {
                    return false;
                }

            }

            return true;
        }
    }
}
