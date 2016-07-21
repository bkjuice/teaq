using System.Collections;
using System.Collections.Generic;

namespace Teaq.Tests.FastReflection
{
    public class EnumerableTypeWithUntypedVoidAddMethod : IEnumerable<int>
    {
        private List<int> values;

        public EnumerableTypeWithUntypedVoidAddMethod(List<int> values)
        {
            this.values = values;
        }

        public void Add(object value)
        {
            this.values.Add((int)value);
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