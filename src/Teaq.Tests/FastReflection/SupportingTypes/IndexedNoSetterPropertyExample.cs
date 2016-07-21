using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teaq.Tests.FastReflection.SupportingTypes
{
    public class IndexedNoSetterPropertyExample
    {
        public string this[string value] 
        { 
            get { return value; }
        }
    }
}
