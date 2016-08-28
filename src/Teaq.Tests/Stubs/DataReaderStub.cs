using System;
using System.Data;

namespace Teaq.Tests.Stubs
{
    internal class DataReaderStub : IDataReader
    {
        public Func<string, object> ValueGetterByName;

        public object this[string name]
        {
            get
            {
                return this.ValueGetterByName?.Invoke(name);
            }
        }

        public Func<int, object> ValueGetterByIndex;

        public object this[int i]
        {
            get
            {
                return this.ValueGetterByIndex?.Invoke(i);
            }
        }

        public Func<int> DepthGetter;

        public int Depth
        {
            get
            {
                return (this.DepthGetter?.Invoke()).GetValueOrDefault();
            }
        }

        public Func<int> FieldCountGetter;

        public int FieldCount
        {
            get
            {
                return (this.FieldCountGetter?.Invoke()).GetValueOrDefault();
            }
        }

        public Func<bool> IsClosedGetter;

        public bool IsClosed
        {
            get
            {
                return this.IsClosedGetter?.Invoke() ?? false;
            }
        }

        public Func<int> RecordsAffectedGetter;

        public int RecordsAffected
        {
            get
            {
                return this.RecordsAffectedGetter?.Invoke() ?? 0;
            }
        }

        public Action CloseAction;

        public void Close()
        {
            this.CloseAction?.Invoke();
        }

        public Action DisposeAction;

        public void Dispose()
        {
            this.DisposeAction?.Invoke();
        }

        public Func<int, bool> GetBooleanFunc;

        public bool GetBoolean(int i)
        {
            return this.GetBooleanFunc?.Invoke(i) ?? false;
        }

        public Func<int, byte> GetByteFunc;

        public byte GetByte(int i)
        {
            return this.GetByteFunc?.Invoke(i) ?? 0;
        }

        public Func<int, long, byte[], int, int, long> GetBytesFunc;

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this.GetBytesFunc?.Invoke(i, fieldOffset, buffer, bufferoffset, length) ?? -1;
        }

        public Func<int, char> GetCharFunc;

        public char GetChar(int i)
        {
            return this.GetCharFunc?.Invoke(i) ?? (char)0;
        }

        public Func<int, long, char[], int, int, long> GetCharsFunc;

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this.GetCharsFunc?.Invoke(i, fieldoffset, buffer, bufferoffset, length) ?? -1;
        }

        public Func<int, IDataReader> GetDataFunc;

        public IDataReader GetData(int i)
        {
            return this.GetDataFunc?.Invoke(i);
        }

        public Func<int, string> GetDataTypeNameFunc;

        public string GetDataTypeName(int i)
        {
            return this.GetDataTypeNameFunc?.Invoke(i);
        }

        public Func<int, DateTime> GetDateTimeFunc;

        public DateTime GetDateTime(int i)
        {
            return this.GetDateTimeFunc?.Invoke(i) ?? DateTime.MinValue;
        }

        public Func<int, decimal> GetDecimalFunc;
         
        public decimal GetDecimal(int i)
        {
            return this.GetDecimalFunc?.Invoke(i) ?? default(decimal);
        }

        public Func<int, double> GetDoubleFunc;

        public double GetDouble(int i)
        {
            return this.GetDoubleFunc?.Invoke(i) ?? default(double);
        }

        public Func<int, Type> GetFieldTypeFunc;

        public Type GetFieldType(int i)
        {
            return this.GetFieldTypeFunc?.Invoke(i);
        }

        public Func<int, float> GetFloatFunc;

        public float GetFloat(int i)
        {
            return this.GetFloatFunc?.Invoke(i) ?? default(float);
        }

        public Func<int, Guid> GetGuidFunc;

        public Guid GetGuid(int i)
        {
            return this.GetGuidFunc?.Invoke(i) ?? default(Guid);
        }

        public Func<int, short> GetInt16Func;

        public short GetInt16(int i)
        {
            return this.GetInt16Func?.Invoke(i) ?? default(short);
        }

        public Func<int, int> GetInt32Func;

        public int GetInt32(int i)
        {
            return this.GetInt32Func?.Invoke(i) ?? default(int);
        }

        public Func<int, long> GetInt64Func;

        public long GetInt64(int i)
        {
            return this.GetInt64Func?.Invoke(i) ?? default(long);
        }

        public Func<int, string> GetNameFunc;

        public string GetName(int i)
        {
            return this.GetNameFunc?.Invoke(i);
        }

        public Func<string, int> GetOrdinalFunc;

        public int GetOrdinal(string name)
        {
            return this.GetOrdinalFunc?.Invoke(name) ?? -1;
        }

        public Func<DataTable> GetSchemaTableFunc;

        public DataTable GetSchemaTable()
        {
            return this.GetSchemaTableFunc?.Invoke();
        }

        public Func<int, string> GetStringFunc;
         
        public string GetString(int i)
        {
            return this.GetStringFunc?.Invoke(i);
        }

        public Func<int, object> GetValueFunc;

        public object GetValue(int i)
        {
            return this.GetValueFunc?.Invoke(i);
        }

        public Func<object[], int> GetValuesFunc;

        public int GetValues(object[] values)
        {
            return this.GetValuesFunc?.Invoke(values) ?? default(int);
        }

        public Func<int, bool> IsDBNullFunc;

        public bool IsDBNull(int i)
        {
            return this.IsDBNullFunc?.Invoke(i) ?? default(bool);
        }

        public Func<bool> NextResultFunc;
     
        public bool NextResult()
        {
            return this.NextResultFunc?.Invoke() ?? default(bool);
        }

        public Func<bool> ReadFunc;

        public bool Read()
        {
            return this.ReadFunc?.Invoke() ?? default(bool);
        }
    }
}
