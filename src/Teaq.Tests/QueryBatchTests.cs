using System;
using System.Data.SqlClient;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.QueryGeneration;
using Teaq.Tests.Stubs;

namespace Teaq.Tests
{
    [TestClass]
    public class QueryBatchTests
    {
        [TestMethod]
        public void QueryBatchWithNoExpectedResultsReturns0()
        {
            var batch = new QueryBatch();
            batch.ExpectedResultSetCount.Should().Be(0);
        }

        [TestMethod]
        public void QueryBatchWithNoExpectedResultsHasResultsReturnsFalse()
        {
            var batch = new QueryBatch();
            batch.HasResult.Should().BeFalse();
        }

        [TestMethod]
        public void QueryBatchWithOneResultReturnsExpectedCountsAndIndicators()
        {
            var batch = new QueryBatch();
            batch.Add<Customer>("test");
            batch.ExpectedResultSetCount.Should().Be(1);
            batch.HasResult.Should().BeTrue();
            batch.CurrentResult.Should().Be(typeof(Customer));
            batch.MoveToNextResultType().Should().BeFalse();
            (batch.CurrentResult == null).Should().BeTrue();
            batch.HasResult.Should().BeFalse();
        }

        [TestMethod]
        public void GlobalParametersExistReturnsFalseWithNoGlobals()
        {
            var spec = new QueryBatch();
            spec.GlobalParametersExist.Should().BeFalse();
        }

        [TestMethod]
        public void CurrentBatchIndexIsAsExpectedWhenIncremented()
        {
            var spec = new QueryBatch();
            spec.CurrentBatchIndex.Should().Be(0);
            spec.NextBatchIndex().Should().Be(1);
            spec.CurrentBatchIndex.Should().Be(1);
            spec.NextBatchIndex().Should().Be(2);
            spec.NextBatchIndex().Should().Be(3);
            spec.CurrentBatchIndex.Should().Be(3);
        }

        [TestMethod]
        public void CountOfGlobalParameterInfoIsAsExpected()
        {
            var spec = new QueryBatch();
            spec.AddEmbeddedParameter("test", "@test");
            var parameter = new SqlParameter("@test", "testValue");
            parameter.SourceColumn = "testColumn";
            spec.AddGlobalParameter(parameter);

            var info = spec.EmbeddedParameters();
            info.Count.Should().Be(2);
        }

        [TestMethod]
        public void CountOfGlobalParameterInfoIsAsExpectedAfterClearingGlobals()
        {
            var spec = new QueryBatch();
            spec.AddEmbeddedParameter("test", "@test");
            var parameter = new SqlParameter("@test", "testValue");
            parameter.SourceColumn = "testColumn";
            spec.AddGlobalParameter(parameter);

            spec.ClearGlobals();
            var info = spec.EmbeddedParameters();
            info.Should().BeNull();
        }

        [TestMethod]
        public void AddEmbeddedParameterResultsInEmbeddedParameterInfo()
        {
            var spec = new QueryBatch();
            spec.AddEmbeddedParameter("test", "@test");
            spec.GlobalParametersExist.Should().BeTrue();
            spec.GlobalNameExists("test").Should().BeTrue();
            spec.GlobalNameExists("@test").Should().BeTrue();
        }

        [TestMethod]
        public void AddEmbeddedParameterThrowsIfColumnNameIsNullOrEmpty()
        {
            var spec = new QueryBatch();
            Action test1 = () => spec.AddEmbeddedParameter(null, "@test");
            Action test2 = () => spec.AddEmbeddedParameter(string.Empty, "@test");

            test1.ShouldThrow<ArgumentNullException>();
            test2.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void AddEmbeddedParameterThrowsIfParameterNameIsNullOrEmpty()
        {
            var spec = new QueryBatch();
            Action test1 = () => spec.AddEmbeddedParameter("test", null);
            Action test2 = () => spec.AddEmbeddedParameter("test", string.Empty);

            test1.ShouldThrow<ArgumentNullException>();
            test2.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void AddGlobalParameterResultsInGlobalParameter()
        {
            var spec = new QueryBatch();
            var parameter = new SqlParameter("@test", "testValue");
            parameter.SourceColumn = "testColumn";

            spec.AddGlobalParameter(parameter);

            spec.GlobalParametersExist.Should().BeTrue();
            spec.GlobalNameExists("testColumn").Should().BeTrue();
            spec.GlobalNameExists("@test").Should().BeTrue();

            var parameters = spec.GlobalParameters();
            parameters.Should().NotBeNull();
            parameters.GetLength(0).Should().Be(1);
            parameters[0].ParameterName.Should().Be("@test");
            parameters[0].Value.Should().Be("testValue");
            parameters[0].SourceColumn.Should().Be("testColumn");
        }

        [TestMethod]
        public void AddGlobalParameterResultsInDeepCopyOnSecondUse()
        {
            var spec = new QueryBatch();
            var parameter = new SqlParameter("@test", "testValue");
            parameter.SourceColumn = "testColumn";

            spec.AddGlobalParameter(parameter);
            var parameters = spec.GlobalParameters();
            parameters.Should().NotBeNull();
            parameters.GetLength(0).Should().Be(1);
            ReferenceEquals(parameters[0], parameter).Should().BeTrue();

            parameters = spec.GlobalParameters();
            parameters.Should().NotBeNull();
            parameters.GetLength(0).Should().Be(1);
            ReferenceEquals(parameters[0], parameter).Should().BeFalse();
        }

        [TestMethod]
        public void AddGlobalParameterThrowsIfParameterNull()
        {
            var target = new QueryBatch();
            Action test1 = () => target.AddGlobalParameter(null);
            test1.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void AddGlobalParameterThrowsIfSourceColumnNotSet()
        {
            var target = new QueryBatch();
            Action test1 = () => target.AddGlobalParameter(new SqlParameter());
            test1.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void QueryBatchHandlesNullParameters()
        {
            var testParameters1 = new SqlParameter[]
            {
                new SqlParameter("@p1", 1),
                new SqlParameter("@p2", 2),
                new SqlParameter("@p3", 3),
                new SqlParameter("@p4", 4),
            };

            SqlParameter[] testParameters2 = null;

            var target = new QueryBatch(1);
            target.Add(new QueryCommand ("TEST1", testParameters1, true));
            target.Add(new QueryCommand ("TESTXYZ", testParameters2, true));

            var result1 = target.NextBatch();
            result1.CommandText.Should().Be("TEST1");
            result1.GetParameters().GetLength(0).Should().Be(4);

            var result2 = target.NextBatch();
            result2.CommandText.Should().Be("TESTXYZ");
            result2.GetParameters().GetLength(0).Should().Be(0);
        }

        [TestMethod]
        public void QueryBatchCanInitializeWithBatchSizeLessThan1()
        {
            Action test = () => new QueryBatch(-1);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void QueryBatchCanInitializeWithBatchGreaterThanMaxStatement()
        {
            Action test = () => new QueryBatch(QueryBatch.MaxAllowedStatements + 1);
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void QueryBatchWithParametersGreaterThanMaxThrowsIfCannotSplit()
        {
            var paramCount = ((int)QueryBatch.MaxAllowedParameters / 100) + 1;
            var batch = new QueryBatch(100);

            for (int i = 0; i < 100; i++)
            {
                var paramList = new SqlParameter[paramCount];
                for (int j = 0; j < paramCount; j++)
                {
                    paramList[j] = new SqlParameter();
                }

                var qc = new QueryCommand("test", paramList);
                batch.Add(qc, false);
            }

            Action test = () => batch.NextBatch();
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void QueryBatchWithParametersGreaterThanMaxSplitsWithNoNullParameters()
        {
            var paramCount = ((int)QueryBatch.MaxAllowedParameters / 100) + 1;
            var batch = new QueryBatch(100);

            for (int i = 0; i < 100; i++)
            {
                var paramList = new SqlParameter[paramCount];
                for (int j = 0; j < paramCount; j++)
                {
                    paramList[j] = new SqlParameter();
                }

                var qc = new QueryCommand("test", paramList);
                batch.Add(qc, true);
            }

            var command = batch.NextBatch();
            var parameters = command.GetParameters();

            for (int i = 0; i < parameters.GetLength(0); i++)
            {
                parameters[i].Should().NotBeNull();
            }
        }

        [TestMethod]
        public void QueryBatchWithParametersGreaterThanMaxDoesNotThrowIfCanSplit()
        {
            var paramCount = ((int)QueryBatch.MaxAllowedParameters / 100) + 1;
            var batch = new QueryBatch(100);

            for (int i = 0; i < 100; i++)
            {
                var paramList = new SqlParameter[paramCount];
                for (int j = 0; j < paramCount; j++)
                {
                    paramList[j] = new SqlParameter();
                }

                var qc = new QueryCommand("test", paramList);
                batch.Add(qc, true);
            }

            Action test = () => batch.NextBatch();
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void QueryBatchWithParametersGreaterThanMaxSplitsAsExpected()
        {
            var paramCount = ((int)QueryBatch.MaxAllowedParameters / 100) + 1;
            var batch = new QueryBatch(100);

            for (int i = 0; i < 100; i++)
            {
                var paramList = new SqlParameter[paramCount];
                for (int j = 0; j < paramCount; j++)
                {
                    paramList[j] = new SqlParameter();
                }

                var qc = new QueryCommand("test", paramList);
                batch.Add(qc, true);
            }

            batch.NextBatch().CommandText.Should().NotBeNullOrEmpty();
            batch.NextBatch().CommandText.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void QueryBatchWithStatementsGreaterThanBatchSizeSplitsAsExpectedWhenCannotSplit()
        {
            var batch = new QueryBatch(100);
            for (int i = 0; i < 200; i++)
            {
                var qc = new QueryCommand("test", new SqlParameter[] { new SqlParameter() });
                batch.Add(qc, false);
            }

            batch.NextBatch().CommandText.Should().NotBeNullOrEmpty();
            batch.HasBatch.Should().BeFalse();
        }

        [TestMethod]
        public void QueryBatchWithStatementsGreaterThanMaxThrowsWhenCannotSplit()
        {
            var batch = new QueryBatch();
            for (int i = 0; i < QueryBatch.MaxAllowedStatements + 1; i++)
            {
                var qc = new QueryCommand("test", new SqlParameter[] { new SqlParameter() });
                batch.Add(qc, false);
            }

            Action test = () => batch.NextBatch();
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void NextBatchReturnsNullForEmptyBatch()
        {
            var target = new QueryBatch();
            target.NextBatch().Should().Be(QueryCommand.Empty);
        }

        [TestMethod]
        public void HasBatchReturnsFalseForEmptyBatch()
        {
            var target = new QueryBatch();
            target.HasBatch.Should().BeFalse();
        }

        [TestMethod]
        public void EmptyReaderBatchHasNullCurrentResult()
        {
            var target = new QueryBatch(20);
            (target.CurrentResult == null).Should().BeTrue();
        }

        [TestMethod]
        public void QueryBatchIncludesGlobals()
        {
            var testParameters1 = new SqlParameter[]
            {
                new SqlParameter("@p1", 1),
                new SqlParameter("@p2", 2),
                new SqlParameter("@p3", 3),
                new SqlParameter("@p4", 4),
            };

            var testParameters2 = new SqlParameter[]
            {
                new SqlParameter("@ps1", "someKey"),
            };

            var target = new QueryBatch(1);
            target.Add(new QueryCommand ("TEST1", testParameters1, true));
            target.Add(new QueryCommand("TESTXYZ", testParameters2, true));

            var global = new SqlParameter("@global", "test");
            global.SourceColumn = "TestColumn";

            target.AddGlobalParameter(global);

            var result1 = target.NextBatch();
            result1.CommandText.Should().Be("TEST1");
            result1.GetParameters().GetLength(0).Should().Be(5);

            var result2 = target.NextBatch();
            result2.CommandText.Should().Be("TESTXYZ");
            result2.GetParameters().GetLength(0).Should().Be(2);
        }

        [TestMethod]
        public void SmallQueryBatchCorrectlyReallocatesAndReturnsExpectedResults()
        {
            var testParameters1 = new SqlParameter[]
            {
                new SqlParameter("@p1", 1),
                new SqlParameter("@p2", 2),
                new SqlParameter("@p3", 3),
                new SqlParameter("@p4", 4),
            };

            var testParameters2 = new SqlParameter[]
            {
                new SqlParameter("@ps1", "someKey"),
            };

            var target = new QueryBatch(1);
            target.Add(new QueryCommand ("TEST1", testParameters1, true));
            target.Add(new QueryCommand ("TESTXYZ", testParameters2, true));

            var result1 = target.NextBatch();
            result1.CommandText.Should().Be("TEST1");
            result1.GetParameters().GetLength(0).Should().Be(4);

            var result2 = target.NextBatch();
            result2.CommandText.Should().Be("TESTXYZ");
            result2.GetParameters().GetLength(0).Should().Be(1);
        }

        [TestMethod]
        public void QueryBatchCorrectlyReallocatesAndReturnsExpectedResultsWithRemainder()
        {
            var testParameters1 = new SqlParameter[]
            {
                new SqlParameter("@p1", 1),
                new SqlParameter("@p2", 2),
                new SqlParameter("@p3", 3),
                new SqlParameter("@p4", 4),
            };

            var testParameters2 = new SqlParameter[]
            {
                new SqlParameter("@ps1", "someKey"),
            };

            var target = new QueryBatch(3);
            target.Add(new QueryCommand ("TEST1;", testParameters1 ));
            target.Add(new QueryCommand ("TESTXYZ;", testParameters2));
            target.Add(new QueryCommand ("TEST1;", testParameters1, true ));

            target.Add(new QueryCommand ("TESTXYZ;", testParameters2));
            target.Add(new QueryCommand ("TEST1;", testParameters1 ));
            target.Add(new QueryCommand ("TESTXYZ;", testParameters2, true));

            target.Add(new QueryCommand("TEST1;", testParameters1));
            target.Add(new QueryCommand ("TESTXYZ;", testParameters2));

            var result1 = target.NextBatch();
            result1.CommandText.Should().Be("TEST1;TESTXYZ;TEST1;");
            result1.GetParameters().GetLength(0).Should().Be(9);

            var result2 = target.NextBatch();
            result2.CommandText.Should().Be("TESTXYZ;TEST1;TESTXYZ;");
            result2.GetParameters().GetLength(0).Should().Be(6);

            var result3 = target.NextBatch();
            result3.CommandText.Should().Be("TEST1;TESTXYZ;");
            result3.GetParameters().GetLength(0).Should().Be(5);
        }

        [TestMethod]
        public void QueryBatchReturnsExpectedResultsWhenBatchLessThanBatchMax()
        {
            var testParameters1 = new SqlParameter[]
            {
                new SqlParameter("@p1", 1),
                new SqlParameter("@p2", 2),
                new SqlParameter("@p3", 3),
                new SqlParameter("@p4", 4),
            };

            var testParameters2 = new SqlParameter[]
            {
                new SqlParameter("@ps1", "someKey"),
            };

            var target = new QueryBatch(25);
            target.Add(new QueryCommand("TEST1;", testParameters1));
            target.Add(new QueryCommand("TESTXYZ;", testParameters2));
            target.Add(new QueryCommand("TEST1;", testParameters1, true));

            target.Add(new QueryCommand("TESTXYZ;", testParameters2));
            target.Add(new QueryCommand("TEST1;", testParameters1));
            target.Add(new QueryCommand("TESTXYZ;", testParameters2, true));

            target.Add(new QueryCommand("TEST1;", testParameters1));
            target.Add(new QueryCommand("TESTXYZ;", testParameters2));

            var result1 = target.NextBatch();
            result1.CommandText.Should().Be("TEST1;TESTXYZ;TEST1;TESTXYZ;TEST1;TESTXYZ;TEST1;TESTXYZ;");
            result1.GetParameters().GetLength(0).Should().Be(20);
        }

        [TestMethod]
        public void QueryReaderBatchReturnsExpectedResultsWhenBatchLessThanBatchMax()
        {
            var testParameters1 = new SqlParameter[]
            {
                new SqlParameter("@p1", 1),
                new SqlParameter("@p2", 2),
                new SqlParameter("@p3", 3),
                new SqlParameter("@p4", 4),
            };

            var testParameters2 = new SqlParameter[]
            {
                new SqlParameter("@ps1", "someKey"),
            };

            var target = new QueryBatch(25);
            target.Add<string>(new QueryCommand("TEST1;", testParameters1));
            target.Add<object>(new QueryCommand("TESTXYZ;", testParameters2));
            target.Add<int>(new QueryCommand("TEST1;", testParameters1));
            target.Add<short>(new QueryCommand("TESTXYZ;", testParameters2));
            target.Add<long>(new QueryCommand("TEST1;", testParameters1));
            target.Add<string>(new QueryCommand("TESTXYZ;", testParameters2));
            target.Add<object>(new QueryCommand("TEST1;", testParameters1));
            target.Add<string>(new QueryCommand("TESTXYZ;", testParameters2));

            target.ExpectedResultSetCount.Should().Be(8);

            var result1 = target.NextBatch();
            result1.CommandText.Should().Be("TEST1;TESTXYZ;TEST1;TESTXYZ;TEST1;TESTXYZ;TEST1;TESTXYZ;");
            result1.GetParameters().GetLength(0).Should().Be(20);

            target.HasResult.Should().BeTrue();
            target.CurrentResult.Should().Be(typeof(string));
            target.MoveToNextResultType().Should().BeTrue();

            target.HasResult.Should().BeTrue();
            target.CurrentResult.Should().Be(typeof(object));
            target.MoveToNextResultType().Should().BeTrue();

            target.HasResult.Should().BeTrue();
            target.CurrentResult.Should().Be(typeof(int));
            target.MoveToNextResultType().Should().BeTrue();

            target.HasResult.Should().BeTrue();
            target.CurrentResult.Should().Be(typeof(short));
            target.MoveToNextResultType().Should().BeTrue();

            target.HasResult.Should().BeTrue();
            target.CurrentResult.Should().Be(typeof(long));
            target.MoveToNextResultType().Should().BeTrue();

            target.HasResult.Should().BeTrue();
            target.CurrentResult.Should().Be(typeof(string));
            target.MoveToNextResultType().Should().BeTrue();

            target.HasResult.Should().BeTrue();
            target.CurrentResult.Should().Be(typeof(object));
            target.MoveToNextResultType().Should().BeTrue();

            target.HasResult.Should().BeTrue();
            target.CurrentResult.Should().Be(typeof(string));
            target.MoveToNextResultType().Should().BeFalse();
        }

        [TestMethod]
        public void QueryBatchReturnsExpectedResultsWhenBatchLessThanBatchMaxWithOneParameter()
        {
            var testParameters1 = new SqlParameter[]
            {
                new SqlParameter("@p1", 1),
            };

            var testParameters2 = new SqlParameter[]
            {
                new SqlParameter("@ps1", "someKey"),
            };

            var target = new QueryBatch(25);
            target.Add(new QueryCommand("TEST1;", testParameters1));
            target.Add(new QueryCommand("TESTXYZ;", testParameters2));
            target.Add(new QueryCommand("TEST1;", testParameters1));
            target.Add(new QueryCommand("TESTXYZ;", testParameters2));
            target.Add(new QueryCommand("TEST1;", testParameters1));
            target.Add(new QueryCommand("TESTXYZ;", testParameters2));
            target.Add(new QueryCommand("TEST1;", testParameters1));
            target.Add(new QueryCommand("TESTXYZ;", testParameters2));

            var result1 = target.NextBatch();
            result1.CommandText.Should().Be("TEST1;TESTXYZ;TEST1;TESTXYZ;TEST1;TESTXYZ;TEST1;TESTXYZ;");
            result1.GetParameters().GetLength(0).Should().Be(8);
        }
    }
}
