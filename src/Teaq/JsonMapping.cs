using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.Net;

namespace Teaq
{
    /// <summary>
    /// The JSON mapping container used to define column name and data type conversion mappings.
    /// </summary>
    public class JsonMapping
    {
        /// <summary>
        /// The default instance singleton.
        /// </summary>
        internal static readonly JsonMapping DefaultInstance = new JsonMapping();

        /// <summary>
        /// The explicit converters.
        /// </summary>
        private static readonly Dictionary<Type, Func<object, string>> defaultConverters = 
            new Dictionary<Type, Func<object, string>>
        {
            [typeof(SqlBinary)] = o => Convert.ToBase64String(((SqlBinary)o).Value),
            [typeof(SqlBoolean)] = o => ((SqlBoolean)o).Value ? "true" : "false",
            [typeof(SqlByte)] = o => ((SqlByte)o).Value.ToString(),
            [typeof(SqlChars)] = o => new string(((SqlChars)o).Value),
            [typeof(SqlDateTime)] = o => ((SqlDateTime)o).Value.ToString("o"),
            [typeof(SqlDecimal)] = o => ((SqlDecimal)o).Value.ToString(),
            [typeof(SqlDouble)] = o => ((SqlDouble)o).Value.ToString(),
            [typeof(SqlGuid)] = o => ((SqlGuid)o).Value.ToString(),
            [typeof(SqlInt16)] = o => ((SqlInt16)o).Value.ToString(),
            [typeof(SqlInt32)] = o => ((SqlInt32)o).Value.ToString(),
            [typeof(SqlInt64)] = o => ((SqlInt64)o).Value.ToString(),
            [typeof(SqlMoney)] = o => ((SqlMoney)o).Value.ToString(),
            [typeof(SqlSingle)] = o => ((SqlSingle)o).Value.ToString(),
            [typeof(SqlString)] = o => ((SqlString)o).Value,
            [typeof(SqlXml)] = o => WebUtility.HtmlEncode(((SqlXml)o).Value),
            [typeof(byte[])] = o => Convert.ToBase64String((byte[])o),
            [typeof(bool)] = o => ((bool)o) ? "true" : "false",
            [typeof(byte)] = o => o.ToString(),
            [typeof(char[])] = o => new string((char[])o),
            [typeof(DateTime)] = o => ((DateTime)o).ToString("o"),
            [typeof(DateTimeOffset)] = o => ((DateTimeOffset)o).ToString("o"),
            [typeof(decimal)] = o => o.ToString(),
            [typeof(double)] = o => o.ToString(),
            [typeof(Guid)] = o => o.ToString(),
            [typeof(short)] = o => o.ToString(),
            [typeof(int)] = o => o.ToString(),
            [typeof(long)] = o => o.ToString(),
            [typeof(float)] = o => o.ToString(),
        };

        /// <summary>
        /// The JSON kind mappings.
        /// </summary>
        private static readonly Dictionary<Type, JsonOutputValueKind> jsonKinds = new Dictionary<Type, JsonOutputValueKind>
        {
            [typeof(SqlBinary)] = JsonOutputValueKind.StringValue,
            [typeof(SqlBoolean)] = JsonOutputValueKind.BooleanValue,
            [typeof(SqlByte)] = JsonOutputValueKind.NumberValue,
            [typeof(SqlChars)] = JsonOutputValueKind.StringValue,
            [typeof(SqlDateTime)] = JsonOutputValueKind.StringValue,
            [typeof(SqlDecimal)] = JsonOutputValueKind.NumberValue,
            [typeof(SqlDouble)] = JsonOutputValueKind.NumberValue,
            [typeof(SqlGuid)] = JsonOutputValueKind.StringValue,
            [typeof(SqlInt16)] = JsonOutputValueKind.NumberValue,
            [typeof(SqlInt32)] = JsonOutputValueKind.NumberValue,
            [typeof(SqlInt64)] = JsonOutputValueKind.NumberValue,
            [typeof(SqlMoney)] = JsonOutputValueKind.StringValue,
            [typeof(SqlSingle)] = JsonOutputValueKind.NumberValue,
            [typeof(SqlString)] = JsonOutputValueKind.StringValue,
            [typeof(SqlXml)] = JsonOutputValueKind.StringValue,
            [typeof(byte[])] = JsonOutputValueKind.StringValue,
            [typeof(bool)] = JsonOutputValueKind.BooleanValue,
            [typeof(byte)] = JsonOutputValueKind.NumberValue,
            [typeof(char[])] = JsonOutputValueKind.StringValue,
            [typeof(DateTime)] = JsonOutputValueKind.StringValue,
            [typeof(DateTimeOffset)] = JsonOutputValueKind.StringValue,
            [typeof(decimal)] = JsonOutputValueKind.NumberValue,
            [typeof(double)] = JsonOutputValueKind.NumberValue,
            [typeof(Guid)] = JsonOutputValueKind.StringValue,
            [typeof(short)] = JsonOutputValueKind.NumberValue,
            [typeof(int)] = JsonOutputValueKind.NumberValue,
            [typeof(long)] = JsonOutputValueKind.NumberValue,
            [typeof(float)] = JsonOutputValueKind.NumberValue,
        };

        /// <summary>
        /// The explicit mappings, if any.
        /// </summary>
        private Dictionary<string, JsonFieldInfo> explicitMappings;

        /// <summary>
        /// Adds the field mapping.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="mappedName">Name of the mapped.</param>
        /// <param name="valueKind">Kind of the value.</param>
        /// <param name="converter">The converter.</param>
        public void AddFieldMapping(string columnName, string mappedName, JsonOutputValueKind? valueKind = null, Func<object, string> converter = null)
        {
            Contract.Requires(!string.IsNullOrEmpty(columnName));
            Contract.Requires(!string.IsNullOrEmpty(mappedName));

            if (this.explicitMappings == null)
            {
                this.explicitMappings = new Dictionary<string, JsonFieldInfo>();
            }

            valueKind = valueKind ?? JsonOutputValueKind.StringValue;
            converter = converter == null ? o => o.ToString() : converter;

            this.explicitMappings.Add(columnName, new JsonFieldInfo { FieldName = mappedName, ValueKind = valueKind, Converter = converter });
        }

        /// <summary>
        /// Gets the json field value.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="index">The index.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="valueKind">Kind of the value.</param>
        /// <returns>The JSON field value.</returns>
        internal string GetJsonFieldValue(IDataReader reader, int index, out string fieldName, out JsonOutputValueKind valueKind)
        {
            fieldName = reader.GetName(index);
            var fieldType = reader.GetFieldType(index);
            Contract.Assume(fieldType != null);

            var fieldInfo = this.GetFieldInfo(fieldName);
            if (!fieldInfo.HasValue)
            {
                if (reader.IsDBNull(index))
                {
                    valueKind = JsonOutputValueKind.NullValue;
                    return null;
                }

                valueKind = this.GetValueKind(fieldType);
                Func<object, string> c = GetConverter(fieldType);
                return c(reader[index]);
            }

            fieldName = fieldInfo.Value.FieldName;
            if (reader.IsDBNull(index))
            {
                valueKind = JsonOutputValueKind.NullValue;
                return null;
            }

            valueKind = fieldInfo.Value.ValueKind ?? this.GetValueKind(fieldType);
            var converter = fieldInfo.Value.Converter ?? this.GetConverter(fieldType);
            return fieldInfo.Value.Converter(reader[index]);
        }

        /// <summary>
        /// Gets the json field value.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="index">The index.</param>
        /// <param name="valueKind">Kind of the value.</param>
        /// <returns>
        /// The JSON field value.
        /// </returns>
        internal string GetJsonFieldValue(IDataReader reader, int index, out JsonOutputValueKind valueKind)
        {
            string fieldName;
            return this.GetJsonFieldValue(reader, index, out fieldName, out valueKind);
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// The literal or mapped name of the field and the expected JSON data kind.
        /// </returns>
        private JsonFieldInfo? GetFieldInfo(string fieldName)
        {
            Contract.Requires(this.explicitMappings == null || fieldName != null);

            if (this.explicitMappings == null)
            {
                return null;
            }

            JsonFieldInfo info;
            if (!this.explicitMappings.TryGetValue(fieldName, out info))
            {
                return null;
            }

            return info;
        }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns>The converion function.</returns>
        private Func<object, string> GetConverter(Type fieldType)
        {
            Func<object, string> converter;
            if (!defaultConverters.TryGetValue(fieldType, out converter))
            {
                return o => o.ToString();
            }

            return converter;
        }

        /// <summary>
        /// Gets the kind of the json value.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns>The JSON value kind to write.</returns>
        private JsonOutputValueKind GetValueKind(Type fieldType)
        {
            Contract.Requires(fieldType != null);

            JsonOutputValueKind valueKind;
            if (!jsonKinds.TryGetValue(fieldType, out valueKind))
            {
                valueKind = JsonOutputValueKind.StringValue;
            }

            return valueKind;
        }

        /// <summary>
        /// The configured JSON field info.
        /// </summary>
        private struct JsonFieldInfo
        {
            public string FieldName;

            public JsonOutputValueKind? ValueKind;

            public Func<object, string> Converter;
        }
    }
}