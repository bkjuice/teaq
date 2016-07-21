using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teaq.IL
{
    public class NullableIL
    {
        public object ILShim(object value)
        {
            var x = value as int?;
            if (x.HasValue)
            {
                return x.Value;
            }

            return value;
        }
    }
}
