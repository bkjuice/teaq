using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Teaq
{
    /// <summary>
    /// Class patial to aid in efficient, well-known type conversions.
    /// </summary>
    public static partial class DataReaderExtensions
    {
        private static readonly Dictionary<TypePair, Func<object, object>> Converters =
            new Dictionary<TypePair, Func<object, object>>(256, new TypePairComparer())
            {
                [new TypePair(T<bool>(), T<bool?>())] = o => new bool?((bool)o),
                [new TypePair(T<byte>(), T<byte?>())] = o => new byte?((byte)o), // Add all integer conversions
                [new TypePair(T<char>(), T<char?>())] = o => new char?((char)o),
                [new TypePair(T<DateTime>(), T<DateTime?>())] = o => (DateTime?)(DateTime)o,
                [new TypePair(T<DateTime>(), T<DateTimeOffset>())] = o => new DateTimeOffset((DateTime)o),
                [new TypePair(T<DateTime>(), T<DateTimeOffset?>())] = o => new DateTimeOffset?((DateTime)o),
                [new TypePair(T<DateTimeOffset>(), T<DateTimeOffset?>())] = o => new DateTimeOffset?((DateTimeOffset)o),
                [new TypePair(T<DateTimeOffset>(), T<DateTime>())] = o => ((DateTimeOffset)o).DateTime,
                [new TypePair(T<DateTimeOffset>(), T<DateTime?>())] = o => new DateTime?(((DateTimeOffset)o).DateTime),
                [new TypePair(T<decimal>(), T<decimal?>())] = o => new decimal?((decimal)o),
                [new TypePair(T<decimal>(), T<long>())] = o => unchecked((long)(decimal)o),
                [new TypePair(T<decimal>(), T<long?>())] = o => new long?(unchecked((long)(decimal)o)),
                [new TypePair(T<decimal>(), T<int>())] = o => unchecked((int)(decimal)o),
                [new TypePair(T<decimal>(), T<int?>())] = o => new int?(unchecked((int)(decimal)o)),
                [new TypePair(T<decimal>(), T<short>())] = o => unchecked((short)(decimal)o),
                [new TypePair(T<decimal>(), T<short?>())] = o => new short?(unchecked((short)(decimal)o)),
                [new TypePair(T<decimal>(), T<byte>())] = o => unchecked((byte)(decimal)o),
                [new TypePair(T<decimal>(), T<byte?>())] = o => new byte?(unchecked((byte)(decimal)o)),
                [new TypePair(T<double>(), T<double?>())] = o => new double?((double)o),
                [new TypePair(T<float>(), T<float?>())] = o => new float?((float)o),
                [new TypePair(T<Guid>(), T<Guid?>())] = o => new Guid?((Guid)o),
                [new TypePair(T<int>(), T<decimal>())] = o => (decimal)(int)o,
                [new TypePair(T<int>(), T<decimal?>())] = o => new decimal?((int)o),
                [new TypePair(T<int>(), T<long>())] = o => (long)(int)o,
                [new TypePair(T<int>(), T<long?>())] = o => new long?((int)o),
                [new TypePair(T<int>(), T<int?>())] = o => new int?((int)o),
                [new TypePair(T<int>(), T<short>())] = o => unchecked((short)(int)o),
                [new TypePair(T<int>(), T<short?>())] = o => new short?(unchecked((short)(int)o)),
                [new TypePair(T<int>(), T<byte>())] = o => unchecked((byte)(int)o),
                [new TypePair(T<int>(), T<byte?>())] = o => new byte?(unchecked((byte)(int)o)),
                [new TypePair(T<long>(), T<decimal>())] = o => (decimal)(long)o,
                [new TypePair(T<long>(), T<decimal?>())] = o => new decimal?((long)o),
                [new TypePair(T<long>(), T<long?>())] = o => new long?((long)o),
                [new TypePair(T<long>(), T<int>())] = o => unchecked((int)(long)o),
                [new TypePair(T<long>(), T<int?>())] = o => new int?(unchecked((int)(long)o)),
                [new TypePair(T<long>(), T<short>())] = o => unchecked((short)(long)o),
                [new TypePair(T<long>(), T<short?>())] = o => new short?(unchecked((short)(long)o)),
                [new TypePair(T<long>(), T<byte>())] = o => unchecked((byte)(long)o),
                [new TypePair(T<long>(), T<byte?>())] = o => new byte?(unchecked((byte)(long)o)),
                [new TypePair(T<short>(), T<decimal>())] = o => (decimal)(short)o,
                [new TypePair(T<short>(), T<decimal?>())] = o => new decimal?((short)o),
                [new TypePair(T<short>(), T<long>())] = o => (long)(short)o,
                [new TypePair(T<short>(), T<long?>())] = o => new long?((short)o),
                [new TypePair(T<short>(), T<int>())] = o => (int)(short)o,
                [new TypePair(T<short>(), T<int?>())] = o => new int?((int)(short)o),
                [new TypePair(T<short>(), T<short?>())] = o => new short?((short)o),
                [new TypePair(T<short>(), T<byte>())] = o => unchecked((byte)(short)o),
                [new TypePair(T<short>(), T<byte?>())] = o => new byte?(unchecked((byte)(short)o)),
                [new TypePair(T<string>(), T<XmlReader>())] = o => CreateReader(o),
                [new TypePair(T<string>(), T<XElement>())] = o => XElement.Load(CreateReader(o)),
                [new TypePair(T<string>(), T<XDocument>())] = o => XDocument.Load(CreateReader(o)),
                [new TypePair(T<string>(), T<XPathDocument>())] = o => new XPathDocument(CreateReader(o)),
                [new TypePair(T<string>(), T<XmlDocument>())] = ConvertStringToXml,
                [new TypePair(T<uint>(), T<decimal>())] = o => (decimal)(uint)o,
                [new TypePair(T<uint>(), T<decimal?>())] = o => new decimal?((uint)o),
                [new TypePair(T<uint>(), T<ulong>())] = o => (ulong)(uint)o,
                [new TypePair(T<uint>(), T<ulong?>())] = o => new ulong?((uint)o),
                [new TypePair(T<uint>(), T<uint?>())] = o => new uint?((uint)o),
                [new TypePair(T<uint>(), T<ushort>())] = o => unchecked((ushort)(uint)o),
                [new TypePair(T<uint>(), T<ushort?>())] = o => new ushort?(unchecked((ushort)(uint)o)),
                [new TypePair(T<uint>(), T<byte>())] = o => unchecked((byte)(uint)o),
                [new TypePair(T<uint>(), T<byte?>())] = o => new byte?(unchecked((byte)(uint)o)),
                [new TypePair(T<ulong>(), T<decimal>())] = o => (decimal)(ulong)o,
                [new TypePair(T<ulong>(), T<decimal?>())] = o => new decimal?((ulong)o),
                [new TypePair(T<ulong>(), T<ulong?>())] = o => new ulong?((ulong)o),
                [new TypePair(T<ulong>(), T<uint>())] = o => unchecked((uint)(ulong)o),
                [new TypePair(T<ulong>(), T<uint?>())] = o => new uint?(unchecked((uint)(ulong)o)),
                [new TypePair(T<ulong>(), T<ushort>())] = o => unchecked((ushort)(ulong)o),
                [new TypePair(T<ulong>(), T<ushort?>())] = o => new ushort?(unchecked((ushort)(ulong)o)),
                [new TypePair(T<ulong>(), T<byte>())] = o => unchecked((byte)(ulong)o),
                [new TypePair(T<ulong>(), T<byte?>())] = o => new byte?(unchecked((byte)(ulong)o)),
                [new TypePair(T<ushort>(), T<decimal>())] = o => (decimal)(ushort)o,
                [new TypePair(T<ushort>(), T<decimal?>())] = o => new decimal?((ushort)o),
                [new TypePair(T<ushort>(), T<ulong>())] = o => (ulong)(ushort)o,
                [new TypePair(T<ushort>(), T<ulong?>())] = o => new ulong?((ushort)o),
                [new TypePair(T<ushort>(), T<uint>())] = o => (uint)(ushort)o,
                [new TypePair(T<ushort>(), T<uint?>())] = o => new uint?((ushort)o),
                [new TypePair(T<ushort>(), T<ushort?>())] = o => new ushort?((ushort)o),
                [new TypePair(T<ushort>(), T<byte>())] = o => unchecked((byte)(ushort)o),
                [new TypePair(T<ushort>(), T<byte?>())] = o => new byte?(unchecked((byte)(ushort)o)),
                [new TypePair(T<SqlBinary>(), T<byte[]>())] = o => ((SqlBinary)o).Value,
                [new TypePair(T<SqlBoolean>(), T<bool>())] = o => ((SqlBoolean)o).Value,
                [new TypePair(T<SqlBoolean>(), T<bool?>())] = o => new bool?(((SqlBoolean)o).Value),
                [new TypePair(T<SqlByte>(), T<byte>())] = o => ((SqlByte)o).Value,
                [new TypePair(T<SqlByte>(), T<byte?>())] = o => new byte?(((SqlByte)o).Value),
                [new TypePair(T<SqlBytes>(), T<byte[]>())] = o => ((SqlBytes)o).Value,
                [new TypePair(T<SqlChars>(), T<char[]>())] = o => ((SqlChars)o).Value,
                [new TypePair(T<SqlDateTime>(), T<DateTime>())] = o => ((SqlDateTime)o).Value,
                [new TypePair(T<SqlDateTime>(), T<DateTime?>())] = o => new DateTime?(((SqlDateTime)o).Value),
                [new TypePair(T<SqlDecimal>(), T<decimal>())] = o => ((SqlDecimal)o).Value,
                [new TypePair(T<SqlDecimal>(), T<decimal?>())] = o => new decimal?(((SqlDecimal)o).Value),
                [new TypePair(T<SqlDouble>(), T<double>())] = o => ((SqlDouble)o).Value,
                [new TypePair(T<SqlDouble>(), T<double?>())] = o => new double?(((SqlDouble)o).Value),
                [new TypePair(T<SqlGuid>(), T<Guid>())] = o => ((SqlGuid)o).Value,
                [new TypePair(T<SqlGuid>(), T<Guid?>())] = o => new Guid?(((SqlGuid)o).Value),
                [new TypePair(T<SqlInt16>(), T<short>())] = o => ((SqlInt16)o).Value,
                [new TypePair(T<SqlInt16>(), T<short?>())] = o => new short?(((SqlInt16)o).Value),
                [new TypePair(T<SqlInt32>(), T<int>())] = o => ((SqlInt32)o).Value,
                [new TypePair(T<SqlInt32>(), T<int?>())] = o => new int?(((SqlInt32)o).Value),
                [new TypePair(T<SqlInt64>(), T<long>())] = o => ((SqlInt64)o).Value,
                [new TypePair(T<SqlInt64>(), T<long?>())] = o => new long?(((SqlInt64)o).Value),
                [new TypePair(T<SqlMoney>(), T<decimal>())] = o => ((SqlMoney)o).Value,
                [new TypePair(T<SqlMoney>(), T<decimal?>())] = o => new decimal?(((SqlMoney)o).Value),
                [new TypePair(T<SqlSingle>(), T<float>())] = o => ((SqlSingle)o).Value,
                [new TypePair(T<SqlSingle>(), T<float?>())] = o => new float?(((SqlSingle)o).Value),
                [new TypePair(T<SqlString>(), T<string>())] = o => ((SqlString)o).Value,
                [new TypePair(T<SqlString>(), T<XmlReader>())] = o => CreateReader(((SqlString)o).Value),
                [new TypePair(T<SqlString>(), T<XElement>())] = o => XElement.Load(CreateReader(((SqlString)o).Value)),
                [new TypePair(T<SqlString>(), T<XDocument>())] = o => XDocument.Load(CreateReader(((SqlString)o).Value)),
                [new TypePair(T<SqlString>(), T<XPathDocument>())] = o => new XPathDocument(CreateReader(((SqlString)o).Value)),
                [new TypePair(T<SqlString>(), T<XmlDocument>())] = o => ConvertStringToXml(((SqlString)o).Value),
                [new TypePair(T<SqlXml>(), T<string>())] = o => ((SqlXml)o).Value,
                [new TypePair(T<SqlXml>(), T<XmlReader>())] = o => ((SqlXml)o).CreateReader(),
                [new TypePair(T<SqlXml>(), T<XElement>())] = o => XElement.Load(((SqlXml)o).CreateReader()),
                [new TypePair(T<SqlXml>(), T<XDocument>())] = o => XDocument.Load(((SqlXml)o).CreateReader()),
                [new TypePair(T<SqlXml>(), T<XPathDocument>())] = o => new XPathDocument(((SqlXml)o).CreateReader()),
                [new TypePair(T<SqlXml>(), T<XmlDocument>())] = ConvertSqlXmlToXml,
            };

        internal static Func<object, object> GetConverter(this RuntimeTypeHandle source, RuntimeTypeHandle target, Type expected)
        {
            Func<object, object> converter;
            if (Converters.TryGetValue(new TypePair(source, target), out converter))
            {
                return converter;
            }

            return o => Convert.ChangeType(o, expected);
        }

        private static object ConvertSqlXmlToXml(object o)
        {
            Contract.Requires(o != null);

            var v = new XmlDocument();
            v.Load(((SqlXml)o).CreateReader());
            return v;
        }

        private static object ConvertStringToXml(object o)
        {
            Contract.Requires(o != null);

            var v = new XmlDocument();
            v.Load(CreateReader(o));
            return v;
        }

        private static XmlReader CreateReader(object xml)
        {
            Contract.Requires(xml != null);

            return new XmlTextReader(new StringReader((string)xml));
        }

        private static RuntimeTypeHandle T<T>()
        {
            return typeof(T).TypeHandle;
        }

        private class TypePairComparer : IEqualityComparer<TypePair>
        {
            public bool Equals(TypePair x, TypePair y)
            {
                return x.Source.Equals(y.Source) && x.Target.Equals(y.Target);
            }

            public int GetHashCode(TypePair obj)
            {
                return obj.Source.GetHashCode() ^ obj.Target.GetHashCode();
            }
        }

        private struct TypePair
        {
            public TypePair(RuntimeTypeHandle source, RuntimeTypeHandle target)
                : this()
            {
                this.Source = source;
                this.Target = target;
            }

            public RuntimeTypeHandle Source;

            public RuntimeTypeHandle Target;
        }
    }
}