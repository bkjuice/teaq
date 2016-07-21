using System.Collections;
using System.Collections.Generic;

namespace Teaq.Tests.FastReflection
{
    public class EnumerableTypeWithUntypedAddMethod : IEnumerable<int>
    {
        private List<int> values;

        public EnumerableTypeWithUntypedAddMethod(List<int> values)
        {
            this.values = values;
        }

        public int Add(object value)
        {
            this.values.Add((int)value);
            return this.values.Count - 1;
        }

        public IEnumerator<int> GetEnumerator()
        {
            if (values == null)
            {
                return new List<int>().GetEnumerator();
            }

            return this.values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}