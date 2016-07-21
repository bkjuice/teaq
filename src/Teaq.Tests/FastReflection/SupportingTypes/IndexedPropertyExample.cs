using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teaq.Tests.FastReflection.SupportingTypes
{
    public class IndexedPropertyExample
    {
        private string[] items1 = new string[20];
        
        private int[] items2 = new int[20];

        public int this[int index]
        { 
            get { return this.items2[index]; } 
            set { this.items2[index] = value; } 
        }

        public string this[string index] 
        { 
            get { return this.items1[int.Parse(index)]; } 
            set { this.items1[int.Parse(index)] = value; } 
        }

        public string ReadOnlyProperty { get; private set; }
    }
}
