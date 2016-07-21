using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teaq.Tests.FastReflection.SupportingTypes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class TestAttribute : Attribute
    {
        public TestAttribute(string ctorArg1, string ctorArg2)
        {
            this.CtorArg1 = ctorArg1;
            this.CtorArg2 = ctorArg2;
        }

        public string CtorArg1 { get; set; }
        public string CtorArg2 { get; set; }

        public string NamedParam1 { get; private set; }
        public string NamedParam2 { get; private set; }
        
    }
}
