using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Teaq.KeyGeneration;
using Teaq.Tests.KeyGeneration.SupportingTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;

namespace Teaq.Tests.KeyGeneration
{
    [TestClass]
    public class SequenceCache32Test
    {
        [TestMethod]
        public void SequenceCache32ThrowsInvalidArgumentOutOfRangeExceptionWithNegativeCache()
        {
            var testSeq = new InMemorySequenceProvider32(int.MinValue);
            Action test = () => new SequenceCache32(testSeq, -1);
            test.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void SequenceCache32ThrowsInvalidArgumentOutOfRangeExceptionWithZeroCache()
        {
            var testSeq = new InMemorySequenceProvider32(int.MinValue);
            Action test = () => new SequenceCache32(testSeq, 0);
            test.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void SequenceCache32DoesNotFailWithCachSizeOf1()
        {
            var testSeq = new InMemorySequenceProvider32(int.MinValue);
            var target = new SequenceCache32(testSeq, 1);

            var ids = new HashSet<int>();
            for (var i = 0; i < 300; i++)
            {
                var value = target.NextId();
                ids.Add(value).Should().BeTrue("iteration {0} resulted in a duplicate value {1}", i, value);
            }
        }

        [TestMethod]
        public void SequenceCache32DoesNotDuplicateWithMinStart()
        {
            var testSeq = new InMemorySequenceProvider32(int.MinValue);
            var target = new SequenceCache32(testSeq, 50);

            var ids = new HashSet<int>();
            for(var i = 0; i < 300; i++)
            {
                var value = target.NextId();
                ids.Add(value).Should().BeTrue("iteration {0} resulted in a duplicate value {1}", i, value);
            }
        }

        [TestMethod]
        public void SequenceCache32DoesNotDuplicateWith1Start()
        {
            var testSeq = new InMemorySequenceProvider32(1);
            var target = new SequenceCache32(testSeq, 50);

            var ids = new HashSet<int>();
            for (var i = 0; i < 300; i++)
            {
                var value = target.NextId();
                ids.Add(value).Should().BeTrue("iteration {0} resulted in a duplicate value {1}", i, value);
            }
        }

        [TestMethod]
        public void SequenceCache32DoesNotDuplicateWithCompetingThreads()
        {
            var testSeq = new InMemorySequenceProvider32(1, 5);
            var target = new SequenceCache32(testSeq, 50);

            var sets = new ConcurrentBag<HashSet<int>>();

            ThreadStart action = () =>
            {
                var ids = new HashSet<int>();
                for (var i = 0; i < 300; i++)
                {
                    var value = target.NextId();
                    ids.Add(value).Should().BeTrue("iteration {0} resulted in a duplicate value {1}", i, value);
                }

                sets.Add(ids);
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

            // merge all the hashsets on each thread and ensure they are all unique:
            sets.Aggregate((s1, s2) =>
            {
                s1.Intersect(s2).Any().Should().BeFalse();
                s2.UnionWith(s1);
                return s2;
            });
        }

        [TestMethod]
        public void SequenceCach32InvokesProviderDispose()
        {
            var testSeq = new InMemorySequenceProvider32(1, 5);
            using (var target = new SequenceCache32(testSeq, 50))
            {
                testSeq.DisposeInvoked.Should().BeFalse();
            }

            testSeq.DisposeInvoked.Should().BeTrue();
        }
    }
}
