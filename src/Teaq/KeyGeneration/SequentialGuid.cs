using System;

namespace Teaq.KeyGeneration
{
    /// <summary>
    /// Sponsor class to support GUID sequencing.
    /// </summary>
    public static class SequentialGuid
    {
        /// <summary>
        /// The version for the sequenced GUID.
        /// </summary>
        private const byte Version = (byte)0xC0;

        /// <summary>
        /// The version; 7th byte 1st nibbble, GUID version.
        /// </summary>
        private const byte VersionMask = (byte)0xF0;

        /// <summary>
        /// Creates a new unique identifier that is sequenced for SQL Server collation.
        /// </summary>
        /// <returns>The sequenced GUID.</returns>
        public static Guid NewGuid()
        {
            var s = NativeMethods.NewRpcSequencedGuid().ToByteArray();
            var time = BitConverter.GetBytes(DateTimeOffset.UtcNow.ToSequenceValue());
            var t = new byte[16];

            // GUID array byte ordering relative to how the string is presented is as follows:
            // 03 02 01 00 - 05 04 - 07 06 - 08 09 - 0A 0B 0C 0D 0E 0F
            // These values are inverted when passed to the GUID, in respective groupings:
            t[3] = s[0];
            t[2] = s[1];
            t[1] = s[2];
            t[0] = s[3];

            t[5] = s[4];
            t[4] = s[5];

            // The 7th byte 1st nibble is the algorithm version. These values are not being inverted, to preserve 4 bits of entropy.
            t[6] = s[6];
            t[7] = (byte)(Version | (s[7] & ~VersionMask));
           
            t[8] = s[8];
            t[9] = s[9];

            // For the sequence value (long, 8 bytes), index 7 is high order value, 0 is low order value; using 6 low order bytes.
            // This replaces the MAC Adddress with an ever increasing timestamp to ensure blocks of GUIDs aren't grouped by the generating server:
            t[10] = time[5];
            t[11] = time[4];
            t[12] = time[3];
            t[13] = time[2];
            t[14] = time[1];
            t[15] = time[0];

            return new Guid(t);
        }

        /// <summary>
        /// Gets the GUID algorithm version.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Aa byte representing the GUID algorithm version.</returns>
        public static byte AlgorithmVersion(this Guid value)
        {
            return (byte)((value.ToByteArray()[7] & VersionMask) >> 4);
        }

        /// <summary>
        /// Determines whether the GUID is generated using sequence version A.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if the GUID version is '10' (or 0xA).</returns>
        public static bool IsVersionC(this Guid value)
        {
            return value.AlgorithmVersion() == Version >> 4;
        }

        /// <summary>
        /// Calculates a sequence time value that can fit 6 bytes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The time value as a 64 bit integer.
        /// </returns>
        internal static long ToSequenceValue(this DateTimeOffset value)
        {
            // base 10 is not ideal but saves 1 byte. DayOfYear could be used but is difficult to reverse.
            var utc = value.UtcDateTime;
            return
                (utc.Year * 10000000000) + (utc.Month * 100000000) + (utc.Day * 1000000) + (utc.Hour * 10000) + (utc.Minute * 100) + utc.Second;
        }

        /// <summary>
        /// Converts a 6 byte sequence time to a date time offset value.
        /// </summary>
        /// <param name="sequence">The sequence time value.</param>
        /// <returns>
        /// The date time offset value represented by the time sequence value.
        /// </returns>
        internal static DateTimeOffset ToSequenceDate(this long sequence)
        {
            var year = sequence / 10000000000;
            sequence -= year * 10000000000;
            var month = sequence / 100000000;
            sequence -= month * 100000000;
            var day = sequence / 1000000;
            sequence -= day * 1000000;
            var hour = sequence / 10000;
            sequence -= hour * 10000;
            var minute = sequence / 100;
            var second = sequence - (minute * 100);

            return new DateTimeOffset(
                new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, (int)second), TimeSpan.Zero);
        }
    }
}
