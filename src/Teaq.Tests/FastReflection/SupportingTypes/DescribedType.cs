using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teaq.Tests.FastReflection.SupportingTypes
{
    public class DescribedType
    {
        private string privateField;

        public string PublicField;

        public string PublicProperty
        {
            get
            {
                return this.privateField;
            }

            set
            {
                this.privateField = value;
            }
        }

        public int ReadOnlyValueProperty { get; private set; }

        public string PropertyWithNoSetter
        {
            get
            {
                return this.privateField;
            }
        }

        public string this[string value]
        {
            get
            {
                return value;
            }
        }


        public int InternalValueProperty { get; set; }

        public void PublicAction(int value)
        {
            this.InternalValueProperty = value;
        }

        public void NoArgsNoOp()
        {
        }

        public string PublicFunc(string value1, string value2, int value3)
        {
            return value1 + value2 + value3;
        }

        public string Overloaded(string value1)
        {
            return value1;
        }

        public string Overloaded(string value1, string value2)
        {
            return value1 + value2;
        }
    }
}
