using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teaq.Tests.FastReflection.SupportingTypes
{
    public class TypeWithoutDefaultCtor
    {
        public TypeWithoutDefaultCtor(string someValue1, int someValue2)
        {
            this.SomeValue1 = someValue1;
            this.SomeValue2 = someValue2;
        }

        public string SomeValue1 { get; private set; }

        [TestAttribute("Test1", "Test2")]
        [TestAttribute("Test1-2", "Test2-2")]
        public int SomeValue2 { get; private set; }
    }
}
