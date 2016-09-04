using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Server;

namespace Teaq
{
    internal static class UdtMappings
    {
        private static readonly Dictionary<Type, SqlDbType> typeMappings = new Dictionary<Type, SqlDbType>
        {
            { typeof(byte?), SqlDbType.TinyInt },
            { typeof(bool?), SqlDbType.Bit },
            { typeof(char?), SqlDbType.Char },
            { typeof(short?), SqlDbType.SmallInt },
            { typeof(int?), SqlDbType.Int },
            { typeof(long?), SqlDbType.BigInt },
            { typeof(float?), SqlDbType.Float },
            { typeof(decimal?), SqlDbType.Decimal },
            { typeof(Guid?), SqlDbType.UniqueIdentifier },
            { typeof(DateTime?), SqlDbType.DateTime },
            { typeof(DateTimeOffset?), SqlDbType.DateTimeOffset },
            { typeof(byte), SqlDbType.TinyInt },
            { typeof(bool), SqlDbType.Bit },
            { typeof(char), SqlDbType.Char },
            { typeof(short), SqlDbType.SmallInt },
            { typeof(int), SqlDbType.Int },
            { typeof(long), SqlDbType.BigInt },
            { typeof(float), SqlDbType.Float },
            { typeof(decimal), SqlDbType.Decimal },
            { typeof(Guid), SqlDbType.UniqueIdentifier },
            { typeof(DateTime), SqlDbType.DateTime },
            { typeof(DateTimeOffset), SqlDbType.DateTimeOffset },
            { typeof(byte[]), SqlDbType.VarBinary },
        };

        private static readonly string supportedTypes = GetSupportedTypeList();

        public static SqlMetaData GetUdtMetaData(this Type t, string columnName)
        {
            if (t == typeof(string))
            {
                var spec = Repository.GetDefaultStringType();
                return new SqlMetaData(columnName, spec.SqlDataType, spec.Size.GetValueOrDefault());
            }

            SqlDbType dbType;
            if (typeMappings.TryGetValue(t, out dbType))
            {
                return new SqlMetaData(columnName, dbType);
            }

            throw new InvalidOperationException($"Cannot map type: '{t.FullName}' to a known SQL primitive type. Ensure the target value type is in the following list of types:\r\n{supportedTypes}");
        }

        private static string GetSupportedTypeList()
        {
            var b = new StringBuilder(2048);
            b.AppendLine($"System.String : { SqlDbType.VarChar.ToString() } | { SqlDbType.NVarChar.ToString()}");
            foreach (var pair in typeMappings)
            {
                b.AppendLine($"{pair.Key.FullName} : { pair.Value.ToString()}");
            }

            return b.ToString();
        }
    }   
}