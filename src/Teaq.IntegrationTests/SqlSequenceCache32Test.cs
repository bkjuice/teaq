using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.KeyGeneration;

namespace Teaq.IntegrationTests
{
    [TestClass]
    public class SqlSequenceCache32Test
    {
        [TestMethod]
        public void SqlSequenceCache32DoesNotDuplicateWithMinStart()
        {
            var testSeq = this.CreateProvider();
            using (var target = new SequenceCache32(testSeq, 50))
            {
                var ids = new HashSet<int>();
                for (var i = 0; i < 300; i++)
                {
                    var value = target.NextId();
                    ids.Add(value).Should().BeTrue("iteration {0} resulted in a duplicate value {1}", i, value);
                }
            }
        }

        [TestMethod]
        public void SqlSequenceCache32DoesNotDuplicateWith1Start()
        {
            var testSeq = this.CreateProvider();
            var target = new SequenceCache32(testSeq, 50);

            var ids = new HashSet<int>();
            for (var i = 0; i < 300; i++)
            {
                var value = target.NextId();
                ids.Add(value).Should().BeTrue("iteration {0} resulted in a duplicate value {1}", i, value);
            }
        }

        [TestMethod]
        public void SqlSequenceCache32DoesNotDuplicateWithCompetingThreads()
        {
            var testSeq = this.CreateProvider();
            var target = new SequenceCache32(testSeq, 50);

            var ids = new HashSet<int>();

            ThreadStart action = () =>
            {
                for (var i = 0; i < 300; i++)
                {
                    var value = target.NextId();
                    ids.Add(value).Should().BeTrue("iteration {0} resulted in a duplicate value {1}", i, value);
                }
            };

            Thread[] tasks = new Thread[15];
            for (int i = 0; i < 15; i++)
            {
                tasks[i] = new Thread(action);
            }

            for (int i = 0; i < 15; i++)
            {
                tasks[i].Start();
            }

            for (int i = 0; i < 15; i++)
            {
                tasks[i].Join();
            }
        }

        private SqlSequenceProvider<int> CreateProvider()
        {
            return new SqlSequenceProvider<int>(
                ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString,
                "dbo.Int32Sequence");
        }
    }
}
