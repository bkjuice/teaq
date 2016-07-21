using System.Collections;
using System.Collections.Generic;

namespace Teaq.Tests.FastReflection.SupportingTypes
{
    public class EnumerableType : IEnumerable
    {
        private List<int> values;

        public EnumerableType(List<int> values)
        {
            this.values = values;
        }

        public IEnumerator GetEnumerator()
        {
            if (values == null)
            {
                return new List<int>().GetEnumerator();
            }

            return this.values.GetEnumerator();
        }
    }
}
