using System;
using System.Data;
using System.Threading.Tasks;

namespace Teaq
{
    /// <summary>
    /// Extensions to enable parsing of results using IL generated delegate calls.
    /// </summary>
    public static class DataReaderJsonExtensions
    {
        /// <summary>
        /// Reads the specified data reader row by row using an iterator.
        /// </summary>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="builderToUse">The builder to use.</param>
        /// <param name="mapping">The optional field mappings.</param>
        /// <returns></returns>
        public static async Task ReadToJsonAsync(
            this IDataReader reader, 
            JsonOutputBuilder builderToUse, 
            JsonMapping mapping = null)
        {
            Action readAsync = () => reader.ReadToJson(builderToUse, mapping);
            await readAsync.InvokeAsAwaitable();
        }

        /// <summary>
        /// Reads the specified data reader.
        /// </summary>
        /// <param name="reader">The reader to be read.</param>
        /// <param name="builderToUse">The builder to use.</param>
        /// <param name="mapping">The optional field mappings.</param>
        public static void ReadToJson(
            this IDataReader reader,
            JsonOutputBuilder builderToUse,
            JsonMapping mapping = null)
        {
            if (reader == null)
            {
                return;
            }

            mapping = mapping ?? JsonMapping.DefaultInstance;
            if (reader.FieldCount == 1)
            {
                while (reader.Read())
                {
                    reader.WritePrimitiveOrString(builderToUse, mapping);
                }

                return;
            }

            while (reader.Read())
            {
                builderToUse.StartObject();
                reader.WriteJsonObject(builderToUse, mapping);
                builderToUse.CloseScope();
            }
        }

        /// <summary>
        /// Reads a complex type from the data stream.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="builderToUse">The builder to use.</param>
        /// <param name="mapping">The optional field mappings.</param>
        private static void WriteJsonObject(
            this IDataReader reader, 
            JsonOutputBuilder builderToUse, 
            JsonMapping mapping)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                reader.WriteNamedValue(builderToUse, i, mapping);
            }
        }

        /// <summary>
        /// Reads the primitive or string.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="builderToUse">The builder to use.</param>
        /// <param name="mapping">The mapping.</param>
        private static void WritePrimitiveOrString(this IDataReader reader, JsonOutputBuilder builderToUse, JsonMapping mapping)
        {
            JsonOutputValueKind valueKind;
            var value = mapping.GetJsonFieldValue(reader, 0, out valueKind);
            builderToUse.WriteArrayMember(value, valueKind);
        }

        /// <summary>
        /// Reads the current value from the data reader and applies a
        /// type conversion if applicable, and sets the target entity property.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="builderToUse">The builder to use.</param>
        /// <param name="index">The column index.</param>
        /// <param name="mapping">The mapping.</param>
        private static void WriteNamedValue(this IDataReader reader, JsonOutputBuilder builderToUse, int index, JsonMapping mapping)
        {
            string fieldName;
            JsonOutputValueKind valueKind;
            var value = mapping.GetJsonFieldValue(reader, index, out fieldName, out valueKind);
            builderToUse.WriteObjectMember(fieldName, value, valueKind);
        }
    }
}
