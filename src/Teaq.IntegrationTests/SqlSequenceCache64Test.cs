using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.KeyGeneration;

namespace Teaq.IntegrationTests
{
    [TestClass]
    public class SqlSequenceCache64Test
    {
        [TestMethod]
        public void SqlSequenceCache64DoesNotDuplicateWithMinStart()
        {
            var testSeq = this.CreateProvider();
            using (var target = new SequenceCache64(testSeq, 50))
            {
                var ids = new HashSet<long>();
                for (var i = 0; i < 300; i++)
                {
                    var value = target.NextId();
                    ids.Add(value).Should().BeTrue("iteration {0} resulted in a duplicate value {1}", i, value);
                }
            }
        }

        [TestMethod]
        public void SqlSequenceCache64DoesNotDuplicateWith1Start()
        {
            var testSeq = this.CreateProvider();
            var target = new SequenceCache64(testSeq, 50);

            var ids = new HashSet<long>();
            for (var i = 0; i < 300; i++)
            {
                var value = target.NextId();
                ids.Add(value).Should().BeTrue("iteration {0} resulted in a duplicate value {1}", i, value);
            }
        }

        [TestMethod]
        public void SqlSequenceCache64DoesNotDuplicateWithCompetingThreads()
        {
            var testSeq = this.CreateProvider();
            var target = new SequenceCache64(testSeq, 50);
            var sets = new ConcurrentBag<HashSet<long>>();

            ThreadStart action = () =>
                {
                    var ids = new HashSet<long>();
                    for (var i = 0; i < 300; i++)
                    {
                        var value = target.NextId();
                        ids.Add(value).Should().BeTrue("iteration {0} resulted in a duplicate value {1}", i, value);
                    }

                    sets.Add(ids);
                };

            Thread[] tasks = new Thread[15];
            for(int i = 0; i < 15; i++)
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

            // merge all the hashsets on each thread and ensure they are all unique:
            sets.Aggregate((s1, s2) =>
            {
                s1.Intersect(s2).Any().Should().BeFalse();
                s2.UnionWith(s1);
                return s2;
            });
        }

        private SqlSequenceProvider<long> CreateProvider()
        {
            return new SqlSequenceProvider<long>(
                ConfigurationManager.ConnectionStrings["TeaqTestDb"].ConnectionString,
                "dbo.Int64Sequence");
        }
    }
}
