using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Teaq.Tests.Stubs
{
    public class DBParameterCollectionStub : IDataParameterCollection
    {
        private List<IDataParameter> parameters = new List<IDataParameter>();

        public int Count
        {
            get { return this.parameters.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return new object(); }
        }

        public object this[string parameterName]
        {
            get
            {
                return this.parameters.Select(p => p.ParameterName == parameterName).FirstOrDefault();
            }

            set
            {
                var index = this.parameters.FindIndex(p => p.ParameterName == parameterName);
                if (index > 0)
                {
                    this.parameters[index] = value as IDataParameter;
                }
            }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public object this[int index]
        {
            get
            {
                return this.parameters[index];
            }

            set
            {
                this.parameters[index] = value as IDataParameter;
            }
        }

        public bool Contains(string parameterName)
        {
            return this.parameters.Select(p => p.ParameterName == parameterName).Any();
        }

        public int IndexOf(string parameterName)
        {
            return this.parameters.FindIndex(p => p.ParameterName == parameterName);
        }

        public void RemoveAt(string parameterName)
        {
            var index = this.parameters.FindIndex(p => p.ParameterName == parameterName);
            if (index > 0)
            {
                this.parameters.RemoveAt(index);
            }
        }

        public int Add(object value)
        {
            var nextIndex = this.parameters.Count;
            this.parameters.Add(value as IDataParameter);
            return nextIndex;
        }

        public void Clear()
        {
            this.parameters.Clear();
        }

        public bool Contains(object value)
        {
            return this.parameters.Contains(value as IDataParameter);
        }

        public int IndexOf(object value)
        {
            return this.parameters.IndexOf(value as IDataParameter);
        }

        public void Insert(int index, object value)
        {
            this.parameters.Insert(index, value as IDataParameter);
        }

        public void Remove(object value)
        {
            this.parameters.Remove(value as IDataParameter);
        }

        public void RemoveAt(int index)
        {
            this.parameters.RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            Array.Copy(this.parameters.ToArray(), 0, array, index, this.parameters.Count);
        }

        public IEnumerator GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }
    }
}