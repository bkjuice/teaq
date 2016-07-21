using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.KeyGeneration;

namespace Teaq.Tests.KeyGeneration
{
    [TestClass]
    public class SequentialGuidTests
    {
        [TestMethod]
        public void BCLSanityEndianessIsAsExpected()
        {
            var value = 0x0100000000000000;
            var data = BitConverter.GetBytes(value);
            
            // 7 is high order value, 0 is low order value.
            data[7].Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void BCLSanityGuidByteOrderingIsAsExpetected()
        {
            var data = new byte[16];
            data[0] = 1;
            data[1] = 2;
            data[2] = 3;
            data[3] = 4;
            data[4] = 5;
            data[5] = 6;
            data[6] = 7;
            data[7] = 8;
            data[8] = 9;
            data[9] = 10;
            data[10] = 11;
            data[11] = 12;
            data[12] = 13;
            data[13] = 14;
            data[14] = 15;
            data[15] = 16;

            /*
             * bytes 0-7 are reversed in respective groupings.
             * bytes 8-15 appear in array order.
             */ 
            new Guid(data).ToString().Should().Be("04030201-0605-0807-090a-0b0c0d0e0f10");
        }

        [TestMethod]
        public void GuidVersionIsAsExpected()
        {
            Guid.NewGuid().AlgorithmVersion().Should().Be(4);
            SequentialGuid.NewGuid().AlgorithmVersion().Should().Be(0x0C);
        }

        [TestMethod]
        public void CaculateSequenceReturnsValueWithin6ByteRange()
        {
            var max = 0xFFFFFFFFFFFF;
            DateTimeOffset.UtcNow.ToSequenceValue().Should().BeLessThan(max);
        }

        [TestMethod]
        public void CalculateSequenceValueAndCalculateSequenceAgereSymmetrical()
        {
            var target = new DateTimeOffset(new DateTime(2014, 3, 20, 7, 45, 32), TimeSpan.Zero);
            target.ToSequenceValue()
                .ToSequenceDate().Should().Be(target);
        }

        [TestMethod]
        public void IsVersionCIsTrueForSequencedGuid()
        {
            SequentialGuid.NewGuid().IsVersionC().Should().BeTrue();
        }

        [TestMethod]
        public void IsVersionCIsFalseForDefaultGuid()
        {
            Guid.NewGuid().IsVersionC().Should().BeFalse();
        }

        [TestMethod]
        public void IsVersionCIsFalseForEmptyuid()
        {
            Guid.Empty.IsVersionC().Should().BeFalse();
        }

        [TestMethod]
        public void Sort10SequentialGuids()
        {
            var g = new Guid[10];

            g[9] = SequentialGuid.NewGuid();
            Thread.MemoryBarrier();
            g[8] = SequentialGuid.NewGuid();
            Thread.MemoryBarrier();
            g[7] = SequentialGuid.NewGuid();
            Thread.MemoryBarrier();
            g[1] = SequentialGuid.NewGuid();
            Thread.MemoryBarrier();
            g[2] = SequentialGuid.NewGuid();
            Thread.MemoryBarrier();
            g[0] = SequentialGuid.NewGuid();
            Thread.MemoryBarrier();
            g[5] = SequentialGuid.NewGuid();
            Thread.MemoryBarrier();
            g[4] = SequentialGuid.NewGuid();
            Thread.MemoryBarrier();
            g[3] = SequentialGuid.NewGuid();
            Thread.MemoryBarrier();
            g[6] = SequentialGuid.NewGuid();

            var sorted = new List<Guid>(g);
            sorted.Sort(SqlServerGuidComparer.Default);

            Assert.AreEqual(g[9], sorted[0]);
            Assert.AreEqual(g[8], sorted[1]);
            Assert.AreEqual(g[7], sorted[2]);
            Assert.AreEqual(g[1], sorted[3]);
            Assert.AreEqual(g[2], sorted[4]);
            Assert.AreEqual(g[0], sorted[5]);
            Assert.AreEqual(g[5], sorted[6]);
            Assert.AreEqual(g[4], sorted[7]);
            Assert.AreEqual(g[3], sorted[8]);
            Assert.AreEqual(g[6], sorted[9]);
        }

        private class SqlServerGuidComparer : IComparer<Guid>
        {
            public static readonly SqlServerGuidComparer Default = new SqlServerGuidComparer();
            public int Compare(Guid x, Guid y)
            {
                /*
             *  •0..3 are evaluated in left to right order and are the less important, then
                •4..5 are evaluated in left to right order, then
                •6..7 are evaluated in left to right order, then
                •8..9 are evaluated in right to left order, then
                •A..F are evaluated in right to left order and are the most important
            */
                var xBytes = x.ToByteArray();
                var yBytes = y.ToByteArray();

                int result;
                if (!TryCompareEquals(xBytes[10], yBytes[10], out result)) return result;
                if (!TryCompareEquals(xBytes[11], yBytes[11], out result)) return result;
                if (!TryCompareEquals(xBytes[12], yBytes[12], out result)) return result;
                if (!TryCompareEquals(xBytes[13], yBytes[13], out result)) return result;
                if (!TryCompareEquals(xBytes[14], yBytes[14], out result)) return result;
                if (!TryCompareEquals(xBytes[15], yBytes[15], out result)) return result;

                if (!TryCompareEquals(xBytes[9], yBytes[9], out result)) return result;
                if (!TryCompareEquals(xBytes[8], yBytes[8], out result)) return result;

                if (!TryCompareEquals(xBytes[7], yBytes[7], out result)) return result;
                if (!TryCompareEquals(xBytes[6], yBytes[6], out result)) return result;

                if (!TryCompareEquals(xBytes[5], yBytes[5], out result)) return result;
                if (!TryCompareEquals(xBytes[4], yBytes[4], out result)) return result;

                if (!TryCompareEquals(xBytes[3], yBytes[3], out result)) return result;
                if (!TryCompareEquals(xBytes[2], yBytes[2], out result)) return result;
                if (!TryCompareEquals(xBytes[1], yBytes[1], out result)) return result;
                if (!TryCompareEquals(xBytes[0], yBytes[0], out result)) return result;

                return 0;
            }

            private static bool TryCompareEquals(byte x, byte y, out int value)
            {
                value = x.CompareTo(y);
                return value == 0;
            }
        }

        #region Debug Method for output to use in SSMS to Test SQL Server Collation
        
        //[TestMethod]
        //public void WriteGuidsToFile()
        //{
        //    #region SQL Sample
        //    /*
        //      * Sample query:
        //          With UIDs As (                        
        //             Select ID =  1, UID = cast ('3e78f36c-ec98-c1e3-9c03-1251428d8a23' as uniqueidentifier)
        //              Union   Select ID =  2, UID = cast ('3f78f36c-ec98-c1e3-9c03-1251428d8a23' as uniqueidentifier)
        //              Union   Select ID =  3, UID = cast ('4078f36c-ec98-c1e3-9c03-1251428d8a23' as uniqueidentifier)
        //              Union   Select ID =  4, UID = cast ('4178f36c-ec98-c1e3-9c03-1251428d8a23' as uniqueidentifier)
        //              Union   Select ID =  5, UID = cast ('4378f36c-ec98-c1e3-9c03-1251428d8a23' as uniqueidentifier)
        //              Union   Select ID =  6, UID = cast ('4478f36c-ec98-c1e3-9c03-1251428d8a24' as uniqueidentifier)
        //              Union   Select ID =  7, UID = cast ('4578f36c-ec98-c1e3-9c03-1251428d8a24' as uniqueidentifier)
        //              Union   Select ID =  8, UID = cast ('4778f36c-ec98-c1e3-9c03-1251428d8a24' as uniqueidentifier)
        //              Union   Select ID =  9, UID = cast ('4878f36c-ec98-c1e3-9c03-1251428d8a24' as uniqueidentifier)
        //              Union   Select ID = 10, UID = cast ('4978f36c-ec98-c1e3-9c03-1251428d8a24' as uniqueidentifier)
        //              Union   Select ID = 11, UID = cast ('4a78f36c-ec98-c1e3-9c03-1251428d8a24' as uniqueidentifier)
        //              Union   Select ID = 12, UID = cast ('4b78f36c-ec98-c1e3-9c03-1251428d8a24' as uniqueidentifier)
        //              Union   Select ID = 13, UID = cast ('4c78f36c-ec98-c1e3-9c03-1251428d8a25' as uniqueidentifier)
        //              Union   Select ID = 14, UID = cast ('4d78f36c-ec98-c1e3-9c03-1251428d8a25' as uniqueidentifier)
        //              Union   Select ID = 15, UID = cast ('5678f36c-ec98-c1e3-9c03-1251428d8a26' as uniqueidentifier)
        //              Union   Select ID = 16, UID = cast ('5778f36c-ec98-c1e3-9c03-1251428d8a26' as uniqueidentifier)
        //          )
        //          Select * From UIDs Order By UID, ID
        //    */
        //    #endregion

        //    using (var f = System.IO.File.Create(@"E:\Guids_" + Guid.NewGuid().ToString() + ".txt"))
        //    using (var s = new System.IO.StreamWriter(f))
        //    {
        //        for (var i = 0; i < 100; ++i)
        //        {
        //            s.WriteLine(SequentialGuid.NewGuid().ToString());
        //            if (i % 2 == 0)
        //            {
        //                System.Threading.Thread.Sleep(250);
        //            }
        //        }

        //        s.Flush();
        //        f.Flush();
        //    }
        //}

        #endregion
    }
}
